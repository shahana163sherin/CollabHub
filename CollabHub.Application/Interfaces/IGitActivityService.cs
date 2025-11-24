using CollabHub.Application.DTO;
using CollabHub.Application.DTO.Git.PullRequest;
using CollabHub.Application.DTO.Git.PushRequest;
using CollabHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Interfaces
{
    public interface IGitActivityService
    {
        Task<ApiResponse<object>> ProcessPushEventAsync(GitPushPayloadDTO payload);
        Task<ApiResponse<object>> ProcessPullRequestEventAsync(GitPullRequestPayloadDTO dto);
        //Task<GitActivity> GetActivityByCommitHashAsync(string commitHash);
        //Task<GitRepository> GetRepositoryByUrlAsync(string url);
       


        //Task UpdateTaskStatusAsync(int taskId, TaskStatus status);
    }
}
