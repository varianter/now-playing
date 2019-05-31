namespace Functions
open Microsoft.WindowsAzure.Storage.Table

module UserTable =
    type UserEntity(userId: string, name: string, spotifyUri: string, spotifyHttpUrl: string, active: bool) =
        inherit TableEntity(TableConstants.UserPartitionKey, userId)
        member val Name: string = name
        member val SpotifyUri: string = spotifyUri
        member val SpotifyHttpUrl: string = spotifyHttpUrl
        member val Active: bool = active
    
    let mapFromSpotifyUser (user: UserInfo) =
        UserEntity(user.id, user.display_name, user.uri, user.external_urls.spotify, true)

    let addUser (table: CloudTable, user: UserEntity) =
        async {
            do! 
                user
                |> TableOperation.Insert
                |> table.ExecuteAsync
                |> Async.AwaitTask
                |> Async.Ignore
        }

    