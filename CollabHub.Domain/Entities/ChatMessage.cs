using CollabHub.Domain.Commom;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Domain.Entities
{
    public class ChatMessage:BaseEntity
    {
        [Key]
        public int MessageId { get; set; }
        public int TeamId { get; set; }
        public Team? Team { get; set; }
        public int SenderId { get; set; }
        public User Sender { get; set; }
        public string MessageText { get; set; }
        public bool IsRead { get; set; } = false;
        public int? FileId { get; set; }
        public FileResource? File { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;




    }
}
