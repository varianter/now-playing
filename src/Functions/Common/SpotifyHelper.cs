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
        private static async Task<string> GetAccessToken(string userId)
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

            return tokens.AccessToken;
        }

        public static async Task<CurrentTrack> GetCurrentTrackAsync(string userId)
        {
            var accessToken = await GetAccessToken(userId);

            return await SpotifyApi.GetCurrentTrackAsync(accessToken);
        }

        public static async Task<PlayHistory> GetRecentlyPlayedTracksAsync(string userId)
        {
            var accessToken = await GetAccessToken(userId);

            return await SpotifyApi.GetRecentlyPlayedTracksAsync(accessToken);
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
                var currentTrack = await GetCurrentTrackAsync(userId);
                return new ListenerTrack
                {
                    userId = userId,
                    currentTrack = currentTrack
                };
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to get current track for user {UserId}, message: {ExceptionMessage}, inner: {ExceptionMessage}, stack: {ExceptionStack}", userId, ex.Message, ex.InnerException?.Message, ex.StackTrace);
                return null;
            }
        }
    }
}