using Google.Cloud.Firestore;
using CELERATE.API.CORE.Entities;
using CELERATE.API.CORE.Interfaces;

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

            var document = snapshot.Documents.FirstOrDefault();
            if (document == null)
                return null;

            // Manuel dönüşüm yap
            var data = document.ToDictionary();

            var user = new User();
            user.Id = document.Id;
            user.FullName = GetStringValue(data, "FullName");
            user.PhoneNumber = GetStringValue(data, "PhoneNumber");
            user.TcIdentityNumber = GetStringValue(data, "TcIdentityNumber");

            // Gender enum'ını güvenli şekilde dönüştür
            if (Enum.TryParse<Gender>(GetStringValue(data, "Gender"), true, out var gender))
            {
                user.Gender = gender;
            }
            else
            {
                user.Gender = Gender.Other; // Varsayılan değer
            }

            user.Age = GetIntValue(data, "Age");

            // Benzer şekilde diğer enum ve property'leri de ayarla
            if (Enum.TryParse<UserType>(GetStringValue(data, "UserType"), true, out var userType))
            {
                user.UserType = userType;
            }

            if (Enum.TryParse<UserRole>(GetStringValue(data, "UserRole"), true, out var userRole))
            {
                user.UserRole = userRole;
            }

            user.IsActive = GetBoolValue(data, "IsActive", true);
            user.BranchId = GetStringValue(data, "BranchId");
            user.CardId = GetStringValue(data, "CardId");

            return user;
        }
        private string GetStringValue(Dictionary<string, object> data, string key)
        {
            return data.TryGetValue(key, out var value) ? value?.ToString() : null;
        }

        private int GetIntValue(Dictionary<string, object> data, string key, int defaultValue = 0)
        {
            if (data.TryGetValue(key, out var value))
            {
                if (value is long longValue)
                    return (int)longValue;
                if (value is int intValue)
                    return intValue;
                if (int.TryParse(value?.ToString(), out var result))
                    return result;
            }
            return defaultValue;
        }

        private bool GetBoolValue(Dictionary<string, object> data, string key, bool defaultValue = false)
        {
            return data.TryGetValue(key, out var value) && value is bool boolValue ? boolValue : defaultValue;
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
