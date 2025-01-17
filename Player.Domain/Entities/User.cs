using Microsoft.AspNetCore.Identity;
using Player.Domain.Interfaces;

namespace Player.Domain.Entities;

public class User : IdentityUser<long>, IBaseEntity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}