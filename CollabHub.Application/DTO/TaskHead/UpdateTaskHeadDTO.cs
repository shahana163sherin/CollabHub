using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.Task
{
    public class UpdateTaskHeadDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? ExpectedEndDate { get; set; }
        public TaskStatus? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? Duedate { get; set; }
        public DateTime? ExtendedTo { get; set; }
    }
}
