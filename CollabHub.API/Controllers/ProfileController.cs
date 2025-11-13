using Asp.Versioning;
using CollabHub.Application.DTO;
using CollabHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollabHub.WebAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [Authorize]
    public class ProfileController:ControllerBase
    {
        private readonly IProfileService _service;
        public ProfileController(IProfileService service)
        {
            _service= service;
        }

        private int GetId()
        {
            var claimValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(claimValue))
                throw new UnauthorizedAccessException("User ID not found in token");

            if (!int.TryParse(claimValue, out var userId))
                throw new UnauthorizedAccessException("Invalid User ID format in token");

            return userId;
        }
        [HttpGet]
        public async Task<IActionResult> VieMyProfile()
        {
            var userId = GetId();
            var result = await _service.GetProfileAsync(userId);
            return Ok(result);

        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody]UpdateProfileDTO dto)
        {
            var userId = GetId();
            var result=await _service.UpdateProfileAsync(userId, dto);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordDTO dto)
        {
            var userId = GetId();
            var result=await _service.ChangePasswordAsync(userId, dto);
            return Ok(result);
        }
    }
}
