using CollabHub.Application.DTO.Task;
using CollabHub.Application.DTO.TaskHead;
using CollabHub.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Infrastructure.Repositories
{
    public class TaskHeadDapper : ITaskHeadDapper
    {
        public Task<IEnumerable<TaskHeadDTO>> GetTaskHeadByTeamAsync(TaskHeadFilterDTO dto, int teamLeadId)
        {
            throw new NotImplementedException();
        }
    }
}
