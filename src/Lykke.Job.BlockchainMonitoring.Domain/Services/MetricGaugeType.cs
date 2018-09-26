namespace Lykke.Job.BlockchainMonitoring.Domain.Services
{
    public enum MetricGaugeType
    {
        [PrometheusName("amount")]
        Amount,

        [PrometheusName("unfinished_duration_seconds")]
        MaxUnfinishedDurationSeconds,

        [PrometheusName("finished_duration_seconds")]
        FinishedDurationSeconds,

        [PrometheusName("duration_from_last_op_seconds")]
        DurationFromLastFinishSeconds
    }
}
