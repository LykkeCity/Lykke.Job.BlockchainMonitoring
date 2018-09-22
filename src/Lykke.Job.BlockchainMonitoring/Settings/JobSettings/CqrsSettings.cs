﻿using System;
using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.BlockchainMonitoring.Settings.JobSettings
{
    [UsedImplicitly]
    public class CqrsSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        [AmqpCheck]
        public string RabbitConnectionString { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public TimeSpan RetryDelay { get; set; }
    }
}
