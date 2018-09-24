using System;
using MessagePack;

namespace Lykke.Job.BlockchainMonitoring.Workflow.Commands.Cashout
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class SetActiveOperationCommand
    {
        public Guid OperationId { get; set; }

        public string AssetId { get; set; }

        public DateTime StartedAt { get; set; }
    }
}
