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
    public class Complaint:BaseEntity
    {
        [Key]
        public int ComplaintId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string? Subject { get; set; }
        [Required]
        [MaxLength(2000)]
        public string Description { get; set; }
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
        public int? AssignedAdminId { get; set; }
        public User? AssignedAdmin { get; set; }
        public string? ResolutionNote { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public Status Status { get; set; } = Status.pending;

    }
}
