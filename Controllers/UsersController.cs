using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolProject1640.Data;
using SchoolProject1640.Models;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using SchoolProject1640.Models.ModelOfView;
using SkiaSharp;
using NuGet.Protocol.Plugins;

namespace SchoolProject1640.Controllers
{
    public class UsersController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment Environment;
        
        public UsersController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            Environment = hostingEnvironment;
        }
        public async Task<IActionResult> DashBoardStudent()
        {
            ApplicationUser author = await _userManager.GetUserAsync(HttpContext.User) ?? new ApplicationUser();
            StudentDashboard tempADDB = new StudentDashboard();

            // query bai cua khoa
            tempADDB.TotalArticleStudent = _context.Article.Where(m=> m.AccountId == author.Id).Count().ToString();
            tempADDB.ArticleRejectedStudent = _context.Article.Where(m => m.State == 2).Count().ToString();
            tempADDB.ArticleAcceptStudent = _context.Article.Where(m => m.State == 1).Count().ToString();
            tempADDB.ArticlePendingStudent = _context.Article.Where(m => m.State == 0).Count().ToString();

            return View(tempADDB);

        }
        public async Task<IActionResult> IndexAsync()
        {
            ApplicationUser author = await _userManager.GetUserAsync(HttpContext.User) ?? new ApplicationUser();
            if (author.Email != null)
            {
                ApplicationUser user = _context.Users.FirstOrDefault(m => m.Email == author.Email);
                return View(user);
            }
            else
            {
                return View();
            }
        }

        // GET: Users/Edit
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(string id, List<IFormFile> files, [Bind("Id,FirstName,LastName,Image")] ApplicationUser user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _userManager.FindByIdAsync(id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    string defaultImagePath = "ImageDefaultUser.png";
                    if (files != null && files.Any())
                    {
                        string wwwPath = this.Environment.WebRootPath;
                        string folderPath = Path.Combine(wwwPath, "imageUser");

                        foreach (var formFile in files)
                        {
                            if (formFile.Length > 0)
                            {
                                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(formFile.FileName);
                                var fileExtension = Path.GetExtension(formFile.FileName);
                                var newFileName = $"{fileNameWithoutExtension}_{timestamp}{fileExtension}";
                                var filePath = Path.Combine(folderPath, newFileName);
                                existingUser.Image = newFileName;

                                using (var stream = System.IO.File.Create(filePath))
                                {
                                    await formFile.CopyToAsync(stream);
                                }
                            }
                        }
                    }
                    else
                    {
                        existingUser.Image = defaultImagePath;
                    }

                    var result = await _userManager.UpdateAsync(existingUser);
                    if (!result.Succeeded)
                    {
                        // Handle error
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    return View(user);
                }
            }
            return View(user);
        }

    }
}
