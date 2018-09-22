using System;

namespace Lykke.Job.BlockchainMonitoring.Workflow.Commands.Cashout
{
    public class RegisterCashoutCompletedCommand
    {
        public Guid OperationId { get; set; }

        public string AssetId { get; set; }
    }
}
