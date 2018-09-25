using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using Common;
using Lykke.Common.Log;
using Lykke.Job.BlockchainMonitoring.Domain.Repositories;
using Lykke.SettingsReader;

namespace Lykke.Job.BlockchainMonitoring.AzureRepositories.ActiveOperations
{
    public class ActiveOperationsRepository: IActiveOperationsRepository
    {
        private readonly INoSQLTableStorage<ActiveOperationEntity> _storage;

        private ActiveOperationsRepository(INoSQLTableStorage<ActiveOperationEntity> storage)
        {
            _storage = storage;
        }

        public static IActiveOperationsRepository Create(IReloadingManager<string> connStringManager, ILogFactory logFactory)
        {
            return new ActiveOperationsRepository(AzureTableStorage<ActiveOperationEntity>.Create(connStringManager, "MonitoringActiveOperation", logFactory));
        }

        public async Task InsertAsync(Guid operationId, string assetId, DateTime startedAt)
        {
            await _storage.InsertOrReplaceAsync(new ActiveOperationEntity
            {
                OperationId = operationId,
                AssetId = assetId,
                Finished = false,
                PartitionKey = BuildPartitionKey(operationId),
                RowKey = BuildRowKey(operationId),
                StartedAt = startedAt
            }, p => false);
        }

        public async Task<IEnumerable<(Guid operationId, string assetId, DateTime startedAt, bool finished)>> GetAllAsync()
        {
            return (await _storage.GetDataAsync())
                .Select(p => (p.OperationId, p.AssetId, p.StartedAt, p.Finished));
        }

        public Task SetFinishedAsync(Guid operationId)
        {
           return _storage.ReplaceAsync(BuildPartitionKey(operationId), BuildRowKey(operationId), entity => {
                entity.Finished = true;

                return entity;
            });
        }

        public Task DeleteIfExistAsync(Guid operationId)
        {
            return _storage.DeleteIfExistAsync(BuildPartitionKey(operationId), BuildRowKey(operationId));
        }

        private string BuildPartitionKey(Guid operationId)
        {
            return operationId.ToString().CalculateHexHash32(3);
        }


        private string BuildRowKey(Guid operationId)
        {
            return operationId.ToString("D");
        }
    }
}
