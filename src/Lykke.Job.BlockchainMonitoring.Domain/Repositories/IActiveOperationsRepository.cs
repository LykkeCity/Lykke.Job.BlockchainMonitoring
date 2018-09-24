using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.BlockchainMonitoring.Domain.Repositories
{
    public interface IActiveOperationsRepository
    {
        Task InsertAsync(Guid operationId, string assetId, DateTime startedAt);
        Task<IEnumerable<(Guid operationId, string assetId, DateTime startedAt, bool finished)>> GetAllAsync();
        Task SetFinishedAsync(Guid operationId);
        Task DeleteIfExistAsync(Guid operationId);
    }
}
