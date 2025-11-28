using CollabHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Interfaces.Git
{
    public interface ITaskStatusService
    {

        Task UpdateSubTasksFromActivityAsync(int taskDefinitionId, GitActivity activity);
        Task UpdateMainTaskIfCompleted(int tskHeadId);
        Task<bool> AreAllSubCompleteAsync(int taskHeadId);
    }
}
