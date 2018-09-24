using System;
using Lykke.AzureStorage.Tables;

namespace Lykke.Job.BlockchainMonitoring.AzureRepositories.ActiveOperations
{
    public class ActiveOperationEntity:AzureTableEntity
    {
        public string AssetId { get; set; }

        public Guid OperationId { get; set; }

        public bool Finished { get; set; }

        public DateTime StartedAt { get; set; }
    }
}
