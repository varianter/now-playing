using System;

namespace Functions.Models.KeyVault
{
    public class TokenSecrets
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresInUtc { get; set; }
    }
}