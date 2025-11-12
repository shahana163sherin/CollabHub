using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.Task
{
    public class CreateTaskHeadDTO
    {
        public string Title { get; set; }
        public int TeamId { get; set; }
        //public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime ExpectedEndDate {  get; set; }
        public DateTime? DueDate { get; set; }
    }
}
