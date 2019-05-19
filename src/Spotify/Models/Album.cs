namespace Spotify.Models
{
    public class Album
    {
        public string id { get; set; }
        public string name { get; set; }
        public string uri { get; set; }
        public Image[] images { get; set; }
    }
}