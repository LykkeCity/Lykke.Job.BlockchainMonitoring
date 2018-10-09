using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.BlockchainMonitoring.Domain.Repositories;
using Lykke.Job.BlockchainMonitoring.Domain.Services;

namespace Lykke.Job.BlockchainMonitoring.Workflow.PeriodicalHandlers.Cashout
{
    public class RegisterUnfinishedCashoutDurationPeriodicalHandler : IStopable, IStartable
    {
        private readonly ITimerTrigger _timer;
        private readonly IActiveCashoutRepository _activeCashoutRepository;
        private readonly IMetricPublishAdapter _metrickPublishAdapter;
        private readonly ILog _log;

        public RegisterUnfinishedCashoutDurationPeriodicalHandler(IActiveCashoutRepository activeCashoutRepository, 
            TimeSpan timerPeriod, 
            ILogFactory logFactory,
            IMetricPublishAdapter metrickPublishAdapter)
        {
            _activeCashoutRepository = activeCashoutRepository;
            _metrickPublishAdapter = metrickPublishAdapter;
            _log = logFactory.CreateLog(this);

            _timer = new TimerTrigger(
                nameof(FinishedCashoutCleanupPeriodicalHandler),
                timerPeriod,
                logFactory);
            
            _timer.Triggered += Execute;
        }


        public void Start()
        {
            _log.Info($"Starting {nameof(RegisterUnfinishedCashoutDurationPeriodicalHandler)}");
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
            var activeOperations = (await _activeCashoutRepository.GetAllAsync()).ToList();

            foreach (var operationsByAssetId in activeOperations.GroupBy(p => p.assetId))
            {
                var longestOperation = operationsByAssetId.OrderByDescending(p => GetUnfinishedDuration(p.startedAt, p.finished)).First();

                await _metrickPublishAdapter.PublishGaugeAsync(MetricGaugeType.MaxUnfinishedDurationSeconds,
                    longestOperation.assetMetricId,
                    MetricOperationType.Cashout,
                    longestOperation.operationId,
                    GetUnfinishedDuration(longestOperation.startedAt, longestOperation.finished).TotalSeconds);
            }
        }

        private TimeSpan GetUnfinishedDuration(DateTime startedAt, bool finished)
        {
            return finished ? TimeSpan.Zero: DateTime.UtcNow - startedAt;
        }
    }
}
