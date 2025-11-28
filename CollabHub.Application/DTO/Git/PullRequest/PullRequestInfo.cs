using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.Git.PullRequest
{
    public class PullRequestInfo
    {
        public int Number { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool Merged { get; set; }
        public DateTime? Merged_At { get; set; }
        public BranchInfo Head { get; set; }
        public BranchInfo Base { get; set; }
        public string Html_Url { get; set; }
    }
}
