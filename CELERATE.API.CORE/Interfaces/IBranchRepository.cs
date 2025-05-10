namespace CELERATE.API.CORE.Interfaces
{
    public interface IBranchRepository : IRepository<Branch>
    {
        Task<Branch> GetByCodeAsync(string code);
    }
}
