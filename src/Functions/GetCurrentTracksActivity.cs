using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Functions.Common;
using Functions.Models.Orchestrator;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

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
            return await SpotifyHelper.GetListenerTracks(listenerIds, log);
        }
    }
}