using LoanPlatform.Contracts.Events;

namespace LoanPlatform.Notification.Application.Notifications
{
    public interface ILoanApprovedNotificationHandler
    {
        Task HandleAsync(LoanApprovedEvent @event, CancellationToken cancellationToken = default);
    }
}