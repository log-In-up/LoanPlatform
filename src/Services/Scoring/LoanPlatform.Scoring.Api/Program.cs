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

namespace LoanPlatform.Scoring.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            // Database
            builder.Services.AddDbContext<ScoringDbContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("ScoringDatabase")));

            // Repositories
            builder.Services.AddScoped<ICreditApplicationRepository, CreditApplicationRepository>();
            builder.Services.AddScoped<ICreditScoreRepository, CreditScoreRepository>();
            builder.Services.AddScoped<IScoringUnitOfWork, ScoringUnitOfWork>();

            // Domain services
            builder.Services.AddScoped<CreditScoringCalculator>();

            // Application handlers
            builder.Services.AddScoped<CreateCreditApplicationHandler>();
            builder.Services.AddScoped<RunCreditScoringHandler>();
            builder.Services.AddScoped<GetCreditApplicationHandler>();
            builder.Services.AddScoped<GetCreditScoreHandler>();

            builder.Services.AddHttpClient<ITaxHistoryProvider, HttpTaxHistoryProvider>(client =>
            {
                client.BaseAddress = new Uri(
                    builder.Configuration["TaxAuthority:BaseUrl"]
                    ?? throw new InvalidOperationException(
                        "TaxAuthority:BaseUrl is not configured."));
            });
            
            builder.Services.AddAuthorization();

            WebApplication application = builder.Build();

            if (application.Environment.IsDevelopment())
            {
                application.MapOpenApi();
            }

            application.UseHttpsRedirection();
            application.UseAuthorization();
            application.MapControllers();
            application.Run();
        }
    }
}