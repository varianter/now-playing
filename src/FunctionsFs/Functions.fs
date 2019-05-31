namespace Functions

open Microsoft.Azure.WebJobs
open Microsoft.AspNetCore.Http
open Microsoft.WindowsAzure.Storage.Table
open Microsoft.Extensions.Logging

module FunctionNames =
    [<Literal>]    
    let HttpAuthorize = "Http_Authorize"
    [<Literal>]    
    let HttpCallback = "Http_AuthorizeCallback"
    [<Literal>]    
    let HttpUsers = "Http_Users"
    [<Literal>]    
    let HttpSignalRNegotiate = "Http_SignalRNegotiate"
    [<Literal>]    
    let NowPlayingOrchestrator = "O_NowPlaying"
    [<Literal>]    
    let AllTracksActivity = "A_AllCurrentTracks"
    [<Literal>]    
    let GetCurrentTracksActivity = "A_GetCurrentTracks"
    [<Literal>]    
    let SignalRTrackActivity = "A_SignalRTrack"

module Functions =

    [<FunctionName(FunctionNames.HttpAuthorize)>]
    let httpAuthorize
        ([<HttpTrigger(Extensions.Http.AuthorizationLevel.Anonymous, "get", Route = "authorize")>]
        req: HttpRequest,
        [<Table(TableConstants.AuthorizeStateTable, Connection = Constants.StorageConnection)>]
        table: CloudTable,
        log: ILogger) =
            HttpAuthorize.run (req, table, log) 
            |> Async.StartAsTask
    
    [<FunctionName(FunctionNames.HttpCallback)>]
    let httpCallback
        ([<HttpTrigger(Extensions.Http.AuthorizationLevel.Anonymous, "get", Route = "callback")>]
        req: HttpRequest,
        [<Table(TableConstants.AuthorizeStateTable, Connection = Constants.StorageConnection)>]
        authorizeTable: CloudTable,
        [<Table(TableConstants.UserTable, Connection = Constants.StorageConnection)>]
        userTable: CloudTable,
        log: ILogger) =
            HttpCallback.run (req, authorizeTable, userTable, log) 
            |> Async.StartAsTask