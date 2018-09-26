using System;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.BlockchainMonitoring.Settings.JobSettings
{
    public class BlockchainMonitoringJobSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public DbSettings Db { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public CqrsSettings Cqrs { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string PrometheusPushGatewayUrl { get; set; }

        [Optional]
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public ChaosSettings ChaosKitty { get; set; }
        
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public ActiveOperationsSettings ActiveOperations { get; set; }
    }
}
