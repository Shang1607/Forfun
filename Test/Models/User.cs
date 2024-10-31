using System;
using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        
        public required string CompanyName { get; set; } // Nytt felt for bedriftsnavn

        
        [EmailAddress(ErrorMessage = "invalid e-mail address")]
        public required string Email { get; set; }

        
        [MinLength(10, ErrorMessage = "password has to be atleast 10 in length")]
        public required string Password { get; set; }

         
         public string Bio { get; set; } 
        
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
