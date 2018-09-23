using System;
using Lykke.AzureStorage.Tables;

namespace Lykke.Job.BlockchainMonitoring.AzureRepositories.MetricDeduplication
{
    public class MetricDeduplicationEntity:AzureTableEntity
    {
        public Guid OperationId { get; set; }

        public string MetricType { get; set; }
    }
}
