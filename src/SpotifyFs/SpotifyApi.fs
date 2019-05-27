module SpotifyFs
open FSharp.Data
open FSharp.Data.HttpRequestHeaders

type RecentlyPlayed = JsonProvider<"./data/recently.json">
type CurrentTrack = JsonProvider<"./data/current.json">
type Token = JsonProvider<"./data/token.json">
type Me = JsonProvider<"./data/me.json">

let private apiUrl = sprintf "%s/me%s" Config.apiBaseUrl 
let private map f op = async {
    let! x = op
    return f x
  }

let private getAsync parse path accessToken =
  Http.AsyncRequestString
    ( apiUrl path,
      headers = [ Authorization ("Bearer " + accessToken) ] )
    |> map parse

let private postTokenRequest body =
    Http.AsyncRequestString
      ( sprintf "%s/api/token" Config.accountBaseUrl,
        body = FormValues body,
        headers = [ Authorization ("Basic " + Config.spotifyApiBasicAuthHeaderValue) ] )
      |> map Token.Parse

let constructAuthorizedRedirectUrl redirectUri state =
  sprintf "%s/authorize?client_id=%s&response_type=code&redirect_uri=%s&state=%s&scope=%s"
    Config.accountBaseUrl Config.clientId redirectUri state Config.requestedScopes


let getUserInfoAsync accessToken =
  getAsync Me.Parse "/me" accessToken
let getCurrentTrackAsync accessToken =
  getAsync CurrentTrack.Parse "/me/player/currently-playing" accessToken
let getRecentlyPlayedTracksAsync accessToken =
  getAsync RecentlyPlayed.Parse "/me/player/recently-played?limit=50" accessToken


let finishAuthorizeAsync code redirectUrl =
  postTokenRequest [ ("grant_type", "authorization_code"); 
                     ("code", code); 
                     ("redirect_uri", redirectUrl); ]

let refreshTokenAsync refreshToken =
  postTokenRequest [ ("grant_type", "refresh_token"); 
                     ("refresh_token", refreshToken); ]
