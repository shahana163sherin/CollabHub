using Asp.Versioning;
using CollabHub.Application.DTO;
using CollabHub.Application.DTO.Auth;
using CollabHub.Application.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc;

namespace CollabHub.WebAPI.Controllers
{
    [ApiController]
 
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }
       

        [HttpPost]
        public async Task<IActionResult> RegisterTeamLead([FromForm] RegisterLeaderDTO dto)
        {
            
            var result = await _auth.RegisterLeaderAsync(dto);
            
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterMember([FromForm] RegisterDTO dto)
        {
            var result = await _auth.RegisterMemberAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginDTO dto)
        {
            var result = await _auth.LoginAsync(dto);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] string refreshToken)
        {
            var refresh = await _auth.RefreshTokenAsync(refreshToken);
            return StatusCode(refresh.StatusCode, refresh);
        }


        [HttpPost]
        public async Task <IActionResult> LogoutAsync([FromBody] string refreshToken)
        {
            var result=await _auth.LogoutAsync(refreshToken);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost]
        public async Task<IActionResult>ResetPassword([FromBody]ResetPasswordDTO dto)
        {
            var result=await _auth.ResetPasswordAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword([FromBody]ForgetPasswordDTO dto)
        {
            var result=await _auth.ForgotPasswordAsync(dto);
            return StatusCode(result.StatusCode, result);
        }
    }
}
