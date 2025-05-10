using Google.Cloud.Firestore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CELERATE.API.Infrastructure.Firebase.Services
{
    public class FirebaseRealtimeService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly FirestoreDb _firestoreDb;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<FirebaseRealtimeService> _logger;
        private List<FirestoreChangeListener> _listeners = new List<FirestoreChangeListener>();

        public FirebaseRealtimeService(
            IServiceProvider serviceProvider,
            FirestoreDb firestoreDb,
            IHubContext<NotificationHub> hubContext,
            ILogger<FirebaseRealtimeService> logger)
        {
            _serviceProvider = serviceProvider;
            _firestoreDb = firestoreDb;
            _hubContext = hubContext;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Transactions collection listener
            var transactionsListener = _firestoreDb.Collection("transactions")
                .OrderBy("CreatedAt", Query.Direction.Descending)
                .Limit(1)
                .Listen(async snapshot =>
                {
                    if (snapshot.Count > 0)
                    {
                        var transaction = snapshot.Documents[0].ConvertTo<Transaction>();

                        // Notify all clients in the branch group
                        await _hubContext.Clients
                            .Group($"Branch_{transaction.BranchId}")
                            .SendAsync("NewTransaction", new NewTransactionNotification
                            {
                                TransactionId = transaction.Id,
                                UserId = transaction.UserId,
                                OperatorId = transaction.OperatorId,
                                BranchId = transaction.BranchId,
                                Type = transaction.Type.ToString(),
                                Amount = transaction.Amount
                            });

                        // Notify user about balance change
                        await _hubContext.Clients
                            .Group($"User_{transaction.UserId}")
                            .SendAsync("BalanceChanged", new BalanceChangedNotification
                            {
                                UserId = transaction.UserId,
                                NewBalance = transaction.BalanceAfter,
                                TransactionType = transaction.Type.ToString()
                            });
                    }
                });

            _listeners.Add(transactionsListener);

            _logger.LogInformation("Firebase Realtime Service started");
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var listener in _listeners)
            {
                await listener.StopAsync();
            }

            _logger.LogInformation("Firebase Realtime Service stopped");
        }
    }
}
