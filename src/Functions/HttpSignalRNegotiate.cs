using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Functions.Common;

namespace Functions
{
    public static class HttpSignalRNegotiate
    {
        [FunctionName(FunctionNames.HttpSignalRNegotiate)]
        public static SignalRConnectionInfo GetSignalRInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "negotiate")] HttpRequest req,
            [SignalRConnectionInfo(HubName = Constants.NowPlayingHubName)] SignalRConnectionInfo connectionInfo,
            [OrchestrationClient] DurableOrchestrationClient starter,
            ILogger log)
        {
            return connectionInfo;
        }
    }
}