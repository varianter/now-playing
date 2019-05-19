namespace Functions.Common
{
    public class Constants
    {
        public const string StorageConnection = "AzureWebJobsStorage";
        public const string NowPlayingHubName = "NowPlayingHub";
        public const string SingletonOrchestratorId = "AA99FA87-1984-4CE3-B1CF-300D09D60AAC";
    }

    public class TableConstants
    {
        public const string AuthorizeStateTable = "AuthorizeStateTable";
        public const string UserTable = "UserTable";
        public const string ActivityTable = "ActivityTable";
        public const string ActivePartitionKey = "ActivePartitionKey";
        public const string ActiveRowKey = "ActiveRowKey";
        public const string AuthorizePartitionKey = "Authorizing";
        public const string UserPartitionKey = "User";
    }
}