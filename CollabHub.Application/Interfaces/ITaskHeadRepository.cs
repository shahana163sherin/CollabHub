using CollabHub.Domain.Entities;
using CollabHub.Infrastructure.Repositories.EF;

public interface ITaskHeadRepository : IGenericRepository<TaskHead>
{
    Task<TaskHead> GetByIdTaskAsync(int id);
    Task<IEnumerable<TaskHead>> GetTaskHeadsAsync();
}
