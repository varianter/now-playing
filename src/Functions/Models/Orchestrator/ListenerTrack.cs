using Spotify.Models;

namespace Functions.Models.Orchestrator
{

    public class ListenerTrack
    {
        public string UserId { get; set; }
        public CurrentTrack CurrentTrack { get; set; }
    }
}