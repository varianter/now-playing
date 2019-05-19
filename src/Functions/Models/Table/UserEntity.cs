using Microsoft.WindowsAzure.Storage.Table;
using Functions.Common;
using Spotify.Models;

namespace Functions.Models.Table
{
    public class UserEntity : TableEntity
    {
        public string Name { get; set; }
        public string SpotifyUri { get; set; }
        public string SpotifyHttpUrl { get; set; }
        public bool Active { get; set; }

        public static UserEntity Map(UserInfo spotifyUser)
        {
            return new UserEntity
            {
                PartitionKey = TableConstants.UserPartitionKey,
                RowKey = spotifyUser.id,
                Name = spotifyUser.display_name,
                SpotifyUri = spotifyUser.uri,
                SpotifyHttpUrl = spotifyUser.external_urls?.spotify,
                Active = true
            };
        }
    }
}