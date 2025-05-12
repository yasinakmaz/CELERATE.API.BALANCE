using CELERATE.API.CORE.Exceptions;
using Google.Cloud.Firestore;

namespace CELERATE.API.CORE.Entities
{
    [FirestoreData]
    public class User
    {
        // Boş constructor ekleme (Firestore için gerekli)
        public User() { }

        // Mevcut constructor'ı koruyun
        public User(string id, string fullName, string phoneNumber, string tcIdentityNumber,
            Gender gender, int age, UserType userType, string cardId, string branchId)
        {
            Id = id;
            FullName = fullName;
            PhoneNumber = phoneNumber;
            TcIdentityNumber = tcIdentityNumber;
            Gender = gender;
            Age = age;
            UserType = userType;
            CardId = cardId;
            BranchId = branchId;
            CreatedAt = DateTime.UtcNow;
            UserRole = userType == UserType.Customer ? UserRole.Customer : UserRole.Staff;
        }

        [FirestoreProperty]
        public string Id { get; set; }

        [FirestoreProperty]
        public string FullName { get; set; }

        [FirestoreProperty]
        public string PhoneNumber { get; set; }

        [FirestoreProperty]
        public string TcIdentityNumber { get; set; }

        [FirestoreProperty]
        public Gender Gender { get; set; }

        [FirestoreProperty]
        public int Age { get; set; }

        [FirestoreProperty]
        public UserType UserType { get; set; }

        [FirestoreProperty]
        public UserRole UserRole { get; set; }

        [FirestoreProperty]
        public bool IsActive { get; set; } = true;

        [FirestoreProperty]
        public string BranchId { get; set; }

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; }

        [FirestoreProperty]
        public DateTime? LastModifiedAt { get; set; }

        [FirestoreProperty]
        public string CardId { get; set; }

        // Kullanıcı rollerini güncelleme metodu
        public void UpdateRole(UserRole role)
        {
            if (UserType == UserType.Customer && role != UserRole.Customer)
            {
                throw new DomainException("Müşteri kullanıcısına personel rolü atanamaz");
            }

            UserRole = role;
            LastModifiedAt = DateTime.UtcNow;
        }
    }

    public enum Gender
    {
        Male,
        Female,
        Other
    }

    public enum UserType
    {
        Customer,
        Staff
    }

    public enum UserRole
    {
        Customer,
        Staff,
        BranchManager,
        Administrator
    }
}