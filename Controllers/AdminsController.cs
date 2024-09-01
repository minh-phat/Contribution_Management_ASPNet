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
using Microsoft.AspNetCore.SignalR;
using SchoolProject1640.Hubs;
using System.Linq;
using Microsoft.Identity.Client;
using SchoolProject1640.Models.ModelOfView;
using System.Security.Cryptography;

namespace SchoolProject1640.Controllers
{
    public class AdminsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        IWebHostEnvironment _environment;
        public AdminsController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context, IHubContext<NotificationHub> hubContext, IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _hubContext = hubContext;
            _environment = environment;
        }
        public async Task<IActionResult> DashBoardAdmin()
        {
            ApplicationUser author = await _userManager.GetUserAsync(HttpContext.User) ?? new ApplicationUser();
            AdminDashboard tempADDB = new AdminDashboard();
            tempADDB.TotalFaculty = _context.Faculty.Count().ToString();
            tempADDB.TotalStudent = _context.UserRoles.Where(m => m.RoleId == "2").Count().ToString(); // 2 mean student
            tempADDB.TotalArticle = _context.Article.Count().ToString();
            tempADDB.ArticleAccept = _context.Article.Where(m => m.State == 1).Count().ToString();
            tempADDB.TotalContribution = _context.Contribution.Count().ToString();
            tempADDB.UserPostArticleCount = (from article in _context.Article
                                             group article by article.AccountId into g
                                             select new { UserId = g.Key, Count = g.Count() })
                                     .ToDictionary(x => x.UserId, x => x.Count);
            return View(tempADDB);

        }
        [Authorize(Roles = "Administrator, Manager")]
        public IActionResult IndexManager()
        {
            return View();
        }
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> IndexManagerValue(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                var listUserWithRoleAndFaculty = _context.User
                    .Join(_context.UserRoles,
                        user => user.Id,
                        userRole => userRole.UserId,
                        (user, userRole) => new { User = user, UserRole = userRole })
                    .Join(_context.Roles,
                        userRole => userRole.UserRole.RoleId,
                        role => role.Id,
                        (userRole, role) => new { User = userRole.User, RoleName = role.Name })
                    .Join(_context.Faculty,
                        userRole => userRole.User.FacultyId,
                        faculty => faculty.Id,
                        (userRole, faculty) => new { User = userRole.User, RoleName = userRole.RoleName, FacultyName = faculty.Name })
                    .Where(u => u.RoleName != "Administrator" && u.RoleName != "Manager" && u.RoleName != "Student" && u.RoleName != "Guest" && u.User.Email.Contains(email))
                    .OrderByDescending(u => u.User.Email)
                    .Select(u => new
                    {
                        User = u.User,
                        RoleName = u.RoleName,
                        FacultyName = u.FacultyName
                    })
                    .ToList();
                return Json(listUserWithRoleAndFaculty);
            }
            else
            {
                var listUserWithRoleAndFaculty = _context.User
                     .Join(_context.UserRoles,
                         user => user.Id,
                         userRole => userRole.UserId,
                         (user, userRole) => new { User = user, UserRole = userRole })
                     .Join(_context.Roles,
                         userRole => userRole.UserRole.RoleId,
                         role => role.Id,
                         (userRole, role) => new { User = userRole.User, RoleName = role.Name })
                     .Join(_context.Faculty,
                         userRole => userRole.User.FacultyId,
                         faculty => faculty.Id,
                         (userRole, faculty) => new { User = userRole.User, RoleName = userRole.RoleName, FacultyName = faculty.Name })
                     .Where(u => u.RoleName != "Administrator" && u.RoleName != "Manager" && u.RoleName != "Student" && u.RoleName != "Guest")
                     .OrderByDescending(u => u.User.Email)
                     .Select(u => new
                     {
                         User = u.User,
                         RoleName = u.RoleName,
                         FacultyName = u.FacultyName
                     })
                     .ToList();

                return Json(listUserWithRoleAndFaculty);
            }
        }

        [Authorize(Roles = "Administrator, Manager")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrator, Manager")]
        [HttpGet]
        public IActionResult SearchAcount(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                var listUserWithRoleAndFaculty = _context.User
                    .Join(_context.UserRoles,
                        user => user.Id,
                        userRole => userRole.UserId,
                        (user, userRole) => new { User = user, UserRole = userRole })
                    .Join(_context.Roles,
                        userRole => userRole.UserRole.RoleId,
                        role => role.Id,
                        (userRole, role) => new { User = userRole.User, RoleName = role.Name })
                    .Join(_context.Faculty,
                        userRole => userRole.User.FacultyId,
                        faculty => faculty.Id,
                        (userRole, faculty) => new { User = userRole.User, RoleName = userRole.RoleName, FacultyName = faculty.Name })
                    .Where(u => u.RoleName != "Administrator" && u.User.Email.Contains(email))
                    .OrderByDescending(u => u.User.Email)
                    .Select(u => new
                    {
                        User = u.User,
                        RoleName = u.RoleName,
                        FacultyName = u.FacultyName
                    })
                    .ToList();

                return Json(listUserWithRoleAndFaculty);
            }
            else
            {
                var listUserWithRoleAndFaculty = _context.User
                     .Join(_context.UserRoles,
                         user => user.Id,
                         userRole => userRole.UserId,
                         (user, userRole) => new { User = user, UserRole = userRole })
                     .Join(_context.Roles,
                         userRole => userRole.UserRole.RoleId,
                         role => role.Id,
                         (userRole, role) => new { User = userRole.User, RoleName = role.Name })
                     .Join(_context.Faculty,
                         userRole => userRole.User.FacultyId,
                         faculty => faculty.Id,
                         (userRole, faculty) => new { User = userRole.User, RoleName = userRole.RoleName, FacultyName = faculty.Name })
                     .Where(u => u.RoleName != "Administrator")
                     .OrderByDescending(u => u.User.Email)
                     .Select(u => new
                     {
                         User = u.User,
                         RoleName = u.RoleName,
                         FacultyName = u.FacultyName
                     })
                     .ToList();

                return Json(listUserWithRoleAndFaculty);
            }
        }
        [Authorize(Roles = "Administrator, Manager,Coordinator")]
        public IActionResult IndexCoor()
        {
            return View();
        }

        [Authorize(Roles = "Administrator, Manager,Coordinator")]
        [HttpGet]
        public async Task<IActionResult> SearchAcountCoorAsync(string email)
        {

            ApplicationUser author = await _userManager.GetUserAsync(HttpContext.User) ?? new ApplicationUser();
            if (!string.IsNullOrEmpty(email))
            {
                var listUserWithRoleAndFaculty = _context.User
                    .Join(_context.UserRoles,
                        user => user.Id,
                        userRole => userRole.UserId,
                        (user, userRole) => new { User = user, UserRole = userRole })
                    .Join(_context.Roles,
                        userRole => userRole.UserRole.RoleId,
                        role => role.Id,
                        (userRole, role) => new { User = userRole.User, RoleName = role.Name })
                    .Join(_context.Faculty,
                        userRole => userRole.User.FacultyId,
                        faculty => faculty.Id,
                        (userRole, faculty) => new { User = userRole.User, RoleName = userRole.RoleName, FacultyName = faculty.Name, facultyNew = faculty })
.Where(u => u.RoleName != "Administrator" && u.RoleName != "Manager" && u.RoleName != "Coordinator" && u.RoleName != "Guest" && u.User.Email.Contains(email) && u.facultyNew.Id == author.FacultyId)
                    .OrderByDescending(u => u.User.Email)
                    .Select(u => new
                    {
                        User = u.User,
                        RoleName = u.RoleName,
                        FacultyName = u.FacultyName
                    })
                    .ToList();

                return Json(listUserWithRoleAndFaculty);
            }
            else
            {
                var listUserWithRoleAndFaculty = _context.User
                    .Join(_context.UserRoles,
                        user => user.Id,
                        userRole => userRole.UserId,
                        (user, userRole) => new { User = user, UserRole = userRole })
                    .Join(_context.Roles,
                        userRole => userRole.UserRole.RoleId,
                        role => role.Id,
                        (userRole, role) => new { User = userRole.User, RoleName = role.Name })
                    .Join(_context.Faculty,
                        userRole => userRole.User.FacultyId,
                        faculty => faculty.Id,
                        (userRole, faculty) => new { User = userRole.User, RoleName = userRole.RoleName, FacultyName = faculty.Name, facultyNew = faculty })
                    .Where(u => u.RoleName != "Administrator" && u.RoleName != "Manager" && u.RoleName != "Coordinator" && u.RoleName != "Guest" && u.facultyNew.Id == author.FacultyId)
                    .OrderByDescending(u => u.User.Email)
                    .Select(u => new
                    {
                        User = u.User,
                        RoleName = u.RoleName,
                        FacultyName = u.FacultyName
                    })
                    .ToList();

                return Json(listUserWithRoleAndFaculty);
            }

        }

        [Authorize(Roles = "Administrator, Manager")]
        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            if (id == null || _context.Files == null)
            {
                return NotFound();
            }
            var User = _context.User
                   .Join(_context.UserRoles,
                       user => user.Id,
                       userRole => userRole.UserId,
                       (user, userRole) => new { User = user, UserRole = userRole }).Where(u => u.User.Id == id)
                   .Join(_context.Roles,
                       userRole => userRole.UserRole.RoleId,
                       role => role.Id,
                       (userRole, role) => new { User = userRole.User, RoleName = role.Name }).Where(u => u.RoleName != "Administrator")
                   .Join(_context.Faculty, // Assuming UserFaculty is the entity representing the relationship between User and Faculty
                       userRole => userRole.User.FacultyId,
                       userFaculty => userFaculty.Id,
                       (userRole, userFaculty) => new { User = userRole.User, RoleName = userRole.RoleName, FacultyName = userFaculty.Name })
                   .Select(u => new
                   {
                       User = u.User,
                       RoleName = u.RoleName,
                       FacultyName = u.FacultyName
                   })
                   .ToList();
            ViewBag.ListFaculty = await _context.Faculty.ToListAsync();
            ViewBag.ListRole = await _context.Roles.ToListAsync();
            if (User == null)
            {
                return NotFound();
            }
            return View(User);
        }

        public async Task AddImage(ApplicationUser user, List<IFormFile> files)
        {
            string wwwPath = this._environment.WebRootPath;
            string contentPath = this._environment.ContentRootPath;
            // kiem tra size
            long size = files.Sum(f => f.Length);
            // duong dan den thu muc + file -> wwwPath se dan den file wwwroot
            var folderPath = Path.Combine(wwwPath, "imageUser");

            string defaultImagePath = "ImageDefaultUser.png";

            if (files != null && files.Any())
            {
                // Process uploaded files
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(formFile.FileName);
                        var fileExtension = Path.GetExtension(formFile.FileName);
                        var newFileName = $"{fileNameWithoutExtension}_{timestamp}{fileExtension}";
                        var filePath = Path.Combine(folderPath, newFileName);
                        DeleteImage(user.Image);
                        user.Image = newFileName;
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                    else
                    {
                        user.Image = defaultImagePath;
                    }
                }
            }
            
        }

        public void DeleteImage(string fileName)
        {

            string wwwPath = this._environment.WebRootPath;
            var folderPath = Path.Combine(wwwPath, "imageUser");
            var filePath = Path.Combine(folderPath, fileName);

            if (System.IO.File.Exists(filePath))
            {
                if (fileName != "ImageDefaultUser.png" && !fileName.IsNullOrEmpty())
                {
                    System.IO.File.Delete(filePath);
                }
            }
            else
            {

            }
        }

        /*[HttpPost]
        public async Task<IActionResult> Edit(string id, string firstName, string email, string lastName,string role, string faculty, List<IFormFile> files)
        {
            // Retrieve the user from the database
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
            var userRole = await _context.UserRoles.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null && userRole == null)
            {
                return NotFound();
            }

            try
            {
               
                await AddImage(user, files);
                user.FirstName = firstName;
                user.Email = email;
                user.LastName = lastName;
                user.FacultyId = faculty;
                userRole.RoleId = role;
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Admins");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating user: {ex.Message}");
            }
        }*/

        [Authorize(Roles = "Administrator, Manager")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Update(string id, string firstName, string email, string lastName, string role, string faculty, List<IFormFile> files)
        {
            ApplicationUser author = await _userManager.GetUserAsync(HttpContext.User) ?? new ApplicationUser();
            try
            {
                // Retrieve the user from the database
                var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound();  
                }

                // Update user properties
                user.FirstName = firstName;
                user.Email = email;
                user.NormalizedEmail = email.ToUpper();
                user.UserName = email;
                user.NormalizedUserName = email.ToUpper();
                user.LastName = lastName;
                user.FacultyId = faculty; // Assuming FacultyId is the correct property to assign
                user.LastModifiedDate = DateTime.Now;
                user.UpdateBy = author;

                // Remove existing user roles
                var existingUserRole = await _context.UserRoles.FirstOrDefaultAsync(u => u.UserId == id);
                if (existingUserRole != null)
                {
                    _context.UserRoles.Remove(existingUserRole);
                }

                // Add new user role
                var newUserRole = new IdentityUserRole<string> { UserId = id, RoleId = role };
                _context.UserRoles.Add(newUserRole);

                // Add or update user image
                await AddImage(user, files);


                // notification 
                var tempNoti = new SchoolProject1640.Models.Notification();
                tempNoti.Message = $"Admin {author.Email} updated your information";
                tempNoti.UserID = id;
                tempNoti.isRead = false;
                _context.Notification.Add(tempNoti);
                SendGmailAsync(author.Email, user.Email);
                // Save changes to the database
                await _context.SaveChangesAsync();

                // Show notification to FE
                _hubContext.Clients.Group($"user_{id}").SendAsync("ReceiveNotification", tempNoti, "info");

                // Redirect to the Admins index action
                return RedirectToAction("Index", "Admins");
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return BadRequest($"Error updating user: {ex.Message}");
            }
        }

        public async Task SendGmailAsync(string? user, string? userrec)
        {
            if (user == null)
            { return; }
            try
            {
                // Create a new MailMessage
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress("duongchilocmail@gmail.com");
                    message.To.Add(userrec);
                    message.Subject = "Admin 1640";
                    message.Body = $"Admin {user} updated your information";

                    // Create a new SMTP client
                    using (var smtpClient = new SmtpClient())
                    {
                        smtpClient.Host = "smtp.gmail.com";
                        smtpClient.Port = 587;
                        smtpClient.EnableSsl = true;
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = new NetworkCredential("duongchilocmail@gmail.com", "sallqsnernrfmxkw");

                        // Send the email
                        await smtpClient.SendMailAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        [Authorize(Roles = "Administrator, Manager")]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                }

                await _userManager.DeleteAsync(user);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return BadRequest($"Error deleting user: {ex.Message}");
            }
        }



    }
}
