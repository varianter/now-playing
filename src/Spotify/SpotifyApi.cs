using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Spotify.Models;

namespace Spotify
{
    public class SpotifyApi
    {
        private static HttpClient _client = new HttpClient();

        public static string StartAuthorizeRedirectUrl(string redirectUri, string state) => $"{Config.AccountBaseUrl}/authorize?client_id={Config.ClientId}&response_type=code&redirect_uri={redirectUri}&state={state}&scope={Config.RequestedScopes}";

        public static async Task<Token> FinishAuthorizeAsync(string code, string redirectUri)
        {
            var form = new Dictionary<string, string>();
            form.Add("grant_type", "authorization_code");
            form.Add("code", code);
            form.Add("redirect_uri", redirectUri);

            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, $"{Config.AccountBaseUrl}/api/token")
            {
                Content = new FormUrlEncodedContent(form)
            };
            tokenRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", Config.SpotifyApiBasicAuthHeaderValue);

            var tokenResponse = await _client.SendAsync(tokenRequest);
            tokenResponse.EnsureSuccessStatusCode();

            return await tokenResponse.Content.ReadAsAsync<Token>();
        }

        public static async Task<UserInfo> GetUserInfoAsync(string accessToken)
        {
            var userInfoRequest = new HttpRequestMessage(HttpMethod.Get, $"{Config.ApiBaseUrl}/v1/me");
            userInfoRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var userInfoResponse = await _client.SendAsync(userInfoRequest);
            userInfoResponse.EnsureSuccessStatusCode();

            return await userInfoResponse.Content.ReadAsAsync<UserInfo>();
        }

        public static async Task<Token> RefreshTokenAsync(string refreshToken)
        {
            var form = new Dictionary<string, string>();
            form.Add("grant_type", "refresh_token");
            form.Add("refresh_token", refreshToken);

            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, $"{Config.AccountBaseUrl}/api/token")
            {
                Content = new FormUrlEncodedContent(form)
            };
            tokenRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", Config.SpotifyApiBasicAuthHeaderValue);

            var tokenResponse = await _client.SendAsync(tokenRequest);
            tokenResponse.EnsureSuccessStatusCode();

            return await tokenResponse.Content.ReadAsAsync<Token>();
        }

        public static async Task<CurrentTrack> GetCurrentTrackAsync(string accessToken)
        {
            var currentTrackRequest = new HttpRequestMessage(HttpMethod.Get, $"{Config.ApiBaseUrl}/v1/me/player/currently-playing");
            currentTrackRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var currentTrackResponse = await _client.SendAsync(currentTrackRequest);
            currentTrackResponse.EnsureSuccessStatusCode();

            return await currentTrackResponse.Content.ReadAsAsync<CurrentTrack>();
        }

        public static async Task<PlayHistory> GetRecentlyPlayedTracksAsync(string accessToken)
        {
            var recentlyPlayedRequest = new HttpRequestMessage(HttpMethod.Get, $"{Config.ApiBaseUrl}/v1/me/player/recently-played?limit=50");
            recentlyPlayedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var recentlyPlayedResponse = await _client.SendAsync(recentlyPlayedRequest);
            recentlyPlayedResponse.EnsureSuccessStatusCode();

            return await recentlyPlayedResponse.Content.ReadAsAsync<PlayHistory>();
        }
    }
}