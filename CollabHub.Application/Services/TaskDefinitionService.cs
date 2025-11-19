using AutoMapper;
using CollabHub.Application.DTO;
using CollabHub.Application.DTO.Task;
using CollabHub.Application.DTO.TaskDefinition;
using CollabHub.Application.DTO.TaskDefinitions;
using CollabHub.Application.Interfaces;
using CollabHub.Domain.Entities;
using CollabHub.Domain.Enum;
using CollabHub.Infrastructure.Repositories.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CollabHub.Application.Services
{
    public class TaskDefinitionService : ITaskDefinition
    {
        private readonly IGenericRepository<Team> _teamRepo;
        private readonly IGenericRepository<TaskHead>_headRepo;
        private readonly IGenericRepository<TaskDefinition> _defRepo;
        private readonly IGenericRepository<TeamMember> _member;
        private readonly IMapper _mapper;
        public TaskDefinitionService(IGenericRepository<Team> teamRepo,IGenericRepository<TaskHead> headRepo,
            IGenericRepository<TaskDefinition> defRepo,IMapper mapper,IGenericRepository<TeamMember> member)
        {
            _teamRepo= teamRepo;
            _headRepo= headRepo;
            _defRepo= defRepo;
            _mapper= mapper;
            _member = member;
        }

        private async Task<ApiResponse<Team>> GetAndValidateTeamAsync(int teamId, int teamLeadId)
        {
            var team = await _teamRepo.GetByIdAsync(teamId);
            if (team == null)
                return ApiResponse<Team>.Fail(
                    statusCode:404,
                    message:"Team not found",
                    type:"NotFound");

            if (team.CreatedBy != teamLeadId)
                return ApiResponse<Team>.Fail(statusCode:403,
                    message:"You are not authorized for this team",
                    type:"Forbidden");

            return ApiResponse<Team>.Success(
                statusCode:200,
                data:team) ;
        }

        public async Task<ApiResponse<bool>> AssignMemberAsync(int taskId, int teamMemberId, int teamLeadId)
        {
            var task = await _defRepo.GetByIdAsync(taskId);
            if (task == null)
                return ApiResponse<bool>.Fail(statusCode:404,
                    message:"Sub task is not found",
                    type:"NotFound");

            var member = await _member.GetByIdAsync(teamMemberId);
            if (member == null)
                return ApiResponse<bool>.Fail(statusCode: 404,
                    message: "Member not found",
                    type: "NotFound");

            if (!member.IsApproved || member.IsRejected || member.IsDeleted)
                return ApiResponse<bool>.Fail(
                    statusCode:403,
                    message: "Cannot assign task to this member",
                    type: "MemberNotAssignable");

            if (task.IsDeleted)
                return ApiResponse<bool>.Fail(
                    statusCode: 409,
                    message: "Cannot assign a deleted task",
                    type: "Task Deleted");


            var taskHead = await _headRepo.GetByIdAsync(task.TaskHeadId);
            if (taskHead == null)
                return ApiResponse<bool>.Fail(statusCode: 404,
                    message: "Main task not found",
                    type: "NotFound");

            if (taskHead.TeamId != member.TeamId)
                return ApiResponse<bool>.Fail(statusCode: 404, message: "Member does not belong to this task's team", type: "NotFound");

            var team = await _teamRepo.GetByIdAsync(member.TeamId);
            if (team.CreatedBy != teamLeadId)
                return ApiResponse<bool>.Fail(statusCode:403,
                    message: "You are not authorized to assign members to this team",
                    type:"Forbidden");

            if (task.AssignedMemberId.HasValue && task.AssignedMemberId != member.TeamMemberId)
                return ApiResponse<bool>.Fail(statusCode:409,
                    message: "Task is already assigned to another member",
                    type:"AlreadyAssigned");

            if (task.ModifiedBy == teamLeadId)
                return ApiResponse<bool>.Fail(
                    statusCode:409,
                    message: "This member was recently removed by you. Cannot reassign immediately.",
                    type:"RecentlyDeleted");


            task.AssignedById = teamLeadId;
            task.AssignedMemberId = member.TeamMemberId;
            task.StartDate = DateTime.Now;

            await _defRepo.UpdateAsync(task);
            await _defRepo.SaveAsync();

            return ApiResponse<bool>.Success(statusCode: 200,
                data:true);
        }



        public async Task<ApiResponse<TaskDefinitionDTO>> CreateTaskDefinitionAsync(CreateTaskDefinitionDTO dto, int teamLeadId)
        {
            var team = await GetAndValidateTeamAsync(dto.TeamId, teamLeadId);
            var taskHead = await _headRepo.GetByIdAsync(dto.TaskHeadId);

            if (taskHead == null) return ApiResponse<TaskDefinitionDTO>.Fail(statusCode: 404,
                    message: "Main task not found",
                    type: "NotFound");

            if (taskHead.TeamId != dto.TeamId)
                return ApiResponse<TaskDefinitionDTO>.Fail(
                    statusCode:404,
                    message: "This task head does not belong to your team",
                    type:"NotFound");


            if (dto.StartDate < taskHead.StartDate)
                return ApiResponse<TaskDefinitionDTO>.Fail(
                    statusCode:400,
                    message: "TaskDefinition start date cannot be before TaskHead start date",
                    type:"InvalidDate");

            if (dto.ExpectedEndDate > taskHead.ExpectedEndDate)
                return ApiResponse<TaskDefinitionDTO>.Fail(
                    statusCode:400,
                    message: "SubTask expected end date cannot be after Main task expected end date",
                    type:"InvalidDate");

            if (dto.DueDate > taskHead.DueDate)
                return ApiResponse<TaskDefinitionDTO>.Fail(
                    statusCode: 400,
                    message: "SubTask Duedate is not after Main task duedate",
                    type: "InvalidDate");

            if (dto.StartDate > dto.DueDate)
                return ApiResponse<TaskDefinitionDTO>.Fail(
                    statusCode: 400,
                    message: "Start date cannot be after due date",
                    type: "InvalidDate");

            var entity = _mapper.Map<TaskDefinition>(dto);
            
            await _defRepo.AddAsync(entity);
            await _defRepo.SaveAsync();

            return ApiResponse<TaskDefinitionDTO>.Success(
                statusCode:200,
                data: _mapper.Map<TaskDefinitionDTO>(entity));

        }

        public async Task<ApiResponse<bool>> DeleteTaskDefinitionAsync(int taskDefinitionId, int teamLeadId)
        {
            var task = await _defRepo.GetByIdAsync(taskDefinitionId);
            if (task == null)
                return ApiResponse<bool>.Fail(statusCode: 404,
                    message: "Sub Task not found",
                    type: "NotFound");

            var taskHead = await _headRepo.GetByIdAsync(task.TaskHeadId);
            if (taskHead == null)
                return ApiResponse<bool>.Fail(statusCode: 404, message: "Main task not found", type: "NotFound");

            var team = await _teamRepo.GetByIdAsync(taskHead.TeamId);
            if (team == null)
                return ApiResponse<bool>.Fail(statusCode: 404,
                   message: "Team not found for this task head.",
                   type: "NotFound");


            if (team.CreatedBy != teamLeadId)
                return ApiResponse<bool>.Fail(
                    statusCode:403,
                    message: "You are not authorized to delete this task definition.",
                    type:"Forbidden");

            if (task.AssignedMemberId.HasValue)
                return ApiResponse<bool>.Fail(
                    statusCode:409,
                    message: "Cannot delete a task that is assigned to a member",
                    type:"AlreadyAssigned");
           


            await _defRepo.DeleteAsync(task);
            await _defRepo.SaveAsync();
            return ApiResponse<bool>.Success(statusCode:200,
                data:true);

        }


        public async Task<ApiResponse<bool>> RemoveMemberAsync(int taskDefinitionId, int memberId, int teamLeadId)
        {
            var task = await _defRepo.GetByIdAsync(taskDefinitionId);
            if (task == null)
                return ApiResponse<bool>.Fail(statusCode: 404,
                     message: "Main task not found",
                     type: "NotFound");

            if (task.AssignedMemberId != memberId)
                return ApiResponse<bool>.Fail(
                    statusCode:404,
                    message:"Member not found",
                    type:"NotFound");
                

            var taskHead = await _headRepo.GetByIdAsync(task.TaskHeadId);
            if (taskHead == null)
                return ApiResponse<bool>.Fail(statusCode: 404,
                   message: "Main task not found for this subTask",
                   type: "NotFound");

            var team = await _teamRepo.GetByIdAsync(taskHead.TeamId);
            if (team == null)
                return ApiResponse<bool>.Fail(statusCode: 404,
                   message: "Team not found for this Main task",
                   type: "NotFound");

            if (team.CreatedBy != teamLeadId)
                return ApiResponse<bool>.Fail(
                    statusCode: 403,
                    message: "You are not authorized to remove this member.",
                    type: "Forbidden");


            task.AssignedMemberId = null;
            task.AssignedById = null;
            task.ModifiedBy = teamLeadId;
            task.ModifiedOn = DateTime.Now;

            await _defRepo.UpdateAsync(task);
            await _defRepo.SaveAsync();

            return ApiResponse<bool>.Success(statusCode:200,
                data:true);
        }


        public async Task<ApiResponse<bool>> UpdateTaskDefinitionAsync(UpdateTaskDefinitionDTO dto, int teamLeadId,int taskDefinitionId)
        {
            var task = await _defRepo.GetByIdAsync(taskDefinitionId);

            if (task == null) return ApiResponse<bool>.Fail(statusCode: 404,
                    message: "Sub task is not found",
                    type: "NotFound");

            var taskHead = await _headRepo.GetByIdAsync(task.TaskHeadId);

            var team = await _teamRepo.GetByIdAsync(taskHead.TeamId);
            if (team.CreatedBy != teamLeadId) return ApiResponse<bool>.Fail(
                    statusCode: 403,
                    message: "You are not authorized to update this task.",
                    type: "Forbidden");


            if(taskHead == null)
                return ApiResponse<bool>.Fail(statusCode: 404,
                      message: "Main task not found",
                      type: "NotFound");

            if (task.IsDeleted)
                return ApiResponse<bool>.Fail(statusCode: 400,
               message: "Cannot update a deleted task",
               type: "SubTaskDeleted");


            if (taskHead == null || taskHead.IsDeleted)
                return ApiResponse<bool>.Fail(statusCode: 400,
              message: "Cannot update sub task for a deleted main task.",
              type: "MainTaskDeleted");



            if (dto.DueDate > taskHead.DueDate)
                return ApiResponse<bool>.Fail(statusCode: 400,
             message: "TaskDefinition Duedate is not after task head duedate",
             type: "DuedateError");

            if (dto.ExtendedTo.HasValue && dto.DueDate.HasValue)
            {
                if (dto.ExtendedTo.Value <= dto.DueDate.Value)
                    return ApiResponse<bool>.Fail(
                statusCode: 400,
                message: "Extended date must be after due date.",
                type: "ExtendedDateError");

                var difference = (dto.ExtendedTo.Value - dto.DueDate.Value).TotalDays;
                if (difference > 2)
                    return ApiResponse<bool>.Fail(
               statusCode: 400,
               message: "Extended date cannot be more than 2 days after the due date.",
               type: "ExtendedDateLimit");
            }

            if (!string.IsNullOrEmpty(dto.Description))
                task.Description = dto.Description;
            if (dto.DueDate.HasValue)
                task.DueDate = dto.DueDate.Value;
            if (dto.ExtendedTo.HasValue)
                task.ExtendedTo = dto.ExtendedTo.Value;
            if (dto.Status.HasValue)
                task.Status = dto.Status.Value;

            task.ModifiedBy = teamLeadId;
            task.ModifiedOn = DateTime.Now;
            await _defRepo.UpdateAsync(task);
            await _defRepo.SaveAsync();
            return ApiResponse<bool>.Success(
                statusCode:200,
                data:true);
        }
        public async Task<ApiResponse<TaskDefinitionDTO>> GetTaskDefinitionById(int taskDefinitionId, int teamLeadId)
        {
            var taskDefinition = await _defRepo.GetOneAsync(t=>t.TaskDefinitionId==taskDefinitionId && !t.IsDeleted);
            if (taskDefinition == null) return ApiResponse<TaskDefinitionDTO>.Fail(statusCode: 404,
                    message: "Main task not found",
                    type: "NotFound");

            var taskHead = await _headRepo.GetByIdAsync(taskDefinition.TaskHeadId);
            if (taskHead == null)  return ApiResponse<TaskDefinitionDTO>.Fail(statusCode: 404,
                    message: "Main task not found",
                    type: "NotFound");

            if (taskHead.CreatedBy != teamLeadId)
                return ApiResponse<TaskDefinitionDTO>.Fail(
                    statusCode: 403,
                    message: "You are not authorized to view this member.",
                    type: "Forbidden");

            return ApiResponse<TaskDefinitionDTO>.Success(
                statusCode:200,
                data: _mapper.Map<TaskDefinitionDTO>(taskDefinition));
                

        }

       public async Task<ApiResponse<IEnumerable<TaskDefinitionDTO>>> GetAllTaskDefinition(int taskHeadId, int teamLeadId)
        {
            var taskHead = await _headRepo.GetByIdAsync(taskHeadId);
            if (taskHead == null) return ApiResponse<IEnumerable<TaskDefinitionDTO>>.Fail(statusCode: 404,
                    message: "Main task not found",
                    type: "NotFound");

            if (taskHead.CreatedBy != teamLeadId)
                return ApiResponse<IEnumerable<TaskDefinitionDTO>>.Fail(
                    statusCode: 403,
                    message: "You are not authorized to view this subtask.",
                    type: "Forbidden");
            var taskDefinition =await _defRepo.GetByConditionAsync(t => t.TaskHeadId == taskHeadId && !t.IsDeleted);

            return ApiResponse<IEnumerable<TaskDefinitionDTO>>.Success(
                statusCode:200,
                data:_mapper.Map<IEnumerable<TaskDefinitionDTO>>(taskDefinition));
                

        }
       

    }
}

