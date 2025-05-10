namespace CELERATE.API.CORE.Entities
{
    public class Card
    {
        public string Id { get; private set; }
        public string NfcId { get; private set; }
        public string UserId { get; private set; }
        public decimal Balance { get; private set; }
        public bool IsAuthorized { get; private set; }
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }

        private List<Permission> _permissions = new List<Permission>();
        public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

        // Yapıcı metod
        public Card(string id, string nfcId, string userId, bool isAuthorized)
        {
            Id = id;
            NfcId = nfcId;
            UserId = userId;
            IsAuthorized = isAuthorized;
            Balance = 0;
            CreatedAt = DateTime.UtcNow;
        }

        // Bakiye yükleme
        public void AddBalance(decimal amount)
        {
            if (amount <= 0)
                throw new DomainException("Yükleme miktarı 0'dan büyük olmalıdır");

            Balance += amount;
            LastModifiedAt = DateTime.UtcNow;
        }

        // Bakiye harcama
        public void SpendBalance(decimal amount)
        {
            if (amount <= 0)
                throw new DomainException("Harcama miktarı 0'dan büyük olmalıdır");

            if (Balance < amount)
                throw new DomainException("Yetersiz bakiye");

            Balance -= amount;
            LastModifiedAt = DateTime.UtcNow;
        }

        // Yetki ekleme (sadece yetkili kartlar için)
        public void AddPermission(Permission permission)
        {
            if (!IsAuthorized)
                throw new DomainException("Bu kart yetkili kart değildir. Yetki eklenemez.");

            if (!_permissions.Contains(permission))
            {
                _permissions.Add(permission);
                LastModifiedAt = DateTime.UtcNow;
            }
        }
    }

    public enum Permission
    {
        Login,
        CreateCard,
        CreateAuthorizedCard,
        ViewDashboard,
        ViewLogs,
        ViewReports,
        CreateBranch,
        AddBalance,
        SpendBalance
    }
}
