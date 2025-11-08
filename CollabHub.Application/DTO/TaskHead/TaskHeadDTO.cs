using CollabHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.Task
{
    public class TaskHeadDTO
    {
        public int TaskHeadId { get; set; }
        
        public string Title { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public Team Team { get; set; }
        
        public DateTime ExpectedEndDate { get; set; }
        public TaskStatus Status { get; set; } 
        public DateTime StartDate { get; set; } 
        public DateTime DueDate { get; set; }
        public DateTime? ExtendedTo { get; set; }
        public ICollection<TaskDefinition> TaskDefinitions { get; set; } = new List<TaskDefinition>();
    }
}
