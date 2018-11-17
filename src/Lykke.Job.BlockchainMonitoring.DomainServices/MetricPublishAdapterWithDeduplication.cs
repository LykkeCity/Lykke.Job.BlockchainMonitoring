using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Job.BlockchainMonitoring.Domain.Services;

namespace Lykke.Job.BlockchainMonitoring.DomainServices
{
    public class MetricPublishAdapterWithDeduplication:IMetricPublishAdapterWithDeduplication
    {
        private readonly IMetricPublishAdapter _adapter;
        private readonly IMetricDeduplicationRepository _deduplicationRepository;
        private readonly SemaphoreSlim _semaphoreSlim;

        public MetricPublishAdapterWithDeduplication(IMetricPublishAdapter adapter, 
            IMetricDeduplicationRepository deduplicationRepository)
        {
            _adapter = adapter;
            _deduplicationRepository = deduplicationRepository;
            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public async Task PublishGaugeAsync(MetricGaugeType metricType, 
            string assetMetricId,
            MetricOperationType operationType, 
            Guid operationId,
            double metricValue, 
            params KeyValuePair<string, string>[] additionalLabels)
        {
            await ExecuteAsync(operationId, metricType, () => _adapter.PublishGaugeAsync(metricType,
                assetMetricId,
                operationType,
                operationId,
                metricValue,
                additionalLabels));
        }

        public async Task IncrementCounterAsync(MetricCounterType metricType, string assetMetricId, MetricOperationType operationType, Guid operationId,
            params KeyValuePair<string, string>[] additionalLabels)
        {
            await ExecuteAsync(operationId, metricType, () => _adapter.IncrementCounterAsync(metricType,
                assetMetricId, 
                operationType,
                operationId,
                additionalLabels));
        }

        private async Task ExecuteAsync(Guid operationId, Enum metricType, Func<Task> publishFunc)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();

                if (!await _deduplicationRepository.IsExistsAsync(operationId, metricType))
                {
                    await publishFunc();

                    await _deduplicationRepository.InsertOrReplaceAsync(operationId, metricType);
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
