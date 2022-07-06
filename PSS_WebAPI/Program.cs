using System.Net.Http.Headers;
using Insight.PermissionsDealer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using PSS_WebAPI.Permissions;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;

//var host = Host.CreateDefaultBuilder().ConfigureLogging(builder =>
//{
//    builder.AddApplicationInsights("5155123d-08bf-4619-83c3-113c19c8861e", opt => { });
//    builder.AddFilter<ApplicationInsightsLoggerProvider>(typeof(Program).FullName, LogLevel.Trace);

//});

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddApplicationInsights("5155123d-08bf-4619-83c3-113c19c8861e");

var services = builder.Services;
var configuration = builder.Configuration;

services.AddCors(options =>
                options.AddDefaultPolicy(policy =>
                        policy.SetIsOriginAllowedToAllowWildcardSubdomains()
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowAnyOrigin()
                        ));
services.AddEndpointsApiExplorer();
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"));

services.AddAuthorization(o => o.AddPolicy("Customers", b => b.RequireRole("customer")
                                 .AddRequirements(new PermissionRequirement("position", "update"))));

services.AddSwaggerGen(t =>
{
    t.OperationFilter<SecurityRequirementsOperationFilter>();

    t.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Name = "authorization",
        Type = SecuritySchemeType.OAuth2,
        BearerFormat = "JWT",
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow()
            {
                AuthorizationUrl = new Uri(configuration["AzureAd:AuthorizationUrl"]),
                TokenUrl = new Uri(configuration["AzureAd:TokenUrl"]),
                Scopes = new Dictionary<string, string> {
                    { configuration["AzureAd:PortfoliosReadScope"], "Allows to read portfolios." }
                }
            }
        }
    });
});

services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
services.AddSingleton<IPermissionManager, PermissionManager>();
services.AddSingleton<IPermissionAssistant, PermissionAssistant>();

services.AddHttpClient("DataSourceRepo", httpClient => httpClient.BaseAddress = new Uri(configuration["PSS_DataSourceRepoUrl"]));

services.AddApplicationInsightsTelemetry();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Running PSS Web API");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API v1");
    c.OAuthConfigObject = new OAuthConfigObject
    {
        AppName = "OPA Styra API",
        ClientId = configuration["AzureAd:ClientId"],
        AdditionalQueryStringParams = new Dictionary<string, string>()
    };
    c.OAuthUsePkce();
});

app.UseCors(policy => policy.AllowAnyMethod()
                            .AllowAnyHeader()
                            .SetIsOriginAllowed(origin => true));
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/freeportfolios", () =>
{
    // Authorization free portfolios
    return new[] { "abcde, bcedf, xwxyir" };
})
.WithName("GetFreePortfolios");

app.MapGet("/portfolios", [Authorize(Policy = "Customers")] () =>
{
    return new[] { "ABCDE, BCEDF, XWXYIR" };
})
.WithName("GetPortfolios");

app.MapGet("/scopes", [Authorize(Policy = "Customers")]
async () =>
{
    var httpClient = new HttpClient
    {
        BaseAddress = new Uri(configuration["GraphUrl"])
    };
    // might need to renew token
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuration["GraphToken"]);

    var httpResponseMessage = await httpClient.GetAsync("v1.0/applications/6a951769-773e-4e92-bb4b-96565b3b5331");

    if (httpResponseMessage.IsSuccessStatusCode)
    {
        return new[] { await httpResponseMessage.Content.ReadAsStringAsync() };
    }

    return new[] { "Unsuccessful call." };
})
.WithName("GetScopes");

logger.LogInformation("Running PSS Web API");

app.Run();