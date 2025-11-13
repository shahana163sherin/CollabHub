using AutoMapper;
using CollabHub.Application.DTO;
using CollabHub.Application.DTO.Task;
using CollabHub.Application.DTO.TaskDefinition;
using CollabHub.Application.DTO.TeamLead;
using CollabHub.Application.DTO.TeamMember;
using CollabHub.Application.Interfaces.TeamMember;
using CollabHub.Domain.Entities;
using CollabHub.Infrastructure.Repositories.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Services
{
    public class TeamMemberService : ITeamMemberService
    {
        private readonly IGenericRepository<Team> _teamRepo;
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<TeamMember> _teamMemberRepo;
        private readonly IGenericRepository<TaskHead> _task;
        private readonly IGenericRepository<TaskDefinition> _def;
        private readonly IMapper _mapper;

        public TeamMemberService(
            IGenericRepository<Team> teamRepo,
            IGenericRepository<User> userRepo,
            IGenericRepository<TeamMember> teamMemberRepo,
            IGenericRepository<TaskHead> task,
            IGenericRepository<TaskDefinition> def,
            IMapper mapper)
        {
            _teamRepo = teamRepo;
            _userRepo = userRepo;
            _teamMemberRepo = teamMemberRepo;
            _mapper = mapper;
            _task = task;
            _def = def;
        }

        public async Task<ApiResponse<JoinResponseDTO>> JoinTeamAsync(JoinRequestDTO dto, int memberId)
        {
            try
            {
               
                var team = await _teamRepo.GetOneAsync(t => t.InviteLink == dto.InviteCode);
                if (team == null)
                    return new ApiResponse<JoinResponseDTO>
                    {
                        Success = false,
                        Message = "Join request sent for approval"
                    };

                
                var user = await _userRepo.GetByIdAsync(memberId);
                if (user == null)
                    return new ApiResponse<JoinResponseDTO>
                    {
                        Success = false,
                        Message = "User not found"
                    };

             
                var alreadyMember = await _teamMemberRepo.GetOneAsync(m => m.TeamId == team.TeamId && m.UserId == memberId && !m.IsDeleted && !m.IsRejected);
                if (alreadyMember != null)
                {
                    if(alreadyMember.IsDeleted || alreadyMember.IsRejected)
                    {
                        alreadyMember.IsRejected = false;
                        alreadyMember.IsDeleted= false;
                        alreadyMember.DeletedOn = null;
                        alreadyMember.DeletedBy= null;

                        await _teamMemberRepo.UpdateAsync(alreadyMember);
                        await _teamMemberRepo.SaveAsync();

                        var result = _mapper.Map<JoinResponseDTO>(alreadyMember);
                        return new ApiResponse<JoinResponseDTO>
                        {
                            Success = true,
                            Message = "Rejoined team successfully",
                            Data = result
                        };
                    }
                    return new ApiResponse<JoinResponseDTO>
                    {
                        Success = false,
                        Message = "User already part of this team"
                    };
                }


              
                var teamMember = _mapper.Map<TeamMember>(dto);
                teamMember.TeamId = team.TeamId;
                teamMember.UserId = memberId;

                await _teamMemberRepo.AddAsync(teamMember);
                await _teamMemberRepo.SaveAsync();

                
                var response = _mapper.Map<JoinResponseDTO>(teamMember);
               

                
                return new ApiResponse<JoinResponseDTO>
                {
                    Success = true,
                    Message = "Join request sent for approval",
                    Data = response
                };
            }
            catch (Exception ex)
            {
               
                
                return new ApiResponse<JoinResponseDTO>
                {
                    Success = false,
                    Message = $"Error while joining team: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<string>> LeaveTeamAsync(int userId)
        {
            var member = await _teamMemberRepo.GetOneAsync(m => m.UserId == userId && !m.IsDeleted && !m.IsRejected);
            if (member == null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "You are not a part of any active team"
                };
            }

            member.IsDeleted = true;
            member.DeletedOn = DateTime.Now;
            member.DeletedBy = member.UserId;
            await _teamMemberRepo.UpdateAsync(member);
            await _teamMemberRepo.SaveAsync();

            return new ApiResponse<string>
            {
                Success = true,
                Message = "You have left the team successfully."
            };


        }
        public async Task<IEnumerable<TeamDTO>> ViewMyTeamsAsync(int userId)
        {
            var teams = _teamRepo.QueryByCondition(t => t.Members.Any(m => m.UserId == userId) && t.IsActive && !t.IsDeleted)
               .Include(t => t.Members.Where(m => m.IsApproved))
               .ThenInclude(m => m.User);

            var result = await teams.ToListAsync();
            return _mapper.Map<IEnumerable<TeamDTO>>(result);
        }

        public async Task <TeamDTO> ViewTeamById(int teamId, int userId)
        {
            var team = _teamRepo.QueryByCondition(t => t.TeamId == teamId && t.IsActive && !t.IsDeleted)
                .Include(t => t.Members.Where(m => m.IsApproved))
                .ThenInclude(m => m.User);
            var result = await team.FirstOrDefaultAsync();
            if (result == null) throw new KeyNotFoundException("Team not found");

            if (!result.Members.Any(m => m.UserId == userId))
                throw new UnauthorizedAccessException("You are not the member to view this team");

            return _mapper.Map<TeamDTO>(result);

        }
        public async Task<IEnumerable<TaskHeadDTO>> GetTasksByTeamAsync(int teamId, int memberId)
        {
            var team = await _teamRepo.GetByIdAsync(teamId);
            if (team == null) throw new KeyNotFoundException("Team not found");

            var member = await _teamMemberRepo.GetByIdAsync(memberId);
            if (member == null || member.TeamId != teamId) throw new UnauthorizedAccessException("Member does not belong to this team");
             var taskHead=await _task.GetByConditionAsync(th=>th.TeamId == teamId);
            return taskHead.Select(th => new TaskHeadDTO
            {
                TaskHeadId = th.TaskHeadId,
                Title = th.Title,
                StartDate = th.StartDate,
                ExpectedEndDate = th.ExpectedEndDate,
                DueDate = th.DueDate
            });
            
        }

        public async Task<IEnumerable<TaskDefinitionDTO>> ViewMyAssignedTask(int memberId)
        {
            var member = await _teamMemberRepo.GetByIdAsync(memberId);
            if (member == null) throw new KeyNotFoundException("Member not found");

            var taskDef = await _def.GetByConditionAsync(td => td.AssignedUserId == memberId && !td.IsDeleted);
            return taskDef.Select(td => new TaskDefinitionDTO
            {
                TaskDefinitionId = td.TaskDefinitionId,
                TaskHeadId = td.TaskHeadId,
                Description = td.Description,
                Status = (TaskStatus)td.Status,
                StartDate = td.StartDate,
                DueDate = td.DueDate,
                ExtendedTo = td.ExtendedTo


            });

        }
    }

}

