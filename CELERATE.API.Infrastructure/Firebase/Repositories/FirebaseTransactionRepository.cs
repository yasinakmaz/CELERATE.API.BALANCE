using CELERATE.API.CORE.Interfaces;
using Google.Cloud.Firestore;

namespace CELERATE.API.Infrastructure.Firebase.Repositories
{
    public class FirebaseTransactionRepository : ITransactionRepository
    {
        private readonly FirestoreDb _db;
        private readonly string _collectionName = "transactions";

        public FirebaseTransactionRepository(FirestoreDb db)
        {
            _db = db;
        }

        public async Task<Transaction> GetByIdAsync(string id)
        {
            var docRef = _db.Collection(_collectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
                return null;

            return snapshot.ConvertTo<Transaction>();
        }

        public async Task<IReadOnlyList<Transaction>> GetAllAsync()
        {
            var query = _db.Collection(_collectionName);
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<Transaction>())
                .ToList();
        }

        public async Task<string> AddAsync(Transaction entity)
        {
            var docRef = _db.Collection(_collectionName).Document(entity.Id);
            await docRef.SetAsync(entity);
            return entity.Id;
        }

        public async Task UpdateAsync(Transaction entity)
        {
            var docRef = _db.Collection(_collectionName).Document(entity.Id);
            await docRef.SetAsync(entity);
        }

        public async Task<IReadOnlyList<Transaction>> GetByUserIdAsync(string userId)
        {
            var query = _db.Collection(_collectionName)
                .WhereEqualTo("UserId", userId)
                .OrderByDescending("CreatedAt");

            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<Transaction>())
                .ToList();
        }

        public async Task<IReadOnlyList<Transaction>> GetByCardIdAsync(string cardId)
        {
            var query = _db.Collection(_collectionName)
                .WhereEqualTo("CardId", cardId)
                .OrderByDescending("CreatedAt");

            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<Transaction>())
                .ToList();
        }

        public async Task<IReadOnlyList<Transaction>> GetByBranchIdAsync(string branchId)
        {
            var query = _db.Collection(_collectionName)
                .WhereEqualTo("BranchId", branchId)
                .OrderByDescending("CreatedAt");

            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<Transaction>())
                .ToList();
        }

        public async Task<IReadOnlyList<Transaction>> GetByOperatorIdAsync(string operatorId)
        {
            var query = _db.Collection(_collectionName)
                .WhereEqualTo("OperatorId", operatorId)
                .OrderByDescending("CreatedAt");

            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<Transaction>())
                .ToList();
        }

        public async Task<IReadOnlyList<Transaction>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            var query = _db.Collection(_collectionName)
                .WhereGreaterThanOrEqualTo("CreatedAt", start)
                .WhereLessThanOrEqualTo("CreatedAt", end)
                .OrderByDescending("CreatedAt");

            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<Transaction>())
                .ToList();
        }
    }
}
