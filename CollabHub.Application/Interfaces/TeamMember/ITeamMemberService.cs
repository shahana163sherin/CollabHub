using CollabHub.Application.DTO;
using CollabHub.Application.DTO.Task;
using CollabHub.Application.DTO.TaskDefinition;
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

        Task<IEnumerable<TeamDTO>> ViewMyTeamsAsync(int userId);
        Task<TeamDTO> ViewTeamById(int teamId, int userId);
        Task<IEnumerable<TaskHeadDTO>> GetTasksByTeamAsync(int teamId, int memberId);
        Task<IEnumerable<TaskDefinitionDTO>> ViewMyAssignedTask(int memberId);
        //Task<ApiResponse<MemberTaskDTO>> GetTaskDetailAsync(int taskId, int memberId);

        //Task<ApiResponse<string>> SubmitCommitAsync(MemberCreateCommitDTO dto, int memberId);
        //Task<ApiResponse<IEnumerable<GitActivityDTO>>> GetMyCommitsAsync(int memberId);

        //Task<ApiResponse<IEnumerable<TeamMemberDTO>>> GetTeamMembersAsync(int teamId,int userId);
        //Task<ApiResponse<MemberProgressDTO>> GetMyDashboardSummaryAsync(int memberId);
    }
}
