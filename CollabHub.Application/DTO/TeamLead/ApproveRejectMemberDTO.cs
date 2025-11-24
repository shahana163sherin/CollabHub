using CollabHub.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.TeamLead
{
    public class ApproveRejectMemberDTO
    {
        public int MemberId { get; set; }
        //public int teamId { get; set; }
        public MemberAction Action { get; set; }
    }
}
