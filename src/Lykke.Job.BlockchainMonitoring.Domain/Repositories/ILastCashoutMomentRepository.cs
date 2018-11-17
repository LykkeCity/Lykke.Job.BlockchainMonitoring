using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.BlockchainMonitoring.Domain.Repositories
{
    public interface ILastCashoutMomentRepository
    {
        Task<IEnumerable<(string assetId, string assetMetricId, DateTime moment, Guid operationId)>> GetAllAsync();

        Task SetLastMomentAsync(string assetId, string assetMetricId, DateTime moment, Guid operationId);
    }
}
