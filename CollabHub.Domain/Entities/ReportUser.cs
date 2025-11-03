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
    public class ReportUser:BaseEntity
    {
        [Key]
        public int ReportId { get; set; }
        public int ReportedUserId { get; set; }
        public User ReportedUser { get; set; }
        public int ReportedAgainstUserId { get; set; }
        public User ReportedAgainstUser { get; set; }
        [MaxLength(500)]
        public string Reason { get; set; }
        [MaxLength(2000)]
        public string? Description { get; set; }
        public Status Status { get; set; } = Status.pending;
        public string? AdminAction { get; set; }
        public DateTime? ReviewedAt { get; set; }
    }
}
