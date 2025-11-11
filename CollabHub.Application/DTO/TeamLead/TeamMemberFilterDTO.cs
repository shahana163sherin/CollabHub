using CollabHub.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.TeamLead
{
    public class TeamMemberFilterDTO
    {
        public int TeamId { get; set; }
        public TeamRole? Role { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsRejected { get; set; }
        public string? SearchTerm { get; set; }
        //public bool? IsDeleted { get; set; }
    }
}
