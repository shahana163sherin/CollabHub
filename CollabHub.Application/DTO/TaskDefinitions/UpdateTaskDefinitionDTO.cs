using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.TaskDefinition
{
    public class UpdateTaskDefinitionDTO
    {
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ExtendedTo { get; set; }
        public Domain.Enum.TaskStatus? Status { get; set; }
    }

}
