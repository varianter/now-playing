using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Functions.Common;
using Functions.Models.Orchestrator;

namespace Functions
{
    public static class GetCurrentTracksActivity
    {
        [FunctionName(FunctionNames.GetCurrentTracksActivity)]
        public static async Task<List<ListenerTrack>> Run(
            [ActivityTrigger] List<string> listenerIds,
            ILogger log
            )
        {
            var tasks = listenerIds.Select((userId) => Get(userId, log));

            var results = await Task.WhenAll(tasks);

            return results.Where(r => r != null).ToList();
        }

        private static async Task<ListenerTrack> Get(string userId, ILogger log)
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