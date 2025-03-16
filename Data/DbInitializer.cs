using EgitimSitesi.Models;
using Microsoft.EntityFrameworkCore;
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
            
            // Apply any pending migrations
            await dbContext.Database.MigrateAsync();
            
            // Ensure SiteSettings exists
            await EnsureSiteSettingsExistsAsync(dbContext);
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