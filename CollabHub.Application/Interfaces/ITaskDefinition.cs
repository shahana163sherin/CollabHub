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
        Task<TaskDefinitionDTO> CreateTaskDefinitionAsync(CreateTaskDefinitionDTO dto, int teamLeadId);
        Task<bool>UpdateTaskDefinitionAsync(UpdateTaskDefinitionDTO dto,int teamLeadId,int taskDefinitionId);
        Task<bool>DeleteTaskDefinitionAsync(int taskDefinitionId,int teamLeadId);
        Task<bool> AssignMemberAsync(int taskId, int teamMemberId, int teamLeadId);
        Task<bool> RemoveMemberAsync(int taskDefinitionId, int memberId, int teamLeadId);

    }
}
