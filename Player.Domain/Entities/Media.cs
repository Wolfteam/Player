namespace Player.Domain.Entities;

public class Media : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public long Length { get; set; }
    public float LengthInSeconds { get; set; }
    public long PlaylistId { get; set; }
}