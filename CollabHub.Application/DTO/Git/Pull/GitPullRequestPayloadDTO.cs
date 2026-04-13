using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.Git.Pull
{
    public class GitPullRequestPayloadDTO
    {
        public string Action { get; set; }
        public PullRequestInfo PullRequest { get; set; }
        public RepositoryInfoDTO Repository { get; set; }
        public SenderInfo Sender { get; set; }
    }

    public class PullRequestInfo
    {
        public int Number { get; set; }
        public string Title { get; set; }
        public bool Merged { get; set; }
        public DateTime? Merged_At { get; set; }
        public BranchInfo Head { get; set; }
    }

    public class RepositoryInfoDTO
    {
        public int RepoId { get; set; }
        public string Name { get; set; }
        public string Full_Name { get; set; }
        public string Html_Url { get; set; }
    }

    public class BranchInfo
    {
        public string Ref { get; set; }
    }

    public class SenderInfo
    {
        public int UserId { get; set; }
        public string Login { get; set; }
    }
}
