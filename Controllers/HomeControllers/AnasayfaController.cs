using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using EgitimSitesi.Models;
using EgitimSitesi.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EgitimSitesi.Controllers.HomeControllers
{
    public class AnasayfaController : Controller
    {
        private readonly ILogger<AnasayfaController> _logger;
        private readonly ApplicationDbContext _context;

        public AnasayfaController(ILogger<AnasayfaController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch active banners
            var activeBanners = await _context.Banners
                .Where(b => b.IsActive)
                .OrderBy(b => b.Order)
                .ToListAsync();
            
            // Fetch active content items
            var activeContentItems = await _context.AnasayfaIcerik
                .Where(c => c.IsActive)
                .OrderBy(c => c.Order)
                .ToListAsync();
            
            // Fetch latest 6 active announcements
            var recentAnnouncements = await _context.Duyurular
                .Where(d => d.IsActive)
                .OrderByDescending(d => d.AnnouncementDate)
                .Take(6)
                .ToListAsync();
            
            // Fetch active courses
            var activeCourses = await _context.Kurslar
                .Where(k => k.IsActive)
                .OrderBy(k => k.Order)
                .ToListAsync();

            ViewBag.Banners = activeBanners;
            ViewBag.ContentItems = activeContentItems;
            ViewBag.RecentAnnouncements = recentAnnouncements;
            ViewBag.Courses = activeCourses;
            
            return View("/Views/Home/Anasayfa/Index.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
} 