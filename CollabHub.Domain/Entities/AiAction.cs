using CollabHub.Domain.Commom;
using CollabHub.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Domain.Entities
{
    public class AiAction:BaseEntity
    {
        public int AiActionId { get; set; }
        public int TaskDefinitionId { get; set; }
        public TaskDefinition TaskDefinition { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public AiActivityType ActivityType { get; set; }
        public string? Details { get; set; }


    }
}
