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
        private readonly IMetricPublishAdapter _metricPublishAdapter;

        public RegisterCashoutResultCommandHandler(IChaosKitty chaosKitty, IMetricPublishAdapter metricPublishAdapter)
        {
            _chaosKitty = chaosKitty;
            _metricPublishAdapter = metricPublishAdapter;
        }

        [UsedImplicitly]
        public Task<CommandHandlingResult> Handle(RegisterCashoutFailedCommand command, IEventPublisher publisher)
        {
            _metricPublishAdapter.IncrementCounter("fail",
                command.AssetId,
                MetricOperationType.Cashout,
                command.OperationId);

            _chaosKitty.Meow(command.OperationId);

            return Task.FromResult(CommandHandlingResult.Ok());
        }

        [UsedImplicitly]
        public Task<CommandHandlingResult> Handle(RegisterCashoutCompletedCommand command, IEventPublisher publisher)
        {
            _metricPublishAdapter.IncrementCounter("completed",
                command.AssetId,
                MetricOperationType.Cashout,
                command.OperationId);

            _chaosKitty.Meow(command.OperationId);

            return Task.FromResult(CommandHandlingResult.Ok());
        }
    }
}
