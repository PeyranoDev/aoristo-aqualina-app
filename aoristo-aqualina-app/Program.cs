using Azure.Identity;
using Common.Infrastructure.Security.Interface;
using Common.Security;
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

builder.Services.AddControllers();
builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddControllers();
builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AqualinaAPIContext>(dbContextOptions => dbContextOptions.UseSqlite(builder.Configuration["ConnectionStrings:AqualinaAPIDBConnectionString"]));

builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(UserProfile));

builder.Services.AddSingleton<ISecretProvider, AzureKeyVaultSecretProvider>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IHashingService, HashingService>();

    .AddHttpContextAccessor()
    .AddAuthorization()
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretProvider = builder.Services.BuildServiceProvider()
        .GetRequiredService<ISecretProvider>();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretProvider.GetSecret("JWT-Secret-Key")))
        };

    });



app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/healthz");

app.MapGet("/", () => "Hello World!");

app.Run();
