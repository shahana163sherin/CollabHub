using CollabHub.Application.Interfaces.TeamLead;
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
    public class TeamRepository:GenericRepository<Team>,ITeamRepository
    {
        public TeamRepository(ApplicationDbContext context):base (context) { }

        public async Task<Team> GetByIdAsync(int id)
        {
            return await _context.Teams
                .Include(t => t.TaskHeads)
                .ThenInclude(th => th.TaskDefinitions)
                .FirstOrDefaultAsync(t => t.TeamId == id);
        }
    }
}
