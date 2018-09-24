using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainMonitoring.Domain.Services;
using Lykke.Job.BlockchainMonitoring.Workflow.Commands.Cashout;

namespace Lykke.Job.BlockchainMonitoring.Workflow.CommandHandlers.Cashout
{
    public class RegisterCashoutResultCommandHandler
    {
        private readonly IChaosKitty _chaosKitty;
        private readonly IMetrickPublishAdapterWithDeduplication _metrickPublishAdapterWithDeduplication;

        public RegisterCashoutResultCommandHandler(IChaosKitty chaosKitty, IMetrickPublishAdapterWithDeduplication metrickPublishAdapterWithDeduplication)
        {
            _chaosKitty = chaosKitty;
            _metrickPublishAdapterWithDeduplication = metrickPublishAdapterWithDeduplication;
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(RegisterCashoutFailedCommand command, IEventPublisher publisher)
        {
            await _metrickPublishAdapterWithDeduplication.IncrementCounterAsync(MetricCounterType.Fail,
                command.AssetId,
                MetricOperationType.Cashout,
                command.OperationId);

            _chaosKitty.Meow(command.OperationId);

            return CommandHandlingResult.Ok();
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(RegisterCashoutCompletedCommand command, IEventPublisher publisher)
        {
            await _metrickPublishAdapterWithDeduplication.IncrementCounterAsync(MetricCounterType.Completed,
                command.AssetId,
                MetricOperationType.Cashout,
                command.OperationId);

            _chaosKitty.Meow(command.OperationId);

            return CommandHandlingResult.Ok();
        }
    }
}
