using System.ComponentModel.DataAnnotations;

namespace Test.Models;
public class User
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Firmanavn er påkrevd")]
    public required string CompanyName { get; set; }

    [Required(ErrorMessage = "E-post er påkrevd")]
    [EmailAddress(ErrorMessage = "Ugyldig e-postadresse")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Passord er påkrevd")]
    [MinLength(6, ErrorMessage = "Passord må være minst 6 tegn")]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    public string? ProfileImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? Bio { get; set; }

    public string? PasswordHash { get; set; }
}
