using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Common;
using Lykke.Common.Log;
using Lykke.Job.BlockchainMonitoring.Domain.Repositories;
using Lykke.Job.BlockchainMonitoring.Domain.Services;

namespace Lykke.Job.BlockchainMonitoring.Workflow.PeriodicalHandlers
{
    public class RegisterUnfinishedOperationDurationPeriodicalHandler : IStopable, IStartable
    {
        private readonly ITimerTrigger _timer;
        private readonly IActiveOperationsRepository _activeOperationsRepository;
        private readonly IMetricPublishAdapter _metrickPublishAdapter;

        public RegisterUnfinishedOperationDurationPeriodicalHandler(IActiveOperationsRepository activeOperationsRepository, 
            TimeSpan timerPeriod, 
            ILogFactory logFactory,
            IMetricPublishAdapter metrickPublishAdapter)
        {
            _activeOperationsRepository = activeOperationsRepository;
            _metrickPublishAdapter = metrickPublishAdapter;

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
            var activeOperations = (await _activeOperationsRepository.GetAllAsync()).ToList();

            foreach (var operationsByAssetId in activeOperations.GroupBy(p => p.assetId))
            {
                var longestOperation = operationsByAssetId.OrderByDescending(p => GetUnfinishedDuration(p.startedAt, p.finished)).First();

                await _metrickPublishAdapter.PublishGaugeAsync(MetricGaugeType.MaxUnfinishedDurationSeconds,
                    longestOperation.assetId,
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
