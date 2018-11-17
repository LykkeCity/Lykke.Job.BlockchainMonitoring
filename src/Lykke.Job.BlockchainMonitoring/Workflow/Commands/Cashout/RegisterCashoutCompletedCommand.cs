using System;
using MessagePack;

namespace Lykke.Job.BlockchainMonitoring.Workflow.Commands.Cashout
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class RegisterCashoutCompletedCommand
    {
        public Guid OperationId { get; set; }

        public string AssetMetricId { get; set; }
    }
}
