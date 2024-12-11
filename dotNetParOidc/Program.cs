using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using DotNetParOidc.Client.Weather;
using DotNetParOidc.Components;
using DotNetParOidc.Weather;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;

const string MS_OIDC_SCHEME = OpenIdConnectDefaults.AuthenticationScheme;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(MS_OIDC_SCHEME)
    .AddOpenIdConnect(MS_OIDC_SCHEME, oidcOptions =>
    {
        oidcOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        oidcOptions.RequireHttpsMetadata = false;
        oidcOptions.Authority = "http://localhost:8080/realms/myrealm"; //"{AUTH SERVER URL}"
        oidcOptions.ClientId = "dotnetconf-client1"; // "{CLIENT ID}"; 
        oidcOptions.ResponseType = OpenIdConnectResponseType.Code;

        oidcOptions.MapInboundClaims = false;
        oidcOptions.SaveTokens = true;
        oidcOptions.GetClaimsFromUserInfoEndpoint = true;

        oidcOptions.Scope.Add("openid");
        oidcOptions.Scope.Add("profile");
        oidcOptions.Scope.Add("email");

        oidcOptions.TokenValidationParameters.NameClaimType = "name";

        oidcOptions.PushedAuthorizationBehavior = PushedAuthorizationBehavior.Require;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);


builder.Services.ConfigureCookieOidcRefresh(CookieAuthenticationDefaults.AuthenticationScheme, MS_OIDC_SCHEME);

builder.Services.AddAuthorization();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddScoped<IWeatherForecaster, ServerWeatherForecaster>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapStaticAssets();
app.UseAntiforgery();

app.MapGet("/weather-forecast", ([FromServices] IWeatherForecaster WeatherForecaster) =>
{
    return WeatherForecaster.GetWeatherForecastAsync();
}).RequireAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(DotNetParOidc.Client._Imports).Assembly);

app.MapGroup("/authentication").MapLoginAndLogout();

app.Run();
