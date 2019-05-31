namespace Functions

open Microsoft.AspNetCore.Http
open Microsoft.WindowsAzure.Storage.Table
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Primitives
open Table
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc

module HttpCallback =
    let getParameter (req: HttpRequest) name =
        let parameter = req.Query.[name]
        match parameter.Count with 
        | 1 -> Some(parameter.ToString())
        | _ -> None
    
    let handleAuthorize (state: string) (authorizeTable: CloudTable) =
        async {
            let! tableResult = 
                retrieve TableConstants.AuthorizeStateTable state authorizeTable

            let entity = tableResult.Result :?> TableEntity 
            
            if (not (isNull entity)) then 
                do! delete entity authorizeTable 
            
            let result = 
                match entity with
                | entity when isNull entity -> 
                    Some NotFoundResult
                | _ ->
                    None

            return result
        }
    
    let handleHappyPath =
        ()

    let run (req: HttpRequest, authorizeTable: CloudTable, userTable: CloudTable, log: ILogger) =
        async {
            do! authorizeTable |> createIfNotExist
            do! userTable |> createIfNotExist

            let error = getParameter req "error"
            let code = getParameter req "code"
            let state = getParameter req "state"
            
            if (state.IsSome) then
                let! result = handleAuthorize state.Value authorizeTable

            let parameters = (error, code, state)

            let result = 
                match parameters with
                | (None, Some state, Some code) -> 
                    match result with
                    | None -> 
                        handleHappyPath
                        RedirectResult "http://localhost:3000"
                | (Some error, Some state, _) -> 
                    log.LogError error
                    let! result = handleAuthorize state authorizeTable
                    match result with
                    | Some result -> result
                    | None -> BadRequestObjectResult error
                | (Some error, _, _) -> 
                    log.LogError error
                    BadRequestResult
                | _ -> ()

            return result
           
            // if (isNull entity) then 
            //     let error = "Could not find associated authorize entity. Callback may have been called twice."
            //     error |> log.LogError
            //     return error |> NotFoundObjectResult :> IActionResult |> Some
            // else
            //     do! 
            //         TableOperation.Delete entity
            //         |> authorizeTable.ExecuteAsync
            //         |> Async.AwaitTask
            //         |> Async.Ignore
            //     return None
        }

    let run (req: HttpRequest, authorizeTable: CloudTable, userTable: CloudTable, log: ILogger): Async<IActionResult> =
        async {
            do! createIfNotExist authorizeTable
            do! createIfNotExist userTable

            let error = req.Query.["error"]

            if error |> StringValues.IsNullOrEmpty then
                let state = req.Query.["state"]

                let! stateError = getAuthStateError (authorizeTable, state.ToString(), log)
                
                match stateError with
                | None -> 
                    let code = req.Query.["code"]

                    let! tokenResponse =
                        SpotifyFs.finishAuthorizeAsync
                        <| code.ToString()
                        <| sprintf "%s://%s/api/callback" req.Scheme req.Host.Value 

                    let! userInfo =
                        tokenResponse.AccessToken
                        |> SpotifyFs.getUserInfoAsync

                    do! 
                        userInfo 
                        |> UserTable.mapFromSpotifyUser
                        |> fun (e) -> UserTable.addUser (userTable, e)

                    // var spotifyUserInfo = await Api.GetUserInfoAsync(tokenResponse.access_token);
                    // var user = UserEntity.Map(spotifyUserInfo);

                    // var repo = new UserRepository(userTable);
                    // await repo.AddUser(user);
                    return RedirectResult("/api/nowplaying") :> IActionResult
                | Some(result) -> 
                    return result
            else
                let error = sprintf "Received error from Spotify authorization %s" (error.ToString())
                error |> log.LogError
                return error |> BadRequestObjectResult :> IActionResult
        }