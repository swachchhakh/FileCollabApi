using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
 public class CollaborationHub : Hub
    {
        // Example: broadcast message to all connected clients
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        // You can add more methods for live collaboration events
        public async Task JoinFolder(string folderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, folderId);
        }

        public async Task LeaveFolder(string folderId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, folderId);
        }
    }

