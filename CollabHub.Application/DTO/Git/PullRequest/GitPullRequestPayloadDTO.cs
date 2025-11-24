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
        public PullRequestAction action { get; set; }
        public RepositoryInfoDTO repository { get; set; }
        public PullRequestInfo pullRequset { get; set; }
        public SenderInfo sender { get; set; }
    }
}
