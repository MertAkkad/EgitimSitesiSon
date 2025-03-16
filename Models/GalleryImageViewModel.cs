using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace EgitimSitesi.Models
{
    public class GalleryImageViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Mevcut Resim")]
        public string CurrentImage { get; set; }
        
        [Display(Name = "Yeni Resim")]
        public IFormFile Image { get; set; }
        
        [Required(ErrorMessage = "Lütfen bir kategori seçin")]
        [Display(Name = "Kategori")]
        public GalleryType Type { get; set; }
        
        [Display(Name = "Açıklama")]
        public string Description { get; set; }
        
        [Display(Name = "Oluşturulma Tarihi")]
        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; } = DateTime.Now;
        
        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;
        
        [Display(Name = "Sıralama")]
        [Range(1, 1000, ErrorMessage = "Sıralama 1-1000 arasında olmalıdır")]
        public int Order { get; set; }
        
        public string CloudinaryPublicId { get; set; }
    }
    
    public class GalleryImageListViewModel
    {
        public List<GalleryModel> Images { get; set; }
        public string TypeFilter { get; set; }
        public string SearchTerm { get; set; }
        public bool? ActiveFilter { get; set; }
    }
} 