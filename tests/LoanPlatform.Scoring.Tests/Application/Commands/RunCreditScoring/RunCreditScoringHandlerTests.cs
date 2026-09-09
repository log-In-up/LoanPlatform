using LoanPlatform.Scoring.Application.Abstractions;
using LoanPlatform.Scoring.Application.Commands.RunCreditScoring;
using LoanPlatform.Scoring.Domain.Entities;
using LoanPlatform.Scoring.Domain.Enums;
using LoanPlatform.Scoring.Domain.Scoring;
using LoanPlatform.Scoring.Domain.ValueObjects;

namespace LoanPlatform.Scoring.Tests.Application.Commands.RunCreditScoring
{
    internal sealed class FakeScoringUnitOfWork : IScoringUnitOfWork
    {
        public bool SaveCalled { get; private set; }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveCalled = true;

            return Task.CompletedTask;
        }
    }
    
    internal sealed class FakeCreditScoreRepository : ICreditScoreRepository
    {
        public CreditScore? CreditScore { get; private set; }

        public Task AddAsync(CreditScore creditScore, CancellationToken cancellationToken)
        {
            CreditScore = creditScore;

            return Task.CompletedTask;
        }

        public Task<CreditScore?> GetByApplicationIdAsync(Guid applicationId, CancellationToken cancellationToken)
        {
            return Task.FromResult(
                CreditScore?.ApplicationId == applicationId
                    ? CreditScore
                    : null);
        }

        public Task SaveAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
    
    internal sealed class FakeCreditApplicationRepository : ICreditApplicationRepository
    {
        public CreditApplication? Application { get; set; }

        public bool SaveCalled { get; private set; }

        public Task<CreditApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Application?.Id == id ? Application : null);
        }

        public Task AddAsync(CreditApplication application, CancellationToken cancellationToken)
        {
            Application = application;

            return Task.CompletedTask;
        }

        public Task SaveAsync(CancellationToken cancellationToken)
        {
            SaveCalled = true;

            return Task.CompletedTask;
        }
    }

    internal sealed class FakeTaxHistoryProvider : ITaxHistoryProvider
    {
        private readonly TaxHistory _history;

        public FakeTaxHistoryProvider(TaxHistory history)
        {
            _history = history;
        }

        public Task<TaxHistory> GetHistoryAsync(ApplicantIdentifier applicantIdentifier, CancellationToken cancellationToken)
        {
            return Task.FromResult(_history);
        }
    }
    
    internal sealed class FailingTaxHistoryProvider : ITaxHistoryProvider
    {
        public Task<TaxHistory> GetHistoryAsync(ApplicantIdentifier applicantIdentifier, CancellationToken cancellationToken)
        {
            throw new HttpRequestException("Government service is unavailable.");
        }
    }
    
    public class RunCreditScoringHandlerTests
    {
        [Fact]
        public async Task Handle_WithValidApplication_CompletesScoring()
        {
            // Arrange
            ApplicantIdentifier identifier =
                new ApplicantIdentifier("123456789012");

            CreditApplication application =
                new CreditApplication(
                    identifier,
                    ApplicantType.IndividualEntrepreneur,
                    10_000_000m,
                    24);

            TaxHistory taxHistory =
                new TaxHistory(
                    identifier,
                    [
                        new TaxPayment(2023, 8_000_000m),
                        new TaxPayment(2024, 9_000_000m),
                        new TaxPayment(2025, 10_000_000m)
                    ]);

            FakeCreditApplicationRepository repository =
                new FakeCreditApplicationRepository
                {
                    Application = application
                };

            FakeTaxHistoryProvider taxProvider = new FakeTaxHistoryProvider(taxHistory);

            CreditScoringCalculator calculator = new CreditScoringCalculator();

            FakeCreditScoreRepository scoreRepository = new FakeCreditScoreRepository();

            FakeScoringUnitOfWork unitOfWork = new FakeScoringUnitOfWork();

            RunCreditScoringHandler handler = new RunCreditScoringHandler(
                    repository,
                    scoreRepository,
                    taxProvider,
                    calculator,
                    unitOfWork);

            RunCreditScoringCommand command = new RunCreditScoringCommand(application.Id);

            // Act
            RunCreditScoringResult result = await handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.Equal(application.Id, result.ApplicationId);

            Assert.InRange(result.Score, 601, 1000);

            Assert.Equal(CreditDecision.PreApproved, result.Decision);

            Assert.Equal(CreditApplicationStatus.Scored, application.Status);

            Assert.True(unitOfWork.SaveCalled);

            Assert.NotNull(scoreRepository.CreditScore);

            Assert.Equal(application.Id, scoreRepository.CreditScore.ApplicationId);

            Assert.Equal(result.Score, scoreRepository.CreditScore.Score);

            Assert.Equal(result.Decision, scoreRepository.CreditScore.Decision);
        }
        
        [Fact]
        public async Task Handle_WhenApplicationDoesNotExist_Throws()
        {
            // Arrange
            FakeCreditApplicationRepository repository = new FakeCreditApplicationRepository();

            ApplicantIdentifier identifier = new ApplicantIdentifier("123456789012");

            TaxHistory taxHistory =
                new TaxHistory(
                    identifier,
                    [
                        new TaxPayment(2025, 1_000_000m)
                    ]);

            FakeTaxHistoryProvider taxProvider = new FakeTaxHistoryProvider(taxHistory);

            CreditScoringCalculator calculator = new CreditScoringCalculator();

            FakeCreditScoreRepository scoreRepository = new FakeCreditScoreRepository();

            FakeScoringUnitOfWork unitOfWork = new FakeScoringUnitOfWork();

            RunCreditScoringHandler handler = new RunCreditScoringHandler(
                    repository,
                    scoreRepository,
                    taxProvider,
                    calculator,
                    unitOfWork);

            RunCreditScoringCommand command = new RunCreditScoringCommand(Guid.NewGuid());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => handler.HandleAsync(
                    command,
                    CancellationToken.None));
        }
        
        [Fact]
        public async Task Handle_WhenTaxProviderFails_MarksApplicationAsFailed()
        {
            // Arrange
            ApplicantIdentifier identifier = new ApplicantIdentifier("123456789012");

            CreditApplication application =
                new CreditApplication(
                    identifier,
                    ApplicantType.IndividualEntrepreneur,
                    10_000_000m,
                    24);

            FakeCreditApplicationRepository repository =
                new FakeCreditApplicationRepository
                {
                    Application = application
                };

            FailingTaxHistoryProvider taxProvider = new FailingTaxHistoryProvider();

            CreditScoringCalculator calculator = new CreditScoringCalculator();

            FakeCreditScoreRepository scoreRepository = new FakeCreditScoreRepository();

            FakeScoringUnitOfWork unitOfWork = new FakeScoringUnitOfWork();

            RunCreditScoringHandler handler = new RunCreditScoringHandler(
                    repository,
                    scoreRepository,
                    taxProvider,
                    calculator,
                    unitOfWork);

            RunCreditScoringCommand command =
                new RunCreditScoringCommand(
                    application.Id);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(
                () => handler.HandleAsync(
                    command,
                    CancellationToken.None));

            Assert.Equal(
                CreditApplicationStatus.Failed,
                application.Status);

            Assert.True(unitOfWork.SaveCalled);
        }
        
        [Fact]
        public async Task Handle_WhenApplicationAlreadyScored_Throws()
        {
            // Arrange
            ApplicantIdentifier identifier = new ApplicantIdentifier("123456789012");

            CreditApplication application = new CreditApplication(
                    identifier,
                    ApplicantType.IndividualEntrepreneur,
                    10_000_000m,
                    24);

            application.StartScoring();

            application.CompleteScoring(CreditDecision.PreApproved);

            FakeCreditApplicationRepository repository =
                new FakeCreditApplicationRepository
                {
                    Application = application
                };

            TaxHistory taxHistory =
                new TaxHistory(
                    identifier,
                    [
                        new TaxPayment(2025, 1_000_000m)
                    ]);
            FakeCreditScoreRepository scoreRepository =
                new FakeCreditScoreRepository();

            FakeScoringUnitOfWork unitOfWork =
                new FakeScoringUnitOfWork();

            RunCreditScoringHandler handler =
                new RunCreditScoringHandler(
                    repository,
                    scoreRepository,
                    new FakeTaxHistoryProvider(taxHistory),
                    new CreditScoringCalculator(),
                    unitOfWork);

            RunCreditScoringCommand command = new RunCreditScoringCommand(application.Id);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => handler.HandleAsync(
                    command,
                    CancellationToken.None));
        }
    }
}
