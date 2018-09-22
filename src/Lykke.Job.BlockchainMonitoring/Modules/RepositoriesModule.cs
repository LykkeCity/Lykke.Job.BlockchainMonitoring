using Autofac;
using Lykke.Common.Log;
using Lykke.Job.BlockchainMonitoring.AzureRepositories.CashoutMetricCollection;
using Lykke.Job.BlockchainMonitoring.Domain.Repositories;
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
        }
    }
}
