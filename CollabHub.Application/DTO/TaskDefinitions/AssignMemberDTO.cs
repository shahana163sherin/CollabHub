using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.TaskDefinitions
{
    public class AssignMemberDTO
    {
        public int TaskDefinitionId { get; set; }
        public List<int> MemberIds { get; set; } = new();
    }

}
