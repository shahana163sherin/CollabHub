using AutoMapper;
using CollabHub.Application.DTO;
using CollabHub.Application.Interfaces;
using CollabHub.Application.Interfaces.Auth;
using CollabHub.Domain.Entities;
using CollabHub.Domain.Enum;
using CollabHub.Infrastructure.Repositories.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Services
{
    public class ProfileService:IProfileService
    {
        private readonly IGenericRepository<User> _repo;
        private readonly IHashPassword _hash;
        private readonly IGenericRepository<FileResource> _file;

        public ProfileService(IGenericRepository<User> repo, IHashPassword hash, IGenericRepository<FileResource> file)
        {
            _repo = repo;
            _hash = hash;
            _file = file;
        }

        
        private async Task<FileResource> ProcessFileAsync(IFormFile file, FileContextType contextType, int referenceId, FileResource? existingFile = null)
        {
            if (file == null) return null;

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var fileBytes = ms.ToArray();
            var base64 = Convert.ToBase64String(fileBytes);

            if (existingFile != null)
            {
                existingFile.FileName = file.FileName;
                existingFile.FileExtension = Path.GetExtension(file.FileName);
                existingFile.FileSizeInKB = Math.Round((decimal)fileBytes.Length / 1024, 2);
                existingFile.FileData = base64;
                existingFile.ModifiedOn = DateTime.Now;
                existingFile.ModifiedBy = referenceId;
                await _file.UpdateAsync(existingFile);
                await _file.SaveAsync();
                return existingFile;
            }

            var newFile = new FileResource
            {
                FileName = file.FileName,
                FileExtension = Path.GetExtension(file.FileName),
                FileSizeInKB = Math.Round((decimal)fileBytes.Length / 1024, 2),
                FilePath = null,
                ContextType = contextType,
                ReferenceId = referenceId,
                FileData = base64,
                IsActive = true
            };

            await _file.AddAsync(newFile);
            await _file.SaveAsync();
            return newFile;
        }
        public async Task<ApiResponse<string>> UpdateProfileAsync(int userId, UpdateProfileDTO dto)
        {
           
            var user = _repo.QueryByCondition(u => u.UserId == userId)
                            .Include(u => u.UploadedFiles)
                            .FirstOrDefault();

            if (user == null)
                return ApiResponse<string>.Fail(
                    statusCode:404,
                    message:"User not found",
                    type: "NotFound",
                    details:"No user exists");

            if (string.IsNullOrEmpty(dto.Name))
                return ApiResponse<string>.Fail(
                    statusCode: 400,
                    message: "Name cannot be empty",
                    type: "ValidationError",
                    details: "Name field was missing or not provided"
                    ); 

            user.Name = dto.Name;

            
            var existingProfileImage = user.UploadedFiles
                .Where(f => f.ContextType == FileContextType.ProfileImage && f.IsActive)
                .OrderByDescending(f => f.CreatedOn)
                .FirstOrDefault();

            if (dto.ProfileImage != null)
            {
           
                var fileResource = await ProcessFileAsync(dto.ProfileImage, FileContextType.ProfileImage, user.UserId, existingProfileImage);

                if (existingProfileImage == null && fileResource != null)
                    user.UploadedFiles.Add(fileResource);
            }

            if (user.Role == Domain.Enum.UserRole.TeamLead)
                user.Qualification = dto.Qualification ?? user.Qualification;

            user.ModifiedBy = user.UserId;
            user.ModifiedOn = DateTime.Now;

            await _repo.UpdateAsync(user);
            await _repo.SaveAsync();

            return ApiResponse<string>.Success(
                statusCode:200,
                message: "Profile updated successfully",
                data:null); 
        }
        public async Task<ApiResponse<object>> GetProfileAsync(int userId)
        {
           
            var user = _repo.QueryByCondition(u => u.UserId == userId)
                            .Include(u => u.UploadedFiles)
                            .FirstOrDefault();

            if (user == null)
                 return ApiResponse<object>.Fail(
                    statusCode: 404,
                    message: "User not found",
                    type: "NotFound",
                    details: "No user exists");


            var profileImageBase64 = user.UploadedFiles
                .Where(f => f.ContextType == FileContextType.ProfileImage && f.IsActive)
                .OrderByDescending(f => f.CreatedOn)
                .Select(f => f.FileData)
                .FirstOrDefault();

            var profile = new
            {
                user.UserId,
                user.Name,
                user.Email,
                ProfileImage = profileImageBase64,
                Qualification = user.Role == Domain.Enum.UserRole.TeamLead ? user.Qualification : null
            };

            return  ApiResponse<object>.Success(
                statusCode: 200,
                message: "Profile fetched successfully",
                data: profile);
        }

        public async Task<ApiResponse<string>> ChangePasswordAsync(int userId, ChangePasswordDTO dto)
        {
            var user = _repo.QueryByCondition(u => u.UserId == userId)
                    .Include(u => u.UploadedFiles)
                    .FirstOrDefault();
            if (user == null) return ApiResponse<string>.Fail(
                    statusCode: 404,
                    message: "User not found",
                    type: "NotFound",
                    details: "No user exists");

            bool validCurrent = _hash.verifyPassword(user.Password, dto.CurrentPassword);
            if (!validCurrent) return ApiResponse<string>.Fail(
                statusCode:400,
                message:"Curren password is incorrect",
                type:"IncorrectPassword",
                details:"The provided current password is incorrect"
                );
            dto.NewPassword = dto.NewPassword.Trim();
            if (string.IsNullOrEmpty(dto.NewPassword) || dto.NewPassword.Length < 6)
                return ApiResponse<string>.Fail(
                           statusCode: 400,
                           message: "New password must be at least 6 characters",
                           type: "InvalidPasswordLength",
                           details: "The new password is either empty or shorter than 6 characters. Provide a valid password.");
            

            if (dto.NewPassword != dto.ConfirmPassword) return ApiResponse<string>.Fail(
                        statusCode: 400,
                        message: "Password do not match",
                        type: "PasswordMismatch",
                        details: "The ConfirmPassword value does not match the NewPassword value."
                    );

            user.Password = _hash.HashPassword(dto.NewPassword);
            user.LastPasswordChangedAt = DateTime.UtcNow;
            user.ModifiedBy = user.UserId;
            user.ModifiedOn = DateTime.Now;

            await _repo.UpdateAsync(user);
            await _repo.SaveAsync();

            return ApiResponse<string>.Success(
                statusCode: 200,
                message: "Password changed successfully",
                data: null);
        }

    }
    
}
