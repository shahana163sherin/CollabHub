using CollabHub.Application.DTO;
using CollabHub.Application.DTO.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Interfaces
{
    public interface ITaskHeadService
    {
        Task<TaskHeadDTO> CreateTaskAsync(CreateTaskHeadDTO dto, int teamLeadId);
        Task<TaskHeadDTO> UpdateTaskAsync(UpdateTaskHeadDTO dto, int teamLeadId);
        Task<bool>DeleteTaskAsync(int teamLeadId);
    }
}
