using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainMonitoring.Domain.Services;
using Lykke.Job.BlockchainMonitoring.Workflow.Commands.Cashout;

namespace Lykke.Job.BlockchainMonitoring.Workflow.CommandHandlers.Cashout
{ 
    public class RegisterCashoutDurationCommandHandler
    {
        private readonly IChaosKitty _chaosKitty;
        private readonly IMetrickPublishAdapterWithDeduplication _metrickPublishAdapterWithDeduplication;

        public RegisterCashoutDurationCommandHandler(IChaosKitty chaosKitty, IMetrickPublishAdapterWithDeduplication metrickPublishAdapterWithDeduplication)
        {
            _chaosKitty = chaosKitty;
            _metrickPublishAdapterWithDeduplication = metrickPublishAdapterWithDeduplication;
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(RegisterCashoutDurationCommand command, IEventPublisher publisher)
        {
            await _metrickPublishAdapterWithDeduplication.PublishGaugeAsync(MetricGaugeType.UnfinishedDurationSeconds,
                command.AssetId,
                MetricOperationType.Cashout,
                command.OperationId,
                (command.Finished - command.Started).TotalSeconds);

            _chaosKitty.Meow(command.OperationId);

            return CommandHandlingResult.Ok();
        }
    }
}
