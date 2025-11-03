using CollabHub.Domain.Commom;
using CollabHub.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Domain.Entities
{
    public class Notification:BaseEntity
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int SenderId { get; set; }
        public User Sender { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
        public NotificationEntityType? NotificationEntityType { get; set; }
        public int? EntityId { get; set; }

    }
}
