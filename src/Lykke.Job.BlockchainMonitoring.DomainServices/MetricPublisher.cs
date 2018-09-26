using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Common;
using Lykke.Job.BlockchainMonitoring.Domain.Services;
using Prometheus;

namespace Lykke.Job.BlockchainMonitoring.DomainServices
{
    public class MetricPublisher: IMetricPublishAdapter, IStartable, IStopable
    {
        private readonly MetricPusher _metricPusher;
        private const string AssetIdLabelName = "assetId";

        public MetricPublisher(MetricPusher metricPusher)
        {
            _metricPusher = metricPusher;
        }

        public Task PublishGaugeAsync(MetricGaugeType metricType, 
            string assetId,
            MetricOperationType operationType, 
            Guid operationId, 
            double metricValue,
            params KeyValuePair<string, string>[] additionalLabels)
        {
            var labels = new[] { new KeyValuePair<string, string>(AssetIdLabelName, assetId) }
                .Union(additionalLabels)
                .ToArray();

            var gauge = Metrics.CreateGauge(
                name: $"bil_{operationType.ToString().ToLower()}_{GetMetricName(metricType)}",
                help: $"measuring {GetMetricName(metricType)} for {operationType.ToString().ToLower()}-operation",
                configuration: new GaugeConfiguration
                {
                    SuppressInitialValue = false,
                    LabelNames = labels.Select(p => p.Key).ToArray()
                });

            gauge.WithLabels(labels.Select(p => p.Value).ToArray())
                .Set(metricValue);

            return Task.CompletedTask;
        }

        public Task IncrementCounterAsync(MetricCounterType metricType, 
            string assetId,
            MetricOperationType operationType, 
            Guid operationId,
            params KeyValuePair<string, string>[] additionalLabels)
        {
            var labels = new[] {new KeyValuePair<string, string>(AssetIdLabelName, assetId)}
                .Union(additionalLabels)
                .ToArray();
            
            var counter = Metrics.CreateCounter(
                name: $"bil_{operationType.ToString().ToLower()}_{GetMetricName(metricType)}",
                help: $"measuring {GetMetricName(metricType)} for {operationType.ToString().ToLower()}-operation",
                configuration: new CounterConfiguration
                {
                    SuppressInitialValue = false,
                    LabelNames = labels.Select(p => p.Key).ToArray()
                });

            counter.WithLabels(labels.Select(p => p.Value).ToArray()).Inc();

            return Task.CompletedTask;
        }

        public void Start()
        {
            _metricPusher.Start();
        }

        public void Dispose()
        {
            (_metricPusher as IDisposable)?.Dispose();
        }

        public void Stop()
        {
            _metricPusher.Stop();
        }

        public static string GetMetricName(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            var attributes =
                (PrometheusName[])fi.GetCustomAttributes(
                    typeof(PrometheusName),
                    false);

            return attributes.Length > 0 ? attributes[0].Name : value.ToString();
        }
    }
}
