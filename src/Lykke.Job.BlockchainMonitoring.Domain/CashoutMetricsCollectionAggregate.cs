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

        private CashoutMetricsCollectionAggregate(string version, 
            Guid operationId,
            DateTime startMoment,
            DateTime? finishMoment, 
            string assetId,
            State currentState)
        {
            Version = version;
            OperationId = operationId;
            StartMoment = startMoment;
            FinishMoment = finishMoment;
            AssetId = assetId;
            CurrentState = currentState;
        }

        public static CashoutMetricsCollectionAggregate StartNew(
            Guid operationId,
            DateTime startMoment,
            string assetId)
        {
            return new CashoutMetricsCollectionAggregate(version: null,
                operationId:operationId,
                startMoment: startMoment,
                finishMoment: null,
                assetId:assetId,
                currentState: State.Started);
        }

        public static CashoutMetricsCollectionAggregate Restore(
            string version,
            Guid operationId,
            DateTime startMoment,
            DateTime? finishMoment,
            string assetId,
            State currentState)
        {
            return new CashoutMetricsCollectionAggregate(version: version,
                operationId: operationId,
                startMoment: startMoment,
                finishMoment: finishMoment,
                assetId: assetId,
                currentState: currentState);
        }

        public bool OnCashoutFinished(DateTime finishMomentTime)
        {
            if (CurrentState == State.Started)
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
            Finished
        }
    }
}
