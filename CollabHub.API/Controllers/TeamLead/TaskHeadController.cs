using Asp.Versioning;
using CollabHub.Application.DTO.Task;
using CollabHub.Application.DTO.TaskHead;
using CollabHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollabHub.WebAPI.Controllers.TeamLead
{
    [ApiController]
    
    [Route("api/[controller]/[action]")]
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
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("{taskHeadId}")]
        public async Task<IActionResult> UpdateTask(int taskHeadId, [FromBody] UpdateTaskHeadDTO dto)
        {
            var teamLeadId = GetId();
            var result = await _service.UpdateTaskAsync(taskHeadId, dto, teamLeadId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpDelete("{taskHeadId}")]
        public async Task<IActionResult> DeleteTask([FromRoute] int taskHeadId)
        {
            var teamLeadId = GetId();

                var result = await _service.DeleteTaskAsync(teamLeadId, taskHeadId);

            return StatusCode(result.StatusCode, result);
        }
        [HttpGet]
        public async Task<IActionResult>GetAllTaskHead([FromQuery]TaskHeadFilterDTO dto)
        {
            var teamLeadId= GetId();
            var result = await _service.GetAllTaskAsync(dto, teamLeadId);
            return StatusCode(result.StatusCode, result);

        }
        [HttpGet("{taskHeadId}")]
        public async Task<IActionResult> GetTaskById([FromRoute]int taskHeadId)
        {
            var teamLeadId = GetId();
          
                var result = await _service.GetTaskHeadByIdAsync( teamLeadId,taskHeadId);
            return StatusCode(result.StatusCode, result);
        }
            

    }
}
