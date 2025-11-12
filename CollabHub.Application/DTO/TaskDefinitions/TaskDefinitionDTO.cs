using CollabHub.Application.DTO.TaskDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.TaskDefinition
{
    public class TaskDefinitionDTO
    {
        public int TaskDefinitionId { get; set; }
        public int TaskHeadId { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ExtendedTo { get; set; }
        public TaskStatus Status { get; set; }

       
        public List<AssignedMemberDTO>? AssignedMembers { get; set; }
    }

}
