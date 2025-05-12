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

            var data = snapshot.ToDictionary();

            var branch = new Branch();
            branch.Id = snapshot.Id;
            branch.Code = GetStringValue(data, "Code");
            branch.Name = GetStringValue(data, "Name");
            branch.Title = GetStringValue(data, "Title");

            if (Enum.TryParse<CompanyType>(GetStringValue(data, "CompanyType"), true, out var companyType))
            {
                branch.CompanyType = companyType;
            }
            else
            {
                branch.CompanyType = CompanyType.Individual;
            }

            branch.TaxOffice = GetStringValue(data, "TaxOffice");
            branch.TaxNumber = GetStringValue(data, "TaxNumber");
            branch.IdentityNumber = GetStringValue(data, "IdentityNumber");
            branch.Address = GetStringValue(data, "Address");

            if (Enum.TryParse<BranchOperationType>(GetStringValue(data, "OperationType"), true, out var operationType))
            {
                branch.OperationType = operationType;
            }
            else
            {
                branch.OperationType = BranchOperationType.Both;
            }

            branch.IsActive = GetBoolValue(data, "IsActive", true);

            return branch;
        }

        private string GetStringValue(Dictionary<string, object> data, string key)
        {
            return data.TryGetValue(key, out var value) ? value?.ToString() : null;
        }

        private bool GetBoolValue(Dictionary<string, object> data, string key, bool defaultValue = false)
        {
            return data.TryGetValue(key, out var value) && value is bool boolValue ? boolValue : defaultValue;
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
