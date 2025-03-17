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
                // In production, ensure database exists and create tables
                if (environment.IsProduction())
                {
                    Console.WriteLine("Running in Production mode - creating database schema");
                    
                    // Ensure database exists
                    await dbContext.Database.EnsureCreatedAsync();
                    
                    // Check if tables exist already
                    var databaseCreator = dbContext.Database.GetService<IRelationalDatabaseCreator>();
                    if (!await databaseCreator.ExistsAsync())
                    {
                        Console.WriteLine("Database does not exist - creating");
                        await databaseCreator.CreateAsync();
                    }
                    
                    if (!await databaseCreator.HasTablesAsync())
                    {
                        Console.WriteLine("Tables do not exist - creating schema");
                        await databaseCreator.CreateTablesAsync();
                    }
                    
                    Console.WriteLine("Database initialization completed successfully");
                }
                else
                {
                    // Apply any pending migrations in development
                    Console.WriteLine("Running in Development mode - applying migrations");
                    await dbContext.Database.MigrateAsync();
                }
                
                // Ensure SiteSettings exists
                await EnsureSiteSettingsExistsAsync(dbContext);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner stack trace: {ex.InnerException.StackTrace}");
                }
                
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
                    LastUpdated = DateTime.Now
                };
                
                dbContext.SiteSettings.Add(defaultSettings);
                await dbContext.SaveChangesAsync();
            }
        }
    }
} 