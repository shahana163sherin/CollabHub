//using CollabHub.Application.DTO;
//using CollabHub.Application.DTO.Git.PullRequest;
//using CollabHub.Application.DTO.Git.PushRequest;
//using CollabHub.Application.Interfaces;
//using CollabHub.Domain.Entities;
//using CollabHub.Domain.Enum;
//using CollabHub.Infrastructure.Repositories.EF;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;

//namespace CollabHub.Application.Services
//{
//    public class GitActivityService : IGitActivityService

//    {
//        private readonly IGitRepoRepository _repo;
//        private readonly IGitActivityRepository _activity;
//        private readonly IGenericRepository<User> _users;
//        private readonly IGenericRepository<TaskDefinition> _tasks;
//        private readonly IGenericRepository<TeamMember> _member;
//        private readonly ITaskHeadRepository _headRepo;
//        public GitActivityService(IGitRepoRepository repo,IGenericRepository<User> users, 
//            IGenericRepository<TaskDefinition> tasks,
//            IGenericRepository<TeamMember> member,
//            IGitActivityRepository activity,
//            ITaskHeadRepository headRepo)
//        {
//            _repo = repo;
//            _users = users;
//            _tasks = tasks;
//            _member = member;
//            _activity = activity;
//            _headRepo = headRepo;
//        }
//        //public Task<IEnumerable<GitActivity>> GetActivitiesByTaskIdAsync(int taskId)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //public Task<GitActivity> GetActivityByCommitHashAsync(string commitHash)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //public Task<GitRepository> GetRepositoryByUrlAsync(string url)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        public async Task<ApiResponse<object>> ProcessPushEventAsync(GitPushPayloadDTO payload)
//        {
//            var repository = await _repo.GetByUrlAsync(payload.Repository.HtmlUrl);

//            if (payload == null || payload.Repository == null || string.IsNullOrWhiteSpace(payload.Repository.HtmlUrl) ||
//                 payload.HeadCommit == null ||string.IsNullOrWhiteSpace(payload.Ref) ||  payload.Pusher == null || string.IsNullOrWhiteSpace(payload.Pusher.Email))
//            {
//                return ApiResponse<object>.Fail(400, "Invalid payload", "BadPayload");
//            }


//            var user = await _users.GetOneAsync(u => u.Email.Trim().ToLower() == payload.Pusher.Email.Trim().ToLower());
//            if (user == null) return ApiResponse<object>.Fail(404, "User not found", "NotFound");

//            var branchParts=payload.Ref.Split('/');
//            var brachName = branchParts.Last();

//            var match = Regex.Match(brachName, @"^task-(\d+)$");
//            if (!match.Success)
//                return ApiResponse<object>.Fail(400, "Invalid branch format", "BadRequest");

//            int taskId = int.Parse(match.Groups[1].Value);

           

//            var task = await _tasks.GetByIdAsync(taskId);
//            if (task == null) return ApiResponse<object>.Fail(404, "Task not found", "NotFound");

//            var assignedMember = await _member.GetByIdAsync(task.AssignedMemberId.Value);

//            if (assignedMember.UserId != user.UserId)
//                return ApiResponse<object>.Fail(403, "User is not assigned to this task", "Forbidden");

//            var existingCommit = await _activity.GetByCommitHashAsync(payload.HeadCommit.Id);
//            if (existingCommit != null)
//                return ApiResponse<object>.Fail(409, "Commit already processed", "Conflict");
//            var head = await _headRepo.GetByIdAsync(task.TaskHeadId);

//            if (repository == null)
//            {
//                string repoName = !string.IsNullOrWhiteSpace(payload.Repository.Name)
//            ? payload.Repository.Name
//            : payload.Repository.HtmlUrl.Split('/').Last();
//                repository = new GitRepository
//                {
//                    RepoName = repoName,
//                    RepoUrl = payload.Repository.HtmlUrl,
//                    UserId = user.UserId,
//                    TaskHeadId = head.TaskHeadId,
//                    BranchName = "main",
//                    CreatedOn = DateTime.Now


//                };
//                await _repo.AddAsync(repository);
//                await _repo.SaveAsync();
//            }
//            var gitActivity = new GitActivity
//            {
//                RepositoryId = repository.RepositoryId,
//                UserId = user.UserId,
//                TaskDefinitionId = taskId,
//                CommitHash = payload.HeadCommit.Id,
//                CommitMessage = payload.HeadCommit.Message,
//                BranchName = brachName,
//                CommittedAt = DateTime.Now,
//                Status = Domain.Enum.GitActivityStatus.Pushed,
                
//                TriggeredNotification = false
//            };
//            await _activity.AddAsync(gitActivity);
//            await _activity.SaveAsync();

//            repository.LastCommitId = payload.HeadCommit.Id;
//            repository.LastCommitMessage = payload.HeadCommit.Message;
//            repository.LastCommitDate = DateTime.Now;
//            repository.ModifiedOn = DateTime.Now;

//            await _repo.UpdateAsync(repository);
//            await _repo.SaveAsync();

//            task.Status = Domain.Enum.TaskStatus.InProgress;
//            task.ModifiedOn= DateTime.Now;
//            await _tasks.UpdateAsync(task);
//            await _tasks.SaveAsync();

           
//            if(head.TaskDefinitions.All(t=>t.Status == Domain.Enum.TaskStatus.Completed))
//            {
//                head.Status= Domain.Enum.TaskStatus.Completed;
//                head.ModifiedOn=DateTime.Now;
//                await _headRepo.UpdateAsync(head);
//                await _headRepo.SaveAsync();
//            }
//            return ApiResponse<object>.Success(statusCode:200,
//                 new
//            {
//                taskId,
//                commit = gitActivity.CommitHash
//            },
//             message: "Push proessed successfully");
            
//        }

//        public async Task<ApiResponse<object>> ProcessPullRequestEventAsync(GitPullRequestPayloadDTO dto)
//        {

//            if (dto == null) return ApiResponse<object>.Fail(400, "Payload is null", "NULL");


//            if (!Enum.IsDefined(typeof(PullRequestAction), dto.action))
//                return ApiResponse<object>.Fail(400, "Invalid pull request action", "InvalidAction");
//            if (dto.pullRequset == null)
//                return ApiResponse<object>.Fail(404, "Pull request data is missing", "NotFound");

//            if (dto.pullRequset.Head == null || string.IsNullOrWhiteSpace(dto.pullRequset.Head.Ref))
//                return ApiResponse<object>.Fail(404, "Head branch is missing", "NotFound");

//            var branchName = dto.pullRequset.Head.Ref;
//            var match = Regex.Match(branchName, @"^task-(\d+)$", RegexOptions.IgnoreCase);
//            if (!match.Success)
//                return ApiResponse<object>.Fail(400, "Branch name does not match task-{id} format", "BadRequest");
//            int taskDefinitionId = int.Parse(match.Groups[1].Value);

//            if (dto.action == PullRequestAction.closed && dto.pullRequset.Merged == false)
//                return ApiResponse<object>.Fail(400, "Pull requset closed but not merged", "NotMerged");

//            if (dto.repository == null || string.IsNullOrWhiteSpace(dto.repository.Html_Url))
//                return ApiResponse<object>.Fail(404, "Repository information is missing", "NotFound");
//            if (dto.sender == null || string.IsNullOrWhiteSpace(dto.sender.Email))
//                return ApiResponse<object>.Fail(404, "Sender information missing", "NotFound");

//            var taskDef = await _tasks.GetByIdAsync(taskDefinitionId);
//            if (taskDef == null)
//                return ApiResponse<object>.Fail(404, $"Task {taskDefinitionId} not found", "NotFound");

//            var repository = await _repo.GetOneAsync(t => t.TaskHeadId == taskDef.TaskHeadId);
//            if (repository == null) return ApiResponse<object>.Fail(404, "Repository not found for this task head", "NoyFound");

//            var user = await _users.GetOneAsync(u => u.Email.Trim().ToLower() == dto.sender.Email.Trim().ToLower());
//            if (user == null) return ApiResponse<object>.Fail(404, "User not found", "NotFound");
//            switch (dto.action)
//            {
//                case PullRequestAction.opened:
//                    taskDef.Status = Domain.Enum.TaskStatus.InReview;
//                    break;

//                case PullRequestAction.closed:
//                    if (dto.pullRequset.Merged == true)
//                    {
//                        taskDef.Status = Domain.Enum.TaskStatus.Completed;
//                        var head = await _headRepo.GetByIdAsync(taskDef.TaskHeadId);
//                        if (head.TaskDefinitions.All(t => t.Status == Domain.Enum.TaskStatus.Completed))
//                        {
//                            head.Status = Domain.Enum.TaskStatus.Completed;
//                            head.ModifiedOn=DateTime.Now;
//                            await _headRepo.UpdateAsync(head);
//                            await _headRepo.SaveAsync();

//                        }

//                    }
//                    else
//                    {
//                        taskDef.Status = Domain.Enum.TaskStatus.Cancelled;
//                    }
//                    break;
//                default:
//                    return ApiResponse<object>.Fail(400, "Unsupported action", "InvalidAction");
//            }
//            taskDef.ModifiedOn= DateTime.Now;
//            await _tasks.UpdateAsync(taskDef);
//            await _tasks.SaveAsync();

//            var gitActivity = new GitActivity
//            {
//                RepositoryId = repository.RepositoryId,
//                UserId = user.UserId,
//                TaskDefinitionId = taskDef.TaskDefinitionId,
//                CommitHash = dto.pullRequset.Number.ToString(),
//                CommitMessage = dto.pullRequset.Title,
//                BranchName = branchName,
//                CommittedAt = DateTime.UtcNow,
//                Status = dto.action == PullRequestAction.opened
//                    ? GitActivityStatus.PullRequested
//                    : dto.pullRequset.Merged
//                        ? GitActivityStatus.Merged
//                        : GitActivityStatus.Closed
//            };
//            await _activity.AddAsync(gitActivity);
//            await _activity.SaveAsync();
//            return ApiResponse<object>.Success(200, new
//            {
//                taskId = taskDef.TaskDefinitionId,
//                action = dto.action.ToString(),
//                taskStatus = taskDef.Status.ToString()
//            }, "Pull request processed successfully");



//        }

//        //public Task UpdateTaskStatusAsync(int taskId, TaskStatus status)
//        //{
//        //    throw new NotImplementedException();
//        //}
//    }
//}
