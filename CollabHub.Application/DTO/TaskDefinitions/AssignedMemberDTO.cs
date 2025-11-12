using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.TaskDefinitions
{
    public class AssignedMemberDTO
    {
        public int MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public DateTime AssignedOn { get; set; }
        public int AssignedById { get; set; }
        public string AssignedByName { get; set; } = string.Empty;
    }
}
