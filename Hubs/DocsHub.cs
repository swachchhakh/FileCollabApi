using Microsoft.AspNetCore.SignalR;

public class DocsHub : Hub {
    public async Task SendUpdate(string docId, string content, string userId) {
        // Broadcast to others editing same doc
        await Clients.OthersInGroup(docId).SendAsync("ReceiveUpdate", content, userId);
    }

    public override async Task OnConnectedAsync() {
        // optionally add to groups based on query string docId
        var docId = Context.GetHttpContext().Request.Query["docId"].ToString();
        if(!string.IsNullOrEmpty(docId)) await Groups.AddToGroupAsync(Context.ConnectionId, docId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception) {
        var docId = Context.GetHttpContext().Request.Query["docId"].ToString();
        if(!string.IsNullOrEmpty(docId)) await Groups.RemoveFromGroupAsync(Context.ConnectionId, docId);
        await base.OnDisconnectedAsync(exception);
    }
}
