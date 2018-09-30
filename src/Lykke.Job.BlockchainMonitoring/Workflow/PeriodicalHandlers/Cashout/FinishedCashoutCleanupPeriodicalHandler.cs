using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Common;
using Lykke.Common.Log;
using Lykke.Job.BlockchainMonitoring.Domain.Repositories;

namespace Lykke.Job.BlockchainMonitoring.Workflow.PeriodicalHandlers.Cashout
{
    public class FinishedCashoutCleanupPeriodicalHandler:IStopable, IStartable
    {
        private readonly ITimerTrigger _timer;
        private readonly IActiveCashoutRepository _activeCashoutRepository;
        private readonly TimeSpan _operationAgeToCleanup;

        public FinishedCashoutCleanupPeriodicalHandler(
            TimeSpan timerPeriod,
            TimeSpan operationAgeToCleanup,
            ILogFactory logFactory, 
            IActiveCashoutRepository activeCashoutRepository)
        {
            _activeCashoutRepository = activeCashoutRepository;

            _operationAgeToCleanup = operationAgeToCleanup;

            _timer = new TimerTrigger(
                nameof(FinishedCashoutCleanupPeriodicalHandler),
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
            var allOperations = await _activeCashoutRepository.GetAllAsync();

            foreach (var op in allOperations.Where(p => p.finished && (DateTime.UtcNow - p.startedAt) >= _operationAgeToCleanup))
            {
                await _activeCashoutRepository.DeleteIfExistAsync(op.operationId);
            }
        }
    }
}
