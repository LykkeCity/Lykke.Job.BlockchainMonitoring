using System;

namespace Lykke.Job.BlockchainMonitoring.Workflow.Commands
{
    public class RegisterCashoutDurationCommand
    {
        public Guid OperationId { get; set; }

        public string AssetId { get; set; }

        public DateTime Started { get; set; }

        public DateTime Finished { get; set; }
    }
}
