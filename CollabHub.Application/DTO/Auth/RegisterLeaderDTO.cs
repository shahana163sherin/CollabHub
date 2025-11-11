using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.Auth
{
    public class RegisterLeaderDTO:RegisterDTO
    {
        [Required]
        public string Qualification { get; set; }
    }
}
