using System;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using Common;
using Lykke.Common.Log;
using Lykke.Job.BlockchainMonitoring.Domain.Services;
using Lykke.SettingsReader;

namespace Lykke.Job.BlockchainMonitoring.AzureRepositories.MetricDeduplication
{
    public class MetricDeduplicationRepository: IMetricDeduplicationRepository
    {
        private readonly INoSQLTableStorage<MetricDeduplicationEntity> _storage;

        private MetricDeduplicationRepository(INoSQLTableStorage<MetricDeduplicationEntity> storage)
        {
            _storage = storage;
        }

        public static IMetricDeduplicationRepository Create(IReloadingManager<string> connStringManager, ILogFactory logFactory)
        {
            return new MetricDeduplicationRepository(AzureTableStorage<MetricDeduplicationEntity>.Create(connStringManager, 
                "MonitoringMetricDeduplications", 
                logFactory));
        }

        public async Task InsertOrReplaceAsync(Guid operationId, Enum metricType)
        {
            await _storage.InsertOrReplaceAsync(new MetricDeduplicationEntity
            {
                OperationId = operationId,
                MetricType = metricType.ToString(),
                PartitionKey = BuildPartitionKey(metricType, operationId),
                RowKey = BuildRowKey(operationId)
            });
        }

        public async Task<bool> IsExistsAsync(Guid operationId, Enum metricType)
        {
            return await _storage.GetDataAsync(BuildPartitionKey(metricType, operationId),
                       BuildRowKey(operationId)) != null;
        }

        private string BuildRowKey(Guid operationId)
        {
            return $"{operationId:D}";
        }
        
        private string BuildPartitionKey(Enum metricType, Guid operationId)
        {
            return $"{metricType}_{operationId.ToString().CalculateHexHash32()}";
        }
    }
}
