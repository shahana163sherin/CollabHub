using CollabHub.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.TeamMember
{
    public class JoinResponseDTO
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public string Role { get; set; }

    }
}
