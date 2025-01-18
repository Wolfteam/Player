using System.ComponentModel.DataAnnotations;

namespace Player.API.ViewModels;

public class PlaylistViewModel
{
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set;} = string.Empty;
    public long MediaCount { get; set;}

    public PlaylistViewModel()
    {
    }

    public PlaylistViewModel(long id, string name, long mediaCount)
    {
        Id = id;
        Name = name;
        MediaCount = mediaCount;
    }
}