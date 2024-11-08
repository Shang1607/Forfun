using System.ComponentModel.DataAnnotations;

public class EditProfileViewModel
{
    [Required(ErrorMessage = "Firmanavn er påkrevd")]
    public string CompanyName { get; set; }

    public string Bio { get; set; }

    // For å vise eksisterende profilbilde
    public string? ExistingProfileImageUrl { get; set; }
}