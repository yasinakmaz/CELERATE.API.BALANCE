// File: CELERATE.API.Infrastructure/Firebase/Repositories/FirebaseCardRepository.cs
using Google.Cloud.Firestore;
using CELERATE.API.CORE.Entities;
using CELERATE.API.CORE.Interfaces;
using CELERATE.API.Infrastructure.Firebase.Converters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            return CardConverter.FromFirestore(snapshot);
        }

        public async Task<IReadOnlyList<Card>> GetAllAsync()
        {
            var query = _db.Collection(_collectionName);
            var snapshot = await query.GetSnapshotAsync();

            var result = new List<Card>();
            foreach (var doc in snapshot.Documents)
            {
                result.Add(CardConverter.FromFirestore(doc));
            }
            return result;
        }

        public async Task<string> AddAsync(Card entity)
        {
            var docRef = _db.Collection(_collectionName).Document(entity.Id);
            await docRef.SetAsync(CardConverter.ToFirestore(entity));
            return entity.Id;
        }

        public async Task UpdateAsync(Card entity)
        {
            var docRef = _db.Collection(_collectionName).Document(entity.Id);
            await docRef.SetAsync(CardConverter.ToFirestore(entity));
        }

        public async Task<Card> GetByNfcIdAsync(string nfcId)
        {
            var query = _db.Collection(_collectionName).WhereEqualTo("NfcId", nfcId);
            var snapshot = await query.GetSnapshotAsync();

            var document = snapshot.Documents.FirstOrDefault();
            if (document == null)
                return null;

            return CardConverter.FromFirestore(document);
        }

        public async Task<IReadOnlyList<Card>> GetByUserIdAsync(string userId)
        {
            var query = _db.Collection(_collectionName).WhereEqualTo("UserId", userId);
            var snapshot = await query.GetSnapshotAsync();

            var result = new List<Card>();
            foreach (var doc in snapshot.Documents)
            {
                result.Add(CardConverter.FromFirestore(doc));
            }
            return result;
        }
    }
}