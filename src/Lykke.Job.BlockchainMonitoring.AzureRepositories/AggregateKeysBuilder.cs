using System;
using Common;

namespace Lykke.Job.BlockchainMonitoring.AzureRepositories
{
    internal static class AggregateKeysBuilder
    {
        public static string BuildPartitionKey(Guid aggregateId)
        {
            return aggregateId.ToString();
        }

        public static string BuildRowKey()
        {
            return "";
        }
    }
}
