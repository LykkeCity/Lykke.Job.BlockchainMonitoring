using Autofac;
using Lykke.Common.Log;
using Lykke.Job.BlockchainMonitoring.AzureRepositories.ActiveOperations;
using Lykke.Job.BlockchainMonitoring.AzureRepositories.CashoutMetricCollection;
using Lykke.Job.BlockchainMonitoring.AzureRepositories.LastFinishedCashoutMoment;
using Lykke.Job.BlockchainMonitoring.AzureRepositories.MetricDeduplication;
using Lykke.Job.BlockchainMonitoring.Domain.Repositories;
using Lykke.Job.BlockchainMonitoring.Domain.Services;
using Lykke.Job.BlockchainMonitoring.Settings.JobSettings;
using Lykke.SettingsReader;

namespace Lykke.Job.BlockchainMonitoring.Modules
{
    public class RepositoriesModule : Module
    {
        private readonly IReloadingManager<DbSettings> _dbSettings;

        public RepositoriesModule(IReloadingManager<DbSettings> dbSettings)
        {
            _dbSettings = dbSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(p => CashoutMetricsCollectionRepository.Create(
                    _dbSettings.Nested(x => x.DataConnString),
                    p.Resolve<ILogFactory>()))
                .As<ICashoutMetricsCollectionRepository>()
                .SingleInstance();


            builder.Register(p => MetricDeduplicationRepository.Create(
                    _dbSettings.Nested(x => x.DataConnString),
                    p.Resolve<ILogFactory>()))
                .As<IMetricDeduplicationRepository>()
                .SingleInstance();

            builder.Register(p => ActiveCashoutRepository.Create(
                    _dbSettings.Nested(x => x.DataConnString),
                    p.Resolve<ILogFactory>()))
                .As<IActiveCashoutRepository>()
                .SingleInstance();

            builder.Register(p => LastCashoutMomentRepository.Create(
                    _dbSettings.Nested(x => x.DataConnString),
                    p.Resolve<ILogFactory>()))
                .As<ILastCashoutMomentRepository>()
                .SingleInstance();
            
        }
    }
}
