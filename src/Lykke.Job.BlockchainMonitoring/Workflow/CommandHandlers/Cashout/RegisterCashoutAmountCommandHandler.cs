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
        private readonly IMetricPublishFacade _metricPublishFacade;

        public RegisterCashoutAmountCommandHandler(IChaosKitty chaosKitty, IMetricPublishFacade metricPublishFacade)
        {
            _chaosKitty = chaosKitty;
            _metricPublishFacade = metricPublishFacade;
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(RegisterCashoutAmountCommand command, IEventPublisher publisher)
        {
            await _metricPublishFacade.PublishGaugeAsync(MetricGaugeType.Amount,
                command.AssetId,
                MetricOperationType.Cashout,
                command.OperationId,
                decimal.ToDouble(command.Amount));

            _chaosKitty.Meow(command.OperationId);

            return CommandHandlingResult.Ok();
        }
    }
}
