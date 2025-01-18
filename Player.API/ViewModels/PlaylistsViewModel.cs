namespace Player.API.ViewModels;

public class PlaylistsViewModel
{
    public List<PlaylistViewModel> Playlists { get; }

    public PlaylistsViewModel(List<PlaylistViewModel> playlists)
    {
        Playlists = playlists;
    }
}