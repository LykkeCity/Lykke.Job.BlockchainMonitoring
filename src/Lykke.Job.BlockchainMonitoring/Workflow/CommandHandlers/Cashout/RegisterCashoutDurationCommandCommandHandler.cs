using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainMonitoring.Domain.Services;
using Lykke.Job.BlockchainMonitoring.Workflow.Commands;

namespace Lykke.Job.BlockchainMonitoring.Workflow.CommandHandlers.Cashout
{
    public class RegisterCashoutDurationCommandCommandHandler
    {
        private readonly IChaosKitty _chaosKitty;
        private readonly IMetricPublishAdapter _metricPublishAdapter;

        public RegisterCashoutDurationCommandCommandHandler(IChaosKitty chaosKitty, IMetricPublishAdapter metricPublishAdapter)
        {
            _chaosKitty = chaosKitty;
            _metricPublishAdapter = metricPublishAdapter;
        }

        [UsedImplicitly]
        public Task<CommandHandlingResult> Handle(RegisterCashoutDurationCommand command, IEventPublisher publisher)
        {
            _metricPublishAdapter.PublishGauge("duration_finished_seconds",
                command.AssetId,
                MetricOperationType.Cashout,
                command.OperationId,
                (command.Finished - command.Started).Seconds);

            _chaosKitty.Meow(command.OperationId);

            return Task.FromResult(CommandHandlingResult.Ok());
        }
    }
}
