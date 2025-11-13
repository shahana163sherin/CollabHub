using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO
{
    public class UpdateProfileDTO
    {
        public string? Name { get; set; }
        public string? ProfileImage { get; set; }
        public string? Qualification {  get; set; }
    }
}
