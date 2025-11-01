using CollabHub.Domain.Commom;
using CollabHub.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Domain.Entities
{
    public class User:BaseEntity
    {
        public int UserId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress(ErrorMessage ="Enter valid Email")]
        public string Email { get; set; }
        [MinLength(6,ErrorMessage ="Password contain minimum 6 characters")]
        [Required]
        public string Password { get; set; }
        public string? ProfileImg { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } 
        public DateTime? LastLoginedAt { get; set; }
        public DateTime? LastEmailNotifiedAt { get; set; }
        public string? Qualification { get; set; }
        public int? TeamId { get; set; }
        public Team Team { get; set; }
        public RegisterAudit RegisterAudit { get; set; }
        public ICollection<LoginAudit> LoginAudits { get; set; } = new List<LoginAudit>();
        public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();

    }
}
