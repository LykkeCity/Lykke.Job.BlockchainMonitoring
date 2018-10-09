using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Cqrs;
using Lykke.Job.BlockchainMonitoring.Domain.Repositories;
using Lykke.Job.BlockchainMonitoring.Workflow.BoundedContexts;
using Lykke.Job.BlockchainMonitoring.Workflow.Commands.Cashout;

namespace Lykke.Job.BlockchainMonitoring.Workflow.PeriodicalHandlers.Cashout
{
    public class FinishTimeoutedCashoutsPeriodicHandler:IStartable, IStopable
    {
        private readonly ITimerTrigger _timer;
        private readonly IActiveCashoutRepository _activeCashoutRepository;
        private readonly TimeSpan _operationTimeout;
        private readonly ILog _log;
        private readonly ICqrsEngine _cqrsEngine;

        public FinishTimeoutedCashoutsPeriodicHandler(IActiveCashoutRepository activeCashoutRepository, 
            TimeSpan operationTimeout,
            TimeSpan timerPeriod,
            ILogFactory logFactory,
            ICqrsEngine cqrsEngine)
        {
            _activeCashoutRepository = activeCashoutRepository;
            _operationTimeout = operationTimeout;
            _cqrsEngine = cqrsEngine;
            _log = logFactory.CreateLog(this);

            _timer = new TimerTrigger(
                nameof(FinishedCashoutCleanupPeriodicalHandler),
                timerPeriod,
                logFactory);

            _timer.Triggered += Execute;
        }

        public void Start()
        {
            _log.Info($"Starting {nameof(FinishTimeoutedCashoutsPeriodicHandler)}");
            _timer.Start();
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private async Task Execute(ITimerTrigger timer,
            TimerTriggeredHandlerArgs args,
            CancellationToken cancellationToken)
        {
            var allOperations = await _activeCashoutRepository.GetAllAsync();
            
            foreach (var operation in allOperations.Where(p => DateTime.UtcNow - p.startedAt > _operationTimeout))
            {
                _log.Warning("Finishing operation after timeout", context: operation);

                _cqrsEngine.SendCommand(new SetActiveCashoutFinishedCommand
                    {
                        OperationId = operation.operationId
                    },
                    CashoutMetricsCollectionBoundedContext.Name,
                    CashoutMetricsCollectionBoundedContext.Name);
            }
        }
    }
}
