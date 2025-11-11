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
        [ForeignKey("AssignedUser")]
        public int? AssignedUserId { get; set; }
        public User AssignedUser { get; set; }
        [ForeignKey("AssignedBy")]
        public int? AssignedById { get; set; }
        public User AssignedBy { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public Enum.TaskStatus Status { get; set; } = Enum.TaskStatus.Pending;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; }
      
        public DateTime? ExtendedTo { get; set; }
        public ICollection<AiAction> AiActions { get; set; } = new List<AiAction>();
        public ICollection<GitActivity> GitActivities { get; set; } = new List<GitActivity>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    }
}
