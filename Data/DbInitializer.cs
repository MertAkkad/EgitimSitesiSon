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
                    dbContext.Database.GetService<IRelationalDatabaseCreator>().CreateTables();
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
                    LastUpdated = DateTime.Now
                };
                
                dbContext.SiteSettings.Add(defaultSettings);
                await dbContext.SaveChangesAsync();
            }
        }
    }
} 