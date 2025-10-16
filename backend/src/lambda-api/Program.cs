using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using Project1.Tools;
using System.ClientModel;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// load secrets from parameter store
await ParameterStoreHelper.LoadSecretsToEnvironmentAsync();

var jwtSettingsJson = Environment.GetEnvironmentVariable("Jwt");
var googleSettingsJson = Environment.GetEnvironmentVariable("Google");
var githubModelsToken = Environment.GetEnvironmentVariable("GitHubModelsToken");

if (string.IsNullOrEmpty(jwtSettingsJson) || string.IsNullOrEmpty(googleSettingsJson))
{
    throw new InvalidOperationException("Environment variable 'jwt' is not set.");
}

var jwtSettings = System.Text.Json.JsonSerializer.Deserialize<JwtSettings>(jwtSettingsJson) ?? throw new InvalidOperationException("Failed to deserialize 'jwt' environment variable into JwtSettings.");
var googleSettings = System.Text.Json.JsonSerializer.Deserialize<GoogleSettings>(googleSettingsJson) ?? throw new InvalidOperationException("Failed to deserialize 'Google' environment variable into GoogleSettings.");
// Register JwtSettings in the DI container
builder.Services.Configure<JwtSettings>(options =>
{
    options.Issuer = jwtSettings.Issuer;
    options.Audience = jwtSettings.Audience;
    options.SecretKey = jwtSettings.SecretKey;
});

builder.Services.Configure<GoogleSettings>(options =>
{
    options.ClientId = googleSettings.ClientId;
});

builder.Services.AddChatClient(
    new ChatClient(
            "gpt-4o",
            new ApiKeyCredential(githubModelsToken!),
            new OpenAIClientOptions { Endpoint = new Uri("https://models.github.ai/inference") })
        .AsIChatClient()
        );
builder.Services.AddHttpClient();
builder.Services.AddSingleton<TavilySearchTool>();
builder.Services.AddSingleton<ScrapinLinkedInTool>();
builder.Services.AddSingleton<LinkedInEnrichmentTool>();
builder.Services.AddSingleton<DateTool>();

builder.AddAIAgent("webSearchAgent", (sp, key) =>
{
    var chatClient = sp.GetRequiredService<IChatClient>();
    var linkedInEnrichmentTool = sp.GetRequiredService<LinkedInEnrichmentTool>();
    var tavilySearchTool = sp.GetRequiredService<TavilySearchTool>();
    var dateTool = sp.GetRequiredService<DateTool>();

    return new ChatClientAgent(
        chatClient,
        name: key,
        instructions: "You are a helpful assistant",
        tools: [
            AIFunctionFactory.Create(linkedInEnrichmentTool.EnrichLinkedInProfileAsync),
            AIFunctionFactory.Create(tavilySearchTool.SearchTavilyAsync),
            AIFunctionFactory.Create(dateTool.GetCurrentDateAsync)
        ]
    );
});

// Use AddIdentityCore instead of AddIdentity for API scenarios
builder.Services.AddIdentityCore<IdentityUser<Guid>>()
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<UserDbContext>();

builder.Services.AddDbContext<UserDbContext>();

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

// Add services to the container.
builder.Services.AddControllers();

// Configure Authorization and Authentication
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

var app = builder.Build();

// Configure to allow any origin, any header, and any method
app.UseCors(policy =>
{
    policy.AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod();
});

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();

app.MapGet("/", () => "Welcome to running ASP.NET Core Minimal API on AWS Lambda");

app.Run();
