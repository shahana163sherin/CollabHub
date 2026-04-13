using CollabHub.Domain.Entities;
using CollabHub.Infrastructure.Repositories.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Interfaces.Git
{
    public interface IGitActivityRepository:IGenericRepository<GitActivity>
    {
        Task<IEnumerable<GitActivity>> GetByTaskIdAsync(int taskId);
        Task<IEnumerable<GitActivity>> GetByUserIdAsync(int userId);
        Task<IEnumerable<GitActivity>> GetByRepositoryIdAsync(int repoId);
        Task<GitActivity> GetLatestByTaskAndUserAsync(int taskId, int userId);
    }
}
