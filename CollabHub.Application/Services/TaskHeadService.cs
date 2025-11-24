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
        //private readonly IGenericRepository<TaskDefinition> _def;
        private readonly IMapper _mapper;

        public TaskHeadService(IGenericRepository<TaskHead> taskHead,
            IGenericRepository<Team> team,
            IMapper mapper,
            //IGenericRepository<TaskDefinition> def,
            ITaskHeadRepository taskHeadRepository)
        {
            _taskHead = taskHead;
            _team = team;
            _mapper = mapper;
            _taskHeadRepository = taskHeadRepository;
            //_def = def;
        }

        public async Task<ApiResponse<TaskHeadDTO>> CreateTaskAsync(CreateTaskHeadDTO dto, int teamLeadId)
        {
           


                var team = await _team.GetByIdAsync(dto.TeamId);
                if (team == null) return ApiResponse<TaskHeadDTO>.Fail(
                    statusCode:404,
                    message:"Team is not exist",
                    type:"NotFound");


                if (team.CreatedBy != teamLeadId) return ApiResponse<TaskHeadDTO>.Fail(
                    statusCode:403,
                    message: "You are not authorized to create the task",
                    type:"Forbidden");
               
                var taskHead = _mapper.Map<TaskHead>(dto);
                taskHead.CreatedBy = teamLeadId;
                taskHead.CreatedOn=DateTime.Now;
                taskHead.Status = Domain.Enum.TaskStatus.Pending;
                taskHead.StartDate = dto.StartDate ?? DateTime.Now;
                taskHead.Title = dto.Title;



                if (dto.DueDate < taskHead.StartDate)
                    return ApiResponse<TaskHeadDTO>.Fail(
                        statusCode:400,
                        message: "Due date cannot be before start date.",
                        type:"InvalidDateRange");

                if (dto.ExpectedEndDate > dto.DueDate)
                    return ApiResponse<TaskHeadDTO>.Fail(
                        statusCode:400,
                        message: "Expected end date cannot be after due date.",
                        type:"InvalidDate");

                await _taskHead.AddAsync(taskHead);
                await _taskHead.SaveAsync();

                var mapped = _mapper.Map<TaskHeadDTO>(taskHead);
                mapped.TeamName = team.TeamName;
                return ApiResponse<TaskHeadDTO>.Success(
                    statusCode:200,
                    message: "Task created Successfully",
                    data:mapped);
               
           
            
        }

        public async Task <ApiResponse<bool>> DeleteTaskAsync(int teamLeadId,int taskHeadId)
        {
            var taskHead = await _taskHead.GetByIdAsync(taskHeadId);
            if (taskHead == null)
                return ApiResponse<bool>.Fail(
                    statusCode:404,
                    message:"Task not found",
                    type:"NotFound");

            var team = await _team.GetByIdAsync(taskHead.TeamId);
            if (team == null)
                return ApiResponse<bool>.Fail(
                    statusCode:404,
                    message:"Team not found",
                    type: "NotFound")
                    ;
            if (team.CreatedBy != teamLeadId)
                return ApiResponse<bool>.Fail(
                    statusCode:403,
                    message:"You are not authorized to delete this task",
                    type:"Forbidden");
            var task = await _taskHeadRepository.GetByIdTaskAsync(taskHeadId);
            if(task.TaskDefinitions.Any(td=>td.AssignedMemberId.HasValue))
                return ApiResponse<bool>.Fail(400, "Cannot delete. This task is already assigned to a user.",
                 "AlreadyAssigned");

            if (taskHead.TaskDefinitions.Any(td=>td.AssignedMemberId.HasValue))
                return ApiResponse<bool>.Fail(400, "Cannot delete. This task is already assigned to a user.",
                    "AlreadyAssigned");

            taskHead.DeletedBy = teamLeadId;
            taskHead.DeletedOn = DateTime.Now;
            taskHead.IsDeleted = true;
            await _taskHead.DeleteAsync(taskHead);
            await _taskHead.SaveAsync();
            return ApiResponse<bool>.Success(
                statusCode:200,
                message:"Task deleted",
                data:true);
        }

        public async Task<ApiResponse<TaskHeadDTO>> GetTaskHeadByIdAsync(int teamLeadId, int taskHeadId)
        {
            var taskHead = await _taskHeadRepository.GetByIdAsync(taskHeadId);
            if (taskHead == null) return ApiResponse<TaskHeadDTO>.Fail(statusCode:404,
                message:"Main task is not found",
                type:"NotFound");

            var team = await _team.GetByIdAsync(taskHead.TeamId);
            if (team == null) return ApiResponse<TaskHeadDTO>.Fail(
                statusCode:404,
                message:"Team is not found",
                type: "NotFound");


            if (team.CreatedBy != teamLeadId) return ApiResponse<TaskHeadDTO>.Fail(statusCode: 403,
                message: "You are not authorized to get the main task",
                type: "Forbidden");

            if (taskHead.IsDeleted) return ApiResponse<TaskHeadDTO>.Fail(
                statusCode:404,
                message:"Task not found",
                type:"NotFound");

            return ApiResponse<TaskHeadDTO>.Success(
                statusCode:200,
                message:$"{taskHead.Title}...",
                data: _mapper.Map<TaskHeadDTO>(taskHead));
        }

        public async Task<ApiResponse<IEnumerable<TaskHeadDTO>>> GetAllTaskAsync(TaskHeadFilterDTO dto, int teamLeadId)
        {
            var taskHeads = await _taskHeadRepository.GetTaskHeadsAsync();
          

            var query = taskHeads
                .Where(th => th.Team.CreatedBy == teamLeadId && !th.IsDeleted);
            if (dto.TeamId > 0)
            {
                var team = await _team.GetByIdAsync(dto.TeamId);
                if (team == null) return ApiResponse<IEnumerable<TaskHeadDTO>>.Fail(404,"Team not found","NotFound");
                if (team.CreatedBy != teamLeadId) return ApiResponse<IEnumerable<TaskHeadDTO>>.Fail(403,"You are not authorized","Forbidden");

                query = query.Where(th => th.TeamId == dto.TeamId);

            }
            if (!string.IsNullOrEmpty(dto.Title))
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

            return ApiResponse<IEnumerable<TaskHeadDTO>>.Success(
                statusCode:200,
                data: _mapper.Map<IEnumerable<TaskHeadDTO>>(query));

        }

        public async Task<ApiResponse<TaskHeadDTO>> UpdateTaskAsync(int taskHeadId,UpdateTaskHeadDTO dto, int teamLeadId)
        {
            var taskHead = await _taskHead.GetByIdAsync(taskHeadId);
            if (taskHead == null) return ApiResponse<TaskHeadDTO>.Fail(statusCode: 404,
                message: "Main task is not found",
                type: "NotFound");

            var team=await _team.GetByIdAsync(taskHead.TeamId);
            if (team == null) return ApiResponse<TaskHeadDTO>.Fail(
                statusCode: 404,
                message: "Team is not found",
                type: "NotFound");

            if (team.CreatedBy != teamLeadId) return ApiResponse<TaskHeadDTO>.Fail(statusCode: 403,
                message: "You are not authorized to update the main task",
                type: "Forbidden");

            if (taskHead.IsDeleted) return ApiResponse<TaskHeadDTO>.Fail(statusCode:400,
                message:"Cannot update a deleted task",
                type: "TaskHeadDeleted");

            taskHead.ModifiedBy= teamLeadId;
            taskHead.ModifiedOn= DateTime.Now;
            //taskHead.Title = dto.Title;

            if (dto.StartDate.HasValue && dto.StartDate.Value < DateTime.Today)
                return ApiResponse<TaskHeadDTO>.Fail(
                    statusCode:400,
                    message: "Start date cannot be in the past.",
                    type: "StartDateInPast");

            
            if (dto.ExpectedEndDate.HasValue && dto.StartDate.HasValue &&
                dto.ExpectedEndDate.Value <= dto.StartDate.Value )
                return ApiResponse<TaskHeadDTO>.Fail(
                  statusCode: 400,
                  message: "Expected end date must be after start date.",
                  type: "ExpectedEndDate");


            if (dto.Duedate.HasValue && dto.ExpectedEndDate.HasValue &&
                dto.Duedate.Value <= dto.ExpectedEndDate.Value)
                return ApiResponse<TaskHeadDTO>.Fail(
                 statusCode: 400,
                 message: "Due date must be after expected end date.",
                 type: "DueDateError");


            if (dto.Duedate.HasValue && dto.Duedate.Value < DateTime.Today)
                return ApiResponse<TaskHeadDTO>.Fail(
                statusCode: 400,
                message: "Due date cannot be in the past.",
                type: "DueDateInPast");


            if (dto.ExtendedTo.HasValue && dto.Duedate.HasValue)
            {
                if (dto.ExtendedTo.Value <= dto.Duedate.Value)
                    return ApiResponse<TaskHeadDTO>.Fail(
                statusCode: 400,
                message: "Extended date must be after due date.",
                type: "ExtendedDateError");

                var difference = (dto.ExtendedTo.Value - dto.Duedate.Value).TotalDays;
                if (difference > 2)
                    return ApiResponse<TaskHeadDTO>.Fail(
               statusCode: 400,
               message: "Extended date cannot be more than 2 days after the due date.",
               type: "ExtendedDateLimit");
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

            return ApiResponse<TaskHeadDTO>.Success(
                statusCode:200,
                data: _mapper.Map<TaskHeadDTO>(taskHead));



        }
    }
}
