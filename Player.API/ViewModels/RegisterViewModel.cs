using System.ComponentModel.DataAnnotations;

namespace Player.API.ViewModels;

public class RegisterViewModel : LoginViewModel
{
    [Required]
    [MaxLength(100)]
    public string? FirstName { get; set; }

    [Required]
    [MaxLength(100)]
    public string? LastName { get; set; }

    public bool? Registered { get; set; }

    public bool? AlreadyExists { get; set; }

    public new static RegisterViewModel FromReturnUrl(string? url) => new()
    {
        ReturnUrl = url
    };

    public static RegisterViewModel FromOther(RegisterViewModel other) => new()
    {
        Email = other.Email,
        Password = other.Password,
        ReturnUrl = other.ReturnUrl,
        FirstName = other.FirstName,
        LastName = other.LastName
    };
}