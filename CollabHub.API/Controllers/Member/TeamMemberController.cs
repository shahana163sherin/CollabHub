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

        [HttpGet]
        public async Task<IActionResult>ViewMyTeam()
        {
            try
            {
                var userId = GetId();
                var result = await _service.ViewMyTeamsAsync(userId);
                return Ok(result);
            }
           
             catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult>ViewTeamById(int teamId)
        {
            try
            {
                var userId = GetId();
                var result = await _service.ViewTeamById(teamId, userId);
                return Ok(result);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult>ViewTaskHead(int teamId)
        {
            var memberId= GetId();
            try
            {
                var taskHeads = await _service.GetTasksByTeamAsync(teamId, memberId);
                return Ok(taskHeads);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpGet]
        public async Task <IActionResult>ViewMyAssignedTask()
        {
            var memberId=GetId();
            try
            {
                var assignedTasks = await _service.ViewMyAssignedTask(memberId);
                return Ok(assignedTasks);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        
    }
}
