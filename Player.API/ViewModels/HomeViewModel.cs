namespace Player.API.ViewModels;

public class HomeViewModel : BaseViewModel
{
    public PlaylistsViewModel Playlists { get; set; }

    public HomeViewModel(PlaylistsViewModel playlists)
    {
        Playlists = playlists;
    }
}