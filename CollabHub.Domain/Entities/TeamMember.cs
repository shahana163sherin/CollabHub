using CollabHub.Domain.Commom;
using CollabHub.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Domain.Entities
{
    public class TeamMember:BaseEntity
    {
        public int TeamMemberId { get; set; }
        public int TeamId { get; set; }
        public int UserId { get; set; }
        public TeamRole Role { get; set; }
        public bool IsApproved { get; set; }= false;
        public bool IsRejected { get; set; }=false;

        public User User { get; set; }
        public Team Team { get; set; }

    }
}
