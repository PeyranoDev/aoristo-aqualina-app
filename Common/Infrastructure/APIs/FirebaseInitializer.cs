
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin;

public static class FirebaseInitializer
{
    public static async Task InitializeAsync(string keyVaultUrl, string secretName)
    {
        var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
        KeyVaultSecret secret = await client.GetSecretAsync(secretName);
        string jsonCredentials = secret.Value;

        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromJson(jsonCredentials)
        });
    }
}
