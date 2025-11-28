using CollabHub.Domain.Entities;
using CollabHub.Domain.Enum;
using CollabHub.Infrastructure.Repositories.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Interfaces.Git
{
    public interface IGitActivityRepository : IGenericRepository<GitActivity>
    {
        Task<IEnumerable<GitActivity>> GetActivitiesByTaskIdAsync(int taskDefinitionId);
        Task<IEnumerable<GitActivity>> GetActivityByRepositoryIdAsync(int repositoryId);
        Task<GitActivity>GetLatestActivityByTaskAsync(int taskDefinitionId);
        Task<IEnumerable<GitActivity>> GetByEventTypeAsync(GitEventType eventType);
        Task<GitActivity?> GetActivityWithAllAsync(int activityId);
        Task<List<GitActivity>> GetActivitiesForRepoAsync(int repoId);  

    }
}
