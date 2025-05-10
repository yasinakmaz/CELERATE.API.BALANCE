using Google.Cloud.Firestore;
using CELERATE.API.CORE.Entites;

namespace CELERATE.API.Infrastructure.Firebase.Repositories
{
    public class FirebaseUserRepository : IUserRepository
    {
        private readonly FirestoreDb _db;
        private readonly string _collectionName = "users";

        public FirebaseUserRepository(FirestoreDb db)
        {
            _db = db;
        }

        public async Task<User> GetByIdAsync(string id)
        {
            var docRef = _db.Collection(_collectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
                return null;

            return snapshot.ConvertTo<User>();
        }

        public async Task<IReadOnlyList<User>> GetAllAsync()
        {
            var query = _db.Collection(_collectionName);
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<User>())
                .ToList();
        }

        public async Task<string> AddAsync(User entity)
        {
            var docRef = _db.Collection(_collectionName).Document(entity.Id);
            await docRef.SetAsync(entity);
            return entity.Id;
        }

        public async Task UpdateAsync(User entity)
        {
            var docRef = _db.Collection(_collectionName).Document(entity.Id);
            await docRef.SetAsync(entity);
        }

        public async Task<User> GetByCardIdAsync(string cardId)
        {
            var query = _db.Collection(_collectionName).WhereEqualTo("CardId", cardId);
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<User>())
                .FirstOrDefault();
        }

        public async Task<User> GetByPhoneNumberAsync(string phoneNumber)
        {
            var query = _db.Collection(_collectionName).WhereEqualTo("PhoneNumber", phoneNumber);
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<User>())
                .FirstOrDefault();
        }

        public async Task<User> GetByTcIdentityNumberAsync(string tcIdentityNumber)
        {
            var query = _db.Collection(_collectionName).WhereEqualTo("TcIdentityNumber", tcIdentityNumber);
            var snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents
                .Select(d => d.ConvertTo<User>())
                .FirstOrDefault();
        }
    }
}
