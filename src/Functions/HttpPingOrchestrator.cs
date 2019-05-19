using System;
using System.Threading.Tasks;
using Functions.Common;
using Functions.Models.Orchestrator;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Functions
{
    public static class HttpPingOrchestrator
    {
        [FunctionName(FunctionNames.HttpPingOrchestrator)]
        public static async Task GetPing(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ping")] HttpRequest req,
            [OrchestrationClient] DurableOrchestrationClient starter,
            ILogger log)
        {
            // TODO: This should live elsewhere, dependent on any frontend calling /ping before orchestration loop starts or is woken up
            // Ideally, should react to users connecting/disconnecting to SignalR. Have to use ping because we know when they connect (negotiate),
            // but not when they disconnect: https://github.com/Azure/azure-signalr/issues/301#issuecomment-474668605
            try
            {
                var existingInstance = await starter.GetStatusAsync(Constants.SingletonOrchestratorId);
                if (existingInstance == null || existingInstance.RuntimeStatus == OrchestrationRuntimeStatus.Failed)
                {
                    // An instance with the specified ID doesn't exist, create one.
                    await starter.StartNewAsync(FunctionNames.NowPlayingOrchestrator, Constants.SingletonOrchestratorId, new OrchestratorData
                    {
                        Interval = 0
                    });

                    log.LogInformation($"Started orchestration with ID = '{Constants.SingletonOrchestratorId}'.");
                }
                else
                {
                    await starter.RaiseEventAsync(Constants.SingletonOrchestratorId, FunctionEvents.WakeUpOrchestrator, true);
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Could not start orchestration or raise event");
            }
        }
    }
}