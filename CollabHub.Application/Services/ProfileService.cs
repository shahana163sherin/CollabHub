using CollabHub.Application.DTO;
using CollabHub.Application.Interfaces;
using CollabHub.Application.Interfaces.Auth;
using CollabHub.Domain.Entities;
using CollabHub.Domain.Enum;
using CollabHub.Infrastructure.Repositories.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
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
                return new ApiResponse<string> { Success = false, Message = "User not found" };

            if (string.IsNullOrEmpty(dto.Name))
                return new ApiResponse<string> { Success = false, Message = "Name cannot be empty" };

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

            return new ApiResponse<string> { Success = true, Message = "Profile updated successfully" };
        }
        public async Task<ApiResponse<object>> GetProfileAsync(int userId)
        {
           
            var user = _repo.QueryByCondition(u => u.UserId == userId)
                            .Include(u => u.UploadedFiles)
                            .FirstOrDefault();

            if (user == null)
                return new ApiResponse<object> { Success = false, Message = "User not found" };

        
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

            return new ApiResponse<object> { Success = true, Message = "Profile fetched successfully", Data = profile };
        }

        public async Task<ApiResponse<string>> ChangePasswordAsync(int userId, ChangePasswordDTO dto)
        {
            var user = _repo.QueryByCondition(u => u.UserId == userId)
                    .Include(u => u.UploadedFiles)
                    .FirstOrDefault();
            if (user == null) return new ApiResponse<string> { Success = false, Message = "User not found" };

            bool validCurrent = _hash.verifyPassword(user.Password, dto.CurrentPassword);
            if (!validCurrent) return new ApiResponse<string> { Success = false, Message = "Current Password is incorrect" };

            if (string.IsNullOrEmpty(dto.NewPassword) || dto.NewPassword.Length < 6)
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "New password must be atleast 6 characters"
                };

            if (dto.NewPassword != dto.ConfirmPassword) return new ApiResponse<string>
            {
                Success = false,
                Message = "Password do not match"
            };

            user.Password = _hash.HashPassword(dto.NewPassword);
            user.ModifiedBy = user.UserId;
            user.ModifiedOn = DateTime.Now;

            await _repo.UpdateAsync(user);
            await _repo.SaveAsync();

            return new ApiResponse<string> { Success = true, Message = "Password changed successfully" };

        }
    }
}
