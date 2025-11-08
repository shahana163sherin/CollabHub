using CollabHub.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.TeamLead
{
    public class TeamMemberDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string? ProfileImg { get; set; }
        public TeamRole Role { get; set; }

    }
}
