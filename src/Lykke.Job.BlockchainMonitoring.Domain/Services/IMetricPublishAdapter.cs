using System;
using System.Collections.Generic;

namespace Lykke.Job.BlockchainMonitoring.Domain.Services
{
    public interface IMetricPublishAdapter
    {
        void PublishGauge(string metricName, 
            string assetId,
            MetricOperationType operationType, 
            Guid operationId, 
            double metricValue,
            params KeyValuePair<string, string>[] additionalLabels);


        void IncrementCounter(string metricName,
            string assetId,
            MetricOperationType operationType,
            Guid operationId,
            params KeyValuePair<string, string>[] additionalLabels);
    }
}
