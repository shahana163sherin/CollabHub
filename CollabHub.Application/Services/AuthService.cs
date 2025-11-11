using CollabHub.Application.DTO;
using CollabHub.Application.DTO.Auth;
using CollabHub.Application.Interfaces.Auth;
using CollabHub.Domain.Entities;
using CollabHub.Domain.Enum;
using CollabHub.Infrastructure.Repositories.EF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CollabHub.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IHashPassword _hash;
        private readonly ITokenService _token;
        private readonly IGenericRepository<User> _repo;
        private readonly IGenericRepository<LoginAudit> _audit;
        private readonly IGenericRepository<RefreshToken> _refresh;
        public AuthService(IHashPassword hash, ITokenService token, IGenericRepository<User> repo, IGenericRepository<LoginAudit> audit, IGenericRepository<RefreshToken> refresh)
        {

            _hash = hash;
            _token = token;
            _repo = repo;
            _audit = audit;
            _refresh = refresh;
        }

        
        public async Task<ApiResponse<object>> RegisterMemberAsync(RegisterDTO dto)
        {
            var existing = await _repo.GetOneAsync(u => u.Email == dto.Email);
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))

            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "All fields are required."
                };
            }


            if (existing != null)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Email already exist.Please use a different one"
                };
            }
            var hashedPassword = _hash.HashPassword(dto.Password);


            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = hashedPassword,
                Role = UserRole.Member,
                ProfileImg = dto.ProfileImg
            };

            await _repo.AddAsync(user);
            await _repo.SaveAsync();

            return new ApiResponse<object>
            {
                Success = true,
                Message = "You’re in! Let’s get started.",
                Data = new { user.UserId, user.Name, user.Email, user.Role }
            };
        }

        public async Task<ApiResponse<object>> RegisterLeaderAsync(RegisterLeaderDTO dto)
        {
            var existing = await _repo.GetOneAsync(u => u.Email == dto.Email);
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password)||
                string.IsNullOrWhiteSpace(dto.Qualification))

            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "All fields are required."
                };
            }


            if (existing != null)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "Email already exist.Please use a different one"
                };
            }
            var hashedPassword = _hash.HashPassword(dto.Password);


            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = hashedPassword,
                Role = UserRole.TeamLead,
                ProfileImg = dto.ProfileImg,
                Qualification = dto.Qualification
            };

            await _repo.AddAsync(user);
            await _repo.SaveAsync();

            return new ApiResponse<object>
            {
                Success = true,
                Message = "You’re in! Let’s get started.",
                Data = new { user.UserId, user.Name, user.Email, user.Qualification, user.Role }
            };
        }

        public async Task<ApiResponse<AuthResponseDTO>> LoginAsync(LoginDTO login)
        {
            var user = await _repo.GetOneAsync(u => u.Email == login.Email);
            if (user == null)
            {
                return new ApiResponse<AuthResponseDTO>
                {
                    Success = false,
                    Message = "Invalid Email"
                };
            }

            if (!_hash.verifyPassword(user.Password, login.Password))
            {
                return new ApiResponse<AuthResponseDTO>
                {
                    Success = false,
                    Message = "Invalid Password"
                };
            }
            if (user.IsActive == false)
            {
                return new ApiResponse<AuthResponseDTO>
                {
                    Success = false,
                    Message = "User is Blocked.Contact admin"
                };
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
            return new ApiResponse<AuthResponseDTO>
            {
                Success = true,
                Message = "Login successfully",
                Data = new AuthResponseDTO
                {
                    AccessToken = token,
                    RefreshToken = refresh
                }
            };


        }
        public async Task<ApiResponse<string>> LogoutAsync(string refreshToken)
        {
            var token = await _refresh.GetOneAsync(t => t.Token == refreshToken);
            if (token == null || token.IsRevoked)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Logout failed"
                };
            }
            token.RevokedAt = DateTime.Now;
            token.IsRevoked = true;
            await _refresh.UpdateAsync(token);
            await _refresh.SaveAsync();
            return new ApiResponse<string>
            {
                Success = true,
                Message = "Logout completed"
            };
             
        }


        public async Task<ApiResponse<AuthResponseDTO>> RefreshTokenAsync(string refreshToken)
        {
            var refresh = await _refresh.GetOneAsync(r => r.Token == refreshToken);
            if(refresh == null || refresh.IsRevoked || refresh.ExpiresAt < DateTime.UtcNow)
            {
                return new ApiResponse<AuthResponseDTO>
                {
                    Success = false,
                    Message = "Invalid or expired refresh token"
                };
            }

            

            var user=await _repo.GetByIdAsync(refresh.UserId);

            if (user == null)
            {
                return new ApiResponse<AuthResponseDTO>
                {
                    Success = false,
                    Message = "Usser not found"
                };
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
                ExpiresAt = DateTime.Now.AddDays(7)
            };

            await _refresh.AddAsync(newRefreshEntity);
            await _refresh.SaveAsync();

            return new ApiResponse<AuthResponseDTO>
            {
                Success = true,
                Message = "Token refreshed succesfully",
                Data = new AuthResponseDTO
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                }
            };
        }
        //public async Task ForgetPasswordAsync(ForgetPasswordDTO fp)
        //{

        //}

    }
}
