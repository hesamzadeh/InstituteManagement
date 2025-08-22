
using InstituteManagement.Application.Common.Interfaces;
using InstituteManagement.Front;
using InstituteManagement.Front.Components;
using InstituteManagement.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add localization services
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddTransient<UiLocalizer>();

// Add Razor components for Blazor Server
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// HttpClient setup for calling API
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:5271/");
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/signin";
    options.LogoutPath = "/api/auth/logout";
    options.AccessDeniedPath = "/accessdenied";
    options.SlidingExpiration = true;
});

builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<ApiAuthenticationStateProvider>());
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("ApiClient");
});

// Define supported cultures
var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("fa")
};

// Configure localization options
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    // Use Cookie to set culture
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new CookieRequestCultureProvider()
    };
});


var app = builder.Build();

// Retrieve the configured localization options
var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;

// Ensure localization is applied
app.UseRequestLocalization(localizationOptions);

// This middleware ensures CultureInfo.CurrentCulture is correctly updated
app.Use(async (context, next) =>
{
    var feature = context.Features.Get<IRequestCultureFeature>();
    if (feature != null)
    {
        CultureInfo.CurrentCulture = feature.RequestCulture.Culture;
        CultureInfo.CurrentUICulture = feature.RequestCulture.UICulture;
    }
    await next();
});

// Error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Endpoint for setting the language via a cookie
app.MapGet("/SetLanguage", async (string culture, string redirectUri, HttpContext context) =>
{
    context.Response.Cookies.Append(
        CookieRequestCultureProvider.DefaultCookieName,
        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
        new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
    );

    context.Response.Redirect(redirectUri);
});

app.Run();
