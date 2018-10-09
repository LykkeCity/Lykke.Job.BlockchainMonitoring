using Autofac;
using Common;
using Lykke.Common.Chaos;
using Lykke.Common.Log;
using Lykke.Cqrs;
using Lykke.Job.BlockchainMonitoring.Domain.Repositories;
using Lykke.Job.BlockchainMonitoring.Domain.Services;
using Lykke.Job.BlockchainMonitoring.Services;
using Lykke.Job.BlockchainMonitoring.Settings.JobSettings;
using Lykke.Job.BlockchainMonitoring.Workflow.PeriodicalHandlers.Cashout;
using Lykke.Sdk;
using Lykke.Sdk.Health;

namespace Lykke.Job.BlockchainMonitoring.Modules
{
    public class JobModule : Module
    {
        private readonly BlockchainMonitoringJobSettings _settings;

        public JobModule(BlockchainMonitoringJobSettings settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
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

            builder.RegisterChaosKitty(_settings.ChaosKitty);

            builder.Register(p => new FinishedCashoutCleanupPeriodicalHandler(
                    timerPeriod: _settings.ActiveOperations.CleanupTimerPeriod, 
                    operationAgeToCleanup: _settings.ActiveOperations.OperationAgeToCleanup,
                    logFactory: p.Resolve<ILogFactory>(), 
                    activeCashoutRepository: p.Resolve<IActiveCashoutRepository>()))
                .As<IStartable>()
                .As<IStopable>()
                .SingleInstance();
            
            builder.Register(p => new FinishTimeoutedCashoutsPeriodicHandler(
                    timerPeriod: _settings.ActiveOperations.OperationTimeoutTimerPeriod,
                    logFactory: p.Resolve<ILogFactory>(),
                    activeCashoutRepository: p.Resolve<IActiveCashoutRepository>(),
                    operationTimeout: _settings.ActiveOperations.OperationTimeout,
                    cqrsEngine: p.Resolve<ICqrsEngine>()))
                .As<IStartable>()
                .As<IStopable>()
                .SingleInstance();

            builder.RegisterType<RegisterUnfinishedCashoutDurationPeriodicalHandler>()
                .WithParameter(TypedParameter.From(_settings.ActiveOperations.RegisterUnifinishedOperationDurationTimerPeriod))
                .As<IStartable>()
                .As<IStopable>()
                .SingleInstance();

            builder.Register(p => new RegisterDurationSinceLastCashoutPeriodicalHandler(
                    timerPeriod: _settings.ActiveOperations.RegisterUnifinishedOperationDurationTimerPeriod,
                    logFactory: p.Resolve<ILogFactory>(),
                    metricPublishAdapter: p.Resolve<IMetricPublishAdapter>(),
                    lastMomentRepository: p.Resolve<ILastCashoutMomentRepository>(),
                    operationTimeout: _settings.ActiveOperations.OperationTimeout))
                .As<IStartable>()
                .As<IStopable>()
                .SingleInstance();
        }
    }
}
