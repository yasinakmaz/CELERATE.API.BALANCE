global using CELERATE.API.CORE.Entities;
namespace CELERATE.API.CORE.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByCardIdAsync(string cardId);
        Task<User> GetByPhoneNumberAsync(string phoneNumber);
        Task<User> GetByTcIdentityNumberAsync(string tcIdentityNumber);
    }
}
