using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Common.Infrastructure.Security.Middlewares;
using Common.Models.Profiles;
using Data;
using Data.Models.Profiles;
using Data.Repositories.Implementations;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Services.Background;
using Services.Main.Implementations;
using Services.Main.Interfaces;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
var isDevelopment = builder.Environment.IsDevelopment();

var keyVaultUri = new Uri("https://aoristo-key-vault.vault.azure.net/");
var credential = new DefaultAzureCredential();
var client = new SecretClient(keyVaultUri, credential);

KeyVaultSecret sqlSecret = await client.GetSecretAsync("ConnectionStr");
KeyVaultSecret jwtSecret = await client.GetSecretAsync("JWTSecret");
string connectionString = sqlSecret.Value;

var jwtOptions = new JwtOptions
{
    Key = jwtSecret.Value,
    Issuer = builder.Configuration["Jwt:Issuer"],
    Audience = builder.Configuration["Jwt:Audience"]
};
builder.Services.AddSingleton(jwtOptions);

await FirebaseInitializer.InitializeAsync(keyVaultUri.ToString(), "FirebaseServiceAccount");

// --- INYECCIÓN DE DEPENDENCIAS ---
builder.Services.AddDbContextPool<AqualinaAPIContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IApartmentRepository, ApartmentRepository>();
builder.Services.AddScoped<IInvitationRepository, InvitationRepository>();
builder.Services.AddScoped<IInvitationService, InvitationService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddSingleton<IHashingService, HashingService>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IVehicleRequestService, VehicleRequestService>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IApartmentService, ApartmentService>();
builder.Services.AddScoped<ITowerRepository, TowerRepository>();
builder.Services.AddScoped<ITowerService, TowerService>();
builder.Services.AddSingleton<IBlobStorageService>(sp =>
{
    var storageAccountUri = builder.Configuration["Azure:Storage:Blob:ServiceUri"];
    return new BlobStorageService(storageAccountUri);
});

builder.Services.AddAutoMapper(typeof(UserProfile), typeof(VehicleProfile), typeof(TowerProfile));
builder.Services.AddHostedService<CleanupService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
        };
    });

builder.Services.AddResponseCaching();


builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("ApiPolicy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("MobileAppPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Aqualina API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }},
        Array.Empty<string>()
    }});
});

var app = builder.Build();

app.UseRateLimiter();
app.UseHttpsRedirection();
if (!isDevelopment)
{
    app.UseHsts();
}

app.UseCors("MobileAppPolicy");

app.UseRouting();

app.UseResponseCaching();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (isDevelopment)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
