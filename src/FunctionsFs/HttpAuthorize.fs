namespace Functions

open System
open Microsoft.AspNetCore.Http
open Microsoft.WindowsAzure.Storage.Table
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Mvc
open Table
open SpotifyFs

module HttpAuthorize =
    let run (req: HttpRequest, table: CloudTable, log: ILogger) =
        async {
            do! table |> createIfNotExist 

            let state = Guid.NewGuid().ToString()

            do! 
                insert (TableEntity(TableConstants.AuthorizePartitionKey, state)) table

            log.LogInformation("Started authorization redirect for {State}", state)

            let redirectUrl =
                    constructAuthorizedRedirectUrl 
                    <| sprintf "%s://%s/api/callback" req.Scheme req.Host.Value
                    <| state

            return RedirectResult(redirectUrl)
        }