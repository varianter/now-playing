module Config

open System
open System.Text

[<Literal>]
let AccountBaseUrl = "https://accounts.spotify.com"

[<Literal>]
let ApiBaseUrl ="https://api.spotify.com"

[<Literal>]
let RequestedScopes = "user-read-currently-playing user-read-recently-played"

let clientId = Environment.GetEnvironmentVariable("SpotifyApiClientId")
let clientSecret = Environment.GetEnvironmentVariable("SpotifyApiClientSecret")
let spotifyApiBasicAuthHeaderValue =
    (clientId + ":" + clientSecret)
    |> Encoding.Default.GetBytes
    |> Convert.ToBase64String




// module Models =
//   type Image =
//     { height: int
//       url: string
//       width: int }
  
//   type Album =
//     { id: string
//       name: string
//       uri: string
//       images: Image[] }
  
//   type Artist =
//     { id: string
//       name: string
//       uri: string }
  
//   type Track =
//     { name: string 
//       uri: string 
//       duration_ms: int 
//       artists: Artist[] 
//       album: Album }    
  
//   type CurrentTrack =
//     {  item: Track
//        is_playing: bool
//        progress_ms: int }
  
//   type ExternalUrls = { spotify: string }
  
//   type UserInfo =
//     { id: string
//       display_name: string
//       uri: string
//       external_urls: ExternalUrls }

//   type Token = 
//     { access_token: string
//       token_type: string
//       scope: string
//       expires_in: int
//       refresh_token: string }
