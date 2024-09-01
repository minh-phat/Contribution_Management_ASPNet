using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolProject1640.Data;
using SchoolProject1640.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.SignalR;
using SchoolProject1640.Hubs;

namespace SchoolProject1640.Controllers
{
    public class ContributionsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ContributionsController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _hubContext = hubContext;
        }
        // GET: Contributions
        [Authorize(Roles = "Administrator,Student,Coordinator,Manager")]
        public async Task<IActionResult> Index()
        {
              return _context.Contribution != null ? 
                          View(await _context.Contribution.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Contribution'  is null.");
        }
        [Authorize(Roles = "Administrator,Student,Coordinator,Manager")]
        public async Task<IActionResult> IndexUser()
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
            foreach (var role in roles)
            {
                switch (role)
                {
                    case "Student":
                        // Handle the role Student
                        ViewBag.listArt = _context.Article.Where(m => m.AccountId == author.Id).ToList();
                        break;
                    case "Guest":
                        // Handle the role Guest
                        break;
                    case "Manager":
                        // Handle the role Manager
                        ViewBag.listArt = _context.Article.ToList();
                        break;
                    case "Administrator":
                        // Handle the role Administrator
                        ViewBag.listArt = _context.Article.ToList();
                        break;
                    case "Coordinator":
                        // Handle the role Coordinator
                        ViewBag.listArt = _context.Article.ToList();
                        break;
                    default:
                        // Handle any other roles if needed
                        break;
                }
            }

            //ViewBag.listArt = _context.Article.ToList();

            ViewBag.listNameUSer = _context.User.ToList();
            ViewBag.idUser = author.Id;
            ViewBag.listNameFaculty = _context.Faculty.ToList();
            return _context.Contribution != null ?
                        View(await _context.Contribution.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Contribution'  is null.");
        }
        // GET: Contributions/Details/5
        [Authorize(Roles = "Administrator,Student,Coordinator,Manager")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Contribution == null)
            {
                return NotFound();
            }

            var contribution = await _context.Contribution
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contribution == null)
            {
                return NotFound();
            }

            return View(contribution);
        }

        // GET: Contributions/Create
        [Authorize(Roles = "Administrator,Student,Coordinator,Manager")]
        public IActionResult Create()
        {
            var listFaculty = _context.Faculty.ToList();
            ViewBag.ListFaculty = listFaculty;
            return View();
        }

        // POST: Contributions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Student,Coordinator,Manager")]
        public async Task<IActionResult> Create([Bind("Id,Title,AcademicYear,StartDate,ClosureDate,FinalClosureDate,CreatedAt,UpdatedAt")] Contribution contribution)
        {
            if (ModelState.IsValid)
            {
                var author = await _userManager.GetUserAsync(HttpContext.User) ?? new ApplicationUser();
                var listfaculty = _context.Faculty.ToList();

                foreach (var tempfaculty in listfaculty)
                {
                    // Check if the faculty is neither Admin nor Manager
                    if (tempfaculty.Name != "Admin" && tempfaculty.Name != "Manager")
                    {
                        var listUserNotification = _context.User.Where(m => m.FacultyId == tempfaculty.Id).ToList();

                        // Create a new contribution for this faculty
                        var newContribution = new Contribution
                        {
                            Title = contribution.Title,
                            AcademicYear = contribution.AcademicYear,
                            StartDate = contribution.StartDate,
                            ClosureDate = contribution.ClosureDate,
                            FinalClosureDate = contribution.FinalClosureDate,
                            CreatedAt = contribution.CreatedAt,
                            UpdatedAt = contribution.UpdatedAt,
                            Faculty = tempfaculty.Id // Assign the faculty to the contribution
                        };

                        _context.Add(newContribution);

                        foreach (var user in listUserNotification)
                        {
                            var notification = new SchoolProject1640.Models.Notification
                            {
                                SendBy = author.Email,
                                FacultyId = tempfaculty.Id,
                                isRead = false,
                                UserID = user.Id,
                                Message = $"{newContribution.Title} has been created please join this."
                            };

                            _context.Add(notification);
                            // Show notification to FE
                            _hubContext.Clients.Group($"user_{user.Id}").SendAsync("ReceiveNotification", notification, "info");
                            SendGmailAsync(author.Email, user.Email);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contribution);
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
                    message.Subject = "New Contribution";
                    message.Body = $"{user}: has been created please join this";

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

        // GET: Contributions/Update/5
        [Authorize(Roles = "Administrator,Student,Coordinator,Manager")]
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || _context.Contribution == null)
            {
                return NotFound();
            }

            var contribution = await _context.Contribution.FindAsync(id);
            if (contribution == null)
            {
                return NotFound();
            }
            return View(contribution);
        }

        // POST: Contributions/Update/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator,Student,Coordinator,Manager")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Update(int id, [Bind("Id,Title,Faculty,AcademicYear,StartDate,ClosureDate,FinalClosureDate")] Contribution contribution)
        {
            if (id != contribution.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    contribution.UpdatedAt = DateTime.Now;
                    _context.Update(contribution);

                    ApplicationUser author = await _userManager.GetUserAsync(HttpContext.User) ?? new ApplicationUser();
                    var listUserNotification = _context.User.Where(m => m.FacultyId == contribution.Faculty).ToList();
                    foreach (var user in listUserNotification)
                    {
                        // notification 
                        var tempNoti = new SchoolProject1640.Models.Notification();
                        tempNoti.SendBy = author.Email;
                        tempNoti.FacultyId = contribution.Faculty;
                        tempNoti.Message = $"{contribution.Title} has been updated.";
                        tempNoti.UserID = user.Id;
                        tempNoti.isRead = false;
                        _context.Notification.Add(tempNoti);
                        // Show notification to FE
                        _hubContext.Clients.Group($"user_{user.Id}").SendAsync("ReceiveNotification", tempNoti, "info");
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContributionExists(contribution.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(contribution);
        }


        // GET: Contributions/Delete/5
        [Authorize(Roles = "Administrator,Student,Coordinator,Manager")]
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Contribution == null)
            {
                return NotFound();
            }

            var contribution = await _context.Contribution
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contribution == null)
            {
                return NotFound();
            }

            return View(contribution);
        }

        // POST: Contributions/Delete/5
        [HttpPost]
        [Authorize(Roles = "Administrator,Student,Coordinator,Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Contribution == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Contribution'  is null.");
            }
            var contribution = await _context.Contribution.FindAsync(id);
            if (contribution != null)
            {
                _context.Contribution.Remove(contribution);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult SearchContribution(string title)
        {
            if (!string.IsNullOrEmpty(title))
            {
                var contributions = _context.Contribution
                    .Join(_context.Faculty,
                        contribution => contribution.Faculty,
                        faculty => faculty.Id,
                        (contribution, faculty) => new { Contribution = contribution, FacultyName = faculty.Name })
                    .Where(contribution => contribution.Contribution.Title.Contains(title))
                    .OrderByDescending(contribution => contribution.Contribution.Title)
                    .Select(contribution => new
                    {
                        Contribution = contribution.Contribution,
                        FacultyName = contribution.FacultyName,
                    })
                    .ToList();

                return Json(contributions);
            }
            else
            {
                var contributions = _context.Contribution
                    .Join(_context.Faculty,
                        contribution => contribution.Faculty,
                        faculty => faculty.Id,
                        (contribution, faculty) => new { Contribution = contribution, FacultyName = faculty.Name })
                    .OrderByDescending(contribution => contribution.Contribution.Title)
                    .Select(contribution => new
                    {
                        Contribution = contribution.Contribution,
                        FacultyName = contribution.FacultyName,
                    })
                    .ToList();

                return Json(contributions);
            }
        }

        private bool ContributionExists(int id)
        {
          return (_context.Contribution?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
