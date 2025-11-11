using CollabHub.Application.DTO;
using CollabHub.Application.DTO.TeamLead;
using CollabHub.Application.DTO.TeamMember;

//using CollabHub.Application.DTO.TeamMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Interfaces.TeamMember
{
    public interface ITeamMemberService

    {

        Task<ApiResponse<JoinResponseDTO>> JoinTeamAsync(JoinRequestDTO dto, int memberId);
        Task<ApiResponse<string>> LeaveTeamAsync(int userId);

        //Task<ApiResponse<IEnumerable<TeamDTO>>> GetMyTeamsAsync(int userId);
        //Task<ApiResponse<IEnumerable<MemberTaskDTO>>> GetTasksByTeamAsync(int teamId, int memberId);
        //Task<ApiResponse<MemberTaskDTO>> GetTaskDetailAsync(int taskId, int memberId);

        //Task<ApiResponse<string>> SubmitCommitAsync(MemberCreateCommitDTO dto, int memberId);
        //Task<ApiResponse<IEnumerable<GitActivityDTO>>> GetMyCommitsAsync(int memberId);

        //Task<ApiResponse<IEnumerable<TeamMemberDTO>>> GetTeamMembersAsync(int teamId);
        //Task<ApiResponse<MemberProgressDTO>> GetMyDashboardSummaryAsync(int memberId);
    }
}
