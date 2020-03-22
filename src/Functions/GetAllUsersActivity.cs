using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Functions.Common;
using Functions.Models.Orchestrator;
using Microsoft.WindowsAzure.Storage.Table;
using Functions.Repositories;
using Functions.Models.Table;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Functions
{
    public static class GetAllUsersActivity
    {
        [FunctionName(FunctionNames.GetAllUsersActivity)]
        public static async Task<IList<UserEntity>> Run(
            [ActivityTrigger] IDurableActivityContext ctx,
            [Table(
                TableConstants.UserTable,
                Connection = Constants.StorageConnection
            )] CloudTable userTable,
            ILogger log
            )
        {
            await userTable.CreateIfNotExistsAsync();

            var repo = new UserRepository(userTable);
            return await repo.GetUsersAsync();
        }
    }
}