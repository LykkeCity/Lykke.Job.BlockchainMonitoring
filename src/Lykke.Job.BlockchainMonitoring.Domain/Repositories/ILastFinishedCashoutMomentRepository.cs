using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.BlockchainMonitoring.Domain.Repositories
{
    public interface ILastFinishedCashoutMomentRepository
    {
        Task<IEnumerable<(string assetId, DateTime lastFinishedCashoutMoment, Guid operationId)>> GetAllAsync();

        Task SetLastMomentAsync(string assetId, DateTime moment, Guid operationId);
    }
}
