namespace CELERATE.API.CORE.Entities
{
    public class User
    {
        public string Id { get; private set; }
        public string FullName { get; private set; }
        public string PhoneNumber { get; private set; }
        public string TcIdentityNumber { get; private set; }
        public Gender Gender { get; private set; }
        public int Age { get; private set; }
        public UserType UserType { get; private set; }
        public UserRole UserRole { get; private set; }
        public bool IsActive { get; private set; } = true;
        public string BranchId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }

        // NFC Kart bilgileri
        public string CardId { get; private set; }

        // Yapıcı metod ve davranışlar
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

            // Müşteri ise varsayılan olarak müşteri rolü
            UserRole = userType == UserType.Customer ? UserRole.Customer : UserRole.Staff;
        }

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
