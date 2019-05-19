namespace Spotify.Models
{
    public class UserInfo
    {
        public string id { get; set; }
        public string display_name { get; set; }
        public string uri { get; set; }
        public ExternalUrls external_urls { get; set; }
    }

    public class ExternalUrls
    {
        public string spotify { get; set; }
    }
}