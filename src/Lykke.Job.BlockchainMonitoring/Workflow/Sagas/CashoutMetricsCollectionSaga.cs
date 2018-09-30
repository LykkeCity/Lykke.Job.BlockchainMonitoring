using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainMonitoring.Domain;
using Lykke.Job.BlockchainMonitoring.Domain.Repositories;
using Lykke.Job.BlockchainMonitoring.Workflow.Commands.Cashout;
using Lykke.Job.BlockchainMonitoring.Workflow.Events;
using Lykke.Job.BlockchainMonitoring.Workflow.Events.Cashout;

namespace Lykke.Job.BlockchainMonitoring.Workflow.Sagas
{
    public class CashoutMetricsCollectionSaga
    {
        private readonly ICashoutMetricsCollectionRepository _repository;
        private readonly IChaosKitty _chaosKitty;

        public const string BoundedContext = "bcn-integration.cashout-metrics";

        public CashoutMetricsCollectionSaga(ICashoutMetricsCollectionRepository repository, IChaosKitty chaosKitty)
        {
            _repository = repository;
            _chaosKitty = chaosKitty;
        }

        [UsedImplicitly]
        private async Task Handle(BlockchainCashoutProcessor.Contract.Events.CashoutStartedEvent evt, ICommandSender sender)
        {
            var aggregate = await _repository.GetOrAddAsync(evt.OperationId,
                () => CashoutMetricsCollectionAggregate.StartNew(operationId: evt.OperationId,
                    startMoment: DateTime.UtcNow, 
                    assetId: evt.AssetId,
                    amount: evt.Amount));

            _chaosKitty.Meow(evt.OperationId);

            if (aggregate.CurrentState == CashoutMetricsCollectionAggregate.State.Started)
            {
                sender.SendCommand(new RetrieveAssetInfoCommand
                    {
                        AssetId = aggregate.AssetId,
                        OperationId = aggregate.OperationId
                    }, 
                    BoundedContext);
            }
        }

        [UsedImplicitly]
        private async Task Handle(AssetInfoRetrievedEvent evt, ICommandSender sender)
        {
            var aggregate = await _repository.TryGetAsync(evt.OperationId);

            if(aggregate.OnAssetInfoRetrieved(evt.BlockchainIntegrationLayerId,
                evt.BlockchainIntegrationLayerAssetId))
            {
                sender.SendCommand(new RegisterCashoutAmountCommand
                    {
                        Amount = aggregate.Amount,
                        AssetMetricId = aggregate.AssetMetricId,
                        OperationId = aggregate.OperationId
                    },
                    BoundedContext);

                _chaosKitty.Meow(evt.OperationId);

                sender.SendCommand(new SetActiveOperationCommand
                {
                    OperationId = aggregate.OperationId,
                    AssetMetricId = aggregate.AssetMetricId,
                    AssetId = aggregate.AssetId,
                    StartedAt = aggregate.StartMoment
                }, BoundedContext);
                
                _chaosKitty.Meow(evt.OperationId);

                await _repository.SaveAsync(aggregate);
            }
        }

        [UsedImplicitly]
        private async Task Handle(BlockchainCashoutProcessor.Contract.Events.CashoutCompletedEvent evt, 
            ICommandSender sender)
        {
            var aggregate = await _repository.TryGetAsync(evt.OperationId);

            if (aggregate == null)
            {
                //cashout start not registered 
                return;
            }

            if (aggregate.OnCashoutFinished(DateTime.UtcNow))
            {
                sender.SendCommand(new RegisterCashoutDurationCommand
                    {
                        AssetMetricId = aggregate.AssetMetricId,
                        OperationId = aggregate.OperationId,
                        Started = aggregate.StartMoment,
                        Finished = aggregate.FinishMoment ?? throw new ArgumentNullException(nameof(aggregate.FinishMoment))
                    },
                    BoundedContext);

                _chaosKitty.Meow(evt.OperationId);

                sender.SendCommand(new RegisterCashoutCompletedCommand
                    {
                        AssetMetricId = aggregate.AssetMetricId,
                        OperationId = aggregate.OperationId
                    }, BoundedContext);

                _chaosKitty.Meow(evt.OperationId);

                sender.SendCommand(new SetActiveCashoutFinishedCommand
                {
                    OperationId = aggregate.OperationId
                }, BoundedContext);
                
                _chaosKitty.Meow(evt.OperationId);

                sender.SendCommand(new SetLastFinishedCashoutMomentCommand
                {
                    OperationId = aggregate.OperationId,
                    AssetId = aggregate.AssetId,
                    AssetMetricId = aggregate.AssetMetricId,
                    Finished = aggregate.FinishMoment ?? throw new ArgumentNullException(nameof(aggregate.FinishMoment))
                }, BoundedContext);

                await _repository.SaveAsync(aggregate);
            }
        }
        
        [UsedImplicitly]
        private async Task Handle(BlockchainCashoutProcessor.Contract.Events.CashoutFailedEvent evt, 
            ICommandSender sender)
        {
            var aggregate = await _repository.TryGetAsync(evt.OperationId);

            if (aggregate == null)
            {
                //cashout start not registered 
                return;
            }

            if (aggregate.OnCashoutFinished(DateTime.UtcNow))
            {
                sender.SendCommand(new RegisterCashoutDurationCommand
                    {
                        AssetMetricId = evt.AssetId,
                        OperationId = aggregate.OperationId,
                        Started = aggregate.StartMoment,
                        Finished = aggregate.FinishMoment ?? throw new ArgumentNullException(nameof(aggregate.FinishMoment))
                    },
                    BoundedContext);

                _chaosKitty.Meow(evt.OperationId);

                sender.SendCommand(new RegisterCashoutFailedCommand
                {
                    AssetMetricId = aggregate.AssetMetricId,
                    OperationId = aggregate.OperationId
                }, BoundedContext);

                _chaosKitty.Meow(evt.OperationId);

                sender.SendCommand(new SetActiveCashoutFinishedCommand
                {
                    OperationId = aggregate.OperationId
                }, BoundedContext);

                _chaosKitty.Meow(evt.OperationId);

                sender.SendCommand(new SetLastFinishedCashoutMomentCommand
                {
                    OperationId = aggregate.OperationId,
                    AssetId = aggregate.AssetId,
                    AssetMetricId = aggregate.AssetMetricId,
                    Finished = aggregate.FinishMoment ?? throw new ArgumentNullException(nameof(aggregate.FinishMoment))
                }, BoundedContext);

                await _repository.SaveAsync(aggregate);
            }
        }
    }
}
