using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Providers
{
    public class FirebaseProvider
    {
        private static FirebaseApp? _firebaseApp;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly IConfiguration _configuration;

        public FirebaseProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<FirebaseApp> GetAppAsync()
        {
            if (_firebaseApp != null) return _firebaseApp;

            await _semaphore.WaitAsync();
            try
            {
                if (_firebaseApp == null)
                {
                    string jsonCredentials = _configuration["FirebaseServiceAccount"];

                    if (string.IsNullOrEmpty(jsonCredentials))
                    {
                        throw new System.InvalidOperationException("El secreto 'FirebaseServiceAccount' no se encontró en la configuración.");
                    }

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