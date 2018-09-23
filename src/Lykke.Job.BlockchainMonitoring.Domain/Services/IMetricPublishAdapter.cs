using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prometheus.Advanced.DataContracts;

namespace Lykke.Job.BlockchainMonitoring.Domain.Services
{
    public interface IMetricPublishAdapter
    {
        Task PublishGaugeAsync(MetricGaugeType metricType, 
            string assetId,
            MetricOperationType operationType, 
            Guid operationId, 
            double metricValue,
            params KeyValuePair<string, string>[] additionalLabels);


        Task IncrementCounterAsync(MetricCounterType metricType,
            string assetId,
            MetricOperationType operationType,
            Guid operationId,
            params KeyValuePair<string, string>[] additionalLabels);
    }
}
