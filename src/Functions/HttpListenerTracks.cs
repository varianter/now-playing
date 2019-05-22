using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Functions.Common;
using Functions.Repositories;
using Functions.Models.Orchestrator;

namespace Functions
{
    public static class HttpListenerTracks
    {
        [FunctionName(FunctionNames.HttpListenerTracks)]
        public static async Task<List<ListenerTrack>> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "listeners")] HttpRequest req,
            [Table(
                TableConstants.UserTable,
                Connection = Constants.StorageConnection
            )] CloudTable userTable,
            ILogger log)
        {
            log.LogInformation("Retrieving non-hidden users");
            await userTable.CreateIfNotExistsAsync();

            var repo = new UserRepository(userTable);
            var entities = await repo.GetUsersAsync();
            var nonHiddenUserIds = entities
                    .Where(e => e.Active)
                    .Select(e => e.RowKey)
                    .ToList();

            log.LogInformation("Found these users: {Users}", string.Join(", ", nonHiddenUserIds));

            return await SpotifyHelper.GetListenerTracks(nonHiddenUserIds, log);
        }
    }
}
