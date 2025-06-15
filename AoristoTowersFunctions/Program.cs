using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Common.Models.Profiles;
using Data;
using Data.Models.Profiles;
using Data.Repositories.Implementations;
using Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Main.Implementations;
using Services.Main.Interfaces;
using Services.Providers;


// --- Configuración Inicial y Key Vault ---
var keyVaultUri = new Uri("https://aoristo-key-vault.vault.azure.net/");
var credential = new DefaultAzureCredential();
var client = new SecretClient(keyVaultUri, credential);

// Obtenemos los secretos al inicio
KeyVaultSecret sqlSecret = await client.GetSecretAsync("ConnectionStr");
KeyVaultSecret jwtSecret = await client.GetSecretAsync("JWTSecret");
string connectionString = sqlSecret.Value;

var host = new HostBuilder()
    .ConfigureAppConfiguration(configBuilder =>
    {
        configBuilder.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables();
    })
    .ConfigureFunctionsWebApplication(worker =>
    {
        worker.UseMiddleware<ExceptionHandlingMiddleware>();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services.AddDbContextPool<AqualinaAPIContext>(options =>
            options.UseSqlServer(connectionString));

        var jwtOptions = new JwtOptions
        {
            Key = jwtSecret.Value,
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"]
        };
        services.AddSingleton(jwtOptions);
        services.AddSingleton(new SecretClient(keyVaultUri, credential));

        services.AddSingleton<FirebaseProvider>();

        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IApartmentRepository, ApartmentRepository>();
        services.AddScoped<IInvitationRepository, InvitationRepository>();
        services.AddScoped<IInvitationService, InvitationService>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddSingleton<IHashingService, HashingService>();
        services.AddScoped<ITokenRepository, TokenRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IVehicleRequestService, VehicleRequestService>();
        services.AddScoped<IRequestRepository, RequestRepository>();
        services.AddScoped<IApartmentService, ApartmentService>();
        services.AddScoped<ITowerRepository, TowerRepository>();
        services.AddScoped<ITowerService, TowerService>();

        services.AddSingleton<IBlobStorageService>(sp =>
        {
            var storageAccountUri = configuration["Azure:Storage:Blob:ServiceUri"];
            return new BlobStorageService(storageAccountUri);
        });

        services.AddAutoMapper(typeof(UserProfile), typeof(VehicleProfile), typeof(TowerProfile));
        services.AddHttpContextAccessor();
    })
    .Build();

host.Run();