using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Functions.Models.KeyVault;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Newtonsoft.Json;

namespace Functions.Repositories
{
    public static class SecretRepository
    {
        private static readonly KeyVaultClient _keyVaultClient;
        private static string _clientSecret = null;
        private static Dictionary<string, TokenSecrets> _cachedTokens = new Dictionary<string, TokenSecrets>();

        static SecretRepository()
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider(Config.AzureTokenProviderConnectionString);
            _keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback)
            );
        }

        private static string TokenSecretName(string userId) => $"tokens-{userId}";

        public static async Task SaveToken(string userId, TokenSecrets tokens)
        {
            await _keyVaultClient.SetSecretAsync(Config.AzureKeyVaultEndpoint, TokenSecretName(userId), JsonConvert.SerializeObject(tokens));
            if (_cachedTokens.ContainsKey(userId))
            {
                _cachedTokens[userId] = tokens;
            }
            else
            {
                _cachedTokens.Add(userId, tokens);
            }
        }

        public static async Task<TokenSecrets> GetToken(string userId)
        {
            TokenSecrets tokens = null;
            if (_cachedTokens.ContainsKey(userId))
            {
                tokens = _cachedTokens[userId];
            }
            else
            {
                var secret = await _keyVaultClient.GetSecretAsync(Config.AzureKeyVaultEndpoint, TokenSecretName(userId));
                tokens = JsonConvert.DeserializeObject<TokenSecrets>(secret.Value);
                _cachedTokens.Add(userId, tokens);
            }

            if (DateTime.UtcNow.AddMinutes(5) > tokens.ExpiresInUtc)
            {
                throw new TokenExpiredException(tokens.RefreshToken);
            }

            return tokens;
        }
    }

    public class TokenExpiredException : Exception
    {
        public TokenExpiredException(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
        public string RefreshToken { get; }
    }
}

