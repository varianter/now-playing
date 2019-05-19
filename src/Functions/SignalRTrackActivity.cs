
using System.Collections.Generic;
using System.Threading.Tasks;
using Functions.Common;
using Functions.Models.Orchestrator;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace Functions
{
    public static class SignalRTrackActivity
    {
        [FunctionName(FunctionNames.SignalRTrackActivity)]
        public static async Task SendMessage(
            [ActivityTrigger] List<ListenerTrack> listenerTracks,
            [SignalR(HubName = Constants.NowPlayingHubName)] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "updateTracks",
                    Arguments = new object[] { listenerTracks }
                });
        }
    }
}