// File: CELERATE.API.Infrastructure/Firebase/Repositories/FirebaseTransactionRepository.cs
using Google.Cloud.Firestore;
using CELERATE.API.CORE.Interfaces;
using CELERATE.API.Infrastructure.Firebase.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// Using alias to avoid name collision with Firestore's Transaction class
using DomainTransaction = CELERATE.API.CORE.Entities.Transaction;

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

        public async Task<DomainTransaction> GetByIdAsync(string id)
        {
            var docRef = _db.Collection(_collectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
                return null;

            return TransactionConverter.FromFirestore(snapshot);
        }

        public async Task<IReadOnlyList<DomainTransaction>> GetAllAsync()
        {
            var query = _db.Collection(_collectionName);
            var snapshot = await query.GetSnapshotAsync();

            var result = new List<DomainTransaction>();
            foreach (var doc in snapshot.Documents)
            {
                result.Add(TransactionConverter.FromFirestore(doc));
            }
            return result;
        }

        public async Task<string> AddAsync(DomainTransaction entity)
        {
            var docRef = _db.Collection(_collectionName).Document(entity.Id);
            await docRef.SetAsync(TransactionConverter.ToFirestore(entity));
            return entity.Id;
        }

        public async Task UpdateAsync(DomainTransaction entity)
        {
            var docRef = _db.Collection(_collectionName).Document(entity.Id);
            await docRef.SetAsync(TransactionConverter.ToFirestore(entity));
        }

        public async Task<IReadOnlyList<DomainTransaction>> GetByUserIdAsync(string userId)
        {
            var query = _db.Collection(_collectionName)
                .WhereEqualTo("UserId", userId)
                .OrderByDescending("CreatedAt");

            var snapshot = await query.GetSnapshotAsync();

            var result = new List<DomainTransaction>();
            foreach (var doc in snapshot.Documents)
            {
                result.Add(TransactionConverter.FromFirestore(doc));
            }
            return result;
        }

        public async Task<IReadOnlyList<DomainTransaction>> GetByCardIdAsync(string cardId)
        {
            var query = _db.Collection(_collectionName)
                .WhereEqualTo("CardId", cardId)
                .OrderByDescending("CreatedAt");

            var snapshot = await query.GetSnapshotAsync();

            var result = new List<DomainTransaction>();
            foreach (var doc in snapshot.Documents)
            {
                result.Add(TransactionConverter.FromFirestore(doc));
            }
            return result;
        }

        public async Task<IReadOnlyList<DomainTransaction>> GetByBranchIdAsync(string branchId)
        {
            var query = _db.Collection(_collectionName)
                .WhereEqualTo("BranchId", branchId)
                .OrderByDescending("CreatedAt");

            var snapshot = await query.GetSnapshotAsync();

            var result = new List<DomainTransaction>();
            foreach (var doc in snapshot.Documents)
            {
                result.Add(TransactionConverter.FromFirestore(doc));
            }
            return result;
        }

        public async Task<IReadOnlyList<DomainTransaction>> GetByOperatorIdAsync(string operatorId)
        {
            var query = _db.Collection(_collectionName)
                .WhereEqualTo("OperatorId", operatorId)
                .OrderByDescending("CreatedAt");

            var snapshot = await query.GetSnapshotAsync();

            var result = new List<DomainTransaction>();
            foreach (var doc in snapshot.Documents)
            {
                result.Add(TransactionConverter.FromFirestore(doc));
            }
            return result;
        }

        public async Task<IReadOnlyList<DomainTransaction>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            var query = _db.Collection(_collectionName)
                .WhereGreaterThanOrEqualTo("CreatedAt", start)
                .WhereLessThanOrEqualTo("CreatedAt", end)
                .OrderByDescending("CreatedAt");

            var snapshot = await query.GetSnapshotAsync();

            var result = new List<DomainTransaction>();
            foreach (var doc in snapshot.Documents)
            {
                result.Add(TransactionConverter.FromFirestore(doc));
            }
            return result;
        }
    }
}