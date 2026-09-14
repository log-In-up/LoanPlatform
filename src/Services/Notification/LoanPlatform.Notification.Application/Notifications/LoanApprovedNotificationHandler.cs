using LoanPlatform.Contracts.Events;

namespace LoanPlatform.Notification.Application.Notifications;

public sealed class LoanApprovedNotificationHandler : ILoanApprovedNotificationHandler
{
    public Task HandleAsync(LoanApprovedEvent @event, CancellationToken cancellationToken = default)
    {
        Console.WriteLine(
            $"[NOTIFICATION] Loan approved. " +
            $"LoanId: {@event.LoanId}, " +
            $"Applicant: {@event.ApplicantIdentifier}, " +
            $"Amount: {@event.PrincipalAmount}");

        return Task.CompletedTask;
    }
}