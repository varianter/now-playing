namespace Spotify.Models
{
    public class CurrentTrack
    {
        public Track item { get; set; }
        public bool is_playing { get; set; }
        public int progress_ms { get; set; }
    }
}