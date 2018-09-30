using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Common;
using Lykke.Common.Log;
using Lykke.Job.BlockchainMonitoring.Domain.Repositories;
using Lykke.Job.BlockchainMonitoring.Domain.Services;

namespace Lykke.Job.BlockchainMonitoring.Workflow.PeriodicalHandlers.Cashout
{
    public class RegisterDurationFromLastCashoutPeriodicalHandler:IStartable, IStopable
    {
        private readonly ITimerTrigger _timer;
        private readonly IMetricPublishAdapter _metricPublishAdapter;
        private readonly ILastCashoutMomentRepository _lastMomentRepository;

        public RegisterDurationFromLastCashoutPeriodicalHandler(
            TimeSpan timerPeriod, 
            IMetricPublishAdapter metricPublishAdapter,
            ILogFactory logFactory, 
            ILastCashoutMomentRepository lastMomentRepository)
        {
            _metricPublishAdapter = metricPublishAdapter;
            _lastMomentRepository = lastMomentRepository;

            _timer = new TimerTrigger(
                nameof(RegisterDurationFromLastCashoutPeriodicalHandler),
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
            var all = await _lastMomentRepository.GetAllAsync();
            var now = DateTime.UtcNow;

            foreach (var lastCashout in all)
            {
                await _metricPublishAdapter.PublishGaugeAsync(MetricGaugeType.DurationFromLastFinishSeconds,
                    lastCashout.assetMetricId,
                    MetricOperationType.Cashout,
                    lastCashout.operationId, (now - lastCashout.moment).TotalSeconds);
            }
        }
    }
}
