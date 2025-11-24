using CollabHub.Domain.Entities;
using CollabHub.Infrastructure.Repositories.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Interfaces
{
    public interface IGitActivityRepository : IGenericRepository<GitActivity>
    {
        Task<GitActivity> GetByCommitHashAsync(string commitHash);
        Task<IEnumerable<GitActivity>> GetActivitiesByTaskIdAsync(int taskDefinitionId);
        Task<List<GitActivity>> GetByRepositoryIdAsync(int repositoryId);
        Task<List<GitActivity>> GetByUserIdAsync(int userId);

    }
}
