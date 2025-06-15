using Azure.Security.KeyVault.Secrets;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.Threading; // <-- 1. Añadimos el using para SemaphoreSlim
using System.Threading.Tasks;

namespace Services.Providers
{
    public class FirebaseProvider
    {
        private static FirebaseApp? _firebaseApp;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly SecretClient _secretClient;

        public FirebaseProvider(SecretClient secretClient)
        {
            _secretClient = secretClient;
        }

        public async Task<FirebaseApp> GetAppAsync()
        {
            if (_firebaseApp != null)
            {
                return _firebaseApp;
            }
            await _semaphore.WaitAsync();
            try
            {
                if (_firebaseApp == null)
                {
                    KeyVaultSecret secret = await _secretClient.GetSecretAsync("FirebaseServiceAccount");
                    string jsonCredentials = secret.Value;

                    _firebaseApp = FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromJson(jsonCredentials)
                    });
                }
            }
            finally
            {
                _semaphore.Release();
            }

            return _firebaseApp;
        }
    }
}