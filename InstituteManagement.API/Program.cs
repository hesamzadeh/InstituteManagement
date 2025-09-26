using FluentValidation;
using FluentValidation.AspNetCore; // <== Required for AddFluentValidationAutoValidation()
using InstituteManagement.API.Mappings;
using InstituteManagement.API.Services;
using InstituteManagement.Application.Common.Interfaces;
using InstituteManagement.Application.Validators.Auth;
using InstituteManagement.Infrastructure;
using InstituteManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// OpenAPI
builder.Services.AddOpenApi();

// DB & Identity
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity (typical)
builder.Services.AddIdentity<AppUser, AppRole>(options => {
    // options.Password/Lockout etc
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, AppUserClaimsPrincipalFactory>();

// FluentValidation integration
builder.Services
    .AddFluentValidationAutoValidation() // <- This requires FluentValidation.AspNetCore
    .AddValidatorsFromAssemblyContaining<SignupDtoValidator>()
    .AddValidatorsFromAssemblyContaining<SignInDtoValidator>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Dependency Injection
builder.Services.AddScoped<IAppDbContext, AppDbContext>();
builder.Services.AddScoped<ICaptchaValidator, GoogleRecaptchaValidator>();

// Localization
//builder.Services.AddLocalization();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en", "fa" };
    options.SetDefaultCulture("en")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);

    options.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());
});

// Authentication cookie configuration
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "MyAppAuth";
    options.Cookie.HttpOnly = true;
    // If your Blazor and API are on different origins, set SameSite=None and Secure
    options.Cookie.SameSite = SameSiteMode.None; // if cross-site
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // production
    options.LoginPath = "/signin";
    options.LogoutPath = "/api/auth/logout";
});

// CORS (if front-end runs on different origin during development)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:7224", "http://localhost:5090") // add your Blazor origin(s)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Auth
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddHttpClient();

// Add Swagger services
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Institute API", Version = "v1" });
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Institute API v1");
        options.RoutePrefix = string.Empty; // Optional: serve Swagger UI at root
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowLocalFrontend");

app.UseRequestLocalization(); // Accept-Language support

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
