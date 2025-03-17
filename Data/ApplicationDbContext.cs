using EgitimSitesi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace EgitimSitesi.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IWebHostEnvironment _environment;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IWebHostEnvironment environment)
            : base(options)
        {
            _environment = environment;
        }

        public DbSet<BannerModel> Banners { get; set; }
        public DbSet<Anasayfa_IcerikModel> AnasayfaIcerik { get; set; }
        public DbSet<DuyurularModel> Duyurular { get; set; }
        public DbSet<KadromuzModel> Kadromuz { get; set; }
        public DbSet<SubeModel> Subeler { get; set; }
        public DbSet<HakkimizdaModel> Hakkimizda { get; set; }
        public DbSet<KayitFormuModel> KayitFormu { get; set; }
        public DbSet<IletisimModel> Iletisim { get; set; }
        public DbSet<BizeYazinModel> BizeYazin { get; set; }
        public DbSet<SiteSettingsModel> SiteSettings { get; set; }
        public DbSet<KursModel> Kurslar { get; set; }
        public DbSet<GalleryModel> Gallery { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Get the default date function based on environment
            string defaultDateFunction = _environment.IsProduction() ? "NOW()" : "GETDATE()";

            // Configure BannerModel
            modelBuilder.Entity<BannerModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ImagePath).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.ButtonText).HasMaxLength(50);
                entity.Property(e => e.ButtonUrl).HasMaxLength(200);
                entity.Property(e => e.Order).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreationDate).HasDefaultValueSql(defaultDateFunction);
            });

            // Configure DuyurularModel
            modelBuilder.Entity<DuyurularModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                entity.Property(e => e.ImagePath).HasMaxLength(200);
                entity.Property(e => e.IconClass).HasMaxLength(50);
                entity.Property(e => e.ButtonText).HasMaxLength(50);
                entity.Property(e => e.ButtonUrl).HasMaxLength(200);
                entity.Property(e => e.Order).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreationDate).HasDefaultValueSql(defaultDateFunction);
                entity.Property(e => e.AnnouncementDate).HasDefaultValueSql(defaultDateFunction);
                entity.Property(e => e.AnnouncementType).HasMaxLength(50);
            });
            
            // Configure KadromuzModel
            modelBuilder.Entity<KadromuzModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Grade).IsRequired();
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.Field).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Exp).HasMaxLength(200).IsRequired(false);
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Order).IsRequired();
                entity.Property(e => e.ImagePath).HasMaxLength(200).IsRequired(false);
                entity.Property(e => e.CreationDate).HasDefaultValueSql(defaultDateFunction);
            });
            
            // Configure SubeModel
            modelBuilder.Entity<SubeModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
                entity.Property(e => e.TelNo).IsRequired().HasMaxLength(20);
                entity.Property(e => e.WorkHours).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Latitude).IsRequired();
                entity.Property(e => e.Longitude).IsRequired();
                entity.Property(e => e.MapUrl).HasMaxLength(500);
                entity.Property(e => e.ZoomLevel).HasDefaultValue(15);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Order).IsRequired();
                entity.Property(e => e.CreationDate).HasDefaultValueSql(defaultDateFunction);
            });
            
            // Configure HakkimizdaModel
            modelBuilder.Entity<HakkimizdaModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ImagePath).HasMaxLength(200);
                entity.Property(e => e.Tarihcemiz).IsRequired();
                entity.Property(e => e.Vizyonumuz).IsRequired();
                entity.Property(e => e.Misyonumuz).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreationDate).HasDefaultValueSql(defaultDateFunction);
            });

            // Configure KayitFormuModel
            modelBuilder.Entity<KayitFormuModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.TelNo).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Grade).IsRequired().HasMaxLength(50);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreationDate).HasDefaultValueSql(defaultDateFunction);
            });

            // Configure IletisimModel
            modelBuilder.Entity<IletisimModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MerkezSube).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Adress).IsRequired().HasMaxLength(500);
                entity.Property(e => e.TelNo).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.GoogleMapsEmbed).HasMaxLength(1000);
                entity.Property(e => e.CalismaSaatleri).HasMaxLength(200);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreationDate).HasDefaultValueSql(defaultDateFunction);
            });
            
            // Configure BizeYazinModel
            modelBuilder.Entity<BizeYazinModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AdSoyad).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.TelefonNo).HasMaxLength(20);
                entity.Property(e => e.Konu).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Mesaj).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.Okundu).HasDefaultValue(false);
                entity.Property(e => e.GonderimTarihi).HasDefaultValueSql(defaultDateFunction);
                entity.Property(e => e.IpAdresi).HasMaxLength(50);
            });

            // Configure SiteSettingsModel
            modelBuilder.Entity<SiteSettingsModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ActiveLayout).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastUpdated).HasDefaultValueSql(defaultDateFunction);
            });
            
            // Configure KursModel
            modelBuilder.Entity<KursModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Details).HasColumnType("text");
                entity.Property(e => e.Image).HasMaxLength(200);
                entity.Property(e => e.Order).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreationDate).HasDefaultValueSql(defaultDateFunction);
            });
            
            // Configure GalleryModel
            modelBuilder.Entity<GalleryModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Image).IsRequired();
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.CloudinaryPublicId).HasMaxLength(200);
                entity.Property(e => e.Order).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreationDate).HasDefaultValueSql(defaultDateFunction);
            });
        }

        public override int SaveChanges()
        {
            HandleDateTimeProperties();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            HandleDateTimeProperties();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void HandleDateTimeProperties()
        {
            // Convert all DateTime properties to UTC before saving when in production
            if (_environment.IsProduction())
            {
                foreach (var entry in ChangeTracker.Entries())
                {
                    if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                    {
                        foreach (var property in entry.Properties)
                        {
                            if (property.Metadata.ClrType == typeof(DateTime) && property.CurrentValue != null)
                            {
                                DateTime dt = (DateTime)property.CurrentValue;
                                if (dt.Kind != DateTimeKind.Utc)
                                {
                                    property.CurrentValue = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
} 