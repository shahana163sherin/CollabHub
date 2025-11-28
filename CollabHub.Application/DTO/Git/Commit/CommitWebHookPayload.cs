using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.Git.Commit
{
    public class CommitWebhookPayload
    {
        public string CommitHash { get; set; }
        public string CommitMessage { get; set; }
        public string Branch { get; set; }
        public string RepoUrl { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
