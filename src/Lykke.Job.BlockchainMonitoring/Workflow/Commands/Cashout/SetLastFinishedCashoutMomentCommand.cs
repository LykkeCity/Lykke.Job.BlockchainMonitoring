using System;
using MessagePack;

namespace Lykke.Job.BlockchainMonitoring.Workflow.Commands.Cashout
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class SetLastFinishedCashoutMomentCommand
    {
        public Guid OperationId { get; set; }

        public string AssetId { get; set; }

        public string AssetMetricId { get; set; }

        public DateTime Finished { get; set; }
    }
}
