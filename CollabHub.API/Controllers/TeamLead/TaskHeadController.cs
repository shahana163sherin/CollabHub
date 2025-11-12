using Asp.Versioning;
using CollabHub.Application.DTO.Task;
using CollabHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollabHub.WebAPI.Controllers.TeamLead
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [Authorize(Roles = "TeamLead")]
    public class TaskHeadController : ControllerBase
    {
        private readonly ITaskHeadService _service;
        public TaskHeadController(ITaskHeadService service)
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
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskHeadDTO dto)
        {
            var teamLeadId = GetId();
            var result = await _service.CreateTaskAsync(dto, teamLeadId);
            if (result == null) return BadRequest("Failed to create");
            return StatusCode(201, result);
        }

        [HttpPatch("{taskHeadId}")]
        public async Task<IActionResult> UpdateTask(int taskHeadId, [FromBody] UpdateTaskHeadDTO dto)
        {
            var teamLeadId = GetId();
            var result = await _service.UpdateTaskAsync(taskHeadId, dto, teamLeadId);
            return Ok(result);
        }

        [HttpDelete("{taskHeadId}")]

        public async Task <IActionResult>DeleteTask([FromRoute]int taskHeadId)
        {
            var teamLeadId = GetId();
            var result=await _service.DeleteTaskAsync(teamLeadId, taskHeadId);
            if (!result) return NotFound($"Task {taskHeadId} not found or unauthorized");
            return NoContent();
        }
    }
}
