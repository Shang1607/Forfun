

 /* using System.ComponentModel.DataAnnotations;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Gammelt passord er påkrevd")]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "Nytt passord er påkrevd")]
    [MinLength(6, ErrorMessage = "Passordet må være minst 6 tegn")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Bekreft nytt passord")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Passordene samsvarer ikke")]
    public string ConfirmPassword { get; set; }
}
 */