using Asp.Versioning;
using AutoMapper.Execution;
using CollabHub.Application.DTO;
using CollabHub.Application.DTO.TeamMember;
using CollabHub.Application.Interfaces.TeamMember;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollabHub.WebAPI.Controllers.Member
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    
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
            //if (!result.Success) return BadRequest(result.Message);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete]
        public async Task<IActionResult> LeaveTeam()
        {
            var memberId = GetId();
            var result=await _service.LeaveTeamAsync(memberId);
            //if(!result.Success)return BadRequest(result.Message);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        public async Task<IActionResult>ViewMyTeam()
        {
           
                var userId = GetId();
                var result = await _service.ViewMyTeamsAsync(userId);
                 return StatusCode(result.StatusCode, result);

        }
        [HttpGet]
        public async Task<IActionResult>ViewTeamById(int teamId)
        {
           
                var userId = GetId();
                var result = await _service.ViewTeamById(teamId, userId);
                return StatusCode(result.StatusCode, result);
          
           
        }
        [HttpGet]
        public async Task<IActionResult>ViewTaskHead(int teamId)
        {
            var memberId= GetId();
           
                var result = await _service.GetTasksByTeamAsync(teamId, memberId);
                return StatusCode(result.StatusCode, result);
           
        }

        [HttpGet]
        public async Task <IActionResult>ViewMyAssignedTask()
        {
            var memberId=GetId();
            
                var result = await _service.ViewMyAssignedTask(memberId);
            return StatusCode(result.StatusCode, result);

        }
        
    }
}
