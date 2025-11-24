using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.Git.PushRequest
{
    public class GitPushPayloadDTO
    {
        public GitRepoDTO Repository { get; set; }
        public string Ref { get; set; }
        public GitCommitDTO HeadCommit { get; set; }
        public GitPusherDTO Pusher { get; set; }
    }
}
