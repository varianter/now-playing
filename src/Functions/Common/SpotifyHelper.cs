using System;
using System.Threading.Tasks;
using Functions.Models.KeyVault;
using Functions.Repositories;
using Spotify;
using Spotify.Models;

namespace Functions.Common
{
    public class SpotifyHelper
    {
        public static async Task<CurrentTrack> GetCurrentTrackAsync(string userId)
        {
            TokenSecrets tokens = null;
            try
            {
                tokens = await SecretRepository.GetToken(userId);
            }
            catch (TokenExpiredException ex)
            {
                var tokenResponse = await SpotifyApi.RefreshTokenAsync(ex.RefreshToken);

                tokens = new TokenSecrets
                {
                    AccessToken = tokenResponse.access_token,
                    RefreshToken = tokenResponse.refresh_token ?? ex.RefreshToken,
                    ExpiresInUtc = DateTime.UtcNow.AddSeconds(tokenResponse.expires_in)
                };

                await SecretRepository.SaveToken(userId, tokens);
            }

            return await SpotifyApi.GetCurrentTrackAsync(tokens.AccessToken);
        }
    }
}