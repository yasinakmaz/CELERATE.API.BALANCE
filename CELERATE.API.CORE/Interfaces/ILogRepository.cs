using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CELERATE.API.CORE.Entities;

namespace CELERATE.API.CORE.Interfaces
{
    public interface ILogRepository
    {
        Task LogActionAsync(string userId, string action, string details, DateTime startTime, DateTime endTime, string branchId);
        Task<IReadOnlyList<LogEntry>> GetLogsByUserIdAsync(string userId);
        Task<IReadOnlyList<LogEntry>> GetLogsByBranchIdAsync(string branchId);
        Task<IReadOnlyList<LogEntry>> GetLogsByDateRangeAsync(DateTime start, DateTime end);
        Task<IReadOnlyList<LogEntry>> GetLogsByActionTypeAsync(string actionType);
    }
}
