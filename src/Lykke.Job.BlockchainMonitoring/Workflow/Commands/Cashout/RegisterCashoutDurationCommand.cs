using System;
using MessagePack;

namespace Lykke.Job.BlockchainMonitoring.Workflow.Commands.Cashout
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class RegisterCashoutDurationCommand
    {
        public Guid OperationId { get; set; }

        public string AssetId { get; set; }

        public DateTime Started { get; set; }

        public DateTime Finished { get; set; }
    }
}
