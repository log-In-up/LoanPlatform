using Confluent.Kafka;
using LoanPlatform.Notification.Application.Notifications;
using LoanPlatform.Notification.Infrastructure.Messaging;

namespace LoanPlatform.Notification.Api.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddNotificationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            string bootstrapServers =
                configuration["Kafka:BootstrapServers"]
                ?? throw new InvalidOperationException(
                    "Kafka:BootstrapServers configuration is missing.");

            services.AddScoped<ILoanApprovedNotificationHandler, LoanApprovedNotificationHandler>();

            services.AddSingleton<IConsumer<Ignore, string>>(_ =>
            {
                ConsumerConfig consumerConfig = new()
                {
                    BootstrapServers = bootstrapServers,
                    GroupId = configuration["Kafka:ConsumerGroup"]
                              ?? "loanplatform-notification",
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };

                return new ConsumerBuilder<Ignore, string>(
                    consumerConfig).Build();
            });

            services.AddHostedService<LoanApprovedConsumer>();

            return services;
        }
    }
}