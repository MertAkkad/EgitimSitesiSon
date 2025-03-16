using System;
using System.ComponentModel.DataAnnotations;

namespace EgitimSitesi.Models
{
    public class SiteSettingsModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ActiveLayout { get; set; } = "_Layout"; // Default layout
        
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
} 