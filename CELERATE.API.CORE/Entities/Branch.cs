namespace CELERATE.API.CORE.Entities
{
    public class Branch
    {
        public string Id { get; private set; }
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string Title { get; private set; }
        public CompanyType CompanyType { get; private set; }
        public string TaxOffice { get; private set; }
        public string TaxNumber { get; private set; }
        public string IdentityNumber { get; private set; }
        public string Address { get; private set; }
        public BranchOperationType OperationType { get; private set; }
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }

        // Yapıcı metod
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
