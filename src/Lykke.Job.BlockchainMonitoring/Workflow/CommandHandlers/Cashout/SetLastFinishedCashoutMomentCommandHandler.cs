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
        private readonly ILastFinishedCashoutMomentRepository _lastFinishedCashoutMomentRepository;

        public SetLastFinishedCashoutMomentCommandHandler(IChaosKitty chaosKitty, 
            ILastFinishedCashoutMomentRepository lastFinishedCashoutMomentRepository)
        {
            _chaosKitty = chaosKitty;
            _lastFinishedCashoutMomentRepository = lastFinishedCashoutMomentRepository;
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(SetLastFinishedCashoutMomentCommand command, IEventPublisher publisher)
        {
            await _lastFinishedCashoutMomentRepository.SetLastMomentAsync(command.AssetId, command.Finished, command.OperationId);

            _chaosKitty.Meow(command.OperationId);

            return CommandHandlingResult.Ok();
        }
    }
}
