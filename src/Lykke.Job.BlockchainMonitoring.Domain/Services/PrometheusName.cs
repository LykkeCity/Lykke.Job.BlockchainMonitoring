using System;

namespace Lykke.Job.BlockchainMonitoring.Domain.Services
{
    [AttributeUsage(AttributeTargets.All)]
    public class PrometheusName: Attribute
    {
        public string Name { get; }

        public PrometheusName(string name)
        {
            Name = name;
        }
    }
}
