using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Functions.Models.KeyVault;
using Functions.Models.Orchestrator;
using Functions.Repositories;
using Microsoft.Extensions.Logging;
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

        public static async Task<List<ListenerTrack>> GetListenerTracks(List<string> userIds, ILogger log)
        {
            var tasks = userIds.Select((userId) => GetListenerTrack(userId, log));

            var results = await Task.WhenAll(tasks);

            return results.Where(r => r != null).ToList();
        }

        private static async Task<ListenerTrack> GetListenerTrack(string userId, ILogger log)
        {
            try
            {
                var currentTrack = await SpotifyHelper.GetCurrentTrackAsync(userId);
                return new ListenerTrack
                {
                    UserId = userId,
                    CurrentTrack = currentTrack
                };
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to get current track for user {UserId}", userId);
                return null;
            }
        }
    }
}