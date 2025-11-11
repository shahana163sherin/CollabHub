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
    [Authorize(Roles = "TeamLead")]
    public class TeamLeadController:ControllerBase
    {
        private readonly ITeamLeadService _service;
        public TeamLeadController(ITeamLeadService service)
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
        public async Task<IActionResult>CreateTeam([FromBody]CreateTeamDTO dto)
        {
            var leaderId=GetId();
            var result = await _service.CreateTeamAsync(dto, leaderId);
            return Ok(result);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateTeam(UpdateTeamDTO dto)
        {
            var leadid = GetId();
            var result = await _service.UpdateTeamAsync(dto, leadid);
            return Ok(result);
        }

      [HttpDelete]
        public async Task<IActionResult> RemoveTeam([FromQuery]int TeamId)
        {
            var teamLeadId= GetId();
            var response = await _service.RemoveTeamAsync(TeamId, teamLeadId);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveMember([FromBody]ApproveMemberDTO dto)
        {
            var TeamLeadId=GetId();
            var response = await _service.ApproveMemberAsync(dto, TeamLeadId);
            return Ok(new { Success = response, Message = response ? "Member Approved" : "Failed" });
        }

        [HttpPut]
        public async Task<IActionResult> RejectMember([FromBody]RejectMemberDTO dto)
        {
            var TeamLeadId=GetId();
            var result = await _service.RejectMemberAsync(dto, TeamLeadId);
            return Ok(new { Success = result, Message = result ? "Member Rejected" : "Failed" });
        }

        [HttpGet]
        public async Task <IActionResult>GetTeamMembers([FromQuery]TeamMemberFilterDTO dto)
        {
            var teamLeadId = GetId();
            var result = await _service.GetTeamMembersAsync(dto, teamLeadId);
            return Ok(result);
        }

        [HttpDelete("{TeamId}/{MemberId}")]
        public async Task<IActionResult> RemoveMember([FromRoute]int TeamId,int MemberId)
        {
            var leadId = GetId();
            var result = await _service.RemoveMemberAsync(TeamId,MemberId, leadId);
            return Ok(new { Success = result, Message = result ? "Member Removed" : "Failed" });
        }

      
    }
}
