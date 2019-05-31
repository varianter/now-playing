namespace Functions
open Microsoft.Azure.KeyVault
open Microsoft.Azure.Services.AppAuthentication
open Microsoft.Azure.KeyVault

module SecretStore =
    let getTokenCallback (authority:string) (resource:string) (scope:string)  =
        let tokenProvider = AzureServiceTokenProvider "ads" 
        tokenProvider.KeyVaultTokenCallback
        

    let getAccessToken (authority:string) (resource:string) (scope:string) =
        let tokenProvider = new AzureServiceTokenProvider("ads")
        let context = new AuthenticationContext(authority, TokenCache.DefaultShared)
            async {
                let! result = context.AcquireTokenAsync(resource, clientCredential)
                return result.AccessToken;
            } |> Async.StartAsTask
            

// var azureServiceTokenProvider = new AzureServiceTokenProvider(Environment.GetEnvironmentVariable("AzureTokenProviderConnectionString"));
//             _keyVaultClient = new KeyVaultClient(
//                 new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback)
//             );