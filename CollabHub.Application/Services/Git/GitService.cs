using CollabHub.Application.DTO;
using CollabHub.Application.DTO.Git.Commit;
using CollabHub.Application.DTO.Git.GitPush;
using CollabHub.Application.DTO.Git.Pull;
using CollabHub.Application.Interfaces.Git;
using CollabHub.Domain.Entities;
using CollabHub.Domain.Enum;
using CollabHub.Infrastructure.Repositories.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Services.Git
{
    public class GitService : IGitService
    {
        private readonly IGitActivityRepository _activityRepo;
        private readonly IGenericRepository<GitRepository> _gitRepo;
        private readonly IGenericRepository<User> _userRepo;
        private readonly ITaskStatusService _taskStatusService;

        public GitService(
            IGitActivityRepository activityRepo,
            IGenericRepository<GitRepository> gitRepo,
            IGenericRepository<User> userRepo,
            ITaskStatusService taskStatusService)
        {
            _activityRepo = activityRepo;
            _gitRepo = gitRepo;
            _userRepo = userRepo;
            _taskStatusService = taskStatusService;
        }

        public async Task<ApiResponse<object>> ProcessPushEventAsync(GitPushPayloadDTO payload)
        {
            if (payload == null)
                return ApiResponse<object>.Fail(400, "Payload cannot be null", "Null");

            var latestCommit = payload.After;
            if (string.IsNullOrEmpty(latestCommit))
                return ApiResponse<object>.Fail(404, "No commit hash found", "NotFound");

            var repo = await _gitRepo.GetOneAsync(r => r.RepoName.ToLower().Trim() == payload.Repository.Full_Name.ToLower().Trim());
            if (repo == null) return ApiResponse<object>.Fail(404, "Repository not found", "NotFound");

            var user = await _userRepo.GetOneAsync(u => u.Email.ToLower().Trim() == payload.Pusher.Email.ToLower().Trim());
            if (user == null) return ApiResponse<object>.Fail(404, "User not found", "NotFound");

            var branch = payload.Ref.Replace("refs/heads/", "");
            int? taskId = ExtractTaskId(branch);
            if (taskId == null) return ApiResponse<object>.Fail(404, "TaskId not found", "NotFound");

            var activity = new GitActivity
            {
                RepositoryId = repo.RepositoryId,
                UserId = user.UserId,
                TaskDefinitionId = taskId,
                EventType = GitEventType.Push,
                Status = GitActivityStatus.Pushed,
                CommitHash = latestCommit,
                CommitMessage = payload.HeadCommit?.FirstOrDefault()?.Message ?? "No commit message",
                BranchName = branch,
                CommittedAt = payload.HeadCommit?.FirstOrDefault()?.TimeStamp ?? DateTime.UtcNow,
                CreatedOn = DateTime.UtcNow
            };

            await _activityRepo.AddAsync(activity);
            await _activityRepo.SaveAsync();

            await _taskStatusService.UpdateSubTasksFromActivityAsync(taskId.Value, activity);

            return ApiResponse<object>.Success(200, new
            {
                RepositoryId = repo.RepositoryId,
                TaskId = taskId,
                UserId = user.UserId,
                Commit = latestCommit,
                Branch = branch
            });
        }

        public async Task<ApiResponse<object>> ProcessCommitWebHookAsync(CommitWebhookPayloadDTO payload)
        {
            if (payload == null)
                return ApiResponse<object>.Fail(400, "Payload cannot be null", "Null");

            if (string.IsNullOrEmpty(payload.CommitHash))
                return ApiResponse<object>.Fail(400, "CommitHash missing", "BadRequest");

            var repo = await _gitRepo.GetOneAsync(r => r.RepoName.ToLower().Trim() == payload.Repository.Full_Name.ToLower().Trim());
            if (repo == null) return ApiResponse<object>.Fail(404, "Repository not found", "NotFound");

            var user = await _userRepo.GetOneAsync(u => u.Email.ToLower().Trim() == payload.Author.Email.ToLower().Trim());
            if (user == null) return ApiResponse<object>.Fail(404, "User not found", "NotFound");

            var branch = payload.Branch;
            int? taskId = ExtractTaskId(branch);
            if (taskId == null) return ApiResponse<object>.Fail(404, "TaskId not found", "NotFound");

            var activity = new GitActivity
            {
                RepositoryId = repo.RepositoryId,
                UserId = user.UserId,
                TaskDefinitionId = taskId,
                EventType = GitEventType.Commit,
                Status = GitActivityStatus.Committed,
                CommitHash = payload.CommitHash,
                CommitMessage = payload.CommitMessage,
                BranchName = branch,
                CommittedAt = payload.Timestamp,
                CreatedOn = DateTime.UtcNow
            };

            await _activityRepo.AddAsync(activity);
            await _activityRepo.SaveAsync();

            await _taskStatusService.UpdateSubTasksFromActivityAsync(taskId.Value, activity);

            return ApiResponse<object>.Success(200, new
            {
                RepositoryId = repo.RepositoryId,
                TaskId = taskId,
                UserId = user.UserId,
                CommitHash = payload.CommitHash,
                Branch = branch
            });
        }

        public async Task<ApiResponse<object>> ProcessPullRequestEventAsync(GitPullRequestPayloadDTO dto)
        {
            if (dto == null) return ApiResponse<object>.Fail(400, "Payload cannot be null", "Null");

            GitActivityStatus status;
            switch (dto.Action.ToLower())
            {
                case "opened":
                    status = GitActivityStatus.PullRequested;
                    break;
                case "closed":
                    status = dto.PullRequest.Merged ? GitActivityStatus.Merged : GitActivityStatus.Closed;
                    break;
                default:
                    status = GitActivityStatus.Failed;
                    break;
            }

            int? taskId = ExtractTaskId(dto.PullRequest.Head.Ref);
            if (taskId == null) return ApiResponse<object>.Fail(404, "TaskId not found", "NotFound");

            var activity = new GitActivity
            {
                RepositoryId = dto.Repository.RepoId,
                UserId = dto.Sender.UserId,
                TaskDefinitionId = taskId,
                EventType = GitEventType.PullRequest,
                Status = status,
                CommitHash = dto.PullRequest.Number.ToString(),
                CommitMessage = dto.PullRequest.Title,
                BranchName = dto.PullRequest.Head.Ref,
                CommittedAt = dto.PullRequest.Merged_At ?? DateTime.UtcNow,
                CreatedOn = DateTime.UtcNow
            };

            await _activityRepo.AddAsync(activity);
            await _activityRepo.SaveAsync();

            await _taskStatusService.UpdateSubTasksFromActivityAsync(taskId.Value, activity);

            return ApiResponse<object>.Success(200, new
            {
                RepositoryId = dto.Repository.RepoId,
                TaskId = taskId,
                UserId = dto.Sender.UserId,
                PullRequestNumber = dto.PullRequest.Number,
                Status = status.ToString()
            });
        }

        private int? ExtractTaskId(string branch)
        {
            if (string.IsNullOrEmpty(branch)) return null;

            branch = branch.Replace("refs/heads/", "");
            if (branch.StartsWith("task-"))
            {
                var parts = branch.Split('-');
                if (parts.Length > 1 && int.TryParse(parts[1], out int taskId))
                    return taskId;
            }

            return null;
        }
    }

}
