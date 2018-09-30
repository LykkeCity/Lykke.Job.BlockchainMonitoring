using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainMonitoring.Domain.Repositories;
using Lykke.Job.BlockchainMonitoring.Workflow.Commands.Cashout;

namespace Lykke.Job.BlockchainMonitoring.Workflow.CommandHandlers.Cashout
{
    public class SetLastFinishedCashoutMomentCommandHandler
    {
        private readonly IChaosKitty _chaosKitty;
        private readonly ILastCashoutMomentRepository _lastCashoutMomentRepository;

        public SetLastFinishedCashoutMomentCommandHandler(IChaosKitty chaosKitty, 
            ILastCashoutMomentRepository lastCashoutMomentRepository)
        {
            _chaosKitty = chaosKitty;
            _lastCashoutMomentRepository = lastCashoutMomentRepository;
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(SetLastFinishedCashoutMomentCommand command, IEventPublisher publisher)
        {
            await _lastCashoutMomentRepository.SetLastMomentAsync(command.AssetId, 
                command.AssetMetricId,
                command.Finished, 
                command.OperationId);

            _chaosKitty.Meow(command.OperationId);

            return CommandHandlingResult.Ok();
        }
    }
}
