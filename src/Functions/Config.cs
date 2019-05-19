using System;

namespace Functions
{
    public class Config
    {
        public static string AzureTokenProviderConnectionString => Environment.GetEnvironmentVariable("AzureTokenProviderConnectionString");
        public static string AzureKeyVaultEndpoint => Environment.GetEnvironmentVariable("AzureKeyVaultEndpoint");
        public static int ActiveListenerIntervalSeconds => int.Parse(Environment.GetEnvironmentVariable("ActiveListenerIntervalSeconds"));
        public static int InactiveListenerIntervalSeconds => int.Parse(Environment.GetEnvironmentVariable("InactiveListenerIntervalSeconds"));
    }
}