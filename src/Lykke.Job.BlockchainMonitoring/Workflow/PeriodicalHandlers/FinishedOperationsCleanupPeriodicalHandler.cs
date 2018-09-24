using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Common;
using Lykke.Common.Log;
using Lykke.Job.BlockchainMonitoring.Domain.Repositories;

namespace Lykke.Job.BlockchainMonitoring.Workflow.PeriodicalHandlers
{
    public class FinishedOperationsCleanupPeriodicalHandler:IStopable, IStartable
    {
        private readonly ITimerTrigger _timer;
        private readonly IActiveOperationsRepository _activeOperationsRepository;
        private readonly TimeSpan _operationAgeToCleanup;

        public FinishedOperationsCleanupPeriodicalHandler(
            TimeSpan timerPeriod,
            TimeSpan operationAgeToCleanup,
            ILogFactory logFactory, 
            IActiveOperationsRepository activeOperationsRepository)
        {
            _activeOperationsRepository = activeOperationsRepository;

            _operationAgeToCleanup = operationAgeToCleanup;

            _timer = new TimerTrigger(
                nameof(FinishedOperationsCleanupPeriodicalHandler),
                timerPeriod,
                logFactory);

            _timer.Triggered += Execute;
        }


        public void Start()
        {
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
            var allOperations = await _activeOperationsRepository.GetAllAsync();

            foreach (var op in allOperations.Where(p => p.finished && (DateTime.UtcNow - p.startedAt) >= _operationAgeToCleanup))
            {
                await _activeOperationsRepository.DeleteIfExistAsync(op.operationId);
            }
        }
    }
}
