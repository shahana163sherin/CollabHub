using CollabHub.Application.DTO;
using CollabHub.Application.DTO.TeamLead;
using CollabHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Interfaces.TeamLead
{
    public interface ITeamLeadService
    {
        Task<ApiResponse<TeamDTO>> CreateTeamAsync(CreateTeamDTO dto, int TeamLeadId);
        Task<ApiResponse<TeamDTO>> UpdateTeamAsync(UpdateTeamDTO dto, int teamLeadId);
        Task<ApiResponse<object>>RemoveTeamAsync(int TeamID, int TeamLeadId);
        Task<IEnumerable<TeamDTO>> ViewMyTeamsAsync(int teamLeadId);
        Task<TeamDTO> ViewMyOneTeamAsync(int teamLeadId, int teamId);


        Task<bool> ApproveMemberAsync(ApproveMemberDTO dto,int TeamLeadid);
        Task<bool> RejectMemberAsync(RejectMemberDTO dto, int TeamLeadid);
        Task<bool> RemoveMemberAsync(int TeamId, int MemberId, int TeamLeadid);
        Task<IEnumerable<TeamMemberDTO>> GetTeamMembersAsync(TeamMemberFilterDTO dto,int teamLeadId);





        




    }
}
