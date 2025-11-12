using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.TaskHead
{
    public class MemberDTO
    {
        public int Id { get; set; }        
        public string Name { get; set; }     
        public string Email { get; set; }    
        public bool IsApproved { get; set; }
    }
}
