using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.Git.GitPush
{
    public class GitPushPayloadDTO
    {
        public GitRepoDTO Repository { get; set; }
        public string Ref { get; set; }
        public string Before { get; set; }
        public string After { get; set; }
        public List<GitCommitDTO> HeadCommit { get; set; }
        public GitPusherDTO Pusher { get; set; }
    }

    public class GitRepoDTO
    {
        public string Full_Name { get; set; }
        public string HtmlUrl { get; set; }
    }

    public class GitCommitDTO
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Url { get; set; }
    }

    public class GitPusherDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
