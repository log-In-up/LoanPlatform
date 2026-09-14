using Confluent.Kafka;
using LoanPlatform.LoanCore.Application.Common;
using LoanPlatform.LoanCore.Application.Loans.ActivateLoan;
using LoanPlatform.LoanCore.Application.Loans.ApproveLoan;
using LoanPlatform.LoanCore.Application.Loans.CloseLoan;
using LoanPlatform.LoanCore.Application.Loans.CreateLoan;
using LoanPlatform.LoanCore.Application.Loans.GetLoan;
using LoanPlatform.LoanCore.Application.Loans.RejectLoan;
using LoanPlatform.LoanCore.Application.Loans.Schedule;
using LoanPlatform.LoanCore.Application.Loans;
using LoanPlatform.LoanCore.Application.Payments.GetPayment;
using LoanPlatform.LoanCore.Application.Payments.GetPaymentsByLoan;
using LoanPlatform.LoanCore.Application.Payments.MakePayment;
using LoanPlatform.LoanCore.Application.Payments;
using LoanPlatform.LoanCore.Infrastructure.Messaging;
using LoanPlatform.LoanCore.Infrastructure.Persistence.Repositories;
using LoanPlatform.LoanCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LoanPlatform.LoanCore.Api.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddLoanCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LoanDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("LoanDatabase")));

        // Repositories
        services.AddScoped<ILoanRepository, LoanRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        services.AddScoped<IUnitOfWork, LoanUnitOfWork>();
        
        // Application handlers
        services.AddScoped<CreateLoanHandler>();
        services.AddScoped<GetLoanHandler>();
        services.AddScoped<ApproveLoanHandler>();
        services.AddScoped<RejectLoanHandler>();
        services.AddScoped<ActivateLoanHandler>();
        services.AddScoped<CloseLoanHandler>();
        services.AddScoped<LoanScheduleCalculator>();
        services.AddScoped<MakePaymentHandler>();
        services.AddScoped<GetPaymentHandler>();
        services.AddScoped<GetPaymentsByLoanHandler>();

        string bootstrapServers =
            configuration["Kafka:BootstrapServers"]
            ?? throw new InvalidOperationException(
                "Kafka:BootstrapServers configuration is missing.");

        services.AddSingleton<IProducer<Null, string>>(_ =>
        {
            ProducerConfig producerConfig = new()
            {
                BootstrapServers = bootstrapServers
            };

            return new ProducerBuilder<Null, string>(producerConfig).Build();
        });

        services.AddScoped<ILoanApprovedPublisher, KafkaLoanApprovedPublisher>();
        
        return services;
    }
}