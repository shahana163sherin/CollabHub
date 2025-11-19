using CollabHub.Application.DTO;
using CollabHub.Application.DTO.TaskDefinition;
using CollabHub.Application.DTO.TaskDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Interfaces
{
    public interface ITaskDefinition
    {
        Task<ApiResponse<TaskDefinitionDTO>> CreateTaskDefinitionAsync(CreateTaskDefinitionDTO dto, int teamLeadId);
        Task<ApiResponse<bool>>UpdateTaskDefinitionAsync(UpdateTaskDefinitionDTO dto,int teamLeadId,int taskDefinitionId);
        Task<ApiResponse<bool>> DeleteTaskDefinitionAsync(int taskDefinitionId,int teamLeadId);
        Task<ApiResponse<bool>> AssignMemberAsync(int taskId, int teamMemberId, int teamLeadId);
        Task<ApiResponse<bool>> RemoveMemberAsync(int taskDefinitionId, int memberId, int teamLeadId);
        Task <ApiResponse<TaskDefinitionDTO>> GetTaskDefinitionById(int taskDefinitionId,int teamLeadId);
        Task<ApiResponse<IEnumerable<TaskDefinitionDTO>>> GetAllTaskDefinition(int taskHeadId, int teamLeadId);


    }
}
