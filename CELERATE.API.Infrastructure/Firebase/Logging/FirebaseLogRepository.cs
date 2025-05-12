using Google.Cloud.Firestore;
using CELERATE.API.CORE.Interfaces;
using CELERATE.API.CORE.Entities;
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
            var logEntry = new LogEntry
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

        public async Task<IReadOnlyList<LogEntry>> GetLogsByUserIdAsync(string userId)
        {
            var query = _db.Collection(_collectionName)
                .WhereEqualTo("UserId", userId)
                .OrderByDescending("StartTime");

            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<LogEntry>())
                .ToList();
        }

        public async Task<IReadOnlyList<LogEntry>> GetLogsByBranchIdAsync(string branchId)
        {
            var query = _db.Collection(_collectionName)
                .WhereEqualTo("BranchId", branchId)
                .OrderByDescending("StartTime");

            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<LogEntry>())
                .ToList();
        }

        public async Task<IReadOnlyList<LogEntry>> GetLogsByDateRangeAsync(DateTime start, DateTime end)
        {
            var query = _db.Collection(_collectionName)
                .WhereGreaterThanOrEqualTo("StartTime", start)
                .WhereLessThanOrEqualTo("EndTime", end)
                .OrderByDescending("StartTime");

            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<LogEntry>())
                .ToList();
        }

        public async Task<IReadOnlyList<LogEntry>> GetLogsByActionTypeAsync(string actionType)
        {
            var query = _db.Collection(_collectionName)
                .WhereEqualTo("Action", actionType)
                .OrderByDescending("StartTime");

            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<LogEntry>())
                .ToList();
        }
    }
}