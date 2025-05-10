using Google.Cloud.Firestore;
using CELERATE.API.CORE.Interfaces;
using CELERATE.API.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<CELERATE.API.CORE.Entities.Transaction> GetByIdAsync(string id)
        {
            var docRef = _db.Collection(_collectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
                return null;

            return snapshot.ConvertTo<CELERATE.API.CORE.Entities.Transaction>();
        }

        public async Task<IReadOnlyList<CELERATE.API.CORE.Entities.Transaction>> GetAllAsync()
        {
            var query = _db.Collection(_collectionName);
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<CELERATE.API.CORE.Entities.Transaction>())
                .ToList();
        }

        public async Task<string> AddAsync(CELERATE.API.CORE.Entities.Transaction entity)
        {
            var docRef = _db.Collection(_collectionName).Document(entity.Id);
            await docRef.SetAsync(entity);
            return entity.Id;
        }

        public async Task UpdateAsync(CELERATE.API.CORE.Entities.Transaction entity)
        {
            var docRef = _db.Collection(_collectionName).Document(entity.Id);
            await docRef.SetAsync(entity);
        }

        public async Task<IReadOnlyList<CELERATE.API.CORE.Entities.Transaction>> GetByUserIdAsync(string userId)
        {
            var query = _db.Collection(_collectionName)
                .WhereEqualTo("UserId", userId)
                .OrderByDescending("CreatedAt");

            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<CELERATE.API.CORE.Entities.Transaction>())
                .ToList();
        }

        public async Task<IReadOnlyList<CELERATE.API.CORE.Entities.Transaction>> GetByCardIdAsync(string cardId)
        {
            var query = _db.Collection(_collectionName)
                .WhereEqualTo("CardId", cardId)
                .OrderByDescending("CreatedAt");

            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<CELERATE.API.CORE.Entities.Transaction>())
                .ToList();
        }

        public async Task<IReadOnlyList<CELERATE.API.CORE.Entities.Transaction>> GetByBranchIdAsync(string branchId)
        {
            var query = _db.Collection(_collectionName)
                .WhereEqualTo("BranchId", branchId)
                .OrderByDescending("CreatedAt");

            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<CELERATE.API.CORE.Entities.Transaction>())
                .ToList();
        }

        public async Task<IReadOnlyList<CELERATE.API.CORE.Entities.Transaction>> GetByOperatorIdAsync(string operatorId)
        {
            var query = _db.Collection(_collectionName)
                .WhereEqualTo("OperatorId", operatorId)
                .OrderByDescending("CreatedAt");

            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<CELERATE.API.CORE.Entities.Transaction>())
                .ToList();
        }

        public async Task<IReadOnlyList<CELERATE.API.CORE.Entities.Transaction>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            var query = _db.Collection(_collectionName)
                .WhereGreaterThanOrEqualTo("CreatedAt", start)
                .WhereLessThanOrEqualTo("CreatedAt", end)
                .OrderByDescending("CreatedAt");

            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<CELERATE.API.CORE.Entities.Transaction>())
                .ToList();
        }
    }
}