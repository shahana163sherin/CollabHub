using CollabHub.Application.DTO;
using CollabHub.Application.Interfaces;
using CollabHub.Application.Interfaces.Auth;
using CollabHub.Domain.Entities;
using CollabHub.Infrastructure.Repositories.EF;
using Microsoft.AspNetCore.Identity;
using System;
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

        public ProfileService(IGenericRepository<User> repo, IHashPassword hash)
        {
            _repo = repo;
            _hash = hash;
        }

        public async Task<ApiResponse<object>> GetProfileAsync(int userId)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user == null)
                return new ApiResponse<object> { Success = false, Message = "User not found" };

            var profile = new
            {
                user.UserId,
                user.Name,
                user.Email,
                user.ProfileImg,
                Qualification = user.Role == Domain.Enum.UserRole.TeamLead ? user.Qualification : null

            };

            return new ApiResponse<object>
            {
                Success = true,
                Message = "Profile fetched successfully",
                Data = profile
            };
        }

        public async Task<ApiResponse<string>> UpdateProfileAsync(int userId, UpdateProfileDTO dto)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user == null) return new ApiResponse<string> { Success = false, Message = "User not found" };

            if (string.IsNullOrEmpty(dto.Name)) return new ApiResponse<string>
            {
                Success = false,
                Message = "Name cannot be empty"
            };

            user.Name = dto.Name;
            user.ProfileImg = dto.ProfileImage ?? user.ProfileImg;

            if (user.Role == Domain.Enum.UserRole.TeamLead) user.Qualification = dto.Qualification ?? user.Qualification;
            user.ModifiedBy = user.UserId;
            user.ModifiedOn = DateTime.Now;

            await _repo.UpdateAsync(user);
            await _repo.SaveAsync();

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Profile updated Successfully"
              
            };
        }

       public async Task<ApiResponse<string>> ChangePasswordAsync(int userId, ChangePasswordDTO dto)
        {
            var user = await _repo.GetByIdAsync(userId);
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
