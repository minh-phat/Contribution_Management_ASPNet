// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using SchoolProject1640.Data;
using SchoolProject1640.Models;

namespace SchoolProject1640.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment Environment;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
            Environment = environment;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public List<ApplicationRole> Roles { get; set; }
        public List<Faculty> Faculties { get; set; }
        public class InputModel
        {
            [Required(ErrorMessage = "First name is required")]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Last name is required")]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Display(Name = "Image")]
            public string Image1 { get; set; }

            [Required(ErrorMessage = "Role is required")]
            public string Role { get; set; }

            [Required(ErrorMessage = "Faculty is required")]
            public string Faculty { get; set; }

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email format")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2}.", MinimumLength = 6)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare("Password", ErrorMessage = "The pw and cpw do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ApplicationUser author = await _userManager.GetUserAsync(HttpContext.User) ?? new ApplicationUser();
            if (author == null)
            {
                return;
            }
            Roles = await _context.Roles.ToListAsync();
            Faculties = await _context.Faculty.ToListAsync();
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }
        public async Task SendEmail2Async(string? user, string? userrec, string generatedPassword)
        {
            if (user == null)
            { return; }
            try
            {
                // Create a new MailMessage
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress("schoolproject1640@gmail.com");
                    message.To.Add(user);
                    message.Subject = "Your Temporary Password";
                    message.Body = $"Dear {user},\n\n" +
                                   $"Your temporary password is: {generatedPassword}.\n\n" +
                                   $"Please log in and change your password immediately.\n\n" +
                                   $"Thank you.";

                    // Create a new SMTP client
                    using (var smtpClient = new SmtpClient())
                    {
                        smtpClient.Host = "smtp.gmail.com";
                        smtpClient.Port = 587;
                        smtpClient.EnableSsl = true;
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = new NetworkCredential("schoolproject1640@gmail.com", "ctiebrgkegpqnqev");

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

        public async Task<IActionResult> OnPostAsync(string returnUrl = null, List<IFormFile> files = null)
        {
            ApplicationUser author = await _userManager.GetUserAsync(HttpContext.User) ?? new ApplicationUser();
            if (author == null)
            {
                return NotFound();
            }
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Check if the coordinator for a faculty already exists
            var role = await _context.Roles.FirstOrDefaultAsync(role => role.Id == Input.Role);
            if (role.Name == "Coordinator")
            {
                var tempUser = await _context.User
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
                        .FirstOrDefaultAsync(user => user.User.FacultyId == Input.Faculty && user.RoleName == "Coordinator");
                
                if (tempUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Coordinator for this faculty already exists.");
                }
            }

            if (!ModelState.IsValid)
            {
                var user = CreateUser();
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.EmailConfirmed = true;
                user.FacultyId = Input.Faculty;


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
                            user.Image = newFileName;

                            using (var stream = System.IO.File.Create(filePath))
                            {
                                await formFile.CopyToAsync(stream);
                            }
                        }
                    }
                }
                else
                {
                    user.Image = defaultImagePath;
                }

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                string generatedPassword = GenerateRandomPassword();


                var result = await _userManager.CreateAsync(user, generatedPassword);

                await SendEmail2Async(Input.Email, "Your Temporary Password",
$"Dear {user.FirstName},<br><br>" +
$"Your temporary password is: <strong>{generatedPassword}</strong>.<br><br>" +
$"Please log in and change your password immediately.<br><br>" +
$"Thank you.<br>");
                if (result.Succeeded)
                {
                    var tempUserRole = new IdentityUserRole<string>();
                    tempUserRole.UserId = user.Id;
                    tempUserRole.RoleId = Input.Role;

                    await _context.UserRoles.AddAsync(tempUserRole);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    //{
                    //    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    //}
                    //else
                    //{
                    //    await _signInManager.SignInAsync(user, isPersistent: false);
                    //    return LocalRedirect(returnUrl);
                    //}
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            // Populate required data again for re-displaying the form
            Roles = await _context.Roles.ToListAsync();
            Faculties = await _context.Faculty.ToListAsync();
            return RedirectToAction("Index", "Admins");
        }
        private string GenerateRandomPassword()
        {
            // Generate a random password
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var randomPassword = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            // Append "@123" to the random password
            return randomPassword + "@@123";
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
