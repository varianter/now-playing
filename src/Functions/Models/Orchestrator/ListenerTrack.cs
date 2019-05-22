using Spotify.Models;

namespace Functions.Models.Orchestrator
{

    public class ListenerTrack
    {
        public string userId { get; set; }
        public CurrentTrack currentTrack { get; set; }
    }
}