using System;
using MessagePack;

namespace Lykke.Job.BlockchainMonitoring.Workflow.Events.Cashout
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class AssetInfoRetrievedEvent
    {
        public Guid OperationId { get; set; }

        public string BlockchainIntegrationLayerId { get; set; }

        public string BlockchainIntegrationLayerAssetId { get; set; }
    }
}
