using Lykke.Job.BlockchainMonitoring.Settings.JobSettings;
using Lykke.Job.BlockchainMonitoring.Settings.SlackNotifications;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.BlockchainMonitoring.Settings
{
    public class AppSettings
    {
        public BlockchainMonitoringJobSettings BlockchainMonitoringJob { get; set; }

        public SlackNotificationsSettings SlackNotifications { get; set; }

        [Optional]
        public MonitoringServiceClientSettings MonitoringServiceClient { get; set; }
    }
}
