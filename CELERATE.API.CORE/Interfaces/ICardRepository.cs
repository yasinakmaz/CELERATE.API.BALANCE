namespace CELERATE.API.CORE.Interfaces
{
    public interface ICardRepository : IRepository<Card>
    {
        Task<Card> GetByNfcIdAsync(string nfcId);
        Task<IReadOnlyList<Card>> GetByUserIdAsync(string userId);
    }
}
