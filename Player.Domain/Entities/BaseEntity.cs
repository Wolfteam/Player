using Player.Domain.Interfaces;

namespace Player.Domain.Entities;

public class BaseEntity : IBaseEntity
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}