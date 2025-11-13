using AutoMapper;
using CollabHub.Application.DTO;
using CollabHub.Application.DTO.Task;
using CollabHub.Application.DTO.TaskHead;
using CollabHub.Application.Interfaces;
using CollabHub.Domain.Entities;
using CollabHub.Domain.Enum;
using CollabHub.Infrastructure.Repositories.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Services
{
    public class TaskHeadService : ITaskHeadService
    {
        private readonly IGenericRepository<TaskHead> _taskHead;
        private readonly IGenericRepository<Team> _team;
        private readonly ITaskHeadRepository _taskHeadRepository;
        private readonly IMapper _mapper;

        public TaskHeadService(IGenericRepository<TaskHead> taskHead, IGenericRepository<Team> team, IMapper mapper, ITaskHeadRepository taskHeadRepository)
        {
            _taskHead = taskHead;
            _team = team;
            _mapper = mapper;
            _taskHeadRepository = taskHeadRepository;
        }

        public async Task<ApiResponse<TaskHeadDTO>> CreateTaskAsync(CreateTaskHeadDTO dto, int teamLeadId)
        {
            try
            {


                var team = await _team.GetByIdAsync(dto.TeamId);
                if (team == null) return new ApiResponse<TaskHeadDTO>
                {
                    Success = false,
                    Message = "Team is not exist"
                };

                if (team.CreatedBy != teamLeadId) return new ApiResponse<TaskHeadDTO>
                {
                    Success = false,
                    Message = "You are not authorized to create the task"
                };
                var taskHead = _mapper.Map<TaskHead>(dto);
                taskHead.CreatedBy = teamLeadId;
                taskHead.CreatedOn=DateTime.Now;
                taskHead.Status = Domain.Enum.TaskStatus.Pending;
                taskHead.StartDate = dto.StartDate ?? DateTime.Now;
                taskHead.Title = dto.Title;

                

                if (dto.DueDate < taskHead.StartDate)
                    throw new ArgumentException("Due date cannot be before start date.");

                if (dto.ExpectedEndDate > dto.DueDate)
                    throw new ArgumentException("Expected end date cannot be after due date.");

                await _taskHead.AddAsync(taskHead);
                await _taskHead.SaveAsync();

                var mapped = _mapper.Map<TaskHeadDTO>(taskHead);
                mapped.TeamName = team.TeamName;
                return new ApiResponse<TaskHeadDTO>
                {
                    Success = true,
                    Message = "Task created Successfully",
                    Data = mapped
                };
            }
            catch (Exception ex) {

                return new ApiResponse<TaskHeadDTO>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<bool> DeleteTaskAsync(int teamLeadId,int taskHeadId)
        {
            var taskHead = await _taskHead.GetByIdAsync(taskHeadId);
            if (taskHead == null) return false;

            var team = await _team.GetByIdAsync(taskHead.TeamId);
            if (team == null)
                throw new KeyNotFoundException("Team not found.");
            if (team.CreatedBy != teamLeadId)
                throw new UnauthorizedAccessException("You are not authorized to delete this task.");

            taskHead.DeletedBy = teamLeadId;
            taskHead.DeletedOn = DateTime.Now;
            taskHead.IsDeleted = true;
            await _taskHead.DeleteAsync(taskHead);
            await _taskHead.SaveAsync();
            return true;
        }

        public async Task<TaskHeadDTO> GetTaskHeadByIdAsync(int teamLeadId, int taskHeadId)
        {
            var taskHead = await _taskHeadRepository.GetByIdAsync(taskHeadId);
            if (taskHead == null) throw new KeyNotFoundException("Task head not found");

            var team = await _team.GetByIdAsync(taskHead.TeamId);
            if (team == null) throw new KeyNotFoundException("Team not found");
            if (team.CreatedBy != teamLeadId) throw new UnauthorizedAccessException("You cannot view this taskhead");

            if (taskHead.IsDeleted) throw new InvalidOperationException("TaskHead has been deleted");
            return _mapper.Map<TaskHeadDTO>(taskHead);
        }

        public async Task<IEnumerable<TaskHeadDTO>> GetAllTaskAsync(TaskHeadFilterDTO dto, int teamLeadId)
        {
            var taskHeads = await _taskHeadRepository.GetTaskHeadsAsync();

            var query = taskHeads
                .Where(th => th.Team.CreatedBy == teamLeadId && !th.IsDeleted);
            if (dto.TeamId > 0)
                query = query.Where(th => th.TeamId == dto.TeamId);
            if(!string.IsNullOrEmpty(dto.Title))
                query=query.Where(th => th.Title.ToLower().Contains(dto.Title.ToLower()));
            if(dto.Status.HasValue)
                query=query.Where(th => th.Status == dto.Status.Value);
            if (dto.FromDate.HasValue)
                query = query.Where(th => th.CreatedOn >= dto.FromDate.Value);
            if (dto.ToDate.HasValue)
                query = query.Where(th => th.CreatedOn <= dto.ToDate.Value);
            if (!string.IsNullOrEmpty(dto.SortBy))
            {
                query = dto.SortBy.ToLower() switch
                {
                    "newest" => query.OrderByDescending(th => th.CreatedOn),
                    "oldest" => query.OrderBy(th => th.CreatedOn), 
                    "title" => query.OrderBy(th => th.Title),
                    _ => query
                };
            }

            return _mapper.Map<IEnumerable<TaskHeadDTO>>(query);
        }

        public async Task<TaskHeadDTO> UpdateTaskAsync(int taskHeadId,UpdateTaskHeadDTO dto, int teamLeadId)
        {
            var taskHead = await _taskHead.GetByIdAsync(taskHeadId);
            if (taskHead == null) throw new Exception("Key not found");

            var team=await _team.GetByIdAsync(taskHead.TeamId);
            if (team == null) throw new KeyNotFoundException("Team not found");
            if (team.CreatedBy != teamLeadId) throw new UnauthorizedAccessException("You are not authorized to update the task");
            if(taskHead.IsDeleted) throw new InvalidOperationException("Cannot update a deleted task.");
            taskHead.ModifiedBy= teamLeadId;
            taskHead.ModifiedOn= DateTime.Now;
            taskHead.Title = dto.Title;

            if (dto.StartDate.HasValue && dto.StartDate.Value < DateTime.Today)
                throw new InvalidOperationException("Start date cannot be in the past.");

            if (dto.ExpectedEndDate.HasValue && dto.StartDate.HasValue &&
                dto.ExpectedEndDate.Value <= dto.StartDate.Value )
                throw new InvalidOperationException("Expected end date must be after start date.");

            if (dto.Duedate.HasValue && dto.ExpectedEndDate.HasValue &&
                dto.Duedate.Value <= dto.ExpectedEndDate.Value)
                throw new InvalidOperationException("Due date must be after expected end date.");

            if (dto.Duedate.HasValue && dto.Duedate.Value < DateTime.Today)
                throw new InvalidOperationException("Due date cannot be in the past.");

            if (dto.ExtendedTo.HasValue && dto.Duedate.HasValue)
            {
                if (dto.ExtendedTo.Value <= dto.Duedate.Value)
                    throw new InvalidOperationException("Extended date must be after due date.");

                var difference = (dto.ExtendedTo.Value - dto.Duedate.Value).TotalDays;
                if (difference > 2)
                    throw new InvalidOperationException("Extended date cannot be more than 2 days after the due date.");
            }



            if (!string.IsNullOrWhiteSpace(dto.Title) && dto.Title != taskHead.Title)
                taskHead.Title = dto.Title;

           

            if (dto.ExpectedEndDate.HasValue)
                taskHead.ExpectedEndDate = dto.ExpectedEndDate.Value;

            if (dto.StartDate.HasValue)
                taskHead.StartDate = dto.StartDate.Value;

            if (dto.Duedate.HasValue)
                taskHead.DueDate = dto.Duedate.Value;

            if (dto.ExtendedTo.HasValue)
                taskHead.ExtendedTo = dto.ExtendedTo.Value;

            if (dto.Status.HasValue)
                taskHead.Status = dto.Status.Value;

            await _taskHead.UpdateAsync(taskHead);
            await _taskHead.SaveAsync();

            return _mapper.Map<TaskHeadDTO>(taskHead);



        }
    }
}
