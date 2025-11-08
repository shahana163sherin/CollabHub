using Asp.Versioning;
using CollabHub.Application.DTO.TeamLead;
using CollabHub.Application.Interfaces.TeamLead;
using CollabHub.Domain.Entities;
using CollabHub.Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollabHub.WebAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [Authorize(Roles ="TeamLead")]
    public class TeamLeadController:ControllerBase
    {
        private readonly ITeamLeadService _service;
        public TeamLeadController(ITeamLeadService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult>CreateTeam([FromBody]CreateTeamDTO dto)
        {
            var leaderClaimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(leaderClaimId)) {

                return Unauthorized(leaderClaimId);
            }
            var leaderId=int.Parse(leaderClaimId);
            var result = await _service.CreateTeamAsync(dto, leaderId);
            return Ok(result);
        }
    }
}
