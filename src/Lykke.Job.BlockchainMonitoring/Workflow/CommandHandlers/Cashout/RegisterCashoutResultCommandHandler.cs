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
        private readonly IMetricPublishAdapterWithDeduplication _metricPublishAdapterWithDeduplication;

        public RegisterCashoutResultCommandHandler(IChaosKitty chaosKitty, IMetricPublishAdapterWithDeduplication metricPublishAdapterWithDeduplication)
        {
            _chaosKitty = chaosKitty;
            _metricPublishAdapterWithDeduplication = metricPublishAdapterWithDeduplication;
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(RegisterCashoutFailedCommand command, IEventPublisher publisher)
        {
            await _metricPublishAdapterWithDeduplication.IncrementCounterAsync(MetricCounterType.Fail,
                command.AssetId,
                MetricOperationType.Cashout,
                command.OperationId);

            _chaosKitty.Meow(command.OperationId);

            return CommandHandlingResult.Ok();
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(RegisterCashoutCompletedCommand command, IEventPublisher publisher)
        {
            await _metricPublishAdapterWithDeduplication.IncrementCounterAsync(MetricCounterType.Completed,
                command.AssetId,
                MetricOperationType.Cashout,
                command.OperationId);

            _chaosKitty.Meow(command.OperationId);

            return CommandHandlingResult.Ok();
        }
    }
}
