using System;
using System.ComponentModel.DataAnnotations;

namespace EgitimSitesi.Models
{
    public abstract class BaseEntityModel
    {
        [Display(Name = "Oluşturma Tarihi")]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        
        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;
    }
} 