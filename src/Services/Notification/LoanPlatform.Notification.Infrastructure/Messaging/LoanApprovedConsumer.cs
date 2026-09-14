using System.Text.Json;
using Confluent.Kafka;
using LoanPlatform.Contracts.Events;
using LoanPlatform.Notification.Application.Notifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LoanPlatform.Notification.Infrastructure.Messaging
{
    public sealed class LoanApprovedConsumer : BackgroundService
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<LoanApprovedConsumer> _logger;
        private readonly string _topicName;
        
        public LoanApprovedConsumer(
            IConsumer<Ignore, string> consumer,
            IServiceScopeFactory scopeFactory,
            ILogger<LoanApprovedConsumer> logger,
            IConfiguration configuration)
        {
            _consumer = consumer;
            _scopeFactory = scopeFactory;
            _logger = logger;

            _topicName = configuration["Kafka:LoanApprovedTopic"]
                         ?? throw new InvalidOperationException(
                             "Kafka:LoanApprovedTopic configuration is missing.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(_topicName);

            _logger.LogInformation(
                "Kafka consumer started. Topic: {Topic}",
                _topicName);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    ConsumeResult<Ignore, string>? result = _consumer.Consume(stoppingToken);

                    LoanApprovedEvent? @event = JsonSerializer.Deserialize<LoanApprovedEvent>(result.Message.Value);

                    if (@event is null)
                    {
                        _logger.LogWarning("Received invalid LoanApprovedEvent message.");

                        continue;
                    }

                    using IServiceScope scope = _scopeFactory.CreateScope();

                    ILoanApprovedNotificationHandler handler =
                        scope.ServiceProvider
                            .GetRequiredService<ILoanApprovedNotificationHandler>();

                    await handler.HandleAsync(@event, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Kafka consumer stopping.");
            }
            finally
            {
                _consumer.Close();
            }
        }
    }
}