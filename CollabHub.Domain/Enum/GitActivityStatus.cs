using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Domain.Enum
{
    public enum GitActivityStatus
    {
            Committed=1,
            Pushed=2,
            PullRequested=3,
            Reviewed=4,
            Merged=5,
            Deployed=6,
            Failed=7,
            Closed=8

    }
}
