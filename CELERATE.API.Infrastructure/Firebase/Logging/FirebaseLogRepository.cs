using Google.Cloud.Firestore;
using CELERATE.API.CORE.Interfaces;
using CoreLogEntry = CELERATE.API.CORE.Entities.LogEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CELERATE.API.Infrastructure.Firebase.Logging
{
    public class FirebaseLogRepository : ILogRepository
    {
        private readonly FirestoreDb _db;
        private readonly string _collectionName = "logs";

        public FirebaseLogRepository(FirestoreDb db)
        {
            _db = db;
        }

        public async Task LogActionAsync(string userId, string action, string details,
            DateTime startTime, DateTime endTime, string branchId)
        {
            // Burada tam tipe açıkça referans veriyoruz
            var logEntry = new CELERATE.API.CORE.Entities.LogEntry
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Action = action,
                Details = details,
                StartTime = startTime,
                EndTime = endTime,
                BranchId = branchId
            };

            var docRef = _db.Collection(_collectionName).Document(logEntry.Id);
            await docRef.SetAsync(logEntry);
        }

        public async Task<IReadOnlyList<CELERATE.API.CORE.Entities.LogEntry>> GetLogsByUserIdAsync(string userId)
        {
            var query = _db.Collection(_collectionName)
                .WhereEqualTo("UserId", userId)
                .OrderByDescending("StartTime");

            var snapshot = await query.GetSnapshotAsync();

            // Dönüş tipini IReadOnlyList<CELERATE.API.CORE.Entities.LogEntry> olarak netleştir
            return snapshot.Documents
                .Select(d => d.ConvertTo<CELERATE.API.CORE.Entities.LogEntry>())
                .ToList();
        }

        public async Task<IReadOnlyList<CELERATE.API.CORE.Entities.LogEntry>> GetLogsByBranchIdAsync(string branchId)
        {
            var query = _db.Collection(_collectionName)
                .WhereEqualTo("BranchId", branchId)
                .OrderByDescending("StartTime");

            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<CELERATE.API.CORE.Entities.LogEntry>())
                .ToList();
        }

        public async Task<IReadOnlyList<CELERATE.API.CORE.Entities.LogEntry>> GetLogsByDateRangeAsync(DateTime start, DateTime end)
        {
            var query = _db.Collection(_collectionName)
                .WhereGreaterThanOrEqualTo("StartTime", start)
                .WhereLessThanOrEqualTo("EndTime", end)
                .OrderByDescending("StartTime");

            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<CELERATE.API.CORE.Entities.LogEntry>())
                .ToList();
        }

        public async Task<IReadOnlyList<CELERATE.API.CORE.Entities.LogEntry>> GetLogsByActionTypeAsync(string actionType)
        {
            var query = _db.Collection(_collectionName)
                .WhereEqualTo("Action", actionType)
                .OrderByDescending("StartTime");

            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<CELERATE.API.CORE.Entities.LogEntry>())
                .ToList();
        }
    }
}