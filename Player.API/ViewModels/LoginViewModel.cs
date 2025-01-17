using System.ComponentModel.DataAnnotations;
using Player.Domain;

namespace Player.API.ViewModels;

public class LoginViewModel
{
    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [MaxLength(100)]
    [RegularExpression(
        AppConstants.PasswordRegex,
        ErrorMessage = "Password is not valid, it must contain only letters and digits, min 8 characters and max 16")]
    public string? Password { get; set; }

    public string? ReturnUrl { get; set; }

    public static LoginViewModel FromReturnUrl(string? url) => new()
    {
        ReturnUrl = url
    };

    public static LoginViewModel FromOther(LoginViewModel other) => new()
    {
        Email = other.Email,
        Password = other.Password,
        ReturnUrl = other.ReturnUrl
    };
}