using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainMonitoring.Domain.Repositories;
using Lykke.Job.BlockchainMonitoring.Workflow.Commands.Cashout;

namespace Lykke.Job.BlockchainMonitoring.Workflow.CommandHandlers.Cashout
{
    public class SetActiveOperationCommandHandler
    {
        private readonly IChaosKitty _chaosKitty;
        private readonly IActiveOperationsRepository _activeOperationsRepository;

        public SetActiveOperationCommandHandler(IChaosKitty chaosKitty,
            IActiveOperationsRepository activeOperationsRepository)
        {
            _chaosKitty = chaosKitty;
            _activeOperationsRepository = activeOperationsRepository;
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(SetActiveOperationCommand command, IEventPublisher publisher)
        {
            await _activeOperationsRepository.InsertAsync(command.OperationId, command.AssetId, command.StartedAt);

            _chaosKitty.Meow(command.OperationId);

            return CommandHandlingResult.Ok();
        }
    }
}
