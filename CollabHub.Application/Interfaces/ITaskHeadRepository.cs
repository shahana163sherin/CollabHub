using CollabHub.Domain.Entities;
using CollabHub.Infrastructure.Repositories.EF;

public interface ITaskHeadRepository : IGenericRepository<TaskHead>
{
    Task<IEnumerable<TaskHead>> GetTaskHeadsAsync();
}
