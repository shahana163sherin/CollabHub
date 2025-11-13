using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.TaskHead
{
    public class TaskHeadFilterDTO
    {
        public int TeamId { get; set; }                    
        public string? Title { get; set; }                    
        public CollabHub.Domain.Enum.TaskStatus? Status { get; set; }              
        public DateTime? FromDate { get; set; }              
        public DateTime? ToDate { get; set; }              
        public string? SortBy { get; set; }
    }
}
