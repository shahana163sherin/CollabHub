using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Domain.Entities
{
    public class RefreshToken
    {
        [Key]
        public int RefreshTokenId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        [Required]
        public string Token { get; set; }
        public string? JwtId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? RevokedAt { get; set; }
        public DateTime ExpiresAt { get; set; }


        [MaxLength(500)]
        public string? ReplacedByToken { get; set; }

        public bool IsRevoked { get; set; } = false;
        public bool IsUsed { get; set; } = false;
      

    }
}
