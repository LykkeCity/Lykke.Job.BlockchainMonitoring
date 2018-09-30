using System;
using MessagePack;

namespace Lykke.Job.BlockchainMonitoring.Workflow.Commands.Cashout
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class SetActiveCashoutFinishedCommand
    {
        public Guid OperationId { get; set; }
    }
}
