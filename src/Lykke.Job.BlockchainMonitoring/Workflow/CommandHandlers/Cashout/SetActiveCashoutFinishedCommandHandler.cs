using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Job.BlockchainMonitoring.Domain.Repositories;
using Lykke.Job.BlockchainMonitoring.Workflow.Commands.Cashout;

namespace Lykke.Job.BlockchainMonitoring.Workflow.CommandHandlers.Cashout
{
    public class SetActiveCashoutFinishedCommandHandler
    {
        private readonly IChaosKitty _chaosKitty;
        private readonly IActiveCashoutRepository _activeCashoutRepository;

        public SetActiveCashoutFinishedCommandHandler(IChaosKitty chaosKitty, IActiveCashoutRepository activeCashoutRepository)
        {
            _chaosKitty = chaosKitty;
            _activeCashoutRepository = activeCashoutRepository;
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(SetActiveCashoutFinishedCommand command, IEventPublisher publisher)
        {
            await _activeCashoutRepository.SetFinishedAsync(command.OperationId);

            _chaosKitty.Meow(command.OperationId);

            return CommandHandlingResult.Ok();
        }
    }
}
