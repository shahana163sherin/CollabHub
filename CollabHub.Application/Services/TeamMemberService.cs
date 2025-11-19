using AutoMapper;
using AutoMapper.Execution;
using CollabHub.Application.DTO;
using CollabHub.Application.DTO.Task;
using CollabHub.Application.DTO.TaskDefinition;
using CollabHub.Application.DTO.TeamLead;
using CollabHub.Application.DTO.TeamMember;
using CollabHub.Application.Interfaces.TeamMember;
using CollabHub.Domain.Entities;
using CollabHub.Domain.Enum;
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

                var team = await _teamRepo.GetOneAsync(t => t.InviteLink == dto.InviteCode);
                if (team == null)
                    return ApiResponse<JoinResponseDTO>.Success(statusCode: 200,
                        message: "Join request sent for approval",
                        data: null);


                var user = await _userRepo.GetByIdAsync(memberId);
                if (user == null)
                    return ApiResponse<JoinResponseDTO>.Fail(statusCode: 404,
                        message: "User not found",
                        type:"NotFound"
                  );
             
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
                        return ApiResponse<JoinResponseDTO>.Success(statusCode:200,
                            message:"Rejoined team successfully",
                            data:result);
                      
                    }
                    return ApiResponse<JoinResponseDTO>.Fail(
                        statusCode:409,
                        message: "User already part of this team",
                        type:"AlreadyPart");
                  
                }


              
                var teamMember = _mapper.Map<TeamMember>(dto);
                teamMember.TeamId = team.TeamId;
                teamMember.UserId = memberId;

                await _teamMemberRepo.AddAsync(teamMember);
                await _teamMemberRepo.SaveAsync();

                
                var response = _mapper.Map<JoinResponseDTO>(teamMember);
               

                
              
                 return ApiResponse<JoinResponseDTO>.Success(statusCode: 200,
                        message: "Join request sent for approval",
                        data: null);
            
          
        }

        public async Task<ApiResponse<string>> LeaveTeamAsync(int userId)
        {
            var member = await _teamMemberRepo.GetOneAsync(m => m.UserId == userId && !m.IsDeleted && !m.IsRejected);
            if (member == null)
            {
                return ApiResponse<string>.Fail(

                    statusCode: 400,
                    message: "You are not a part of any active team",
                    type: "NotInActiveTeam");

            }

            member.IsDeleted = true;
            member.DeletedOn = DateTime.Now;
            member.DeletedBy = member.UserId;
            await _teamMemberRepo.UpdateAsync(member);
            await _teamMemberRepo.SaveAsync();

            return ApiResponse<string>.Success(
                statusCode:200,
                message: "You have left the team successfully.",
                data:null);
            


        }
       

        public async Task<ApiResponse<IEnumerable<TeamDTO>>> ViewMyTeamsAsync(int userId)
        {
            var teams = await _teamRepo.QueryByCondition(t =>
                t.Members.Any(m => m.UserId == userId && m.IsApproved) &&
                t.IsActive && !t.IsDeleted)
                .Include(t => t.Members)
                    .ThenInclude(m => m.User)
                        .ThenInclude(u => u.UploadedFiles)
                .ToListAsync();

            var result = teams.Select(t => new TeamDTO
            {
                TeamId = t.TeamId,
                TeamName = t.TeamName,
                Members = t.Members
                    .Where(m => m.IsApproved)
                    .Select(m => new TeamMemberDTO
                    {
                        UserId = m.UserId,
                        UserName = m.User.Name,
                        Role = m.Role,
                        ProfileImg = m.User.UploadedFiles
                            .Where(f => f.ContextType == FileContextType.ProfileImage && f.IsActive)
                            .OrderByDescending(f => f.CreatedOn)
                            .Select(f => f.FileData)
                            .FirstOrDefault()
                    }).ToList()
            });

            return ApiResponse<IEnumerable<TeamDTO>>.Success(
                statusCode:200,
                message:"My teams...",
                data:result);
        }

        public async Task<ApiResponse<TeamDTO>> ViewTeamById(int teamId, int userId)
        {
            var team = await _teamRepo.QueryByCondition(t => t.TeamId == teamId && t.IsActive && !t.IsDeleted)
                .Include(t => t.Members)
                    .ThenInclude(m => m.User)
                        .ThenInclude(u => u.UploadedFiles)
                .FirstOrDefaultAsync();

            if (team == null) return ApiResponse<TeamDTO>.Fail(
                  statusCode: 404,
                  message: "Team not found",
                  type: "NotFound");

            if (!team.Members.Any(m => m.UserId == userId && m.IsApproved))
                return ApiResponse<TeamDTO>.Fail(
                     statusCode: 403,
                     message: "You are not authorized to view this team",
                     type: "Forbidden"
                     );

            return ApiResponse<TeamDTO>.Success(
                statusCode: 200,
                message: $"{team.TeamName}....",
                data: new TeamDTO
                {
                    TeamId = team.TeamId,
                    TeamName = team.TeamName,
                    Members = team.Members
                    .Where(m => m.IsApproved)
                    .Select(m => new TeamMemberDTO
                    {
                        UserId = m.UserId,
                        UserName = m.User.Name,
                        Role = m.Role,
                        ProfileImg = m.User.UploadedFiles
                            .Where(f => f.ContextType == FileContextType.ProfileImage && f.IsActive)
                            .OrderByDescending(f => f.CreatedOn)
                            .Select(f => f.FileData)
                            .FirstOrDefault()
                    }).ToList()
                });


        }

        public async Task<ApiResponse<IEnumerable<TaskHeadDTO>>> GetTasksByTeamAsync(int teamId, int userId)
        {
            var team = await _teamRepo.GetByIdAsync(teamId);
            if (team == null)
                if (team == null) return ApiResponse < IEnumerable < TaskHeadDTO >>.Fail(
                      statusCode: 404,
                      message: "Team not found",
                      type: "NotFound");

            var member = await _teamMemberRepo
        .QueryByCondition(m => m.UserId == userId && m.TeamId == teamId && m.IsApproved)
        .FirstOrDefaultAsync();
            if (member == null || member.TeamId != teamId) return ApiResponse<IEnumerable<TaskHeadDTO>>.Fail(
                     statusCode: 403,
                     message: "You are not authorized to view this task",
                     type: "Forbidden"
                     );

            var taskHead =await _task.GetByConditionAsync(th=>th.TeamId == teamId);
            return ApiResponse<IEnumerable<TaskHeadDTO>>.Success(
                statusCode:200,
                message:"View the task",
                data: taskHead.Select(th => new TaskHeadDTO
                {
                    TaskHeadId = th.TaskHeadId,
                    Title = th.Title,
                    StartDate = th.StartDate,
                    ExpectedEndDate = th.ExpectedEndDate,
                    DueDate = th.DueDate
                }));
            
             }

        public async Task <ApiResponse<IEnumerable<TaskDefinitionDTO>>> ViewMyAssignedTask(int userId)
        {
            var member = await _teamMemberRepo
                .QueryByCondition(m => m.UserId == userId && m.IsApproved)
                .FirstOrDefaultAsync();
            if (member == null) return ApiResponse<IEnumerable<TaskDefinitionDTO>>.Fail(
                  statusCode: 404,
                  message: "Member not found",
                  type: "NotFound");

            var taskDef = await _def.GetByConditionAsync(td => td.AssignedMemberId == member.TeamMemberId && !td.IsDeleted);
            return ApiResponse<IEnumerable<TaskDefinitionDTO>>.Success(
                statusCode:200,
                data: taskDef.Select(td => new TaskDefinitionDTO
                {
                    TaskDefinitionId = td.TaskDefinitionId,
                    TaskHeadId = td.TaskHeadId,
                    Description = td.Description,
                    Status = td.Status,
                    StartDate = td.StartDate,
                    DueDate = td.DueDate,
                    ExtendedTo = td.ExtendedTo


                }));

        }
    }

}

