using Autofac;
using Common;
using Lykke.Common.Chaos;
using Lykke.Common.Log;
using Lykke.Job.BlockchainMonitoring.Domain.Repositories;
using Lykke.Job.BlockchainMonitoring.Services;
using Lykke.Job.BlockchainMonitoring.Settings.JobSettings;
using Lykke.Job.BlockchainMonitoring.Workflow.PeriodicalHandlers;
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


            builder.RegisterType<RegisterUnfinishedCashoutDurationPeriodicalHandler>()
                .WithParameter(TypedParameter.From(_settings.ActiveOperations.RegisterUnifinishedOperationDurationTimerPeriod))
                .As<IStartable>()
                .As<IStopable>()
                .SingleInstance();



            builder.RegisterType<RegisterDurationFromLastCashoutPeriodicalHandler>()
                .WithParameter(TypedParameter.From(_settings.ActiveOperations.RegisterUnifinishedOperationDurationTimerPeriod))
                .As<IStartable>()
                .As<IStopable>()
                .SingleInstance();
        }
    }
}
