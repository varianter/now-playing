using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Functions.Models.Orchestrator;
using Functions.Common;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using Functions.Models.Table;

namespace Functions
{
    public static class NowPlayingOrchestrator
    {
        [FunctionName(FunctionNames.NowPlayingOrchestrator)]
        public static async Task Run(
            [OrchestrationTrigger] DurableOrchestrationContext ctx,
            ILogger log)
        {
            var data = ctx.GetInput<OrchestratorData>();

            if (!ctx.IsReplaying)
            {
                log.LogInformation("Orchestration interval {Interval}. Current listeners: {Listeners}", data.Interval, string.Join(", ", data.MusicListeners?.Select(l => l.UserId)));
            }

            try
            {
                var forceUpdateUsers = data.MusicListeners == null || data.Interval == 0;
                List<string> listeners;
                if (forceUpdateUsers)
                {
                    var users = await ctx.CallActivityAsync<IList<UserEntity>>(FunctionNames.GetAllUsersActivity, null);
                    listeners = users.Select(u => u.RowKey).ToList();
                }
                else
                {
                    listeners = GetListenersToUpdate(data.MusicListeners, ctx.CurrentUtcDateTime);
                }

                List<ListenerTrack> currentListenerTracks;
                if (listeners.Any())
                {
                    currentListenerTracks =
                        await ctx.CallActivityAsync<List<ListenerTrack>>(
                            FunctionNames.GetCurrentTracksActivity,
                            listeners
                        );
                }
                else
                {
                    currentListenerTracks = new List<ListenerTrack>();
                }

                await ctx.CallActivityAsync(
                        FunctionNames.SignalRTrackActivity,
                        currentListenerTracks
                    );

                if (data.MusicListeners == null)
                    data.MusicListeners = new List<MusicListener>();

                foreach (var playingTrack in currentListenerTracks.Where(t => t.CurrentTrack?.is_playing == true))
                {
                    var listener = data.MusicListeners.FirstOrDefault(m => m.UserId == playingTrack.UserId);
                    if (listener != null)
                    {
                        listener.LastSeenActivity = ctx.CurrentUtcDateTime;
                    }
                    else
                    {
                        data.MusicListeners.Add(new MusicListener { UserId = playingTrack.UserId, LastSeenActivity = ctx.CurrentUtcDateTime });
                    }
                }
            }
            catch (Exception ex)
            {
                if (!ctx.IsReplaying)
                    log.LogError(ex, "Failure while retrieving or signalling tracks.");
            }

            DateTime nextIntervalTime;
            if (data.LastActivity > ctx.CurrentUtcDateTime.AddMinutes(-5))
            {
                nextIntervalTime = ctx.CurrentUtcDateTime.AddSeconds(Config.ActiveListenerIntervalSeconds);
            }
            else
            {
                nextIntervalTime = ctx.CurrentUtcDateTime.AddSeconds(Config.InactiveListenerIntervalSeconds);
            }

            using (var timeoutCts = new CancellationTokenSource())
            {
                Task durableTimeout = ctx.CreateTimer(nextIntervalTime, timeoutCts.Token);

                Task<bool> wakeUpEvent = ctx.WaitForExternalEvent<bool>(FunctionEvents.WakeUpOrchestrator);

                var winner = await Task.WhenAny(wakeUpEvent, durableTimeout);
                if (winner == wakeUpEvent)
                {
                    timeoutCts.Cancel();
                }
            }

            if (data.Interval > 2)
            {
                data.Interval = 0;
            }
            else
            {
                data.Interval = data.Interval + 1;
            }

            ctx.ContinueAsNew(data);
        }

        public static List<string> GetListenersToUpdate(List<MusicListener> listeners, DateTime currentUtcDateTime)
        {
            DateTime fiveMinutesAgo = currentUtcDateTime.AddMinutes(-5);

            return listeners
                    .Where(l =>
                        l.LastSeenActivity.HasValue
                        && l.LastSeenActivity > fiveMinutesAgo
                    )
                    .Select(l => l.UserId).ToList();
        }
    }
}
