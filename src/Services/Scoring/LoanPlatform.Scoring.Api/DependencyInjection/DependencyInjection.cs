using LoanPlatform.Scoring.Application.Abstractions;
using LoanPlatform.Scoring.Application.Commands.CreateCreditApplication;
using LoanPlatform.Scoring.Application.Commands.RunCreditScoring;
using LoanPlatform.Scoring.Application.Queries.GetCreditApplication;
using LoanPlatform.Scoring.Application.Queries.GetCreditScore;
using LoanPlatform.Scoring.Domain.Scoring;
using LoanPlatform.Scoring.Infrastructure.Persistence;
using LoanPlatform.Scoring.Infrastructure.Repositories;
using LoanPlatform.Scoring.Infrastructure.TaxAuthority;
using Microsoft.EntityFrameworkCore;

namespace LoanPlatform.Scoring.Api.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddScoringServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ScoringDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("ScoringDatabase")));

        // Repositories
        services.AddScoped<ICreditApplicationRepository, CreditApplicationRepository>();
        services.AddScoped<ICreditScoreRepository, CreditScoreRepository>();
        services.AddScoped<IScoringUnitOfWork, ScoringUnitOfWork>();

        // Domain services
        services.AddScoped<CreditScoringCalculator>();

        // Application handlers
        services.AddScoped<CreateCreditApplicationHandler>();
        services.AddScoped<RunCreditScoringHandler>();
        services.AddScoped<GetCreditApplicationHandler>();
        services.AddScoped<GetCreditScoreHandler>();

        // Tax Authority HTTP client
        services.AddHttpClient<ITaxHistoryProvider, HttpTaxHistoryProvider>(client =>
        {
            client.BaseAddress = new Uri(
                configuration["TaxAuthority:BaseUrl"]
                ?? throw new InvalidOperationException(
                    "TaxAuthority:BaseUrl is not configured."));
        });

        return services;
    }
}
