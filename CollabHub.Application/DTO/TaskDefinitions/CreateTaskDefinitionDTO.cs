using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.TaskDefinition
{
    public class CreateTaskDefinitionDTO
    {
        public int TeamId { get; set; }
        public int TaskHeadId { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpectedEndDate { get; set; }  
    }

}
