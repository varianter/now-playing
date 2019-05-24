using System.Collections.Generic;
using System.Threading.Tasks;
using Functions.Common;
using Functions.Extensions;
using Functions.Models.Table;
using Microsoft.WindowsAzure.Storage.Table;

namespace Functions.Repositories
{
    public class UserRepository
    {
        private readonly CloudTable _table;
        public UserRepository(CloudTable table)
        {
            _table = table;
        }

        public async Task<IList<UserEntity>> GetUsersAsync()
        {
            return await _table.ExecuteQueryAsync(new TableQuery<UserEntity>());
        }

        public async Task<UserEntity> GetUserAsync(string userId)
        {
            try
            {
                var operation = TableOperation.Retrieve<UserEntity>(TableConstants.UserPartitionKey, userId);
                var result = await _table.ExecuteAsync(operation);
                return result.Result as UserEntity;
            }
            catch
            {
                return null;
            }
        }

        public async Task AddUser(UserEntity user)
        {
            var insertUserOperation = TableOperation.InsertOrReplace(user);

            await _table.ExecuteAsync(insertUserOperation);
        }
    }
}