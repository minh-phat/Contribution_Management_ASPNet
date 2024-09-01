using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolProject1640.Data;
using SchoolProject1640.Models;
using SQLitePCL;
using System.Diagnostics;

namespace SchoolProject1640.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        IWebHostEnvironment _environment;
        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _environment = environment;
        }
        public async Task<string> GetFaculty()
        {
            ApplicationUser author = await _userManager.GetUserAsync(HttpContext.User) ?? new ApplicationUser();
            var getFaculty = await _context.Faculty.FirstOrDefaultAsync(m => m.Id == author.FacultyId);

            if (getFaculty != null)
            {
                return getFaculty.Name; 
            }
            else
            {
                return ""; 
            }
        }
        [Authorize(Roles = "Administrator,Guest,Student,Coordinator,Manager")]
        public async Task<IActionResult> IndexAsync()
        {
            ApplicationUser author = await _userManager.GetUserAsync(HttpContext.User) ?? new ApplicationUser();
            var tempListFaculty = await _context.Faculty.FirstOrDefaultAsync(m => m.Id == author.FacultyId);
            if (tempListFaculty != null)
            {
                ViewBag.getFacultyOfStudent = tempListFaculty.Id;
            }
            else
            {
                ViewBag.getFacultyOfStudent = "";
            }
            var roles = await _userManager.GetRolesAsync(author);
            ViewBag.listNameFaculty = _context.Faculty.ToList();

            ViewBag.listArt = _context.Article.Where(m => m.State == 1).ToList();

            //ViewBag.listArt = _context.Article.ToList();

            ViewBag.listNameUSer = _context.User.ToList();
            ViewBag.idUser = author.Id;

            if (HttpContext.Session.GetString("NewLogin") == "true")
            {
                var notifications = _context.Notification
                    .Where(noti => noti.UserID == author.Id && noti.isRead == false)
                    .ToList();

                HttpContext.Session.SetString("NewLogin", "false");
                ViewBag.notifications = notifications;
            }
            else
            {
                ViewBag.notifications = null;
            }

            return _context.Contribution != null ?
                        View(await _context.Contribution.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Contribution'  is null.");
            //return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
