using CollabHub.Application.Interfaces.Git;
using CollabHub.Domain.Entities;
using CollabHub.Infrastructure.Repositories.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Services.Git
{
    public class TaskStatusService : ITaskStatusService
    {
        private readonly IGenericRepository<TaskDefinition> _taskDefRepo;
        private readonly IGenericRepository<TaskHead> _taskHeadRepo;

        public TaskStatusService(
            IGenericRepository<TaskDefinition> taskDefRepo,
            IGenericRepository<TaskHead> taskHeadRepo)
        {
            _taskDefRepo = taskDefRepo;
            _taskHeadRepo = taskHeadRepo;
        }

        public async Task UpdateSubTasksFromActivityAsync(int taskDefinitionId, GitActivity activity)
        {
            var subTask = await _taskDefRepo.GetOneAsync(t => t.TaskDefinitionId == taskDefinitionId);
            if (subTask == null) return;

            subTask.Status = Domain.Enum.TaskStatus.Completed;
            await _taskDefRepo.SaveAsync();

            if (subTask.TaskHeadId !=0)
                await UpdateMainTaskIfCompleted(subTask.TaskHeadId);
        }

        public async Task UpdateMainTaskIfCompleted(int taskHeadId)
        {
            var allSubComplete = await AreAllSubCompleteAsync(taskHeadId);
            if (!allSubComplete) return;

            var headTask = await _taskHeadRepo.GetOneAsync(h => h.TaskHeadId == taskHeadId);
            if (headTask == null) return;

            headTask.Status = Domain.Enum.TaskStatus.Completed;
            await _taskHeadRepo.SaveAsync();
        }

        public async Task<bool> AreAllSubCompleteAsync(int taskHeadId)
        {
            var subtasks = await _taskDefRepo.GetByConditionAsync(t => t.TaskHeadId == taskHeadId);
            return subtasks.All(t => t.Status == Domain.Enum.TaskStatus.Completed);
        }
    }
}
