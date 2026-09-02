using LoanPlatform.Scoring.Domain.Entities;
using LoanPlatform.Scoring.Domain.ValueObjects;

namespace LoanPlatform.Scoring.Application.Abstractions
{
    public interface ITaxHistoryProvider
    {
        Task<TaxHistory> GetHistoryAsync(ApplicantIdentifier applicantIdentifier, CancellationToken cancellationToken);
    }
}