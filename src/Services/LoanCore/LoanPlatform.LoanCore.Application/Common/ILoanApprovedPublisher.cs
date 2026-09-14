using LoanPlatform.Contracts.Events;

namespace LoanPlatform.LoanCore.Application.Common
{
    public interface ILoanApprovedPublisher
    {
        Task PublishAsync(LoanApprovedEvent @event, CancellationToken cancellationToken = default);
    }
}