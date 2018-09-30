using System;
using System.Net.Http;
using Autofac;
using Common;
using Lykke.Job.BlockchainMonitoring.Domain.Services;
using Lykke.Job.BlockchainMonitoring.DomainServices;
using Lykke.Job.BlockchainMonitoring.Settings;
using Lykke.Job.BlockchainMonitoring.Settings.JobSettings;
using Lykke.Service.Assets.Client;

namespace Lykke.Job.BlockchainMonitoring.Modules
{
    public class ServiceModule : Module
    {
        private readonly BlockchainMonitoringJobSettings _settings;
        private readonly AssetServiceClientSettings _assetServiceClientSettings;

        public ServiceModule(BlockchainMonitoringJobSettings settings, AssetServiceClientSettings assetServiceClientSettings)
        {
            _settings = settings;
            _assetServiceClientSettings = assetServiceClientSettings;
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
                .As<IMetricPublishAdapterWithDeduplication>()
                .SingleInstance();

            builder.RegisterInstance(new AssetsService(new Uri(_assetServiceClientSettings.ServiceUrl), new HttpClient()))
                .As<IAssetsService>()
                .SingleInstance();
        }
    }
}
