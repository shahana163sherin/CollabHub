using CollabHub.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Interfaces
{
    public interface IProfileService
    {
        Task<ApiResponse<object>> GetProfileAsync(int userId);
        Task<ApiResponse<string>> UpdateProfileAsync(int userId, UpdateProfileDTO dto);
        Task<ApiResponse<string>> ChangePasswordAsync(int userId, ChangePasswordDTO dto);
    }
}
