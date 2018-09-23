﻿using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainMonitoring.Domain.Services;
using Lykke.Job.BlockchainMonitoring.Workflow.Commands.Cashout;

namespace Lykke.Job.BlockchainMonitoring.Workflow.CommandHandlers.Cashout
{
    public class RegisterCashoutDurationCommandCommandHandler
    {
        private readonly IChaosKitty _chaosKitty;
        private readonly IMetricPublishFacade _metricPublishFacade;

        public RegisterCashoutDurationCommandCommandHandler(IChaosKitty chaosKitty, IMetricPublishFacade metricPublishFacade)
        {
            _chaosKitty = chaosKitty;
            _metricPublishFacade = metricPublishFacade;
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(RegisterCashoutDurationCommand command, IEventPublisher publisher)
        {
            await _metricPublishFacade.PublishGaugeAsync(MetricGaugeType.DurationSeconds,
                command.AssetId,
                MetricOperationType.Cashout,
                command.OperationId,
                (command.Finished - command.Started).Seconds);

            _chaosKitty.Meow(command.OperationId);

            return CommandHandlingResult.Ok();
        }
    }
}
