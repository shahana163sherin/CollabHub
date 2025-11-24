using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Domain.Enum
{
    public enum TaskStatus
    {
        Pending=1,
        InProgress=2,
        InReview=3,
        Approved=4,
        Completed=5,
        Reassigned=6,
        Cancelled=7
    }
}
