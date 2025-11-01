using CollabHub.Domain.Commom;
using CollabHub.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Domain.Entities
{
    public class RegisterAudit
    {
        [Key]
        public int RegisterAuditId { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        [Required, MaxLength(150)]
        public string Email { get; set; }

        [Required]
        public UserRole Role { get; set; }

        public DateTime RegisteredOn { get; set; } = DateTime.UtcNow;
        public User User { get; set; }


    }
}
