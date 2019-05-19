using Spotify.Models;

namespace Functions.Models.Response
{
    public class NowPlayingModel
    {
        public string User { get; set; }
        public CurrentTrack CurrentTrack { get; set; }
    }
}