using CollabHub.Application.DTO;
using CollabHub.Application.DTO.Git.Commit;
using CollabHub.Application.DTO.Git.GitPush;
using CollabHub.Application.DTO.Git.Pull;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Interfaces.Git
{
    public interface IGitService
    {
        Task<ApiResponse<object>> ProcessPushEventAsync(GitPushPayloadDTO payload);
        Task<ApiResponse<object>> ProcessCommitWebHookAsync(CommitWebhookPayloadDTO payload);
        Task<ApiResponse<object>> ProcessPullRequestEventAsync(GitPullRequestPayloadDTO dto);
    }
}
