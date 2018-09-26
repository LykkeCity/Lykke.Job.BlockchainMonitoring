using System;
using JetBrains.Annotations;

namespace Lykke.Job.BlockchainMonitoring.Settings.JobSettings
{
    public class ActiveOperationsSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public TimeSpan OperationAgeToCleanup { get; set; }
        
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public TimeSpan CleanupTimerPeriod { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public TimeSpan RegisterUnifinishedOperationDurationTimerPeriod { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public TimeSpan RegisterDurationFromLastFinishedCashoutTimerPeriod { get; set; }
    }
}
