using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Functions.Models.Response;
using Functions.Common;
using Functions.Repositories;
using Functions.Models.Orchestrator;

namespace Functions
{
    public static class HttpUsers
    {
        [FunctionName(FunctionNames.HttpUsers)]
        public static async Task<List<UserModel>> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users")] HttpRequest req,
            [Table(
                TableConstants.UserTable,
                Connection = Constants.StorageConnection
            )] CloudTable userTable,
            ILogger log)
        {
            await userTable.CreateIfNotExistsAsync();

            var repo = new UserRepository(userTable);
            var entities = await repo.GetUsersAsync();
            var nonHiddenUsers = entities
                    .Where(e => e.Active)
                    .Select(UserModel.Map)
                    .ToList();

            return nonHiddenUsers;
        }
    }
}
