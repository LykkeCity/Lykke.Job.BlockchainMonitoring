using System;
using MessagePack;

namespace Lykke.Job.BlockchainMonitoring.Workflow.Commands.Cashout
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class RegisterCashoutAmountCommand
    {
        public Guid OperationId { get; set; }

        public string AssetId { get; set; }

        public decimal Amount { get; set; }
    }
}
