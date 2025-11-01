using CollabHub.Domain.Commom;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Domain.Entities
{
    public class TaskDefinition:BaseEntity
    {
        [Key]
        public int TaskDefinitionId { get; set; }
        [ForeignKey("TaskHead")]
        public int TaskHeadId { get; set; }
        public TaskHead TaskHead { get; set; }
        [Required]
        public string Description { get; set; }
        [ForeignKey("User")]
        public int AssignedTo { get; set; }
        public User User { get; set; }
        public DateTime ExpectedEdndDate { get; set; }
        public Enum.TaskStatus Status { get; set; } = Enum.TaskStatus.Pending;
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; }
        //public DateTime? CompletedAt { get; set; }
        public DateTime? ExtendedTo { get; set; }

    }
}
