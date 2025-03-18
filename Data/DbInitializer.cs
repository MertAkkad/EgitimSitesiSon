using EgitimSitesi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EgitimSitesi.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var environment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
            
            try
            {
                // In production, suppress the pending model changes warning
                if (environment.IsProduction())
                {
                    try
                    {
                        // Try to create tables but ignore errors if tables already exist
                        var dbCreator = dbContext.Database.GetService<IRelationalDatabaseCreator>();
                        if (!dbCreator.Exists())
                        {
                            dbCreator.Create();
                        }
                        
                        if (!dbCreator.HasTables())
                        {
                            dbCreator.CreateTables();
                        }
                    }
                    catch (Exception ex) when (ex.Message.Contains("already exists"))
                    {
                        Console.WriteLine("Some tables already exist. This is normal in a deployed application.");
                    }
                }
                else
                {
                    // Apply any pending migrations in development
                    await dbContext.Database.MigrateAsync();
                }
                
                // Ensure SiteSettings exists
                await EnsureSiteSettingsExistsAsync(dbContext);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization error: {ex.Message}");
                // Continue anyway - we don't want to crash the application
            }
        }
        
        private static async Task EnsureSiteSettingsExistsAsync(ApplicationDbContext dbContext)
        {
            // Check if any SiteSettings record exists
            if (!await dbContext.SiteSettings.AnyAsync())
            {
                // Create default SiteSettings
                var defaultSettings = new SiteSettingsModel
                {
                    ActiveLayout = "_Layout",
                    LastUpdated = DateTime.UtcNow
                };
                
                dbContext.SiteSettings.Add(defaultSettings);
                await dbContext.SaveChangesAsync();
            }

            // Check if Hakkimizda record exists
            if (!await dbContext.Hakkimizda.AnyAsync())
            {
                try
                {
                    // Create a default Hakkimizda record
                    var hakkimizda = new HakkimizdaModel
                    {
                        Tarihcemiz = "Kurum Tarihçesi burada yer alacaktır.",
                        Vizyonumuz = "Kurum Vizyonu burada yer alacaktır.",
                        Misyonumuz = "Kurum Misyonu burada yer alacaktır.",
                        ImagePath = "/images/about.jpg",
                        CreationDate = DateTime.UtcNow,
                        IsActive = true
                    };
                    
                    dbContext.Hakkimizda.Add(hakkimizda);
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating Hakkimizda: {ex.Message}");
                    // Continue processing
                }
            }
        }
    }
} 