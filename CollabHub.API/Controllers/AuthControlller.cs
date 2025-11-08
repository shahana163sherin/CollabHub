using Asp.Versioning;
using CollabHub.Application.DTO.Auth;
using CollabHub.Application.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc;

namespace CollabHub.WebAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterTeamLead([FromBody] RegisterLeaderDTO dto)
        {

            var result = await _auth.RegisterLeaderAsync(dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterMember([FromBody] RegisterDTO dto)
        {
            var result = await _auth.RegisterMemberAsync(dto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginDTO dto)
        {
            var login = await _auth.LoginAsync(dto);
            if (!login.Success)
            {
                return Unauthorized(login);
            }
            return Ok(login);
        }
        [HttpPost]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] string refreshToken)
        {
            var refresh = await _auth.RefreshTokenAsync(refreshToken);
            if (!refresh.Success)
            {
                return Unauthorized(refresh);
            }
            return Ok(refresh);
        }


        [HttpPost]
        public async Task <IActionResult> LogoutAsync([FromBody] string refreshToken)
        {
            var result=await _auth.LogoutAsync(refreshToken);
            if(!result.Success)return BadRequest(result);
            return Ok(result);
        }
    }
}
