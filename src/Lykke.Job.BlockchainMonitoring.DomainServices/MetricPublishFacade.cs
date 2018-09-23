using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Job.BlockchainMonitoring.Domain.Services;

namespace Lykke.Job.BlockchainMonitoring.DomainServices
{
    public class MetricPublishFacade:IMetricPublishFacade
    {
        private readonly IMetricPublishAdapter _adapter;
        private readonly IMetricDeduplicationRepository _deduplicationRepository;

        public MetricPublishFacade(IMetricPublishAdapter adapter, IMetricDeduplicationRepository deduplicationRepository)
        {
            _adapter = adapter;
            _deduplicationRepository = deduplicationRepository;
        }

        public async Task PublishGaugeAsync(MetricGaugeType metricType, 
            string assetId,
            MetricOperationType operationType, 
            Guid operationId,
            double metricValue, 
            params KeyValuePair<string, string>[] additionalLabels)
        {
            await ExecuteAsync(operationId, metricType, () => _adapter.PublishGaugeAsync(metricType,
                assetId,
                operationType,
                operationId,
                metricValue,
                additionalLabels));
        }

        public async Task IncrementCounterAsync(MetricCounterType metricType, string assetId, MetricOperationType operationType, Guid operationId,
            params KeyValuePair<string, string>[] additionalLabels)
        {
            await ExecuteAsync(operationId, metricType, () => _adapter.IncrementCounterAsync(metricType,
                assetId, 
                operationType,
                operationId,
                additionalLabels));
        }

        private async Task ExecuteAsync(Guid operationId, Enum metricType, Func<Task> publishFunc)
        {
            if (!await _deduplicationRepository.IsExistsAsync(operationId, metricType))
            {
                await publishFunc();

                await _deduplicationRepository.InsertOrReplaceAsync(operationId, metricType);
            }
        }
    }
}
