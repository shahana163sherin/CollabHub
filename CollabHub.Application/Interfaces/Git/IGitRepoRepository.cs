using CollabHub.Domain.Entities;
using CollabHub.Infrastructure.Repositories.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Interfaces.Git
{
    public interface IGitRepoRepository:IGenericRepository<GitRepository>
    {
        Task<GitRepository> GetRepositoryWithActivitiesAsync(int repositoryId);
        Task<GitRepository> GetByRepoUrlAsync(string url);
        Task<GitRepository> GetUserAndRepoName(int userId, string repoName);

       
    }
}
