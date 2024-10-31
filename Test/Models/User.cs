using System;
using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Bedriftsnavn er påkrevd")]
        public string CompanyName { get; set; } // Nytt felt for bedriftsnavn

        [Required(ErrorMessage = "E-mail is requiered!")]
        [EmailAddress(ErrorMessage = "invalid e-mail address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is requiered!")]
        [MinLength(10, ErrorMessage = "password has to be atleast 10 in length")]
        public string Password { get; set; }

         
         public string Bio { get; set; } 
        public string ProfileImageUrl { get; set; } // Lenke til profilbilde

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
