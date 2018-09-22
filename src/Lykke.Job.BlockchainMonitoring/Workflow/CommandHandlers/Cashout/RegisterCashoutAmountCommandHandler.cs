using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainMonitoring.Domain.Services;
using Lykke.Job.BlockchainMonitoring.Workflow.Commands;
using Lykke.Job.BlockchainMonitoring.Workflow.Commands.Cashout;

namespace Lykke.Job.BlockchainMonitoring.Workflow.CommandHandlers.Cashout
{
    public class RegisterCashoutAmountCommandHandler
    {
        private readonly IChaosKitty _chaosKitty;
        private readonly IMetricPublishAdapter _metricPublishAdapter;

        public RegisterCashoutAmountCommandHandler(IChaosKitty chaosKitty, IMetricPublishAdapter metricPublishAdapter)
        {
            _chaosKitty = chaosKitty;
            _metricPublishAdapter = metricPublishAdapter;
        }

        [UsedImplicitly]
        public Task<CommandHandlingResult> Handle(RegisterCashoutAmountCommand command, IEventPublisher publisher)
        {
            _metricPublishAdapter.PublishGauge("amount",
                command.AssetId,
                MetricOperationType.Cashout,
                command.OperationId,
                decimal.ToDouble(command.Amount));

            _chaosKitty.Meow(command.OperationId);

            return Task.FromResult(CommandHandlingResult.Ok());
        }
    }
}
