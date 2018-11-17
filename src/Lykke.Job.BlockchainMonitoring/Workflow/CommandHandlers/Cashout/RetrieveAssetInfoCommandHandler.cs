using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainMonitoring.Workflow.Commands.Cashout;
using Lykke.Job.BlockchainMonitoring.Workflow.Events.Cashout;
using Lykke.Service.Assets.Client;

namespace Lykke.Job.BlockchainMonitoring.Workflow.CommandHandlers.Cashout
{
    public class RetrieveAssetInfoCommandHandler
    {
        private readonly IChaosKitty _chaosKitty;
        private readonly IAssetsService _assetsService;

        public RetrieveAssetInfoCommandHandler(IChaosKitty chaosKitty, IAssetsService assetsService)
        {
            _chaosKitty = chaosKitty;
            _assetsService = assetsService;
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(RetrieveAssetInfoCommand command,
            IEventPublisher publisher)
        {
            var asset = await _assetsService.AssetGetAsync(command.AssetId);

            if (asset == null)
            {
                throw new ArgumentNullException(nameof(asset), $"Asset {command.AssetId} not found");
            }
            
            publisher.PublishEvent(new AssetInfoRetrievedEvent
            {
                OperationId = command.OperationId,
                BlockchainIntegrationLayerAssetId = asset.BlockchainIntegrationLayerAssetId,
                BlockchainIntegrationLayerId = asset.BlockchainIntegrationLayerId
            });

            _chaosKitty.Meow(command.OperationId);

            return CommandHandlingResult.Ok();
        }
    }
}
