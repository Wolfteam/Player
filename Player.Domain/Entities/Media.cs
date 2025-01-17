namespace Player.Domain.Entities;

public class Media : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public double Duration { get; set; }
    public long PlaylistId { get; set; }
}