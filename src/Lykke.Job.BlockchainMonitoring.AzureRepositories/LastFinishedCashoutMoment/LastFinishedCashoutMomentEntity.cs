using System;
using System.Collections.Generic;
using System.Text;
using Lykke.AzureStorage.Tables;

namespace Lykke.Job.BlockchainMonitoring.AzureRepositories.LastFinishedCashoutMoment
{
    public class LastFinishedCashoutMomentEntity: AzureTableEntity
    {
        public string AssetId { get; set; }

        public DateTime Moment { get; set; }

        public Guid OperationId { get; set; }

        public static string GetPartitionKey(string assetId)
        {
            return assetId;
        }

        public static string GetRowKey(string assetId)
        {
            return assetId;
        }
    }
}
