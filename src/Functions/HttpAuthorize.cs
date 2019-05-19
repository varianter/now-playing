using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Functions.Models.Table;
using Spotify;
using Functions.Common;

namespace Functions
{
    public static class HttpAuthorize
    {
        [FunctionName(FunctionNames.HttpAuthorize)]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "authorize")] HttpRequest req,
            [Table(
                TableConstants.AuthorizeStateTable,
                Connection = Constants.StorageConnection
            )] CloudTable table,
            ILogger log)
        {
            await table.CreateIfNotExistsAsync();

            var state = Guid.NewGuid().ToString();

            var operation = TableOperation.Insert(new AuthorizeEntity
            {
                PartitionKey = TableConstants.AuthorizePartitionKey,
                RowKey = state
            });

            await table.ExecuteAsync(operation);

            return new RedirectResult(SpotifyApi.StartAuthorizeRedirectUrl($"{req.Scheme}://{req.Host}/api/callback", state));
        }
    }
}
