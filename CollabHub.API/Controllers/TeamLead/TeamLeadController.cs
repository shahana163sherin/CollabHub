using Asp.Versioning;
using CollabHub.Application.DTO.TeamLead;
using CollabHub.Application.Interfaces.TeamLead;
using CollabHub.Domain.Entities;
using CollabHub.Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollabHub.WebAPI.Controllers.TeamLead
{
    [ApiController]
   
    [Route("api/[controller]/[action]")]
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
                throw new UnauthorizedAccessException("Token does not contain a valid user identifier");

            if (!int.TryParse(claimValue, out var userId))
                throw new UnauthorizedAccessException("User ID in token is not a valid number");

            return userId;
        }


        [HttpPost]
        public async Task<IActionResult>CreateTeam([FromBody]CreateTeamDTO dto)
        {
            var leaderId=GetId();
            var result = await _service.CreateTeamAsync(dto, leaderId);
            return StatusCode(result.StatusCode, result);

        }

        [HttpPatch]
        public async Task<IActionResult> UpdateTeam(UpdateTeamDTO dto)
        {
            var leadid = GetId();
         
                var result = await _service.UpdateTeamAsync(dto, leadid);
                return StatusCode(result.StatusCode, result);

        }

      [HttpDelete]
        public async Task<IActionResult> RemoveTeam([FromQuery]int TeamId)
        {
            var teamLeadId= GetId();
            var response = await _service.RemoveTeamAsync(TeamId, teamLeadId);
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> ApproveOrRejectMember([FromBody] ApproveRejectMemberDTO dto)
        {
            var TeamLeadId=GetId();
            var result = await _service.ApproveOrRejectMemberAsync(dto, TeamLeadId);
            return StatusCode(result.StatusCode, result);

        }

        //[HttpPut]
        //public async Task<IActionResult> RejectMember([FromBody]RejectMemberDTO dto)
        //{
        //    var TeamLeadId=GetId();
        //    var result = await _service.RejectMemberAsync(dto, TeamLeadId);
        //    return StatusCode(result.StatusCode, result);
        //}

        [HttpGet]
        public async Task <IActionResult>GetTeamMembers([FromQuery]TeamMemberFilterDTO dto)
        {
            var teamLeadId = GetId();
            var result = await _service.GetTeamMembersAsync(dto, teamLeadId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{TeamId}/{MemberId}")]
        public async Task<IActionResult> RemoveMember([FromRoute]int TeamId,int MemberId)
        {
            var leadId = GetId();
            var result = await _service.RemoveMemberAsync(TeamId,MemberId, leadId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet]
        public async Task<IActionResult> ViewMyTeams()
        {
                var teamLeadId = GetId();
                var result= await _service.ViewMyTeamsAsync(teamLeadId);
                return Ok(result);

        }
        [HttpGet]
        public async Task<IActionResult>ViewTeamById(int teamId)
        {
         
                var teamLeadId = GetId();
                var result = await _service.ViewMyOneTeamAsync(teamLeadId, teamId);
            return StatusCode(result.StatusCode, result);
        }
      
    }
}
