namespace Lykke.Job.BlockchainMonitoring.Domain.Services
{
    public enum MetricCounterType
    {
        [PrometheusName("fail_total")]
        Fail,

        [PrometheusName("completed_total")]
        Completed
    }
}
