using System.Text.Json;
using Confluent.Kafka;
using LoanPlatform.Contracts.Events;
using LoanPlatform.LoanCore.Application.Common;
using Microsoft.Extensions.Configuration;

namespace LoanPlatform.LoanCore.Infrastructure.Messaging
{
    public sealed class KafkaLoanApprovedPublisher : ILoanApprovedPublisher
    {
        private readonly string _topicName;

        private readonly IProducer<Null, string> _producer;

        public KafkaLoanApprovedPublisher(
            IProducer<Null, string> producer,
            IConfiguration configuration)
        {
            _producer = producer;

            _topicName = configuration["Kafka:LoanApprovedTopic"]
                         ?? throw new InvalidOperationException(
                             "Kafka:LoanApprovedTopic configuration is missing.");
        }

        public async Task PublishAsync(
            LoanApprovedEvent @event,
            CancellationToken cancellationToken = default)
        {
            string message = JsonSerializer.Serialize(@event);

            await _producer.ProduceAsync(
                _topicName,
                new Message<Null, string>
                {
                    Value = message
                },
                cancellationToken);
        }
    }
}