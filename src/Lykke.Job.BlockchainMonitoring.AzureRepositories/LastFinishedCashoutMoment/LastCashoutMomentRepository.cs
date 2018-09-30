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
    public class LastCashoutMomentRepository: ILastCashoutMomentRepository
    {
        private readonly INoSQLTableStorage<LastCashoutMomentEntity> _storage;

        private LastCashoutMomentRepository(INoSQLTableStorage<LastCashoutMomentEntity> storage)
        {
            _storage = storage;
        }

        public static ILastCashoutMomentRepository Create(
            IReloadingManager<string> connectionString,
            ILogFactory log)
        {
            var storage = AzureTableStorage<LastCashoutMomentEntity>.Create(
                connectionString,
                "MonitoringLastCashoutMoments",
                log);

            return new LastCashoutMomentRepository(storage);
        }
        
        public async Task<IEnumerable<(string assetId, string assetMetricId, DateTime moment, Guid operationId)>> GetAllAsync()
        {
            return (await _storage.GetDataAsync()).Select(p => (p.AssetId, p.AssetMetricId, p.Moment, p.OperationId));
        }

        public Task SetLastMomentAsync(string assetId, string assetMetricId, DateTime moment, Guid operationId)
        {
            return _storage.InsertOrReplaceAsync(new LastCashoutMomentEntity
            {
                AssetId = assetId,
                AssetMetricId = assetMetricId,
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
