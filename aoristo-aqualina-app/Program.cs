using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Data;

using Data.Repositories.Implementations;
using Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Services.Main.Implementations;
using Services.Main.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var credential = new DefaultAzureCredential();
var client = new SecretClient(new Uri("https://aoristo-key-vault.vault.azure.net/"), credential);

KeyVaultSecret secret = await client.GetSecretAsync("SQL-CONNECTION-STR");
KeyVaultSecret secretSalt = await client.GetSecretAsync("JWT-Secret-Key");

string connectionString = secret.Value;
string salt = secretSalt.Value;

builder.Services.AddDbContext<AqualinaAPIContext>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionStrings:AoristoAqualinaAPIDBConnectionString"]));

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IApartmentRepository, ApartmentRepository>();
builder.Services.AddScoped<IInvitationRepository, InvitationRepository>();
builder.Services.AddScoped<IInvitationService, InvitationService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddSingleton<IHashingService, HashingService>();
builder.Services.AddAutoMapper(typeof(UserProfile));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(salt))
        };
    });


builder.Services.AddHealthChecks();
builder.Services.AddControllers();
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
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});


var app = builder.Build();


app.UseExceptionHandler("/error"); 
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/healthz");
app.MapGet("/", () => "Aqualina API (Production)");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");
        await next();
    });
}

app.Run();