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
        private readonly IMetricPublishAdapterWithDeduplication _metricPublishAdapterWithDeduplication;

        public RegisterCashoutAmountCommandHandler(IChaosKitty chaosKitty, IMetricPublishAdapterWithDeduplication metricPublishAdapterWithDeduplication)
        {
            _chaosKitty = chaosKitty;
            _metricPublishAdapterWithDeduplication = metricPublishAdapterWithDeduplication;
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(RegisterCashoutAmountCommand command, IEventPublisher publisher)
        {
            await _metricPublishAdapterWithDeduplication.PublishGaugeAsync(MetricGaugeType.Amount,
                command.AssetId,
                MetricOperationType.Cashout,
                command.OperationId,
                decimal.ToDouble(command.Amount));

            _chaosKitty.Meow(command.OperationId);

            return CommandHandlingResult.Ok();
        }
    }
}
