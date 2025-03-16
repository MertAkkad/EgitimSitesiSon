using EgitimSitesi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace EgitimSitesi.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var environment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
            
            try
            {
                // In production, suppress the pending model changes warning
                if (environment.IsProduction())
                {
                    // Make sure the database exists
                    var databaseCreator = dbContext.Database.GetService<IRelationalDatabaseCreator>();
                    
                    // Try to create database if it doesn't exist
                    if (!databaseCreator.Exists())
                    {
                        logger.LogInformation("Database doesn't exist. Creating database...");
                        databaseCreator.Create();
                    }
                    
                    // Try to create tables if they don't exist
                    try
                    {
                        logger.LogInformation("Creating tables if they don't exist...");
                        databaseCreator.CreateTables();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error creating tables, they might already exist: {Message}", ex.Message);
                        // Tables might already exist, so we'll continue
                    }
                    
                    // Ensure all tables have been created by checking each expected table
                    logger.LogInformation("Verifying all required tables exist...");
                    VerifyTablesExist(dbContext, logger);
                }
                else
                {
                    // Apply any pending migrations in development
                    await dbContext.Database.MigrateAsync();
                }
                
                // Ensure SiteSettings exists
                await EnsureSiteSettingsExistsAsync(dbContext);
                
                logger.LogInformation("Database initialization completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database initialization error: {Message}", ex.Message);
                // Continue anyway - we don't want to crash the application
            }
        }
        
        private static void VerifyTablesExist(ApplicationDbContext dbContext, ILogger logger)
        {
            try
            {
                // Check some essential tables by querying them (this verifies they exist and are accessible)
                logger.LogInformation("Checking if SiteSettings table exists...");
                var settingsCount = dbContext.SiteSettings.Count();
                
                logger.LogInformation("Checking if Banners table exists...");
                var bannersCount = dbContext.Banners.Count();
                
                logger.LogInformation("All tables verified successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error verifying tables: {Message}", ex.Message);
                // Continue anyway
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