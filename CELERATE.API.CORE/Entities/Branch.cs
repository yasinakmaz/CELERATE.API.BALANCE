using Google.Cloud.Firestore;

namespace CELERATE.API.CORE.Entities
{
    [FirestoreData]
    public class Branch
    {
        // Boş constructor ekleme (Firestore için gerekli)
        public Branch() { }

        // Mevcut constructor'ı koruyun
        public Branch(string id, string code, string name, string title, CompanyType companyType,
            string taxOffice, string taxNumber, string identityNumber, string address,
            BranchOperationType operationType)
        {
            Id = id;
            Code = code;
            Name = name;
            Title = title;
            CompanyType = companyType;
            TaxOffice = taxOffice;
            TaxNumber = taxNumber;
            IdentityNumber = identityNumber;
            Address = address;
            OperationType = operationType;
            CreatedAt = DateTime.UtcNow;
        }

        [FirestoreProperty]
        public string Id { get; set; }

        [FirestoreProperty]
        public string Code { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string Title { get; set; }

        [FirestoreProperty]
        public CompanyType CompanyType { get; set; }

        [FirestoreProperty]
        public string TaxOffice { get; set; }

        [FirestoreProperty]
        public string TaxNumber { get; set; }

        [FirestoreProperty]
        public string IdentityNumber { get; set; }

        [FirestoreProperty]
        public string Address { get; set; }

        [FirestoreProperty]
        public BranchOperationType OperationType { get; set; }

        [FirestoreProperty]
        public bool IsActive { get; set; } = true;

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; }

        [FirestoreProperty]
        public DateTime? LastModifiedAt { get; set; }
    }

    public enum CompanyType
    {
        Individual,
        Corporate
    }

    public enum BranchOperationType
    {
        AddBalance,
        SpendBalance,
        Both
    }
}