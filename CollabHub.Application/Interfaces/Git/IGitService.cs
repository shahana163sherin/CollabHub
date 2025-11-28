using CollabHub.Application.DTO;
using CollabHub.Application.DTO.Git.Commit;
using CollabHub.Application.DTO.Git.PullRequest;
using CollabHub.Application.DTO.Git.PushRequest;
using CollabHub.Domain.Entities;
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
        Task<ApiResponse<object>> ProcessPullRequestEventAsync(GitPullRequestPayloadDTO dto);
        Task<ApiResponse<object>> ProcessCommitWebHookAsync(CommitWebhookPayload commit);
    }
}
