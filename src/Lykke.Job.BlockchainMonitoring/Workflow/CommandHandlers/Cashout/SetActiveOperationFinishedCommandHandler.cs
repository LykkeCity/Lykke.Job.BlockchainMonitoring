using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainMonitoring.Domain.Repositories;
using Lykke.Job.BlockchainMonitoring.Workflow.Commands.Cashout;

namespace Lykke.Job.BlockchainMonitoring.Workflow.CommandHandlers.Cashout
{
    public class SetActiveOperationFinishedCommandHandler
    {
        private readonly IChaosKitty _chaosKitty;
        private readonly IActiveOperationsRepository _activeOperationsRepository;

        public SetActiveOperationFinishedCommandHandler(IChaosKitty chaosKitty, IActiveOperationsRepository activeOperationsRepository)
        {
            _chaosKitty = chaosKitty;
            _activeOperationsRepository = activeOperationsRepository;
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(SetActiveOperationFinishedCommand command, IEventPublisher publisher)
        {
            await _activeOperationsRepository.SetFinishedAsync(command.OperationId);

            _chaosKitty.Meow(command.OperationId);

            return CommandHandlingResult.Ok();
        }
    }
}
