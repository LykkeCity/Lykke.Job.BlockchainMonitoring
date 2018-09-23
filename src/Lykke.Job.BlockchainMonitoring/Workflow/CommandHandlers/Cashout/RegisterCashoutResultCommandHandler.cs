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
        private readonly IMetricPublishFacade _metricPublishFacade;

        public RegisterCashoutResultCommandHandler(IChaosKitty chaosKitty, IMetricPublishFacade metricPublishFacade)
        {
            _chaosKitty = chaosKitty;
            _metricPublishFacade = metricPublishFacade;
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(RegisterCashoutFailedCommand command, IEventPublisher publisher)
        {
            await _metricPublishFacade.IncrementCounterAsync(MetricCounterType.Fail,
                command.AssetId,
                MetricOperationType.Cashout,
                command.OperationId);

            _chaosKitty.Meow(command.OperationId);

            return CommandHandlingResult.Ok();
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(RegisterCashoutCompletedCommand command, IEventPublisher publisher)
        {
            await _metricPublishFacade.IncrementCounterAsync(MetricCounterType.Completed,
                command.AssetId,
                MetricOperationType.Cashout,
                command.OperationId);

            _chaosKitty.Meow(command.OperationId);

            return CommandHandlingResult.Ok();
        }
    }
}
