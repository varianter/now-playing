using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Functions.Models.Table;
using Functions.Models.KeyVault;
using Spotify;
using Functions.Common;
using Functions.Repositories;

namespace Functions
{
    public static class HttpCallback
    {
        [FunctionName(FunctionNames.HttpCallback)]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "callback")] HttpRequest req,
            [Table(
                TableConstants.AuthorizeStateTable,
                Connection = Constants.StorageConnection
            )] CloudTable authorizeTable,
            [Table(
                TableConstants.UserTable,
                Connection = Constants.StorageConnection
            )] CloudTable userTable,
            ILogger log)
        {
            await authorizeTable.CreateIfNotExistsAsync();
            await userTable.CreateIfNotExistsAsync();

            string code = req.Query["code"];
            string error = req.Query["error"];
            string state = req.Query["state"];

            if (!string.IsNullOrWhiteSpace(error))
            {
                log.LogError("Received error from Spotify authorization: {Error}", error);
                return new BadRequestResult();
            }

            var getAuthorizedOperation = TableOperation.Retrieve<AuthorizeEntity>(TableConstants.AuthorizePartitionKey, state);

            var result = await authorizeTable.ExecuteAsync(getAuthorizedOperation);
            var entity = result.Result as AuthorizeEntity;

            if (entity == null)
            {
                log.LogError("Could not find associated authorize entity. Callback may have been called twice.");
                return new NotFoundResult();
            }

            var deleteAuthorizeOperation = TableOperation.Delete(entity);
            await authorizeTable.ExecuteAsync(deleteAuthorizeOperation);

            var tokenResponse = await SpotifyApi.FinishAuthorizeAsync(code, $"{req.Scheme}://{req.Host}/api/callback");

            var spotifyUserInfo = await SpotifyApi.GetUserInfoAsync(tokenResponse.access_token);
            var user = UserEntity.Map(spotifyUserInfo);

            var repo = new UserRepository(userTable);
            await repo.AddUser(user);

            await SecretRepository.SaveToken(user.RowKey, new TokenSecrets
            {
                AccessToken = tokenResponse.access_token,
                RefreshToken = tokenResponse.refresh_token,
                ExpiresInUtc = DateTime.UtcNow.AddSeconds(tokenResponse.expires_in)
            });

            return new RedirectResult("http://localhost:3000");
        }
    }
}
