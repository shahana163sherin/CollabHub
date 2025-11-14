using Asp.Versioning;
using CollabHub.Application.DTO.TaskDefinition;
using CollabHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollabHub.WebAPI.Controllers.TeamLead
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize (Roles="TeamLead")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class TaskDefinitionController:ControllerBase
    {
        private readonly ITaskDefinition _def;
        public TaskDefinitionController(ITaskDefinition def)
        {
            _def = def;
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
        public async Task<IActionResult> CreateTaskDefinition(CreateTaskDefinitionDTO dto)
        {
            var teamLeadId = GetId();

            try
            {
                var createdTaskDef = await _def.CreateTaskDefinitionAsync(dto, teamLeadId);
                return Ok(createdTaskDef); 
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpPut("{taskDefinitionId}")]
        public async Task<IActionResult> UpdateTaskDefinition([FromRoute]int taskDefinitionId,[FromBody]UpdateTaskDefinitionDTO dto)
        {
            var teamLeadId = GetId();
            var result = await _def.UpdateTaskDefinitionAsync(dto, teamLeadId, taskDefinitionId);
            if (!result)
            {
                return NotFound(new { message = "You are not authorized to update or task not found" });
            }
            return Ok(new { message = "Task updated successfully" });
        }

        [HttpDelete("{taskDefinitionId}")]
        public async Task<IActionResult>DeleteTask([FromRoute]int taskDefinitionId)
        {
            var teamleadId = GetId();
            var response = await _def.DeleteTaskDefinitionAsync(taskDefinitionId, teamleadId);
            if (!response)
            {
                return NotFound(new { message = "You are not authorized to delete or task not found" });
            }
            return NoContent();
        }

        [HttpPut("{taskId}/{memberId}")]
        public async Task<IActionResult> AssignTask([FromRoute]int taskId, [FromRoute] int memberId)
        {
            var teamLeadId = GetId();
            var result = await _def.AssignMemberAsync(taskId, memberId, teamLeadId);
            if (!result)
            {
                return NotFound(new { message = "You are not authorized to update or task not found" });
            }
            return Ok(new { message = "Member assigned successfully" });

        }
        [HttpDelete("{taskDefinitionId}/{memberId}")]

        public async Task<IActionResult>RemoveMember([FromRoute]int taskDefinitionId,[FromRoute]int memberId)
        {
            var teamLeadId = GetId();
            var result = await _def.RemoveMemberAsync(taskDefinitionId, memberId, teamLeadId);
            if (!result)
            {
                return NotFound(new { message = "You are not authorized to update or task not found" });
            }
            return NoContent();
        }
        [HttpGet]
        public async Task<IActionResult>GetTaskDefinitionById(int taskDefinitionId)
        {
            var teamLeadId= GetId();
            try
            {
                var result = await _def.GetTaskDefinitionById(taskDefinitionId, teamLeadId);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult>GetAllTaskDefinition(int taskHeadId)
        {
            var teamLead= GetId();
            try
            {
                var result = await _def.GetAllTaskDefinition(taskHeadId, teamLead);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }
    }
}
