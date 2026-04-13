using CollabHub.Application.Interfaces.Git;
using CollabHub.Domain.Entities;
using CollabHub.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollabHub.Infrastructure.Repositories.EF
{
    public class GitActivityRepository : GenericRepository<GitActivity>, IGitActivityRepository
    {
        public GitActivityRepository(ApplicationDbContext context) : base(context) { }


        public async Task<IEnumerable<GitActivity>> GetByTaskIdAsync(int taskId)
        {
            return await _context.GitActivities
                .Where(g => g.TaskDefinitionId == taskId)
                .Include(g => g.User)
                .Include(g => g.Repository)
                .ToListAsync();
        }

        public async Task<IEnumerable<GitActivity>> GetByRepositoryIdAsync(int repoId)
        {
            return await _context.GitActivities
                .Where(g => g.RepositoryId == repoId)
                .Include(g => g.User)
                .Include(g => g.TaskDefinition)
                .ToListAsync();
        }

        public async Task<IEnumerable<GitActivity>> GetByUserIdAsync(int userId)
        {
            return await _context.GitActivities
                .Where(g => g.UserId == userId)
                .Include(g => g.Repository)
                .Include(g => g.TaskDefinition)
                .ToListAsync();
        }

        public async Task<GitActivity> GetLatestByTaskAndUserAsync(int taskId, int userId)
        {
            return await _context.GitActivities
                .Where(g => g.TaskDefinitionId == taskId && g.UserId == userId)
                .OrderByDescending(g => g.CommittedAt)
                .FirstOrDefaultAsync();
        }
    }
}
