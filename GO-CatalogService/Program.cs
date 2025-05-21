using GO_CatalogService.Service;
using GO_CatalogService.Interface;
using GO_CatalogService.Repository;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using NLog;
using NLog.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using Microsoft.OpenApi.Models; // Denne skal stadig bruges til OpenApiInfo, OpenApiSecurityScheme osv.
using System; // Til Exception

Console.WriteLine("CatalogService starter...");
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings()
.GetCurrentClassLogger();
logger.Debug("start min service");

var builder = WebApplication.CreateBuilder(args);

// Registr�r korrekt Guid-serializer for MongoDB
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

// Async Vault secret loader med retry (kopieret fra AuthService/UserService)
async Task<Dictionary<string, string>> LoadVaultSecretsAsync()
{
    var retryCount = 0;
    while (true)
    {
        try
        {
            var vaultAddress = Environment.GetEnvironmentVariable("VAULT_ADDR") ?? "http://vault:8200";
            var vaultToken = Environment.GetEnvironmentVariable("VAULT_TOKEN") ?? "wopwopwop123";

            Console.WriteLine($"Henter secrets fra Vault p� {vaultAddress} med token...");

            var vaultClientSettings = new VaultClientSettings(vaultAddress, new TokenAuthMethodInfo(vaultToken));
            var vaultClient = new VaultClient(vaultClientSettings);

            var secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(
                path: "go-authservice", // Brug samme sti som i AuthService for konsistens
                mountPoint: "secret"
            );

            Console.WriteLine("Secrets hentet fra Vault!");

            return secret.Data.Data.ToDictionary(
                kv => kv.Key,
                kv => kv.Value?.ToString() ?? ""
            );
        }
        catch (Exception ex)
        {
            retryCount++;
            if (retryCount > 5)
            {
                Console.WriteLine($"Fejl ved indl�sning af Vault secrets efter 5 fors�g: {ex.Message}");
                throw;
            }
            Console.WriteLine($"Vault ikke klar endnu, pr�ver igen om 3 sek... ({retryCount}/5): {ex.Message}");
            await Task.Delay(3000);
        }
    }
}

// Indl�s secrets fra Vault
var vaultSecrets = await LoadVaultSecretsAsync();
builder.Configuration.AddInMemoryCollection(vaultSecrets);

// Hent JWT konfiguration fra Vault
var secretKey = builder.Configuration["Jwt__Secret"];
var issuer = builder.Configuration["Jwt__Issuer"];
var audience = builder.Configuration["Jwt__Audience"];

// Print JWT konfiguration til debug
Console.WriteLine($"Jwt__Secret fra Vault i CatalogService: '{secretKey}' (Length: {secretKey?.Length ?? 0})");
Console.WriteLine($"Jwt__Issuer fra Vault i CatalogService: '{issuer}'");
Console.WriteLine($"Jwt__Audience fra Vault i CatalogService: '{audience}'");


// Add services to the container.
builder.Services.AddSingleton<CatalogService>();
builder.Services.AddSingleton<ICatalogRepository, CatalogRepository>();


// Tilf�j autentificering
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
        {
            throw new Exception("JWT konfiguration mangler fra Vault!");
        }
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

// Tilf�j autorisering
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});


builder.Services.AddControllers();

// Swashbuckle/Swagger konfiguration
builder.Services.AddEndpointsApiExplorer(); // Denne er stadig n�dvendig for at opdage endpoints
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CatalogService API", Version = "v1" });

    // Konfigurer Swagger til at inkludere JWT Bearer token autentificering
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });
});

//Nlog add
builder.Logging.ClearProviders();
builder.Host.UseNLog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Brug app.UseSwagger() fra Swashbuckle
    app.UseSwaggerUI(); // Brug app.UseSwaggerUI() fra Swashbuckle
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Tilf�j autentificeringsmiddleware
app.UseAuthorization(); // Tilf�j autoriseringsmiddleware

app.MapControllers();

app.Run();