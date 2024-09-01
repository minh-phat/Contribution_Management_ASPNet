using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Aspose.Words.Bibliography;
using Aspose.Words.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SchoolProject1640.Data;
using SchoolProject1640.Hubs;
using SchoolProject1640.Models;
using Xceed.Words.NET;
//Install-Package Aspose.Words

namespace SchoolProject1640.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        IWebHostEnvironment _environment;
        private readonly IHubContext<NotificationHub> _hubContext;
        public ArticlesController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context, IWebHostEnvironment environment, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _environment = environment;
            _hubContext = hubContext;
        }
        // GET: Articles
        [Authorize(Roles = "Administrator,Student,Coordinator,Manager")]
        public async Task<IActionResult> Index()
        {
            return _context.Article != null ?
                        View(await _context.Article.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Article'  is null.");
        }

        // GET: Articles/Details/5
        [Authorize(Roles = "Administrator,Student,Coordinator,Manager")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Article == null)
            {
                return NotFound();
            }

            var article = _context.Article
                .Where(article => article.Id == id)
                .Join(_context.User,
                        article => article.AccountId,
                        account => account.Id,
                        (article, account) => new { Article = article, Account = account })
                .Join(_context.Contribution,
                        article => article.Article.ContributionId,
                        contribution => contribution.Id,
                        (article, contribution) => new { Article = article, Contribution = contribution })
                .Join(_context.Faculty,
                        article => article.Contribution.Faculty,
                        faculty => faculty.Id,
                        (article, faculty) => new { article.Article, FacultyName = faculty.Name })
                .Select(article => new
                {
                    Article = article.Article.Article,
                    Account = article.Article.Account,
                    FacultyName = article.FacultyName
                })
                .FirstOrDefault();

            if (article == null)
            {
                return NotFound();
            }

            var messages = _context.Message
                .Where(message => message.ArtID == id)
                .Join(_context.User,
                        message => message.AccountID,
                        account => account.Id,
                        (message, account) => new { Message = message, Account = account })
                .Select(message => new
                {
                    Message = message.Message,
                    Account = message.Account
                })
                .ToList();

            var currentUser = await _userManager.GetUserAsync(User);

            //TODO: Check validation for coordinator and manager as well
            //if (currentUser == null || currentUser.Id != article.Account.Id)
            //{
            //    return Unauthorized();
            //}

            ViewBag.Article = article;
            ViewBag.User = currentUser.Id;
            ViewBag.Messages = messages;
            var filename = await _context.Article.FirstOrDefaultAsync(m => m.Id == id);
            ViewBag.FileName = filename.FileName;

            return View();
        }

        // GET: Articles/Create
        [Authorize(Roles = "Administrator,Student,Coordinator,Manager")]
        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            ViewBag.ContributionId = id;
            var checkCloseContri = await _context.Contribution.FirstOrDefaultAsync(m => m.Id == id);

            if (checkCloseContri != null && checkCloseContri.ClosureDate < DateTime.Now)
            {
                ViewBag.MessErroClose = checkCloseContri.ClosureDate;
                ViewBag.DatetimeNow = DateTime.Now;
            }
            else
            {
                ViewBag.MessErroClose = DateTime.Now;
                ViewBag.DatetimeNow = DateTime.Now;
            }
            var tempAndCon = _context.TermAndCon.FirstOrDefault();
            if (tempAndCon != null)
            {
                ViewBag.tempAndCon = tempAndCon.TermsAndCondition;
            }
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*  [Authorize(Roles = "Administrator,Student,Coordinator,Manager")]
          [HttpPost]
          public async Task<IActionResult> Create(Article article, int idContri, List<IFormFile> files)
          {
              ApplicationUser author = await _userManager.GetUserAsync(HttpContext.User) ?? new ApplicationUser();

              if (files == null || files.Count == 0)
              {
                  // Handle this error
                  ViewBag.MessErro = "You need to choose at least 1 file to upload.";

              }

              foreach (var file in files)
              {
                  if (file.Length > 0)
                  {
                      // Generate a unique identifier
                      var uniqueIdentifier = Guid.NewGuid().ToString();

                      // Construct the new file name with the unique identifier appended
                      var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{uniqueIdentifier}{Path.GetExtension(file.FileName)}";

                      var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/SubmitDocx", fileName);

                      using (var stream = new FileStream(path, FileMode.Create))
                      {
                          await file.CopyToAsync(stream);
                      }
                      // Assign the file name and path to the article properties
                      var newArticle = new Article
                      {
                          Title = article.Title,
                          State = article.State,
                          Description = article.Description,
                          ContributionId = idContri,
                          AccountId = author.Id,
                          FileName = fileName,
                          FilePath = path
                      };
                      newArticle.ContributionId = idContri;
                      newArticle.AccountId = author.Id;
                      newArticle.FileName = fileName;
                      newArticle.FilePath = path;
                      _context.Add(newArticle);
                  }
              }
              ViewBag.Success = "Upload Sucessfully!.";
              await _context.SaveChangesAsync();
              return RedirectToAction("IndexUser", "Contributions");
              //return View(article);
          }*/
        [Authorize(Roles = "Administrator,Student,Coordinator,Manager")]
        [HttpPost]
        public async Task<IActionResult> Create(Article article, int idContri, List<IFormFile> files, IFormFile imageFile)
        {
            ApplicationUser author = await _userManager.GetUserAsync(HttpContext.User) ?? new ApplicationUser();

            if ((files == null || files.Count == 0) && (imageFile == null))
            {
                // Handle this error
                ViewBag.MessErro = "You need to choose at least 1 file or image to upload.";
                return RedirectToAction("IndexUser", "Contributions");
            }

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var uniqueIdentifier = Guid.NewGuid().ToString();
                    var fileName = $"{uniqueIdentifier}_{Path.GetFileName(file.FileName)}"; // Simplified file name generation
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "SubmitDocx", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    var newArticle = new Article
                    {
                        Title = article.Title,
                        State = article.State,
                        Description = article.Description,
                        ContributionId = idContri,
                        AccountId = author.Id,
                        FileName = fileName,
                        FilePath = path
                    };
                    if (imageFile != null)
                    {
                        var uniqueIdentifierImage = Guid.NewGuid().ToString();
                        var fileNameImage = $"{uniqueIdentifierImage}_{Path.GetFileName(imageFile.FileName)}"; // Simplified image file name generation
                        var pathImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imageArticle", fileNameImage);

                        using (var stream = new FileStream(pathImage, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        newArticle.Image = fileNameImage;
                    }
                    _context.Add(newArticle);

                    // Add notification and show to coordinator
                    var coordinator = await _context.User
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
                        .FirstOrDefaultAsync(user => user.User.FacultyId == author.FacultyId && user.RoleName == "Coordinator");
                    var contribution = await _context.Contribution.FirstOrDefaultAsync(m => m.Id == idContri);
                    if (coordinator != null && contribution != null)
                    {
                        var notification = new Notification();
                        notification.SendBy = author.Email;
                        notification.FacultyId = author.FacultyId;
                        notification.isRead = false;
                        notification.UserID = coordinator.User.Id;
                        notification.Message = $"{author.FirstName} {author.LastName} has submitted an article for {contribution.Title}. Please review within 14 days!";
                        _context.Add(notification);
                        // Show notification to FE
                        await _hubContext.Clients.Group($"user_{coordinator.User.Id}").SendAsync("ReceiveNotification", notification, "info");
                        await SendGmailAsync(author.FirstName + " " + author.LastName, coordinator.User.Email);
                    }
                }
            }

            ViewBag.Success = "Upload Successfully!";
            await _context.SaveChangesAsync();
            return RedirectToAction("IndexUser", "Contributions");
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
                    message.Subject = "New Article Submitted";
                    message.Body = $"{user} has submitted an article. Please review within 14 days!";

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

        // GET: Articles/Edit/5
        [Authorize(Roles = "Administrator,Student,Coordinator,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {


            if (id == null || _context.Article == null)
            {
                return NotFound();
            }

            var article = await _context.Article.FindAsync(id);
            if (article != null)
            {
                var checkCloseContri = await _context.Contribution.FirstOrDefaultAsync(m => m.Id == article.ContributionId);


                if (checkCloseContri != null && checkCloseContri.ClosureDate < DateTime.Now)
                {
                    ViewBag.MessErroClose = checkCloseContri.FinalClosureDate;
                    ViewBag.DatetimeNow = DateTime.Now;
                }
                else
                {
                    ViewBag.MessErroClose = DateTime.Now;
                    ViewBag.DatetimeNow = DateTime.Now;
                }
            }
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePublicForGuest(int? id, bool isPublicForGuest)
        {
            if (id == null)
            {
                return BadRequest("Invalid article ID");
            }

            var tempArti = await _context.Article.FirstOrDefaultAsync(m => m.Id == id);

            if (tempArti == null)
            {
                return NotFound();
            }

            tempArti.isPublicForGuest = isPublicForGuest;

            try
            {
                _context.Update(tempArti);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }


        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Student,Coordinator,Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description")] Article article, List<IFormFile> files, IFormFile imageFile)
        {
            var tempArti = _context.Article.Where(m => m.Id == article.Id).FirstOrDefault();

            if (id != tempArti.Id)
            {
                return NotFound();
            }


            try
            {
                ApplicationUser author = await _userManager.GetUserAsync(HttpContext.User) ?? new ApplicationUser();
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        // Generate a unique identifier
                        var uniqueIdentifier = Guid.NewGuid().ToString();

                        // Construct the new file name with the unique identifier appended
                        var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{uniqueIdentifier}{Path.GetExtension(file.FileName)}";

                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/SubmitDocx", fileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        if (System.IO.File.Exists(tempArti.FilePath))
                        {
                            System.IO.File.Delete(tempArti.FilePath);
                        }

                        tempArti.FileName = fileName;
                        tempArti.FilePath = path;
                        _context.Update(tempArti);
                    }
                }
                tempArti.Title = article.Title;
                tempArti.Description = article.Description;
                tempArti.UpdatedAt = DateTime.Now;
                if (imageFile != null)
                {
                    var uniqueIdentifierImage = Guid.NewGuid().ToString();
                    var fileNameImage = $"{uniqueIdentifierImage}_{Path.GetFileName(imageFile.FileName)}"; // Simplified image file name generation
                    var pathImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imageArticle", fileNameImage);

                    using (var stream = new FileStream(pathImage, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    tempArti.Image = fileNameImage;
                }
                await _context.SaveChangesAsync();
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(article.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("IndexUser", "Contributions");

        }

        // GET: Articles/Delete/5
        [Authorize(Roles = "Administrator,Student,Coordinator,Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Article == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Article == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Article' is null.");
            }

            var article = await _context.Article.FindAsync(id);

            if (article == null)
            {
                return NotFound();
            }

            // Get the path of the file associated with the article
            var filePath = article.FilePath;

            // Remove the article from the context
            _context.Article.Remove(article);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Delete the associated file from the wwwroot directory
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return RedirectToAction("IndexUser", "Contributions");
        }


        private bool ArticleExists(int id)
        {
            return (_context.Article?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        [HttpPost]
        public IActionResult DownloadMultipleFiles(List<string> fileNames)
        {
            var memoryStream = new MemoryStream();
            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var fileName in fileNames)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/SubmitDocx", fileName);
                    if (!System.IO.File.Exists(filePath))
                    {
                        return NotFound();
                    }

                    var entry = zipArchive.CreateEntry(fileName);
                    using (var entryStream = entry.Open())
                    using (var fileStream = new FileStream(filePath, FileMode.Open))
                    {
                        fileStream.CopyTo(entryStream);
                    }
                }
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            return File(memoryStream, "application/zip", "multiple_files.zip");
        }


        public IActionResult DownloadFile(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/SubmitDocx", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                stream.CopyTo(memory);
            }
            memory.Position = 0;

            return File(memory, "application/octet-stream", Path.GetFileName(filePath));
        }
        //1
        private void ConvertToPdf(string docxFilePath, string pdfFilePath)
        {
            // Load the document
            Aspose.Words.Document doc = new Aspose.Words.Document(docxFilePath);

            // Save the document in PDF format
            doc.Save(pdfFilePath, Aspose.Words.SaveFormat.Pdf);
        }
        public IActionResult ViewFile(string fileName)
        {
            var docxFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/SubmitDocx", fileName);
            var pdfFilePath = Path.ChangeExtension(docxFilePath, "pdf"); // PDF file path

            if (!System.IO.File.Exists(pdfFilePath))
            {
                // Convert .docx to PDF if the PDF doesn't exist
                ConvertToPdf(docxFilePath, pdfFilePath);
            }

            if (!System.IO.File.Exists(pdfFilePath))
            {
                // If conversion fails, return not found
                return NotFound();
            }

            // Return the PDF file
            return File(System.IO.File.OpenRead(pdfFilePath), "application/pdf", null);
        }
        //2 readpdf file
        /*public IActionResult ViewFile(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/SubmitDocx", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            return File(System.IO.File.OpenRead(filePath), "application/pdf", null);
        }*/
        // 3 user view docx
        /*public string ReadDocxFile(string filePath)
        {
            string content = string.Empty;

            // Load the document
            using (DocX doc = DocX.Load(filePath))
            {
                // Extract text from the document
                content = doc.Text;
            }

            return content;
        }
        public IActionResult ViewFile(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/SubmitDocx", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            string fileContent = ReadDocxFile(filePath);

            // Pass the content to the view
            ViewBag.FileContent = fileContent;

            return View("ViewDocx");
        }*/
        [HttpPost]
        public async Task<IActionResult> AcceptArt(int? id)
        {
            ApplicationUser coordinator = await _userManager.GetUserAsync(HttpContext.User) ?? new ApplicationUser();

            if (id == null)
            {
                return NotFound();
            }

            var tempArt = await _context.Article
                .Join(_context.User,
                    article => article.AccountId,
                    user => user.Id,
                    (article, user) => new { Article = article, User = user })
                .FirstOrDefaultAsync(m => m.Article.Id == id);

            if (tempArt == null)
            {
                return NotFound();
            }

            tempArt.Article.State = 1;

            // Send notification to the student
            var notification = new Notification();
            notification.SendBy = coordinator.Email;
            notification.FacultyId = coordinator.FacultyId;
            notification.isRead = false;
            notification.UserID = tempArt.User.Id;
            notification.Message = $"Your article {tempArt.Article.Title} has been accepted!";
            _context.Add(notification);
            // Show notification to FE
            await _hubContext.Clients.Group($"user_{tempArt.User.Id}").SendAsync("ReceiveNotification", notification, "info");
            // Send email to the student
            await SendGmailWithMessageAsync(tempArt.User.Email, notification.Message, "Article Accepted");

            await _context.SaveChangesAsync();

            return RedirectToAction("IndexUser", "Contributions");
        }
        [HttpPost]
        public async Task<IActionResult> RejectArt(int? id)
        {
            ApplicationUser coordinator = await _userManager.GetUserAsync(HttpContext.User) ?? new ApplicationUser();

            if (id == null)
            {
                return NotFound();
            }

            var tempArt = await _context.Article
                .Join(_context.User,
                    article => article.AccountId,
                    user => user.Id,
                    (article, user) => new { Article = article, User = user })
                .FirstOrDefaultAsync(m => m.Article.Id == id);

            if (tempArt == null)
            {
                return NotFound();
            }

            tempArt.Article.State = 2;

            // Send notification to the student
            var notification = new Notification();
            notification.SendBy = coordinator.Email;
            notification.FacultyId = coordinator.FacultyId;
            notification.isRead = false;
            notification.UserID = tempArt.User.Id;
            notification.Message = $"Your article {tempArt.Article.Title} has been rejected!";
            _context.Add(notification);
            // Show notification to FE
            await _hubContext.Clients.Group($"user_{tempArt.User.Id}").SendAsync("ReceiveNotification", notification, "info");
            // Send email to the student
            await SendGmailWithMessageAsync(tempArt.User.Email, notification.Message, "Article Rejected");

            await _context.SaveChangesAsync();

            return RedirectToAction("IndexUser", "Contributions");
        }

        public async Task SendGmailWithMessageAsync(string? userrec, string? content, string? subject)
        {
            if (userrec == null)
            { return; }
            try
            {
                // Create a new MailMessage
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress("duongchilocmail@gmail.com");
                    message.To.Add(userrec);
                    message.Subject = subject;
                    message.Body = content;

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
    }
}
