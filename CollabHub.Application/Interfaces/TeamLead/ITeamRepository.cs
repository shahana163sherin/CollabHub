using CollabHub.Domain.Entities;
using CollabHub.Infrastructure.Repositories.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Interfaces.TeamLead
{
    public interface ITeamRepository:IGenericRepository<Team>
    {
        Task<Team> GetByIdAsync(int id);
    }
}
