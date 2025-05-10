using CELERATE.API.CORE.Entities;
using CELERATE.API.CORE.Interfaces;
using Google.Cloud.Firestore;

namespace CELERATE.API.Infrastructure.Firebase.Repositories
{
    public class FirebaseBranchRepository : IBranchRepository
    {
        private readonly FirestoreDb _db;
        private readonly string _collectionName = "branches";

        public FirebaseBranchRepository(FirestoreDb db)
        {
            _db = db;
        }

        public async Task<Branch> GetByIdAsync(string id)
        {
            var docRef = _db.Collection(_collectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
                return null;

            return snapshot.ConvertTo<Branch>();
        }

        public async Task<IReadOnlyList<Branch>> GetAllAsync()
        {
            var query = _db.Collection(_collectionName);
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<Branch>())
                .ToList();
        }

        public async Task<string> AddAsync(Branch entity)
        {
            var docRef = _db.Collection(_collectionName).Document(entity.Id);
            await docRef.SetAsync(entity);
            return entity.Id;
        }

        public async Task UpdateAsync(Branch entity)
        {
            var docRef = _db.Collection(_collectionName).Document(entity.Id);
            await docRef.SetAsync(entity);
        }

        public async Task<Branch> GetByCodeAsync(string code)
        {
            var query = _db.Collection(_collectionName).WhereEqualTo("Code", code);
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<Branch>())
                .FirstOrDefault();
        }
    }
}
