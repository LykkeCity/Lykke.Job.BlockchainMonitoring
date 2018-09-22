using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common;
using Common.Log;
using Lykke.Common.Chaos;
using Lykke.Job.BlockchainMonitoring.Services;
using Lykke.Job.BlockchainMonitoring.Settings.JobSettings;
using Lykke.Sdk;
using Lykke.Sdk.Health;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Job.BlockchainMonitoring.Modules
{
    public class JobModule : Module
    {
        private readonly BlockchainMonitoringJobSettings _settings;
        private readonly IReloadingManager<BlockchainMonitoringJobSettings> _settingsManager;
        private readonly ChaosSettings _chaosSettings;

        public JobModule(BlockchainMonitoringJobSettings settings,
            IReloadingManager<BlockchainMonitoringJobSettings> settingsManager,
            ChaosSettings chaosSettings)
        {
            _settings = settings;
            _settingsManager = settingsManager;
            _chaosSettings = chaosSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // NOTE: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            // builder.RegisterType<QuotesPublisher>()
            //  .As<IQuotesPublisher>()
            //  .WithParameter(TypedParameter.From(_settings.Rabbit.ConnectionString))

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .AutoActivate()
                .SingleInstance();

            // TODO: Add your dependencies here

            builder.RegisterChaosKitty(_chaosSettings);
        }
    }
}
