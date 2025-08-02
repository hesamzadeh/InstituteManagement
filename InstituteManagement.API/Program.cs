using InstituteManagement.API.Mappings;
using InstituteManagement.API.Services;
using InstituteManagement.Application.Common.Interfaces;
using InstituteManagement.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new() { Title = "Institute API", Version = "v1" });
    // Optional: Add JWT support later
});

builder.Services.AddAuthentication();   
builder.Services.AddAuthorization();

//builder.Services.AddMediatR(cfg =>
//    cfg.RegisterServicesFromAssembly(typeof(AutoMapperProfile).Assembly));

builder.Services.AddScoped<IAppDbContext, AppDbContext>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Add localization services
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en", "fa" };
    options.SetDefaultCulture("fa")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);

    options.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());
});

builder.Services.AddHttpClient();
builder.Services.AddScoped<ICaptchaValidator, GoogleRecaptchaValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Institute API v1");
});

app.UseRequestLocalization(); // <- This enables Accept-Language support

app.Run();
