using CollabHub.Application.Interfaces;
using CollabHub.Domain.Entities;
using CollabHub.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Infrastructure.Repositories.EF
{
    public class GitActivityRepository : GenericRepository<GitActivity>, IGitActivityRepository
    {
        public GitActivityRepository(ApplicationDbContext context) : base(context) { }

        public async Task<GitActivity> GetByCommitHashAsync(string commitHash)
        {
            return await _context.GitActivities
                 .Include(a => a.User)
                 .Include(a => a.Repository)
                 .ThenInclude(a => a.TaskHead)
                 .Include(th => th.TaskDefinition)
                 .FirstOrDefaultAsync(a => a.CommitHash == commitHash);
        }

        public async Task<List<GitActivity>> GetByTaskDefinitionIdAsync(int taskDefinitionId)
        {
            return await _context.GitActivities
                .Include(a => a.User)
                .Include(a => a.Repository)
                .Where(a => a.TaskDefinitionId == taskDefinitionId)
                .OrderByDescending(a => a.CommittedAt)
                .ToListAsync();
        }


    }
}
