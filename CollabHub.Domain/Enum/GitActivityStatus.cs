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
          
            Merged=4,
           
            Failed=5,
            Closed=6

    }
}
