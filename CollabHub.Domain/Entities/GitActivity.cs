using CollabHub.Domain.Commom;
using CollabHub.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Domain.Entities
{
    public class GitActivity:BaseEntity
    {
        public int GitActivityId { get; set; }
        public int RepositoryId { get; set; }
        public GitRepository Repository { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int? TaskDefinitionId { get; set; }

        public TaskDefinition TaskDefinition { get; set; }
        [Required]
        public string CommitHash { get; set; }
        public string? CommitMessage { get; set; }
        public string BranchName { get; set; }
        public DateTime CommittedAt { get; set; }
        public GitEventType EventType { get; set; }
        public GitActivityStatus? Status { get; set; }
        public bool TriggeredNotification { get; set; } = false;
        public int? PullRequestId { get; set; }
        public string? PullRequestUrl { get; set; }
        public string? PullRequestAction { get; set; }
        public string? DetectedTaskCode { get; set; } 
        public bool TaskIdExtracted { get; set; } = false;
        //public string? SourceBranch { get; set; }
        //public string? TargetBranch { get; set; }


    }
}
