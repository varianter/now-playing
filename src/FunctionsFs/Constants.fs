namespace Functions

module Constants = 
    [<Literal>]        
    let StorageConnection = "AzureWebJobsStorage"
    [<Literal>]        
    let NowPlayingHubName = "NowPlayingHub"
    [<Literal>]        
    let SingletonOrchestratorId = "AA99FA87-1984-4CE3-B1CF-300D09D60AAC"

module TableConstants =
    [<Literal>]        
    let AuthorizeStateTable = "AuthorizeStateTable"
    [<Literal>]        
    let UserTable = "UserTable"
    [<Literal>]        
    let ActivityTable = "ActivityTable"
    [<Literal>]        
    let ActivePartitionKey = "ActivePartitionKey"
    [<Literal>]        
    let ActiveRowKey = "ActiveRowKey"
    [<Literal>]        
    let AuthorizePartitionKey = "Authorizing"
    [<Literal>]        
    let UserPartitionKey = "User"