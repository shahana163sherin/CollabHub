using CollabHub.Application.Interfaces;
using CollabHub.Application.Interfaces.Git;
using CollabHub.Domain.Entities;
using CollabHub.Domain.Enum;
using CollabHub.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Infrastructure.Repositories.EF
{
    public class GitActivityRepository : GenericRepository<GitActivity>, IGitActivityRepository
    {
        public GitActivityRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<GitActivity>> GetActivitiesByTaskIdAsync(int taskDefinitionId)
        {
            return await GetByConditionAsync(a=>a.TaskDefinitionId == taskDefinitionId);
        }

        public async Task<List<GitActivity>> GetActivitiesForRepoAsync(int repoId)
        {
            return await _context.GitActivities
                 .Where(a => a.RepositoryId == repoId)
                 .Include(a => a.TaskDefinition)
                 .Include(a => a.User)
                 .ToListAsync();
        }

        public async Task<IEnumerable<GitActivity>> GetActivityByRepositoryIdAsync(int repositoryId)
        {
            return await GetByConditionAsync(a => a.RepositoryId == repositoryId);
        }

        public async Task<GitActivity?> GetActivityWithAllAsync(int activityId)
        {
            return await _context.GitActivities
                .Include(a=>a.Repository)
                .Include(a=>a.User)
                .Include(a=>a.TaskDefinition)
                .ThenInclude(td=>td.TaskHead)
                .FirstOrDefaultAsync(a=>a.GitActivityId == activityId);

           
        }

        public async Task<IEnumerable<GitActivity>> GetByEventTypeAsync(GitEventType eventType)
        {
            return await GetByConditionAsync(a=>a.EventType == eventType);
        }

        public async Task<GitActivity> GetLatestActivityByTaskAsync(int taskDefinitionId)
        {
            var activities = await GetByConditionAsync(a => a.TaskDefinitionId == taskDefinitionId);

            return activities.OrderByDescending(a => a.CommittedAt).FirstOrDefault();
        }
    }
}
