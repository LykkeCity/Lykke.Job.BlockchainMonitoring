﻿using System.Threading.Tasks;
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
        private readonly IMetricPublishAdapterWithDeduplication _metricPublishAdapterWithDeduplication;

        public RegisterCashoutDurationCommandHandler(IChaosKitty chaosKitty, IMetricPublishAdapterWithDeduplication metricPublishAdapterWithDeduplication)
        {
            _chaosKitty = chaosKitty;
            _metricPublishAdapterWithDeduplication = metricPublishAdapterWithDeduplication;
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(RegisterCashoutDurationCommand command, IEventPublisher publisher)
        {
            await _metricPublishAdapterWithDeduplication.PublishGaugeAsync(MetricGaugeType.FinishedDurationSeconds,
                command.AssetMetricId,
                MetricOperationType.Cashout,
                command.OperationId,
                (command.Finished - command.Started).TotalSeconds);

            _chaosKitty.Meow(command.OperationId);

            return CommandHandlingResult.Ok();
        }
    }
}
