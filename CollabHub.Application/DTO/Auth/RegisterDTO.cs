using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.DTO.Auth
{
    public class RegisterDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Enter valid Email")]
        public string Email { get; set; }
        [MinLength(6, ErrorMessage = "Password contain minimum 6 characters")]
        [Required]
        public string Password { get; set; }
        public string? ProfileImg { get; set; }


    }
}
