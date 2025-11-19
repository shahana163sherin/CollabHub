using CollabHub.Application.DTO;
using CollabHub.Application.DTO.Auth;
using CollabHub.Application.Interfaces.Auth;
using CollabHub.Domain.Entities;
using CollabHub.Domain.Enum;
using CollabHub.Infrastructure.Repositories.EF;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CollabHub.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IHashPassword _hash;
        private readonly ITokenService _token;
        private readonly IGenericRepository<User> _repo;
        private readonly IGenericRepository<LoginAudit> _audit;
        private readonly IGenericRepository<RefreshToken> _refresh;
        private readonly IGenericRepository<PasswordResetToken> _reset;
        private readonly IGenericRepository<FileResource> _file;
        public AuthService(IHashPassword hash, ITokenService token, IGenericRepository<User> repo, IGenericRepository<LoginAudit> audit, IGenericRepository<RefreshToken> refresh,
            IGenericRepository<PasswordResetToken> reset, IGenericRepository<FileResource> file)
        {

            _hash = hash;
            _token = token;
            _repo = repo;
            _audit = audit;
            _refresh = refresh;
            _reset = reset;
            _file = file;
        }

        
        private async Task<FileResource?> ProcessFileAsync(IFormFile file, FileContextType contextType,int referenceId)
        {
            if (file == null) return (null);

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var fileBytes = ms.ToArray();
            var base64 = Convert.ToBase64String(fileBytes);

            var fileResource = new FileResource
            {
                FileName = file.FileName,
                FileExtension = Path.GetExtension(file.FileName),
                FileSizeInKB = Math.Round((decimal)fileBytes.Length / 1024, 2),
                FilePath = null,
                ContextType = contextType,
                ReferenceId = referenceId,
                FileData=base64,
                IsActive=true
            };

            await _file.AddAsync(fileResource);
            await _file.SaveAsync();

            return fileResource;
        }

        public async Task<ApiResponse<object>> RegisterMemberAsync(RegisterDTO dto)
        {
            dto.Email = dto.Email.Trim().ToLower();
            dto.Name=dto.Name.Trim();

            var existing = await _repo.GetOneAsync(u => u.Email == dto.Email);
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))

            {
                return ApiResponse<object>.Fail(statusCode: 400,
                                                    message: "All fields are required",
                                                    type: "Validation Error",
                                                    details: "Name,Email and Password cannot be empty");

            }


            if (existing != null)
            {
                return ApiResponse<object>.Fail(statusCode: 409,
                                                      message: "Email already exist.Use new Email",
                                                      type: "DuplicateEmail",
                                                      details: "The provided email is already registered");
            }


            var hashedPassword = _hash.HashPassword(dto.Password);


            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = hashedPassword,
                Role = UserRole.Member,
            };
            await _repo.AddAsync(user);
            await _repo.SaveAsync();

            var fileResource = await ProcessFileAsync(dto.ProfileImg, FileContextType.ProfileImage,user.UserId);

            if (fileResource != null)
            {
                fileResource.ReferenceUser = user; 
                user.UploadedFiles.Add(fileResource);
            }

           
            await _repo.UpdateAsync(user);
            await _repo.SaveAsync();


            return ApiResponse<object>.Success(
                statusCode: 201,
                message: "You’re in! Let’s get started.",
                data: new { user.UserId, user.Name, user.Email, user.Role });
        }

        public async Task<ApiResponse<object>> RegisterLeaderAsync(RegisterLeaderDTO dto)
        {
            dto.Email = dto.Email.Trim().ToLower();
            dto.Name=dto.Name.Trim();
            var existing = await _repo.GetOneAsync(u => u.Email == dto.Email);
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password)||
                string.IsNullOrWhiteSpace(dto.Qualification))

            {
                return ApiResponse<object>.Fail(statusCode: 400,
                                                    message: "All fields are required",
                                                    type: "Validation Error",
                                                    details: "Name,Email,Passwod and Qualification cannot be empty");
                
            }


            if (existing != null)
            {
                return ApiResponse<object>.Fail(statusCode: 409,
                                                    message: "Email already exist.Use new Email",
                                                    type: "DuplicateEmail",
                                                    details: "The provided email is already registered");
            }

            


            var hashedPassword = _hash.HashPassword(dto.Password);


            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = hashedPassword,
                Role = UserRole.TeamLead,
               
                Qualification = dto.Qualification
            };
            await _repo.AddAsync(user);
            await _repo.SaveAsync();
            var  fileResource = await ProcessFileAsync(dto.ProfileImg, FileContextType.ProfileImage,user.UserId);


            if (fileResource != null)
            {
                fileResource.ReferenceUser = user; 
                user.UploadedFiles.Add(fileResource);
            }

           

            await _repo.UpdateAsync(user);
            await _repo.SaveAsync();

            return ApiResponse<object>.Success(
                statusCode: 201,
                message: "You’re in! Let’s get started.",
                data: new { user.UserId, user.Name, user.Email, user.Qualification, user.Role });
           
        }

        public async Task<ApiResponse<AuthResponseDTO>> LoginAsync(LoginDTO login)
        {
            login.Email = login.Email.Trim().ToLower();
            var user = await _repo.GetOneAsync(u => u.Email == login.Email);
            if (user == null)
            {
                return ApiResponse<AuthResponseDTO>.Fail(
                    statusCode: 400,
                    message: "Invalid Email",
                    type: "InvalidEmail",
                    details: "The email you entered does not match our records");
            }

            if (!_hash.verifyPassword(user.Password, login.Password))
            {
                return ApiResponse<AuthResponseDTO>.Fail(
                    statusCode:400,
                    message:"Invalid password",
                    type:"InvalidPassword",
                    details: "The password you entered does not match our records");
               
            }
            if (user.IsActive == false)
            {
                return ApiResponse<AuthResponseDTO>.Fail(
                    statusCode: 403,
                    message:"User is blocked.Please contact the admin",
                    type:"UserBlocked",
                    details:"This account has been disabled by admin");
            }
            user.LastLoginedAt = DateTime.Now;
            await _repo.UpdateAsync(user);
            await _repo.SaveAsync();

            var loginaudit = new LoginAudit
            {
                UserId = user.UserId,
                Role = user.Role,
                Email = user.Email,
                PasswordHash = user.Password,
                LoginAt = DateTime.Now
            };

            await _audit.AddAsync(loginaudit);
            await _audit.SaveAsync();
            var token = _token.GenerateAccessToken(user);
            var refresh = _token.GenerateRefreshToken(user);
            var refreshEntity = new RefreshToken
            {
                Token = refresh,
                UserId = user.UserId,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _refresh.AddAsync(refreshEntity);
            await _refresh.SaveAsync();
            return ApiResponse<AuthResponseDTO>.Success(
                statusCode:200,
                message: "All set. Let’s continue.",
                data:new AuthResponseDTO
                {
                    AccessToken=token,
                    RefreshToken=refresh
                });
            


        }
        public async Task<ApiResponse<string>> LogoutAsync(string refreshToken)
        {
            var token = await _refresh.GetOneAsync(t => t.Token == refreshToken);
            if (token == null || token.IsRevoked)
            {
                return ApiResponse<string>.Fail(
                             statusCode: 401,
                             message: "Logout failed",
                             type: "InvalidToken",
                             details: "Token is missing, invalid, or already revoked. Try again."
                         );

            }
            token.RevokedAt = DateTime.Now;
            token.IsRevoked = true;
            await _refresh.UpdateAsync(token);
            await _refresh.SaveAsync();
            return ApiResponse<string>.Success(
                statusCode:200,
                message: "Logout complete — catch you later.",
                data:null);
           
             
        }


        public async Task<ApiResponse<AuthResponseDTO>> RefreshTokenAsync(string refreshToken)
        {
            var refresh = await _refresh.GetOneAsync(r => r.Token == refreshToken);
            if(refresh == null || refresh.IsRevoked || refresh.ExpiresAt < DateTime.UtcNow)
            {
                return ApiResponse<AuthResponseDTO>.Fail(
                    statusCode:401,
                    message:"Invalid or expired refresh token",
                    type:"InvalidRefreshToken",
                    details:"The provided refresh token is invalid,revoked or no longer valid");
            }

            

            var user=await _repo.GetByIdAsync(refresh.UserId);

            if (user == null)
            {
                return ApiResponse<AuthResponseDTO>.Fail(
                    statusCode: 404,
                    message: "User not found",
                    type: "NotFound",
                    details: "No user exists for this refresh token");
               
            }

            
            var newAccessToken = _token.GenerateAccessToken(user);
            var newRefreshToken=_token.GenerateRefreshToken(user);

            refresh.IsRevoked = true;
            refresh.RevokedAt = DateTime.Now;
            refresh.ReplacedByToken = newRefreshToken;
            await _refresh.UpdateAsync(refresh);



            var newRefreshEntity = new RefreshToken
            {
                Token = newRefreshToken,
                UserId = user.UserId,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _refresh.AddAsync(newRefreshEntity);
            await _refresh.SaveAsync();

            return ApiResponse<AuthResponseDTO>.Success(statusCode: 200,
                message: "Token refreshed succesfully",
                data: new AuthResponseDTO
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                });
           
        }

        public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDTO dto)
        {
            dto.Email = dto.Email.Trim().ToLower();
            var user = await _repo.GetOneAsync(u => u.Email == dto.Email);
            if (user == null) return  ApiResponse<string>.Fail(
                    statusCode: 404,
                    message: "User not found",
                    type: "NotFound",
                    details: "No user exists for this email");

            var tokenEntity = await _reset.GetOneAsync(r => r.UserId == user.UserId && !r.IsUsed);
            if (tokenEntity == null || tokenEntity.ExpiresAt < DateTime.UtcNow)
                return ApiResponse<string>.Fail(
                    statusCode:401,
                    message: "Expired token",
                    type:"ExpiredResetToken",
                    details: "The provided reset token is expired");

            var isValid = _hash.verifyPassword(tokenEntity.TokenHash, dto.Token);

            if (!isValid) return ApiResponse<string>.Fail(
                statusCode:401,
                message: "Invalid reset token",
                type: "InvalidResetToken",
                details: "The provided reset token is invalid");

            user.Password = _hash.HashPassword(dto.NewPassword);
            await _repo.UpdateAsync(user);

            tokenEntity.IsUsed = true;
            await _reset.UpdateAsync(tokenEntity);

            await _repo.SaveAsync();
            await _reset.SaveAsync();
            return ApiResponse<string>.Success(
                statusCode: 200,
                message: "Password reset successfully.",
                data: null);
           

        }

        public async Task<ApiResponse<string>> ForgotPasswordAsync(ForgetPasswordDTO fp)
        {
            var user = await _repo.GetOneAsync(u => u.Email == fp.Email);
            if (user == null) return ApiResponse<string>.Fail(statusCode: 404,
                message: "User not found",
                type: "NotFound",
                details: "No user exists for this email");

            var rawToken=Guid.NewGuid().ToString("N");
            var hashedToken = _hash.HashPassword(rawToken);
            var tokenEntity = new PasswordResetToken
            {
                UserId = user.UserId,
                TokenHash = hashedToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            await _reset.AddAsync(tokenEntity);
            await _reset.SaveAsync();

            return ApiResponse<string>.Success(statusCode: 200,
                message:"Token set completed",
                data: null);
           
        }



    }
}
