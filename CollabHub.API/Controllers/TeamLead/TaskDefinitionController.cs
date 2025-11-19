using Asp.Versioning;
using CollabHub.Application.DTO.TaskDefinition;
using CollabHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollabHub.WebAPI.Controllers.TeamLead
{
    [ApiController]
   
    [Authorize (Roles="TeamLead")]
    [Route("api/[controller]/[action]")]
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

           
                var createdTaskDef = await _def.CreateTaskDefinitionAsync(dto, teamLeadId);
                return StatusCode(createdTaskDef.StatusCode, createdTaskDef);
            
        }
        [HttpPut("{taskDefinitionId}")]
        public async Task<IActionResult> UpdateTaskDefinition([FromRoute]int taskDefinitionId,[FromBody]UpdateTaskDefinitionDTO dto)
        {
            var teamLeadId = GetId();
            var result = await _def.UpdateTaskDefinitionAsync(dto, teamLeadId, taskDefinitionId);
            return StatusCode(result.StatusCode, result);
           
        }

        [HttpDelete("{taskDefinitionId}")]
        public async Task<IActionResult>DeleteTask([FromRoute]int taskDefinitionId)
        {
            var teamleadId = GetId();
            var response = await _def.DeleteTaskDefinitionAsync(taskDefinitionId, teamleadId);
           
            return StatusCode(response.StatusCode,response);
        }

        [HttpPut("{taskId}/{memberId}")]
        public async Task<IActionResult> AssignTask([FromRoute]int taskId, [FromRoute] int memberId)
        {
            var teamLeadId = GetId();
            var result = await _def.AssignMemberAsync(taskId, memberId, teamLeadId);

            return StatusCode(result.StatusCode, result);

        }
        [HttpDelete("{taskDefinitionId}/{memberId}")]

        public async Task<IActionResult>RemoveMember([FromRoute]int taskDefinitionId,[FromRoute]int memberId)
        {
            var teamLeadId = GetId();
            var result = await _def.RemoveMemberAsync(taskDefinitionId, memberId, teamLeadId);

            return StatusCode(result.StatusCode, result);
        }
        [HttpGet]
        public async Task<IActionResult>GetTaskDefinitionById(int taskDefinitionId)
        {
            var teamLeadId= GetId();
           
                var result = await _def.GetTaskDefinitionById(taskDefinitionId, teamLeadId);
                return StatusCode(result.StatusCode, result);


        }

        [HttpGet]
        public async Task<IActionResult>GetAllTaskDefinition(int taskHeadId)
        {
            var teamLead= GetId();
           
                var result = await _def.GetAllTaskDefinition(taskHeadId, teamLead);

                return StatusCode(result.StatusCode, result);


        }
    }
}
