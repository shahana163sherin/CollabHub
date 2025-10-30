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
        Completed=3,
        Reassigned=4,
        Cancelled=5
    }
}
