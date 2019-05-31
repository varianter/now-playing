module SpotifyFs
open FSharp.Data
open FSharp.Data.HttpRequestHeaders

type RecentlyPlayed = JsonProvider<"../../data/recently.json", RootName="recently">
type CurrentTrack = JsonProvider<"../../data/current.json", RootName="currentTrack">
type Token = JsonProvider<"../../data/token.json", RootName="token">
type Me = JsonProvider<"../../data/me.json", RootName="userInfo">

let private apiUrl = sprintf "%s/me%s" Config.ApiBaseUrl 
let private map f op = async {
    let! x = op
    return f x
  }

let private getAsync parse path accessToken =
  Http.AsyncRequestString
    ( apiUrl path,
      headers = [ Authorization ("Bearer " + accessToken) ] )
    |> map parse

let private postTokenRequestAsync body =
    Http.AsyncRequestString
      ( sprintf "%s/api/token" Config.AccountBaseUrl,
        body = FormValues body,
        headers = [ Authorization ("Basic " + Config.spotifyApiBasicAuthHeaderValue) ] )
      |> map Token.Parse

let constructAuthorizedRedirectUrl redirectUri state =
  sprintf "%s/authorize?client_id=%s&response_type=code&redirect_uri=%s&state=%s&scope=%s"
    Config.AccountBaseUrl Config.clientId redirectUri state Config.RequestedScopes

let getUserInfoAsync = getAsync Me.Parse "/me"
let getCurrentTrackAsync = getAsync CurrentTrack.Parse "/me/player/currently-playing"
let getRecentlyPlayedTracksAsync = getAsync RecentlyPlayed.Parse "/me/player/recently-played?limit=50"

let finishAuthorizeAsync code redirectUrl =
  postTokenRequestAsync [ ("grant_type", "authorization_code"); 
                     ("code", code); 
                     ("redirect_uri", redirectUrl); ]

let refreshTokenAsync refreshToken =
  postTokenRequestAsync [ ("grant_type", "refresh_token"); 
                     ("refresh_token", refreshToken); ]
