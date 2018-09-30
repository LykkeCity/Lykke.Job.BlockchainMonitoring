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
    public class ActiveCashoutRepository: IActiveCashoutRepository
    {
        private readonly INoSQLTableStorage<ActiceCashoutEntity> _storage;

        private ActiveCashoutRepository(INoSQLTableStorage<ActiceCashoutEntity> storage)
        {
            _storage = storage;
        }

        public static IActiveCashoutRepository Create(IReloadingManager<string> connStringManager, 
            ILogFactory logFactory)
        {
            return new ActiveCashoutRepository(AzureTableStorage<ActiceCashoutEntity>.Create(connStringManager,
                "MonitoringActiveCashouts", logFactory));
        }

        public async Task InsertAsync(Guid operationId, string assetId, string assetMetricId, DateTime startedAt)
        {
            await _storage.InsertOrReplaceAsync(new ActiceCashoutEntity
            {
                OperationId = operationId,
                AssetId = assetId,
                AssetMetricId = assetMetricId,
                Finished = false,
                PartitionKey = BuildPartitionKey(operationId),
                RowKey = BuildRowKey(operationId),
                StartedAt = startedAt
            }, p => false);
        }

        public async Task<IEnumerable<(Guid operationId, string assetId, string assetMetricId, DateTime startedAt, bool finished)>> GetAllAsync()
        {
            return (await _storage.GetDataAsync())
                .Select(p => (p.OperationId, p.AssetId, p.AssetMetricId, p.StartedAt, p.Finished));
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
