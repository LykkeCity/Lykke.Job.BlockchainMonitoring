﻿using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.BlockchainMonitoring.Settings.JobSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }

        [AzureTableCheck]
        public string DataConnString { get; set; }
    }
}
