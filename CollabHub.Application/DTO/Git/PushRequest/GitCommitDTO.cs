using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.Git.PushRequest
{
    public class GitCommitDTO
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Url { get; set; }

    }
}
