using CollabHub.Domain.Entities;
using CollabHub.Infrastructure.Repositories.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Interfaces
{
    public interface IGitRepoRepository:IGenericRepository<GitRepository>
    {
        Task<GitRepository> GetByUrlAsync(string url);
       
    }
}
