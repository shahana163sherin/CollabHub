using CollabHub.Application.DTO;
using CollabHub.Application.DTO.Git.Commit;
using CollabHub.Application.DTO.Git.PullRequest;
using CollabHub.Application.DTO.Git.PushRequest;
using CollabHub.Application.Interfaces.Git;
using CollabHub.Domain.Entities;
using CollabHub.Infrastructure.Repositories.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Services
{
    public class GitService : IGitService
    {
        private readonly IGitActivityRepository _activityRepo;
        private readonly IGenericRepository<GitRepository> _gitrepo;
        private readonly IGenericRepository<User> _userRepo;

        public GitService(IGitActivityRepository activityRepo,
             IGenericRepository<GitRepository> gitrepo,
              IGenericRepository<User> userRepo)
        {
            _activityRepo = activityRepo;
            _userRepo = userRepo;
            _gitrepo = gitrepo;
        }
        public Task<ApiResponse<object>> ProcessCommitWebHookAsync(CommitWebhookPayload commit)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<object>> ProcessPullRequestEventAsync(GitPullRequestPayloadDTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<object>> ProcessPushEventAsync(GitPushPayloadDTO payload)
        {
            if (payload == null)
                return ApiResponse<object>.Fail(400, "Payload cannot be null", "Null");
            var latestCommit = payload.after;
            if (string.IsNullOrEmpty(latestCommit))
                return ApiResponse<object>.Fail(404,"No commit hash found in payload","NotFound");

            var repository = await _gitrepo.GetOneAsync(r => r.RepoName.ToLower().Trim() == payload.Repository.Full_Name.ToLower().Trim());
            if (repository == null)
                return ApiResponse<object>.Fail(404, "Repository not found", "NotFound");
            int repositoryID = repository.RepositoryId;

            var user = await _userRepo.GetOneAsync(u => u.Email.ToLower().Trim() == payload.Pusher.Email.ToLower().Trim());
            if (user == null)
                return ApiResponse<object>.Fail(404, "User not found", "NotFound");
            int userId = user.UserId;

            var branch = payload.Ref.Replace("refs/heads/", "");
            int? taskDefinitionId = ExtractTaskId(branch);

            if (taskDefinitionId == null)
                return ApiResponse<object>.Fail(404, "TaskId not found", "NotFound");

            var activity = new GitActivity
            {
                RepositoryId = repositoryID,
                UserId = userId,
                TaskDefinitionId = taskDefinitionId,
                EventType = Domain.Enum.GitEventType.Push,
                Status = Domain.Enum.GitActivityStatus.Pushed,
                CommitHash = payload.after,
                CommitMessage = payload.HeadCommit?[0]?.Message,
                BranchName = branch,
                CommittedAt = payload.HeadCommit?[0]?.TimeStamp ?? DateTime.Now,
                CreatedOn = DateTime.Now




            };
            await _activityRepo.AddAsync(activity);
            await _activityRepo.SaveAsync();

            return ApiResponse<object>.Success(200, new
            {
                RepositoryId = repositoryID,
                TaskId = taskDefinitionId,
                UserId = userId,
                Commit = payload.after,
                Branch = branch
            });
        }

        private int ExtractTaskId(string branch)
        {
            if (string.IsNullOrEmpty(branch)) return 0;
            branch = branch.Replace("/refs/heads/", "");
            if (branch.StartsWith("task-"))
            {
                var parts = branch.Split('-');
                if(parts.Length > 1 && int.TryParse(parts[1],out int taskId))
                {
                    return taskId;
                }
            }
            return 0;
        }
    }
}
