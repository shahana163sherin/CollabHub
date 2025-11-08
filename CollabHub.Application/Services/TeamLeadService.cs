using AutoMapper;
using CollabHub.Application.DTO;
using CollabHub.Application.DTO.TeamLead;
using CollabHub.Application.Interfaces.TeamLead;
using CollabHub.Domain.Entities;
using CollabHub.Infrastructure.Repositories.EF;
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
            if (string.IsNullOrEmpty(dto.TeamName))
            {
                return new ApiResponse<TeamDTO>
                {
                    Success = false,
                    Message = "Team name is required"
                };
            }
            var inviteToken=Guid.NewGuid().ToString();
            var inviteLink = $"https://collabHub.com/join-team/{inviteToken}";
            var team = _mapper.Map<Team>(dto);
            team.InviteLink = inviteLink;
            team.CreatedBy = TeamLeadId;
            team.IsActive = true;
            await _repoTeam.AddAsync(team);
            await _repoTeam.SaveAsync();

            var teamdto = _mapper.Map<TeamDTO>(team);
            return new ApiResponse<TeamDTO>
            {
                Success = true,
                Message = "Team created successfully",
                Data = teamdto
            };



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

            return new ApiResponse<object>
            {
                Success = true,
                Message="Team Deleted"
            };
        }

        public async Task<bool> ApproveMemberAsync(ApproveMemberDTO dto, int TeamLeadid)
        {
            var member = await _teamMember.GetByIdAsync(dto.TeamMemberId);
            if (member == null)
                throw new Exception("Team member not found");

            var team = await _repoTeam.GetByIdAsync(member.TeamId);
            if (team == null)
                throw new Exception("Team not found");

            if(team.CreatedBy != TeamLeadid)
            {
                throw new UnauthorizedAccessException("You are not authorized to approve members for this team");
            }

            var teamMembers=await _teamMember.GetByConditionAsync(t=>t.TeamId==team.TeamId);
            var approvedCount = teamMembers.Count(a=>a.IsApproved);
            if(approvedCount >= team.MemberLimit && dto.IsApproved)
            {
                throw new InvalidOperationException("Member limit reached");
            }

            member.IsApproved = true;
            await _teamMember.UpdateAsync(member);
            await _teamMember.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<TeamMemberDTO>>GetTeamMembersAsync(int teamId, int teamLeadId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RejectMemberAsync(int TeamID, int memberId, int TeamLeadid)
        {
            var teamMember = (await _teamMember.GetByConditionAsync(m => m.TeamMemberId == memberId && m.TeamId == TeamID)).FirstOrDefault();
            if (teamMember == null) throw new Exception("Member Not found");

            var team = await _repoTeam.GetByIdAsync(TeamID);
            if (team == null) throw new Exception("Team not found");

            if(team.CreatedBy != TeamLeadid) throw new UnauthorizedAccessException("You are not authorized to reject members for this team");

            if (teamMember.IsApproved)
                throw new Exception("Already approved");
            if (teamMember.IsRejected)
                throw new Exception("Already rejected");

            teamMember.IsRejected = true;
            await _teamMember.UpdateAsync(teamMember);
            await _teamMember.SaveAsync();
            return true;



            
        }

        public async Task<bool> RemoveMemberAsync(int TeamID, int memberId, int TeamLeadid)
        {
            var member = (await _teamMember.GetByConditionAsync(m => m.TeamMemberId == memberId && m.TeamId == TeamID)).FirstOrDefault();
            if (member == null) throw new Exception("Member is not found");

            var team = await _repoTeam.GetByIdAsync(TeamID);
            if (team == null) throw new KeyNotFoundException("Team is not found");

            if (team.CreatedBy != TeamLeadid) throw new UnauthorizedAccessException("You are not authorized to remove this member ");

            if (member.Role == Domain.Enum.TeamRole.TeamLeader) throw new InvalidOperationException("You cannot remove the Team Lead");
            if (member.IsDeleted) throw new InvalidOperationException("Member already removed");
            member.IsDeleted = true;
            member.DeletedBy= TeamLeadid;
            member.DeletedOn=DateTime.Now;
            await _teamMember.UpdateAsync(member);
            await _teamMember.SaveAsync();
            return true;

        }

       public async  Task<ApiResponse<TeamDTO>> UpdateTeamAsync(UpdateTeamDTO dto, int teamLeadId)
        {
            var team = await _repoTeam.GetByIdAsync(dto.TeamId);
            if (team == null) throw new KeyNotFoundException("Team is not found");

            if (team.CreatedBy != teamLeadId) throw new UnauthorizedAccessException("You are not authorized to update Team ");

            _mapper.Map(dto, team);

            //team.TeamName = dto.Name;
            //if (!string.IsNullOrEmpty(dto.Description)){ 
            //team.Description = dto.Description;
            //}
            //team.IsActive = dto.IsActive;
            //team.MemberLimit = dto.MemberLimit;
            //team.ModifiedBy = teamLeadId;
            //team.ModifiedOn = DateTime.Now;

            await _repoTeam.UpdateAsync(team);
            await _repoTeam.SaveAsync();
            return new ApiResponse<TeamDTO>
            {
                Success = true,
                Message = "Team Updated Successfully",
                Data = _mapper.Map<TeamDTO>(team)
            };

        }
    }
}
