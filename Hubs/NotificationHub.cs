using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SchoolProject1640.Data;
using SchoolProject1640.Models;
using System.Linq;

namespace SchoolProject1640.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public NotificationHub(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var user = await _userManager.GetUserAsync(Context.User);
            if (user != null && user.Id != null)
            {
                // Add the connection to a group based on the user's ID
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{user.Id}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = await _userManager.GetUserAsync(Context.User);
            if (user != null && user.Id != null)
            {
                // Remove the connection from the group based on the user's ID
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{user.Id}");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task DismissNotification(int messageId)
        {
            var notification = await _context.Notification.FirstOrDefaultAsync(notification => notification.Id == messageId);
            if (notification != null)
            {
                notification.isRead = true;
                _context.Notification.Update(notification);
                await _context.SaveChangesAsync();
            }
        }
    }
}
