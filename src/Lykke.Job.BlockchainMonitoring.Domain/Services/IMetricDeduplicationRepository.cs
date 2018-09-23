using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Job.BlockchainMonitoring.Domain.Services
{
    public interface IMetricDeduplicationRepository
    {
        Task InsertOrReplaceAsync(Guid operationId, Enum metricType);
        Task<bool> IsExistsAsync(Guid operationId, Enum metricType);
    }
}
