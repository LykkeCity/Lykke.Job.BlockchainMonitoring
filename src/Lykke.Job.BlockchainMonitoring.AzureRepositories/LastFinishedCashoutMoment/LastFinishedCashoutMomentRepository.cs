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

namespace Lykke.Job.BlockchainMonitoring.AzureRepositories.LastFinishedCashoutMoment
{
    public class LastFinishedCashoutMomentRepository: ILastFinishedCashoutMomentRepository
    {
        private readonly INoSQLTableStorage<LastFinishedCashoutMomentEntity> _storage;

        private LastFinishedCashoutMomentRepository(INoSQLTableStorage<LastFinishedCashoutMomentEntity> storage)
        {
            _storage = storage;
        }

        public static ILastFinishedCashoutMomentRepository Create(
            IReloadingManager<string> connectionString,
            ILogFactory log)
        {
            var storage = AzureTableStorage<LastFinishedCashoutMomentEntity>.Create(
                connectionString,
                "MonitoringLastFinishedCashoutMoments",
                log);

            return new LastFinishedCashoutMomentRepository(storage);
        }
        
        public async Task<IEnumerable<(string assetId, DateTime lastFinishedCashoutMoment, Guid operationId)>> GetAllAsync()
        {
            return (await _storage.GetDataAsync()).Select(p => (p.AssetId, p.Moment, p.OperationId));
        }

        public Task SetLastMomentAsync(string assetId, DateTime moment, Guid operationId)
        {
            return _storage.InsertOrReplaceAsync(new LastFinishedCashoutMomentEntity
            {
                AssetId = assetId,
                Moment = moment,
                OperationId = operationId,
                PartitionKey = BuildPartitionKey(assetId),
                RowKey = BuildRowKey(assetId)
            }, p => p.Moment < moment);
        }

        private string BuildPartitionKey(string assetId)
        {
            return assetId.CalculateHexHash32(3);
        }

        private string BuildRowKey(string assetId)
        {
            return assetId;
        }
    }
}
