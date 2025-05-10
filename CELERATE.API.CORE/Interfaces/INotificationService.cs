namespace CELERATE.API.CORE.Interfaces
{
    /// <summary>
    /// Uygulama genelinde gerçek zamanlı bildirim göndermek için kullanılan servis interface'i.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Bir kullanıcının grup üyeliğine katılmasını sağlar
        /// </summary>
        Task JoinUserGroup(string userId);

        /// <summary>
        /// Bir şubenin grup üyeliğine katılmasını sağlar
        /// </summary>
        Task JoinBranchGroup(string branchId);

        /// <summary>
        /// Kullanıcıya bakiye değişikliği bildirimi gönderir
        /// </summary>
        Task NotifyUserBalanceChanged(string userId, decimal newBalance);

        /// <summary>
        /// Kullanıcıya detaylı bakiye değişikliği bildirimi gönderir
        /// </summary>
        Task NotifyUserBalanceChangedWithDetails(string userId, string transactionType, decimal oldBalance, decimal newBalance);

        /// <summary>
        /// Şubeye yeni bir işlem bildirimi gönderir (temel bilgiler)
        /// </summary>
        Task NotifyBranchTransaction(string branchId, string userId, decimal amount);

        /// <summary>
        /// Şubeye yeni bir işlem bildirimi gönderir (detaylı)
        /// </summary>
        Task NotifyBranchTransactionWithDetails(string branchId, string transactionId, string userId,
            string operatorId, string transactionType, decimal amount);

        /// <summary>
        /// Kullanıcıya kart işlemi bildirimi gönderir
        /// </summary>
        Task NotifyCardOperation(string userId, string cardId, string operationType);

        /// <summary>
        /// Sisteme giriş yapan yetkili personel sayısını günceller
        /// </summary>
        Task UpdateActiveStaffCount(string branchId, int activeStaffCount);

        /// <summary>
        /// Hata durumlarında bildirim gönderir
        /// </summary>
        Task NotifyError(string userId, string errorMessage, string errorCode = null);
    }
}