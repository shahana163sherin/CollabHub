using CollabHub.Application.DTO.Task;
using CollabHub.Application.DTO.TaskHead;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Interfaces
{
    public interface ITaskHeadDapper
    {
        Task<IEnumerable<TaskHeadDTO>> GetTaskHeadByTeamAsync(TaskHeadFilterDTO dto, int teamLeadId);

    }
}
