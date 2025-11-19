using AutoMapper;
using AutoMapper.Execution;
using CollabHub.Application.DTO;
using CollabHub.Application.DTO.TeamLead;
using CollabHub.Application.Interfaces.TeamLead;
using CollabHub.Domain.Entities;
using CollabHub.Domain.Enum;
using CollabHub.Infrastructure.Repositories.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Services
{
    public class TeamLeadService:ITeamLeadService
    {
        private readonly IGenericRepository<Team> _repoTeam;
        private readonly IGenericRepository<TeamMember>_teamMember;
        private readonly IMapper _mapper;
        public TeamLeadService(IGenericRepository<Team> repoTeam,IMapper mapper,IGenericRepository<TeamMember> teamMember)
        {
            _repoTeam= repoTeam;
            _mapper= mapper;
            _teamMember= teamMember;
        }

        public async Task<ApiResponse<TeamDTO>> CreateTeamAsync(CreateTeamDTO dto, int TeamLeadId) {
            if ((string.IsNullOrEmpty(dto.TeamName)) || (string.IsNullOrEmpty(dto.Description)))
            {
                return ApiResponse<TeamDTO>.Fail(
                    statusCode: 400,
                    message: "All fields are required",
                    type: "ValidationError");
               
            }
            if(dto.MemberLimit <= 1)
            {
                return ApiResponse<TeamDTO>.Fail(statusCode: 400,
                    message: "Team required atleast two members",
                    type: "ValidationError");
               
            }
            dto.TeamName = dto.TeamName.Trim();
            var exists = await _repoTeam
               .QueryByCondition(t => t.TeamName == dto.TeamName && !t.IsDeleted)
               .AnyAsync();

            if (exists)
                return ApiResponse<TeamDTO>.Fail(409, "Team name already exists", "DuplicateTeamName");
            var inviteToken=Guid.NewGuid().ToString();
            var inviteLink = $"https://collabHub.com/join-team/{inviteToken}";
            var team = _mapper.Map<Team>(dto);
            team.InviteLink = inviteLink;

            team.CreatedBy = TeamLeadId;
            team.IsActive = true;
            await _repoTeam.AddAsync(team);
            await _repoTeam.SaveAsync();

            var teamdto = _mapper.Map<TeamDTO>(team);
            return ApiResponse<TeamDTO>.Success(
                statusCode:201,
                message:"Team Created succssfully",
                data:teamdto);
            


        }

        public async Task<ApiResponse<object>> RemoveTeamAsync(int TeamID, int TeamLeadId)
        {
            var team = await _repoTeam.GetByIdAsync(TeamID);
            if (team == null) throw new KeyNotFoundException("Team is not found");
            if (team.CreatedBy != TeamLeadId) throw new UnauthorizedAccessException("You are not authorized to remove this team");

            team.IsDeleted = true;
            team.IsActive = false;
            team.DeletedBy = TeamLeadId;
            team.DeletedOn = DateTime.Now;

            await _repoTeam.UpdateAsync(team);
            await _repoTeam.SaveAsync();

            return ApiResponse<object>.Success(
                statusCode : 204,
                message:"Team deleted",
                data:null);
           
        }

        public async Task<ApiResponse<bool>> ApproveMemberAsync(ApproveMemberDTO dto, int TeamLeadid)
        {
            var member = await _teamMember.GetByIdAsync(dto.TeamMemberId);
            if (member == null || member.IsDeleted)
                //throw new Exception("Member not found or already deleted");
                return ApiResponse<bool>.Fail(
                    statusCode:404,
                    message:"Member not found",
                    type:"NotFound");

            var team = await _repoTeam.GetByIdAsync(member.TeamId);
            if (team == null)
                //throw new Exception("Team not found");
                return ApiResponse<bool>.Fail(
                   statusCode: 404,
                   message: "Team not found",
                   type: "NotFound");

            if (team.CreatedBy != TeamLeadid)
            {
                //throw new UnauthorizedAccessException("You are not authorized to get members for this team");
                return ApiResponse<bool>.Fail(
                    statusCode:403,
                    message: "You are not authorized to approve members for this team",
                    type:"Forbidden"
                    );
            }

            var teamMembers=await _teamMember.GetByConditionAsync(t=>t.TeamId==team.TeamId);
            var approvedCount = teamMembers.Count(a=>a.IsApproved);
            if(approvedCount >= team.MemberLimit && dto.IsApproved)
            {
                //throw new InvalidOperationException("Member limit reached");
                return ApiResponse<bool>.Fail(
                    statusCode:409,
                    message:"Member limit reached",
                    type:"LimitExceed");
            }
           

            member.IsApproved = true;
            member.IsRejected = false;
            await _teamMember.UpdateAsync(member);
            await _teamMember.SaveAsync();
            return ApiResponse<bool>.Success(
                statusCode:200,
                message:"Member approved",
                data:true);
        }

        public async Task<ApiResponse<IEnumerable<TeamMemberDTO>>> GetTeamMembersAsync(TeamMemberFilterDTO dto, int teamLeadId)
        {
            var team = await _repoTeam.GetByIdAsync(dto.TeamId);
            if (team == null)
                //throw new KeyNotFoundException("Team is not found");
                //throw new Exception("Team not found");
                return ApiResponse < IEnumerable < TeamMemberDTO >>.Fail(
                   statusCode: 404,
                   message: "Team not found",
                   type: "NotFound");

            if (team.CreatedBy != teamLeadId)
                //throw new UnauthorizedAccessException("You are not authorized to approve members for this team");
                return ApiResponse<IEnumerable<TeamMemberDTO>>.Fail(
                   statusCode: 403,
                   message: "You are not authorized to get members for this team",
                   type: "Forbidden"
                   );

            var query = _teamMember.QueryByCondition(t => t.TeamId == dto.TeamId && !t.IsDeleted);
                

            
            if (dto.Role.HasValue)
            {
                query = query.Where(q => q.Role == dto.Role.Value);
            }
            if (dto.IsApproved.HasValue)
            {
                query = query.Where(q => q.IsApproved == dto.IsApproved.Value);
            }
            if (dto.IsRejected.HasValue)
            {
                query = query.Where(q => q.IsRejected == dto.IsRejected.Value);
            }
            if (!string.IsNullOrEmpty(dto.SearchTerm))
            {
                query = query.Where(q => q.User.Name.Contains(dto.SearchTerm));
            }

            var members =  query
               .Select(m => new TeamMemberDTO
               {
                   UserId = m.User.UserId,
                   UserName = m.User.Name,
                   ProfileImg = m.User.UploadedFiles.Where(f=>f.ContextType== Domain.Enum.FileContextType.ProfileImage)
                   .OrderByDescending(f=>f.CreatedOn)
                   .Select(f=>f.FileData).FirstOrDefault(),
                   Role = m.Role
               }).ToList();

            return ApiResponse<IEnumerable<TeamMemberDTO>>.Success(
                statusCode:200,
                message:$"{team.TeamName} Members....",
                data:members);
        }

        public async Task<ApiResponse<bool>> RejectMemberAsync( RejectMemberDTO dto, int teamLeadId)
        {
            
            var team = await _repoTeam.GetByIdAsync(dto.teamId);
            if (team == null)
                return ApiResponse<bool>.Fail(
                  statusCode: 404,
                  message: "Team not found",
                  type: "NotFound");


            if (team.CreatedBy != teamLeadId)
                return ApiResponse<bool>.Fail(
                   statusCode: 403,
                   message: "You are not authorized to reject members for this team",
                   type: "Forbidden"
                   );


            var teamMember = await _teamMember.GetOneAsync(m => m.TeamMemberId == dto.MemberId && m.TeamId == dto.teamId);
            if (teamMember == null)
                return ApiResponse<bool>.Fail(
                     statusCode: 404,
                     message: "Member not found",
                     type: "NotFound");
            if (teamMember.IsApproved)
                return ApiResponse<bool>.Fail(
                   statusCode: 409,
                   message: "This member has already been approved and cannot be rejected",
                   type: "AlreadyApproved"
                   );
          
            if (teamMember.IsRejected)
                return ApiResponse<bool>.Fail(
                   statusCode: 400,
                   message: "This member has already rejected",
                   type: "AlreadyRejected"
                   );

            teamMember.IsRejected = true;
            teamMember.IsApproved = false;
            teamMember.ModifiedOn = DateTime.Now;
            teamMember.ModifiedBy = teamLeadId;

            await _teamMember.UpdateAsync(teamMember);
            await _teamMember.SaveAsync();

            return ApiResponse<bool>.Success(
                   statusCode: 200,
                   message: "Rejected the member",
                  data:true
                   );
        }


        public async Task<ApiResponse<bool>> RemoveMemberAsync(int TeamId,int MemberId, int TeamLeadid)
        {
            var member = (await _teamMember.GetByConditionAsync(m => m.TeamMemberId == MemberId && m.TeamId == TeamId)).FirstOrDefault();
            if (member == null) return ApiResponse<bool>.Fail(
                     statusCode: 404,
                     message: "Member not found",
                     type: "NotFound");

            var team = await _repoTeam.GetByIdAsync(TeamId);
            if (team == null) return ApiResponse<bool>.Fail(
                  statusCode: 404,
                  message: "Team not found",
                  type: "NotFound");

            if (team.CreatedBy != TeamLeadid) return ApiResponse<bool>.Fail(
                   statusCode: 403,
                   message: "You are not authorized to remove member from this team",
                   type: "Forbidden"
                   );
            if (member.IsRejected) return ApiResponse<bool>.Fail(
                    statusCode: 409,
                    message: "Member already rejected",
                    type: "MemberALreadyRejected");

            if (member.Role == Domain.Enum.TeamRole.TeamLeader)
                return ApiResponse<bool>.Fail(
                    statusCode:400,
                    message: "You cannot remove the Team Lead",
                    type:"RemoveLeader");
            if (member.IsDeleted)
                return ApiResponse<bool>.Fail(
                    statusCode:409,
                    message: "Member already removed",
                    type:"AlreayRemoved");

            member.IsDeleted = true;
            member.DeletedBy= TeamLeadid;
            member.DeletedOn=DateTime.Now;
            await _teamMember.UpdateAsync(member);
            await _teamMember.SaveAsync();
            return ApiResponse<bool>.Success(
                  statusCode: 200,
                  message: "Removed the member",
                 data: true
                  );

        }

       public async  Task<ApiResponse<TeamDTO>> UpdateTeamAsync(UpdateTeamDTO dto, int teamLeadId)
        {
            var team = await _repoTeam.GetByIdAsync(dto.TeamId);
            if (team == null) return ApiResponse<TeamDTO>.Fail(
                  statusCode: 404,
                  message: "Team not found",
                  type: "NotFound");
            if (team.IsDeleted) return ApiResponse<TeamDTO>.Fail(
                statusCode:409,
                message:"Team is alredy deleted",
                type:"AlreadyDeleted");

            if (team.CreatedBy != teamLeadId) return ApiResponse<TeamDTO>.Fail(
                   statusCode: 403,
                   message: "You are not authorized to update this team",
                   type: "Forbidden"
                   );
            if ((string.IsNullOrEmpty(dto.TeamName)) || (string.IsNullOrEmpty(dto.Description)))
            {
                return ApiResponse<TeamDTO>.Fail(
                    statusCode:400,
                    message:"All fields are required",
                    type:"ValidationError");
              
            }
            if (dto.MemberLimit <= 1)
            {
                return ApiResponse<TeamDTO>.Fail(statusCode: 400,
                    message: "Team required atleast two members",
                    type: "ValidationError");
            }
            _mapper.Map(dto, team);

            team.ModifiedBy = teamLeadId;
           



            await _repoTeam.UpdateAsync(team);
            await _repoTeam.SaveAsync();
            return ApiResponse<TeamDTO>.Success(
                statusCode: 200,
                message: "Team updated succeesully",
                data: null);
           

        }

       

        public async Task<IEnumerable<TeamDTO>> ViewMyTeamsAsync(int teamLeadId)
        {
            var teams = await _repoTeam.QueryByCondition(t => t.CreatedBy == teamLeadId && t.IsActive && !t.IsDeleted)
                .Include(t => t.Members)
                    .ThenInclude(m => m.User)
                        .ThenInclude(u => u.UploadedFiles)
                .ToListAsync();

            return teams.Select(t => new TeamDTO
            {
                TeamId = t.TeamId,
                TeamName = t.TeamName,
                Description=t.Description,
                InviteLink=t.InviteLink,
                MemberLimit=t.MemberLimit,
                IsActive=t.IsActive,
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
        }

        public async Task<ApiResponse<TeamDTO>> ViewMyOneTeamAsync(int teamLeadId, int teamId)
        {
            var team = await _repoTeam.QueryByCondition(t => t.TeamId == teamId && t.CreatedBy == teamLeadId && t.IsActive && !t.IsDeleted)
                .Include(t => t.Members)
                    .ThenInclude(m => m.User)
                        .ThenInclude(u => u.UploadedFiles)
                .FirstOrDefaultAsync();

            if (team == null) return ApiResponse<TeamDTO>.Fail(
                     statusCode: 404,
                     message: "Member not found",
                     type: "NotFound");

            return ApiResponse<TeamDTO>.Success(
                statusCode:200,
                message:$"View {team.TeamName}",
                data: new TeamDTO
                {
                    TeamId = team.TeamId,
                    TeamName = team.TeamName,
                    Description=team.Description,
                    InviteLink=team.InviteLink,
                    MemberLimit=team.MemberLimit,
                    IsActive=team.IsActive,
                    //InviteLink=team.InviteLink,
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
            //{
            //    TeamId = team.TeamId,
            //    TeamName = team.TeamName,
            //    Members = team.Members
            //        .Where(m => m.IsApproved)
            //        .Select(m => new TeamMemberDTO
            //        {
            //            UserId = m.UserId,
            //            UserName = m.User.Name,
            //            Role = m.Role,
            //            ProfileImg = m.User.UploadedFiles
            //                .Where(f => f.ContextType == FileContextType.ProfileImage && f.IsActive)
            //                .OrderByDescending(f => f.CreatedOn)
            //                .Select(f => f.FileData)
            //                .FirstOrDefault()
            //        }).ToList()
            //};
        }



    }
}
