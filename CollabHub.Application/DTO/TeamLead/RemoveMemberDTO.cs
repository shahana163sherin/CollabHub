using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.TeamLead
{
    public class RemoveMemberDTO
    {
        public int TeamId { get; set; }
        public int MemberId { get; set; }
    }
}
