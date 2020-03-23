module Config

open System
open System.Text

[<Literal>]
let AccountBaseUrl = "https://accounts.spotify.com"

[<Literal>]
let ApiBaseUrl ="https://api.spotify.com"

[<Literal>]
let RequestedScopes = "user-read-currently-playing user-read-recently-played user-top-read"

let clientId = Environment.GetEnvironmentVariable("SpotifyApiClientId")
let clientSecret = Environment.GetEnvironmentVariable("SpotifyApiClientSecret")
let spotifyApiBasicAuthHeaderValue =
    (clientId + ":" + clientSecret)
    |> Encoding.Default.GetBytes
    |> Convert.ToBase64String
