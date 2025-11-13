using CollabHub.Application.DTO;
using CollabHub.Application.DTO.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<ApiResponse<object>> RegisterMemberAsync(RegisterDTO dto);
        Task<ApiResponse<object>> RegisterLeaderAsync(RegisterLeaderDTO dto);
        Task<ApiResponse<AuthResponseDTO>> LoginAsync(LoginDTO login);
        Task<ApiResponse<string>> LogoutAsync(string refreshToken);
        Task<ApiResponse<AuthResponseDTO>> RefreshTokenAsync(string refreshToken);
        Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDTO dto);
        Task<ApiResponse<string>> ForgotPasswordAsync(ForgetPasswordDTO fp);

    }
}
