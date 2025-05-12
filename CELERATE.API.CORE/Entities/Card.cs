using System;
using System.Collections.Generic;
using System.Linq;
using CELERATE.API.CORE.Exceptions;
using Google.Cloud.Firestore;

namespace CELERATE.API.CORE.Entities
{
    [FirestoreData]
    public class Card
    {
        // Boş constructor ekleme (Firestore dönüşümü için gerekli)
        public Card() { }

        // Mevcut constructor'ı koruyun
        public Card(string id, string nfcId, string userId, bool isAuthorized)
        {
            Id = id;
            NfcId = nfcId;
            UserId = userId;
            IsAuthorized = isAuthorized;
            Balance = 0;
            CreatedAt = DateTime.UtcNow;
            _permissions = new List<Permission>();
        }

        // Property'lere public setter ekleyin ve FirestoreProperty niteliği ekleyin
        [FirestoreProperty]
        public string Id { get; set; }

        [FirestoreProperty]
        public string NfcId { get; set; }

        [FirestoreProperty]
        public string UserId { get; set; }

        [FirestoreProperty]
        public decimal Balance { get; set; }

        [FirestoreProperty]
        public bool IsAuthorized { get; set; }

        [FirestoreProperty]
        public bool IsActive { get; set; } = true;

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; }

        [FirestoreProperty]
        public DateTime? LastModifiedAt { get; set; }

        // Permissions alanı için özel işlem
        private List<Permission> _permissions = new List<Permission>();

        // Serileştirme işlemi için bu property'yi koruyun
        public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

        // Firestore için string listesi olarak izinleri saklamak
        [FirestoreProperty]
        public List<string> PermissionStrings
        {
            get => _permissions.Select(p => p.ToString()).ToList();
            set
            {
                if (value != null)
                {
                    _permissions = value.Select(p => Enum.Parse<Permission>(p)).ToList();
                }
                else
                {
                    _permissions = new List<Permission>();
                }
            }
        }

        // Metodlar aynı kalabilir
        public void AddBalance(decimal amount)
        {
            if (amount <= 0)
                throw new DomainException("Yükleme miktarı 0'dan büyük olmalıdır");

            Balance += amount;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void SpendBalance(decimal amount)
        {
            if (amount <= 0)
                throw new DomainException("Harcama miktarı 0'dan büyük olmalıdır");

            if (Balance < amount)
                throw new DomainException("Yetersiz bakiye");

            Balance -= amount;
            LastModifiedAt = DateTime.UtcNow;
        }

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