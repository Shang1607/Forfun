using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public string? ProfileImageUrl { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;

        
        public string? Bio { get; set; }

        [NotMapped]
        public object PasswordHash { get; internal set; }
    }
}
