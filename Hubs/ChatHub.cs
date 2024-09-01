using Aspose.Words.Bibliography;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SchoolProject1640.Data;
using SchoolProject1640.Models;

namespace SchoolProject1640.Hubs
{
    public class ChatHub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ChatHub(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _userManager = userManager;
            _context = context;
            _hubContext = hubContext;
        }

        public async Task JoinGroup(string articleId)
        {
           await Groups.AddToGroupAsync(Context.ConnectionId, articleId);
        }

        public async Task LeaveGroup(string articleId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, articleId);
        }

        public async Task SendMessage(string articleId, string message)
        {
            var currentUser = await _userManager.GetUserAsync(Context.User);
            var user = await _context.User
                .Join(_context.UserRoles,
                    user => user.Id,
                    userRole => userRole.UserId,
                    (user, userRole) => new { User = user, UserRole = userRole })
                .Join(_context.Roles,
                    userRole => userRole.UserRole.RoleId,
                    role => role.Id,
                    (userRole, role) => new { User = userRole.User, RoleName = role.Name })
                //.Join(_context.Faculty,
                //    userRole => userRole.User.FacultyId,
                //    faculty => faculty.Id,
                //    (userRole, faculty) => new { User = userRole.User, RoleName = userRole.RoleName, FacultyName = faculty.Name })
                .FirstOrDefaultAsync(user => user.User.Id == currentUser.Id);
            // Add message
            var newMessage = new Message
            {
                AccountID = user.User.Id,
                ArtID = int.Parse(articleId),
                Mess = message
            };
            _context.Add(newMessage);

            if (user.RoleName == "Coordinator")
            {
                // Get student in the article
                var notiReceiver = await _context.Article
                    .Join(_context.User,
                        article => article.AccountId,
                        user => user.Id,
                        (article, user) => new { Article = article, User = user })
                    .FirstOrDefaultAsync(article => article.Article.Id == int.Parse(articleId));
                // Add notification
                var notification = new Notification();
                notification.SendBy = user.User.Email;
                notification.FacultyId = user.User.FacultyId;
                notification.isRead = false;
                notification.UserID = notiReceiver.User.Id;
                notification.Message = $"{user.User.FirstName} {user.User.LastName} sent you a message!";
                _context.Add(notification);
                // Show notification to FE
                await _hubContext.Clients.Group($"user_{notiReceiver.User.Id}").SendAsync("ReceiveNotification", notification, "info");
            } else
            {
                // Get he coordinator of the student's faculty
                var notiReceiver = await _context.User
                        .Join(_context.UserRoles,
                            user => user.Id,
                            userRole => userRole.UserId,
                            (user, userRole) => new { User = user, UserRole = userRole })
                        .Join(_context.Roles,
                            userRole => userRole.UserRole.RoleId,
                            role => role.Id,
                            (userRole, role) => new { User = userRole.User, RoleName = role.Name })
                        .FirstOrDefaultAsync(coordinator => coordinator.User.FacultyId == user.User.FacultyId && coordinator.RoleName == "Coordinator");
                // Add notification
                var notification = new Notification();
                notification.SendBy = user.User.Email;
                notification.FacultyId = user.User.FacultyId;
                notification.isRead = false;
                notification.UserID = notiReceiver.User.Id;
                notification.Message = $"{user.User.FirstName} {user.User.LastName} sent you a message!";
                _context.Add(notification);
                // Show notification to FE
                await _hubContext.Clients.Group($"user_{notiReceiver.User.Id}").SendAsync("ReceiveNotification", notification, "info");
            }

            await _context.SaveChangesAsync();

            await Clients.Group(articleId).SendAsync("ReceiveMessage", user.User, message);
        }
    }
}
