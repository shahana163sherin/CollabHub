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
    public class GitRepoRepository:GenericRepository<GitRepository>, IGitRepoRepository
    {
        public GitRepoRepository(ApplicationDbContext context) : base(context) { }

      

        public async Task<GitRepository> GetByUrlAsync(string url)
        {
            return await _context.GitRepositories
               .Include(r => r.User)
               .Include(r => r.TaskHead)
               .Include(r => r.GitActivities)
               .FirstOrDefaultAsync(r => r.RepoUrl.ToLower() == url.ToLower());
           
        }
    }
}
