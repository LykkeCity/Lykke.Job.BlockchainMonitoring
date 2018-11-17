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
    public class RegisterDurationSinceLastCashoutPeriodicalHandler:IStartable, IStopable
    {
        private readonly ITimerTrigger _timer;
        private readonly IMetricPublishAdapter _metricPublishAdapter;
        private readonly ILastCashoutMomentRepository _lastMomentRepository;
        private readonly ILog _log;
        private readonly TimeSpan _operationTimeout;

        public RegisterDurationSinceLastCashoutPeriodicalHandler(
            TimeSpan timerPeriod, 
            IMetricPublishAdapter metricPublishAdapter,
            ILogFactory logFactory, 
            ILastCashoutMomentRepository lastMomentRepository, 
            TimeSpan operationTimeout)
        {
            _metricPublishAdapter = metricPublishAdapter;
            _lastMomentRepository = lastMomentRepository;
            _operationTimeout = operationTimeout;
            _log = logFactory.CreateLog(this);

            _timer = new TimerTrigger(
                nameof(RegisterDurationSinceLastCashoutPeriodicalHandler),
                timerPeriod,
                logFactory);

            _timer.Triggered += Execute;
        }

        public void Start()
        {
            _log.Info($"Starting {nameof(RegisterDurationSinceLastCashoutPeriodicalHandler)}");
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
            var all = await _lastMomentRepository.GetAllAsync();
            var now = DateTime.UtcNow;

            foreach (var lastCashout in all.Where(p => DateTime.UtcNow - p.moment <= _operationTimeout))
            {
                await _metricPublishAdapter.PublishGaugeAsync(MetricGaugeType.DurationFromLastFinishSeconds,
                    lastCashout.assetMetricId,
                    MetricOperationType.Cashout,
                    lastCashout.operationId, (now - lastCashout.moment).TotalSeconds);
            }
        }
    }
}
