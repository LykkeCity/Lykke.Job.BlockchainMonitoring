using System;

namespace Lykke.Job.BlockchainMonitoring.Workflow.Commands.Cashout
{
    public class RegisterCashoutAmountCommand
    {
        public Guid OperationId { get; set; }

        public string AssetId { get; set; }

        public decimal Amount { get; set; }
    }
}
