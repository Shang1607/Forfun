using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace YourNamespace.ViewModels
{
    public class EditProfileViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Selskapets navn er påkrevd")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "E-post er påkrevd")]
        [EmailAddress(ErrorMessage = "Ugyldig e-postadresse")]
        public string Email { get; set; }

        // For å vise og beholde eksisterende profilbilde
        public string ExistingProfileImageUrl { get; set; }

        // For opplasting av nytt profilbilde (valgfritt)
        public IFormFile ProfileImageFile { get; set; }


    }
}
