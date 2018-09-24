using Autofac;
using Common;
using Lykke.Job.BlockchainMonitoring.Domain.Services;
using Lykke.Job.BlockchainMonitoring.DomainServices;
using Lykke.Job.BlockchainMonitoring.Settings.JobSettings;

namespace Lykke.Job.BlockchainMonitoring.Modules
{
    public class ServiceModule : Module
    {
        private readonly BlockchainMonitoringJobSettings _settings;

        public ServiceModule(BlockchainMonitoringJobSettings settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MetricPublisher>()
                .WithParameter(TypedParameter.From(new Prometheus.MetricPusher(
                        _settings.PrometheusPushGatewayUrl.RemoveLastSymbolIfExists('/') + "/metrics", 
                    "bil-monitoring-job")))
                .As<IMetricPublishAdapter>()
                .As<IStartable>()
                .As<IStopable>()
                .SingleInstance();

            builder.RegisterType<MetricPublishAdapterWithDeduplication>()
                .As<IMetricPublishAdapterWithDeduplication>();
        }
    }
}
