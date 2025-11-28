using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Domain.Enum
{
    public enum GitEventType
    {
        Commit = 1,
        Push = 2,
        PullRequest = 3,
        Merge = 4
    }

}
