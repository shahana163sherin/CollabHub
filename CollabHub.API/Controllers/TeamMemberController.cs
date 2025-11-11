using Asp.Versioning;
using CollabHub.Application.DTO;
using CollabHub.Application.DTO.TeamMember;
using CollabHub.Application.Interfaces.TeamMember;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollabHub.WebAPI.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [ApiVersion("1.0")]
    [Authorize (Roles="Member")]
    public class TeamMemberController:ControllerBase
    {
        private readonly ITeamMemberService _service;
        public TeamMemberController(ITeamMemberService service)
        {
            _service = service;
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

        [HttpPost]
        public async Task <IActionResult> JoinTeam([FromBody]JoinRequestDTO dto)
        {
            var memberId = GetId();
            var result=await _service.JoinTeamAsync(dto, memberId);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> LeaveTeam()
        {
            var memberId = GetId();
            var result=await _service.LeaveTeamAsync(memberId);
            if(!result.Success)return BadRequest(result.Message);
            return Ok(result);
        }
    }
}
