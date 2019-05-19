using System;
using System.Text;

namespace Spotify
{
    internal class Config
    {
        public const string AccountBaseUrl = "https://accounts.spotify.com";
        public const string ApiBaseUrl = "https://api.spotify.com";
        public static readonly string ClientId = Environment.GetEnvironmentVariable("SpotifyApiClientId");
        public static readonly string ClientSecret = Environment.GetEnvironmentVariable("SpotifyApiClientSecret");
        public const string RequestedScopes = "user-read-currently-playing user-read-recently-played";

        public static string SpotifyApiBasicAuthHeaderValue =>
            Convert.ToBase64String(
                Encoding.Default.GetBytes(
                    $"{ClientId}:{ClientSecret}"
                )
            );
    }
}
