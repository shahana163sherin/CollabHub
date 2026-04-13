using CollabHub.Application.DTO.Git.GitPush;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.Git.Commit
{
    public class CommitWebhookPayloadDTO
    {
        public string CommitHash { get; set; }
        public string CommitMessage { get; set; }
        public string Branch { get; set; }
        public DateTime Timestamp { get; set; }
        public GitRepoDTO Repository { get; set; }
        public GitPusherDTO Author { get; set; }
    }
}
