using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace CELERATE.API.API.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public async Task JoinUserGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        public async Task JoinBranchGroup(string branchId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Branch_{branchId}");
        }

        // Bu metotlar, bir transaction veya işlem gerçekleştiğinde
        // ilgili kullanıcı veya şubeye bildirim göndermek için kullanılır
        public async Task NotifyUserBalanceChanged(string userId, decimal newBalance)
        {
            await Clients.Group($"User_{userId}").SendAsync("BalanceChanged", newBalance);
        }

        public async Task NotifyBranchTransaction(string branchId, TransactionDto transaction)
        {
            await Clients.Group($"Branch_{branchId}").SendAsync("NewTransaction", transaction);
        }
    }
}
