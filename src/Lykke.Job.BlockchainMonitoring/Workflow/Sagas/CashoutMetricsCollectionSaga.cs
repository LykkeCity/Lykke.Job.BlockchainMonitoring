using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainMonitoring.Domain;
using Lykke.Job.BlockchainMonitoring.Domain.Repositories;
using Lykke.Job.BlockchainMonitoring.Workflow.BoundedContexts;
using Lykke.Job.BlockchainMonitoring.Workflow.Commands.Cashout;
using Lykke.Job.BlockchainMonitoring.Workflow.Events.Cashout;

namespace Lykke.Job.BlockchainMonitoring.Workflow.Sagas
{
    public class CashoutMetricsCollectionSaga
    {
        private readonly ICashoutMetricsCollectionRepository _repository;
        private readonly IChaosKitty _chaosKitty;
        
        public CashoutMetricsCollectionSaga(ICashoutMetricsCollectionRepository repository, IChaosKitty chaosKitty)
        {
            _repository = repository;
            _chaosKitty = chaosKitty;
        }

        #region Start

        [UsedImplicitly]
        private  Task Handle(BlockchainCashoutProcessor.Contract.Events.CashoutStartedEvent evt, ICommandSender sender)
        {
            return OnStarted(evt.OperationId, DateTime.UtcNow, evt.AssetId, evt.Amount, sender);
        }

        [UsedImplicitly]
        private Task Handle(BlockchainCashoutProcessor.Contract.Events.BatchedCashoutStartedEvent evt, ICommandSender sender)
        {
            return OnStarted(evt.OperationId, DateTime.UtcNow, evt.AssetId, evt.Amount, sender);
        }

        private async Task OnStarted(
            Guid operationId,
            DateTime startMoment,
            string assetId,
            decimal amount, 
            ICommandSender sender)
        {
            var aggregate = await _repository.GetOrAddAsync(operationId,
                () => CashoutMetricsCollectionAggregate.StartNew(operationId: operationId,
                    startMoment: startMoment,
                    assetId: assetId,
                    amount: amount));

            _chaosKitty.Meow(operationId);

            if (aggregate.CurrentState == CashoutMetricsCollectionAggregate.State.Started)
            {
                sender.SendCommand(new RetrieveAssetInfoCommand
                    {
                        AssetId = aggregate.AssetId,
                        OperationId = aggregate.OperationId
                    },
                    CashoutMetricsCollectionBoundedContext.Name);
            }
        }
        
        #endregion

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
                    CashoutMetricsCollectionBoundedContext.Name);

                _chaosKitty.Meow(evt.OperationId);

                sender.SendCommand(new SetActiveOperationCommand
                {
                    OperationId = aggregate.OperationId,
                    AssetMetricId = aggregate.AssetMetricId,
                    AssetId = aggregate.AssetId,
                    StartedAt = aggregate.StartMoment
                }, CashoutMetricsCollectionBoundedContext.Name);
                
                _chaosKitty.Meow(evt.OperationId);

                await _repository.SaveAsync(aggregate);
            }
        }

        #region Finished

        [UsedImplicitly]
        private Task Handle(BlockchainCashoutProcessor.Contract.Events.CashoutCompletedEvent evt, ICommandSender sender)
        {
            return OnFinished(evt.OperationId, isFailed: false, finishedAt: evt.FinishMoment, sender: sender);
        }

        [UsedImplicitly]
        private  Task Handle(BlockchainCashoutProcessor.Contract.Events.CashoutFailedEvent evt,
            ICommandSender sender)
        {
            return OnFinished(evt.OperationId, isFailed: true, finishedAt: evt.FinishMoment, sender: sender);
        }

        [UsedImplicitly]
        private Task Handle(BlockchainCashoutProcessor.Contract.Events.CashoutsBatchCompletedEvent evt, ICommandSender sender)
        {
            return OnFinished(evt.BatchId, isFailed: false, finishedAt: evt.FinishMoment, sender: sender);
        }

        [UsedImplicitly]
        private Task Handle(BlockchainCashoutProcessor.Contract.Events.CashoutsBatchFailedEvent evt,
            ICommandSender sender)
        {
            return OnFinished(evt.BatchId, isFailed: true, finishedAt: evt.FinishMoment, sender: sender);
        }

        private async Task OnFinished(Guid operationId, bool isFailed, DateTime finishedAt, ICommandSender sender)
        {
            var aggregate = await _repository.TryGetAsync(operationId);

            if (aggregate == null)
            {
                //cashout start not registered 
                return;
            }

            if (aggregate.OnCashoutFinished(finishedAt))
            {
                sender.SendCommand(new RegisterCashoutDurationCommand
                    {
                        AssetMetricId = aggregate.AssetMetricId,
                        OperationId = aggregate.OperationId,
                        Started = aggregate.StartMoment,
                        Finished = aggregate.FinishMoment ??
                                   throw new ArgumentNullException(nameof(aggregate.FinishMoment))
                    },
                    CashoutMetricsCollectionBoundedContext.Name);

                _chaosKitty.Meow(aggregate.OperationId);

                if (isFailed)
                {
                    sender.SendCommand(new RegisterCashoutCompletedCommand
                    {
                        AssetMetricId = aggregate.AssetMetricId,
                        OperationId = aggregate.OperationId
                    }, CashoutMetricsCollectionBoundedContext.Name);
                }
                else
                {
                    sender.SendCommand(new RegisterCashoutFailedCommand
                    {
                        AssetMetricId = aggregate.AssetMetricId,
                        OperationId = aggregate.OperationId
                    }, CashoutMetricsCollectionBoundedContext.Name);
                }

                _chaosKitty.Meow(aggregate.OperationId);

                sender.SendCommand(new SetActiveCashoutFinishedCommand
                {
                    OperationId = aggregate.OperationId
                }, CashoutMetricsCollectionBoundedContext.Name);

                _chaosKitty.Meow(aggregate.OperationId);

                sender.SendCommand(new SetLastFinishedCashoutMomentCommand
                {
                    OperationId = aggregate.OperationId,
                    AssetId = aggregate.AssetId,
                    AssetMetricId = aggregate.AssetMetricId,
                    Finished = aggregate.FinishMoment ?? throw new ArgumentNullException(nameof(aggregate.FinishMoment))
                }, CashoutMetricsCollectionBoundedContext.Name);

                await _repository.SaveAsync(aggregate);
            }
        }

        #endregion


    }
}
