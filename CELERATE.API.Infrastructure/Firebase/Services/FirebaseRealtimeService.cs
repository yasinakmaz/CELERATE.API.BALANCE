using CELERATE.API.CORE.Interfaces;
using CELERATE.API.CORE.Entities; // Domain Transaction için
using Google.Cloud.Firestore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CELERATE.API.Application.DTOs.ReactOptimized;
using CELERATE.API.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CELERATE.API.Infrastructure.Firebase.Services
{
    public class FirebaseRealtimeService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly FirestoreDb _firestoreDb;
        private readonly INotificationService _notificationService;
        private readonly ILogger<FirebaseRealtimeService> _logger;
        private List<FirestoreChangeListener> _listeners = new List<FirestoreChangeListener>();

        public FirebaseRealtimeService(
            IServiceProvider serviceProvider,
            FirestoreDb firestoreDb,
            INotificationService notificationService,
            ILogger<FirebaseRealtimeService> logger)
        {
            _serviceProvider = serviceProvider;
            _firestoreDb = firestoreDb;
            _notificationService = notificationService;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Transactions collection listener
            var transactionsListener = _firestoreDb.Collection("transactions")
                .OrderByDescending("CreatedAt") // Düzeltilmiş sıralama
                .Limit(1)
                .Listen(async snapshot =>
                {
                    if (snapshot.Count > 0)
                    {
                        try
                        {
                            // Domain Transaction'a dönüştürme
                            var docSnapshot = snapshot.Documents[0];
                            var transactionData = docSnapshot.ToDictionary();

                            // Bu verileri doğrudan almanız gerekiyor
                            string transactionId = docSnapshot.Id;
                            string userId = transactionData.ContainsKey("UserId") ? transactionData["UserId"].ToString() : null;
                            string operatorId = transactionData.ContainsKey("OperatorId") ? transactionData["OperatorId"].ToString() : null;
                            string branchId = transactionData.ContainsKey("BranchId") ? transactionData["BranchId"].ToString() : null;
                            string transactionType = transactionData.ContainsKey("Type") ? transactionData["Type"].ToString() : null;
                            decimal amount = transactionData.ContainsKey("Amount") ? Convert.ToDecimal(transactionData["Amount"]) : 0;
                            decimal balanceAfter = transactionData.ContainsKey("BalanceAfter") ? Convert.ToDecimal(transactionData["BalanceAfter"]) : 0;

                            // Şube grubuna bildirim gönder
                            await _notificationService.NotifyBranchTransactionWithDetails(
                                branchId,
                                transactionId,
                                userId,
                                operatorId,
                                transactionType,
                                amount
                            );

                            // Kullanıcıya bakiye değişikliği bildirimi gönder
                            await _notificationService.NotifyUserBalanceChangedWithDetails(
                                userId,
                                transactionType,
                                balanceAfter - amount, // Bakiye önceki değeri hesapla
                                balanceAfter
                            );
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Firestore transaction dinlerken hata oluştu");
                        }
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