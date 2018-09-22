using System;

namespace Lykke.Job.BlockchainMonitoring.Workflow.Commands.Cashout
{
    public class RegisterCashoutFailedCommand
    {
        public Guid OperationId { get; set; }
        public string AssetId { get; set; }
    }
}
