using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.TeamLead
{
    public class ApproveMemberDTO
    {
        public int TeamMemberId { get; set; }
        public bool IsApproved { get; set; }
    }
}
