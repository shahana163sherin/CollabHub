using CollabHub.Domain.Commom;
using CollabHub.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CollabHub.Domain.Entities
{
    public class TaskHead:BaseEntity
    {
        [Key]
        public int TaskHeadId { get; set; }
        [Required]
        public string Title { get; set; }
        public int TeamId { get; set; }
        [JsonIgnore]
        public Team Team { get; set; }
        [Required]
        public DateTime ExpectedEndDate { get; set; }
        public Enum.TaskStatus Status { get; set; } = Enum.TaskStatus.Pending;
        public DateTime StartDate { get; set; }= DateTime.Now;
        public DateTime DueDate { get; set; }
        public DateTime? ExtendedTo { get; set; }
        public ICollection<TaskDefinition> TaskDefinitions { get; set; }=new List<TaskDefinition>();
        public ICollection<GitRepository> GitRepositories { get; set; }=new List<GitRepository>();


    }
}
