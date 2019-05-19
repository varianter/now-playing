namespace Spotify.Models
{
    public class Track
    {
        public string name { get; set; }
        public string uri { get; set; }
        public int duration_ms { get; set; }
        public Artist[] artists { get; set; }
        public Album album { get; set; }
    }
}