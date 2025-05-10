using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using CELERATE.API.Application.DTOs;
using CELERATE.API.CORE.Interfaces;

namespace CELERATE.API.API.Hubs
{
    [Authorize]
    public class NotificationHub : Hub, INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationHub(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task JoinUserGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        public async Task JoinBranchGroup(string branchId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Branch_{branchId}");
        }

        // Original method - renamed to avoid conflict with interface method
        public async Task SendTransactionNotification(string branchId, TransactionDto transaction)
        {
            await _hubContext.Clients.Group($"Branch_{branchId}").SendAsync("TransactionDetails", transaction);
        }

        // Interface implementation methods
        public async Task NotifyUserBalanceChanged(string userId, decimal newBalance)
        {
            await _hubContext.Clients.Group($"User_{userId}").SendAsync("BalanceChanged", newBalance);
        }

        public async Task NotifyUserBalanceChangedWithDetails(string userId, string transactionType, decimal oldBalance, decimal newBalance)
        {
            await _hubContext.Clients.Group($"User_{userId}").SendAsync("BalanceChangedWithDetails", new
            {
                TransactionType = transactionType,
                OldBalance = oldBalance,
                NewBalance = newBalance
            });
        }

        public async Task NotifyBranchTransaction(string branchId, string transactionId, decimal amount)
        {
            await _hubContext.Clients.Group($"Branch_{branchId}").SendAsync("NewTransaction", new
            {
                TransactionId = transactionId,
                Amount = amount
            });
        }

        public async Task NotifyBranchTransactionWithDetails(string branchId, string transactionId, string userId, string operatorId, string type, decimal amount)
        {
            await _hubContext.Clients.Group($"Branch_{branchId}").SendAsync("NewTransactionWithDetails", new
            {
                TransactionId = transactionId,
                UserId = userId,
                OperatorId = operatorId,
                Type = type,
                Amount = amount
            });
        }

        public async Task NotifyCardOperation(string userId, string operationType, string details)
        {
            await _hubContext.Clients.Group($"User_{userId}").SendAsync("CardOperation", new
            {
                OperationType = operationType,
                Details = details
            });
        }

        public async Task UpdateActiveStaffCount(string branchId, int count)
        {
            await _hubContext.Clients.Group($"Branch_{branchId}").SendAsync("ActiveStaffCount", count);
        }

        public async Task NotifyError(string userId, string errorCode, string errorMessage)
        {
            await _hubContext.Clients.Group($"User_{userId}").SendAsync("Error", new
            {
                ErrorCode = errorCode,
                Message = errorMessage
            });
        }
    }
}