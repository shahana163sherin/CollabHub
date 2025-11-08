using CollabHub.Domain.Commom;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Domain.Entities
{
    public class Team:BaseEntity
    {
        public int TeamId { get; set; }
        [Required]
        public string TeamName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public string? InviteLink { get; set; }
        [Required]
        public int MemberLimit { get; set; } = 4;
        
        public ICollection<TeamMember> Members { get; set; } = new List<TeamMember>();
        public ICollection<TaskHead> TaskHeads { get; set; } = new List<TaskHead>();
        public ICollection<ChatMessage> ChatMessages { get; set; } 

    }
}
