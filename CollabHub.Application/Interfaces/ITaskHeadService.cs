using CollabHub.Application.DTO;
using CollabHub.Application.DTO.Task;
using CollabHub.Application.DTO.TaskHead;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Interfaces
{
    public interface ITaskHeadService
    {
        Task<ApiResponse<TaskHeadDTO>> CreateTaskAsync(CreateTaskHeadDTO dto, int teamLeadId);
        Task<TaskHeadDTO> UpdateTaskAsync(int taskHeadId,UpdateTaskHeadDTO dto, int teamLeadId);
        Task<bool>DeleteTaskAsync(int teamLeadId, int taskHeadId);
        Task <TaskHeadDTO> GetTaskHeadByIdAsync(int teamLeadId, int taskHeadId);
        Task<IEnumerable<TaskHeadDTO>>GetAllTaskAsync(TaskHeadFilterDTO dto,int teamLeadId);
    }
}
