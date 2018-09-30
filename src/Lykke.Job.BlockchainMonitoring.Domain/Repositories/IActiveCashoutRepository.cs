using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.BlockchainMonitoring.Domain.Repositories
{
    public interface IActiveCashoutRepository
    {
        Task InsertAsync(Guid operationId, string assetId, string assetMetricId, DateTime startedAt);
        Task<IEnumerable<(Guid operationId, string assetId, string assetMetricId, DateTime startedAt, bool finished)>> GetAllAsync();
        Task SetFinishedAsync(Guid operationId);
        Task DeleteIfExistAsync(Guid operationId);
    }
}
