namespace Player.Domain.Entities;

public class Playlist : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public long UserId { get; set; }
}