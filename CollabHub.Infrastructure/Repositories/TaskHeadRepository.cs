using CollabHub.Application.DTO.Task;
using CollabHub.Application.DTO.TaskHead;
using CollabHub.Application.Interfaces;
using CollabHub.Domain.Entities;
using CollabHub.Infrastructure.Persistence.Data;
using CollabHub.Infrastructure.Repositories.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Infrastructure.Repositories
{
    public class TaskHeadRepository : GenericRepository<TaskHead>, ITaskHeadRepository
    {
        public TaskHeadRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<TaskHead>> GetTaskHeadsAsync()
        {
            return await _context.TaskHeads
                .Include(th => th.Team)
                .Include(th => th.TaskDefinitions)
                .ToListAsync();
        }
    }

}
