using LoanPlatform.Scoring.Application.Abstractions;
using LoanPlatform.Scoring.Application.Queries.GetCreditScore;
using LoanPlatform.Scoring.Domain.Entities;
using LoanPlatform.Scoring.Domain.Enums;

namespace LoanPlatform.Scoring.Tests.Application.Queries.GetCreditScore
{
    public sealed class GetCreditScoreHandlerTests
    {
        [Fact]
        public async Task Handle_WhenScoreExists_ReturnsResult()
        {
            // Arrange
            Guid applicationId = Guid.NewGuid();
    
            CreditScore creditScore = new(applicationId, 631, CreditDecision.PreApproved);
    
            FakeCreditScoreRepository repository = new(creditScore);
    
            GetCreditScoreHandler handler = new(repository);
    
            GetCreditScoreQuery query = new(applicationId);
    
            // Act
            GetCreditScoreResult? result = await handler.HandleAsync(query, CancellationToken.None);
    
            // Assert
            Assert.NotNull(result);
            Assert.Equal(applicationId, result.ApplicationId);
            Assert.Equal(631, result.Score);
            Assert.Equal(CreditDecision.PreApproved, result.Decision);
    
            Assert.Equal(creditScore.CalculatedAt, result.CalculatedAt);
        }

        [Fact]
        public async Task Handle_WhenScoreDoesNotExist_ReturnsNull()
        {
            // Arrange
            FakeCreditScoreRepository repository = new(null);
    
            GetCreditScoreHandler handler = new(repository);
    
            GetCreditScoreQuery query = new(Guid.NewGuid());
    
            // Act
            GetCreditScoreResult? result = await handler.HandleAsync(query, CancellationToken.None);
    
            // Assert
            Assert.Null(result);
        }
    }
    
    internal sealed class FakeCreditScoreRepository : ICreditScoreRepository
    {
        private readonly CreditScore? _creditScore;

        public FakeCreditScoreRepository(CreditScore? creditScore)
        {
            _creditScore = creditScore;
        }

        public Task AddAsync(CreditScore creditScore, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<CreditScore?> GetByApplicationIdAsync(Guid applicationId, CancellationToken cancellationToken)
        {
            CreditScore? result = _creditScore?.ApplicationId == applicationId
                ? _creditScore
                : null;

            return Task.FromResult(result);
        }

        public Task SaveAsync(CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
}