using AutoMapper;
using CollabHub.Application.DTO.TaskDefinition;
using CollabHub.Application.DTO.TaskDefinitions;
using CollabHub.Application.Interfaces;
using CollabHub.Domain.Entities;
using CollabHub.Infrastructure.Repositories.EF;
using CollabHub.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private async Task<Team> GetAndValidateTeamAsync(int teamId, int teamLeadId)
        {
            var team = await _teamRepo.GetByIdAsync(teamId);
            if (team == null)
                throw new KeyNotFoundException("Team not found");

            if (team.CreatedBy != teamLeadId)
                throw new UnauthorizedAccessException("You are not authorized for this team");

            return team;
        }

        public async Task<bool> AssignMemberAsync(int taskId, int teamMemberId, int teamLeadId)
        {
            var task = await _defRepo.GetByIdAsync(taskId);
            if (task == null)
                throw new KeyNotFoundException("Task not found");

            var member = await _member.GetByIdAsync(teamMemberId);
            if (member == null || !member.IsApproved || member.IsRejected)
                throw new InvalidOperationException("Member not found or not approved");

            var taskHead = await _headRepo.GetByIdAsync(task.TaskHeadId);
            if (taskHead == null)
                throw new KeyNotFoundException("Task head not found");

            if (taskHead.TeamId != member.TeamId)
                throw new InvalidOperationException("Member does not belong to this task's team");

            var team = await _teamRepo.GetByIdAsync(member.TeamId);
            if (team.CreatedBy != teamLeadId)
                throw new UnauthorizedAccessException("You are not authorized to assign members to this team");

            if (task.AssignedUserId.HasValue && task.AssignedUserId != member.UserId)
                throw new InvalidOperationException("Task is already assigned to another member");

            task.AssignedById = teamLeadId;
            task.AssignedUserId = member.UserId;
            task.StartDate = DateTime.Now;

            await _defRepo.UpdateAsync(task);
            await _defRepo.SaveAsync();

            return true;
        }



        public async Task<TaskDefinitionDTO> CreateTaskDefinitionAsync(CreateTaskDefinitionDTO dto, int teamLeadId)
        {
            var team = await GetAndValidateTeamAsync(dto.TeamId, teamLeadId);
            var taskHead = await _headRepo.GetByIdAsync(dto.TaskHeadId);
            if (taskHead == null) throw new KeyNotFoundException("Task head not found");
            if (taskHead.TeamId != dto.TeamId) throw new UnauthorizedAccessException("This task head does not belong to your team");
            var entity = _mapper.Map<TaskDefinition>(dto);
            
            await _defRepo.AddAsync(entity);
            await _defRepo.SaveAsync();

            return _mapper.Map<TaskDefinitionDTO>(entity);

        }

        public async Task<bool> DeleteTaskDefinitionAsync(int taskDefinitionId, int teamLeadId)
        {
            var task=await _defRepo.GetByIdAsync(taskDefinitionId);
            if (task == null) return false;

            var team = await _teamRepo.GetByIdAsync(teamLeadId);
            if (team.CreatedBy != teamLeadId) return false;

            await _defRepo.DeleteAsync(task);
            await _defRepo.SaveAsync();
            return true;
           
        }

        public async Task<bool> RemoveMemberAsync(int taskDefinitionId, int memberId, int teamLeadId)
        {
            var task = await _defRepo.GetByIdAsync(taskDefinitionId);
            if (task == null)
                return false;

            if (task.AssignedUserId != memberId)
                return false;

            var taskHead = await _headRepo.GetByIdAsync(task.TaskHeadId);
            if (taskHead == null)
                throw new KeyNotFoundException("Task head not found for this task definition");

            var team = await _teamRepo.GetByIdAsync(taskHead.TeamId);
            if (team == null)
                throw new KeyNotFoundException("Team not found for this task head");

            if (team.CreatedBy != teamLeadId)
                throw new UnauthorizedAccessException("You are not authorized to remove this member");

           
            task.AssignedUserId = null;
            task.AssignedById = null;
            task.ModifiedBy = teamLeadId;
            task.ModifiedOn = DateTime.Now;

            await _defRepo.UpdateAsync(task);
            await _defRepo.SaveAsync();

            return true;
        }


        public async Task<bool> UpdateTaskDefinitionAsync(UpdateTaskDefinitionDTO dto, int teamLeadId,int taskDefinitionId)
        {
            var task = await _defRepo.GetByIdAsync(taskDefinitionId);

            if (task == null) return false;

            var taskHead = await _headRepo.GetByIdAsync(task.TaskHeadId);

            var team = await _teamRepo.GetByIdAsync(taskHead.TeamId);
            if (team.CreatedBy != teamLeadId || taskHead == null) return false;


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
            return true;
        }
    }
   }

