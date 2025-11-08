using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.TeamLead
{
    public class TeamDTO
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public string? InviteLink { get; set; }
        public int MemberLimit { get; set; }
        public List<TeamMemberDTO> Members { get; set; } = new List<TeamMemberDTO>();
    }
}
