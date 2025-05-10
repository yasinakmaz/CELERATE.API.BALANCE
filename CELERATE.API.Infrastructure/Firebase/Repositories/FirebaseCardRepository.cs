using Google.Cloud.Firestore;
using CELERATE.API.CORE.Entities;
using CELERATE.API.CORE.Interfaces;

namespace CELERATE.API.Infrastructure.Firebase.Repositories
{
    public class FirebaseCardRepository : ICardRepository
    {
        private readonly FirestoreDb _db;
        private readonly string _collectionName = "cards";

        public FirebaseCardRepository(FirestoreDb db)
        {
            _db = db;
        }

        public async Task<Card> GetByIdAsync(string id)
        {
            var docRef = _db.Collection(_collectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
                return null;

            return snapshot.ConvertTo<Card>();
        }

        public async Task<IReadOnlyList<Card>> GetAllAsync()
        {
            var query = _db.Collection(_collectionName);
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<Card>())
                .ToList();
        }

        public async Task<string> AddAsync(Card entity)
        {
            var docRef = _db.Collection(_collectionName).Document(entity.Id);
            await docRef.SetAsync(entity);
            return entity.Id;
        }

        public async Task UpdateAsync(Card entity)
        {
            var docRef = _db.Collection(_collectionName).Document(entity.Id);
            await docRef.SetAsync(entity);
        }

        public async Task<Card> GetByNfcIdAsync(string nfcId)
        {
            var query = _db.Collection(_collectionName).WhereEqualTo("NfcId", nfcId);
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<Card>())
                .FirstOrDefault();
        }

        public async Task<IReadOnlyList<Card>> GetByUserIdAsync(string userId)
        {
            var query = _db.Collection(_collectionName).WhereEqualTo("UserId", userId);
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<Card>())
                .ToList();
        }
    }
}
