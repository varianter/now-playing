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
using Spotify.Models;
using Microsoft.AspNetCore.Mvc;

namespace Functions
{
    public static class HttpRecentlyPlayed
    {
        [FunctionName(FunctionNames.HttpRecentlyPlayed)]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "recent/{userId}")] HttpRequest req,
            [Table(
                TableConstants.UserTable,
                Connection = Constants.StorageConnection
            )] CloudTable userTable,
            string userId,
            ILogger log)
        {
            await userTable.CreateIfNotExistsAsync();

            var repo = new UserRepository(userTable);
            var entity = await repo.GetUserAsync(userId);

            if (entity != null && entity.Active)
            {
                var playHistory = await SpotifyHelper.GetRecentlyPlayedTracksAsync(userId);

                return new JsonResult(playHistory);
            }

            return new NotFoundResult();
        }
    }
}
