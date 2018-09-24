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
        private readonly IMetricPublishFacade _metricPublishFacade;

        public RegisterUnfinishedOperationDurationPeriodicalHandler(IActiveOperationsRepository activeOperationsRepository, 
            TimeSpan timerPeriod, 
            ILogFactory logFactory, 
            IMetricPublishFacade metricPublishFacade)
        {
            _activeOperationsRepository = activeOperationsRepository;
            _metricPublishFacade = metricPublishFacade;

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
            var activeOperations = (await _activeOperationsRepository.GetAllAsync()).Where(p => !p.finished).ToList();

            foreach (var operationsByAssetId in activeOperations.GroupBy(p => p.assetId))
            {
                var longestOperation = operationsByAssetId.OrderBy(p => p.startedAt).First();

                await _metricPublishFacade.PublishGaugeAsync(MetricGaugeType.UnfinishedDurationSeconds,
                    longestOperation.assetId,
                    MetricOperationType.Cashout,
                    longestOperation.operationId,
                    (DateTime.UtcNow - longestOperation.startedAt).TotalSeconds);
            }
        }
    }
}
