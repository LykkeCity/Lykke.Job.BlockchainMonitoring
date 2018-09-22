using System;
using Lykke.AzureStorage.Tables;
using Lykke.Job.BlockchainMonitoring.Domain;

namespace Lykke.Job.BlockchainMonitoring.AzureRepositories.CashoutMetricCollection
{
    internal class CashoutMetricsCollectionEntity: AzureTableEntity
    {
        #region Fields

        public Guid OperationId { get; set; }
        public DateTime StartMoment { get; set; }
        public DateTime? FinishMoment { get; set; }

        public CashoutMetricsCollectionAggregate.State CurrentState { get; set; }

        public string AssetId { get; set; }

        #endregion

        #region Conversion

        public static CashoutMetricsCollectionEntity FromDomain(CashoutMetricsCollectionAggregate aggregate)
        {
            return new CashoutMetricsCollectionEntity
            {
                ETag = aggregate.Version,
                PartitionKey = AggregateKeysBuilder.BuildPartitionKey(aggregate.OperationId),
                RowKey = AggregateKeysBuilder.BuildRowKey(aggregate.OperationId),
                CurrentState = aggregate.CurrentState,
                StartMoment = aggregate.StartMoment,
                FinishMoment = aggregate.FinishMoment,
                OperationId = aggregate.OperationId,
                AssetId = aggregate.AssetId
            };
        }

        public CashoutMetricsCollectionAggregate ToDomain()
        {
            return CashoutMetricsCollectionAggregate.Restore(version: ETag,
                operationId: OperationId,
                startMoment: StartMoment,
                currentState: CurrentState,
                assetId: AssetId,
                finishMoment: FinishMoment);
        }

        #endregion
    }
}
