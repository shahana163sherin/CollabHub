using CollabHub.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.Git.PullRequest
{
    public class GitPullRequestPayloadDTO
    {
        public string action { get; set; }
        public int Number { get; set; }
        public RepositoryInfoDTO repository { get; set; }
        public PullRequestInfo pullRequest { get; set; }
        public SenderInfo sender { get; set; }
    }
}
