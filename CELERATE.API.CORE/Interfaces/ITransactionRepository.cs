namespace CELERATE.API.CORE.Interfaces
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<IReadOnlyList<Transaction>> GetByUserIdAsync(string userId);
        Task<IReadOnlyList<Transaction>> GetByCardIdAsync(string cardId);
        Task<IReadOnlyList<Transaction>> GetByBranchIdAsync(string branchId);
        Task<IReadOnlyList<Transaction>> GetByOperatorIdAsync(string operatorId);
        Task<IReadOnlyList<Transaction>> GetByDateRangeAsync(DateTime start, DateTime end);
    }
}
