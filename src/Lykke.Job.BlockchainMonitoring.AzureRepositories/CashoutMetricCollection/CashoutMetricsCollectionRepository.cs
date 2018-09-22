using System;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using Lykke.Common.Log;
using Lykke.Job.BlockchainMonitoring.Domain;
using Lykke.Job.BlockchainMonitoring.Domain.Repositories;
using Lykke.SettingsReader;

namespace Lykke.Job.BlockchainMonitoring.AzureRepositories.CashoutMetricCollection
{
    public class CashoutMetricsCollectionRepository: ICashoutMetricsCollectionRepository
    {
        private readonly AggregateRepository<CashoutMetricsCollectionAggregate, CashoutMetricsCollectionEntity> _aggregateRepository;

        public static ICashoutMetricsCollectionRepository Create(
            IReloadingManager<string> connectionString,
            ILogFactory log)
        {
            var storage = AzureTableStorage<CashoutMetricsCollectionEntity>.Create(
                connectionString,
                "CashoutMetricsCollections",
                log);

            return new CashoutMetricsCollectionRepository(storage);
        }

        private CashoutMetricsCollectionRepository(INoSQLTableStorage<CashoutMetricsCollectionEntity> storage)
        {

            _aggregateRepository = new AggregateRepository<CashoutMetricsCollectionAggregate, CashoutMetricsCollectionEntity>(
                storage,
                mapAggregateToEntity: CashoutMetricsCollectionEntity.FromDomain,
                mapEntityToAggregate: entity => Task.FromResult(entity.ToDomain()));
        }

        public Task<CashoutMetricsCollectionAggregate> GetOrAddAsync(Guid aggregateId, Func<CashoutMetricsCollectionAggregate> newAggregateFactory)
        {
            return _aggregateRepository.GetOrAddAsync(aggregateId, newAggregateFactory);
        }

        public Task<CashoutMetricsCollectionAggregate> GetAsync(Guid operationId)
        {
            return _aggregateRepository.GetAsync(operationId);
        }

        public Task SaveAsync(CashoutMetricsCollectionAggregate aggregate)
        {
            return _aggregateRepository.SaveAsync(aggregate);
        }

        public Task<CashoutMetricsCollectionAggregate> TryGetAsync(Guid aggregateId)
        {
            return _aggregateRepository.TryGetAsync(aggregateId);
        }
    }
}
