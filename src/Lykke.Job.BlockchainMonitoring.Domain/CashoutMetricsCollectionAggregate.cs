using System;

namespace Lykke.Job.BlockchainMonitoring.Domain
{
    public class CashoutMetricsCollectionAggregate
    {
        public string Version { get; }
        public Guid OperationId { get; }
        public DateTime StartMoment { get; }
        public DateTime? FinishMoment { get; private set; }

        public State CurrentState { get; private set; }

        public string AssetId { get; }

        public string BlockchainIntegrationLayerId { get; set; }

        public string BlockchainIntegrationLayerAssetId { get; set; }

        public decimal Amount { get; set; }

        public string AssetMetricId => $"{BlockchainIntegrationLayerId}_{BlockchainIntegrationLayerAssetId}";

        private CashoutMetricsCollectionAggregate(string version, 
            Guid operationId,
            DateTime startMoment,
            DateTime? finishMoment, 
            string assetId,
            State currentState,
            string blockchainIntegrationLayerId, 
            string blockchainIntegrationLayerAssetId,
            decimal amount)
        {
            Version = version;
            OperationId = operationId;
            StartMoment = startMoment;
            FinishMoment = finishMoment;
            AssetId = assetId;
            CurrentState = currentState;
            BlockchainIntegrationLayerId = blockchainIntegrationLayerId;
            BlockchainIntegrationLayerAssetId = blockchainIntegrationLayerAssetId;
            Amount = amount;
        }

        public static CashoutMetricsCollectionAggregate StartNew(
            Guid operationId,
            DateTime startMoment,
            string assetId,
            decimal amount)
        {
            return new CashoutMetricsCollectionAggregate(version: null,
                operationId:operationId,
                startMoment: startMoment,
                finishMoment: null,
                assetId:assetId,
                currentState: State.Started,
                blockchainIntegrationLayerAssetId: null,
                blockchainIntegrationLayerId: null,
                amount: amount);
        }

        public static CashoutMetricsCollectionAggregate Restore(
            string version,
            Guid operationId,
            DateTime startMoment,
            DateTime? finishMoment,
            string assetId,
            State currentState,
            string blockchainIntegrationLayerId,
            string blockchainIntegrationLayerAssetId,
            decimal amount)
        {
            return new CashoutMetricsCollectionAggregate(version: version,
                operationId: operationId,
                startMoment: startMoment,
                finishMoment: finishMoment,
                assetId: assetId,
                currentState: currentState,
                blockchainIntegrationLayerAssetId: blockchainIntegrationLayerAssetId,
                blockchainIntegrationLayerId: blockchainIntegrationLayerId,
                amount: amount);
        }

        public bool OnAssetInfoRetrieved(string blockchainIntegrationLayerId,
            string blockchainIntegrationLayerAssetId)
        {
            if (CurrentState == State.Started)
            {
                BlockchainIntegrationLayerId = blockchainIntegrationLayerId;
                BlockchainIntegrationLayerAssetId = blockchainIntegrationLayerAssetId;

                CurrentState = State.AssetInfoRetrieved;

                return true;
            }

            return false;
        }

        public bool OnCashoutFinished(DateTime finishMomentTime)
        {
            if (CurrentState == State.Started)
            {
                throw new ArgumentException("Asset info not retrieved yet");
            }

            if (CurrentState == State.AssetInfoRetrieved)
            {
                FinishMoment = finishMomentTime;
                CurrentState = State.Finished;

                return true;
            }

            return false;
        }


        
        public enum State 
        {
            Started,
            AssetInfoRetrieved,
            Finished
        }
    }
}
