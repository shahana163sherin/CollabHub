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
    public class FeedBack:BaseEntity
    {
        [Key]
        public int FeedbackId { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [MaxLength(150)]
        public string? Title { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; }

        public int? Rating { get; set; }

        [Required]
        [MaxLength(50)]
        public Status Status { get; set; } = Status.pending;

        [MaxLength(1000)]
        public string? AdminResponse { get; set; }


    }
}
