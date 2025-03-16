using Microsoft.EntityFrameworkCore;
using EgitimSitesi.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using EgitimSitesi.Models;
using CloudinaryDotNet;
using EgitimSitesi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add database context
if (builder.Environment.IsProduction())
{
    // Use PostgreSQL in production
    // Build connection string from environment variables
    var host = Environment.GetEnvironmentVariable("PGHOST");
    var port = Environment.GetEnvironmentVariable("PGPORT");
    var database = Environment.GetEnvironmentVariable("PGDATABASE");
    var username = Environment.GetEnvironmentVariable("PGUSER");
    var password = Environment.GetEnvironmentVariable("PGPASSWORD");
    
    var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
    
    builder.Services.AddDbContext<EgitimSitesi.Data.ApplicationDbContext>(options =>
    {
        options.UseNpgsql(connectionString, npgsqlOptions =>
        {
            // Set migrations compatibility
            npgsqlOptions.MigrationsAssembly("EgitimSitesi");
            
            // Set retry on failure
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null);
        });
        
        // Suppress the pending model changes warning in production
        options.ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        
        // Add SqlServer to PostgreSQL translation
        options.ReplaceService<Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.IMethodCallTranslatorProvider,
            EgitimSitesi.Data.PostgresMethodCallTranslatorProvider>();
    });
}
else
{
    // Use SQL Server in development
    builder.Services.AddDbContext<EgitimSitesi.Data.ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// Add authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.AccessDeniedPath = "/Admin/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

// Configure Cloudinary
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddSingleton(provider => {
    var config = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<CloudinarySettings>>().Value;
    return new Cloudinary(new Account(
        config.CloudName,
        config.ApiKey,
        config.ApiSecret));
});
builder.Services.AddScoped<EgitimSitesi.Services.CloudinaryService>();

var app = builder.Build();

// Use error handling middleware
app.UseErrorHandling();

// Initialize the database and ensure default data exists
await EgitimSitesi.Data.DbInitializer.Initialize(app.Services);

// Apply migrations and seed the database in development only
if (app.Environment.IsDevelopment())
{
    // Call the seeder
    await DbSeeder.SeedDatabase(app);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Map root and /Home to Anasayfa controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Anasayfa}/{action=Index}/{id?}",
    defaults: new { controller = "Anasayfa" })
    .WithStaticAssets();

// Home controllers in HomeControllers folder
app.MapControllerRoute(
    name: "homeControllers",
    pattern: "{controller}/{action=Index}/{id?}",
    defaults: new { area = "" },
    constraints: new { controller = @"(Anasayfa|Hakkimizda|Kadromuz|Subelerimiz|BasvuruFormu|Iletisim)" })
    .WithStaticAssets();

// Course controllers in HomeControllers/Kurslar folder
app.MapControllerRoute(
    name: "kurslarControllers",
    pattern: "Kurslar/{controller=Kurslar}/{action=Index}/{id?}",
    defaults: new { area = "" })
    .WithStaticAssets();

await app.RunAsync();
