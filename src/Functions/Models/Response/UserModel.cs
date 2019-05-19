using Functions.Models.Table;

namespace Functions.Models.Response
{
    public class UserModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SpotifyUri { get; set; }
        public string SpotifyHttpUrl { get; set; }

        public static UserModel Map(UserEntity entity)
        {
            return new UserModel
            {
                Id = entity.RowKey,
                Name = entity.Name,
                SpotifyUri = entity.SpotifyUri,
                SpotifyHttpUrl = entity.SpotifyHttpUrl,
            };
        }
    }
}