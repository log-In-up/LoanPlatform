using Confluent.Kafka;
using LoanPlatform.Contracts.Events;
using LoanPlatform.LoanCore.Infrastructure.Messaging;
using Moq;
using Microsoft.Extensions.Configuration;

namespace LoanPlatform.LoanCore.Tests.Messaging
{
    public class KafkaLoanApprovedPublisherTests
    {
        [Fact]
        public async Task PublishAsync_ShouldPublishLoanApprovedEvent()
        {
            // Arrange
            Mock<IProducer<Null, string>> producer = new Mock<IProducer<Null, string>>();

            Message<Null, string> publishedMessage = new Message<Null, string>();

            producer
                .Setup(x => x.ProduceAsync(
                    "loan-approved",
                    It.IsAny<Message<Null, string>>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, Message<Null, string>, CancellationToken>(
                    (_, message, _) => publishedMessage = message)
                .ReturnsAsync(
                    new DeliveryResult<Null, string>
                    {
                        Topic = "loan-approved",
                        Message = publishedMessage
                    });

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Kafka:LoanApprovedTopic"] = "loan-approved"
                })
                .Build();
            
            KafkaLoanApprovedPublisher publisher =
                new KafkaLoanApprovedPublisher(
                    producer.Object,
                    configuration);

            Guid loanId = Guid.NewGuid();

            LoanApprovedEvent @event = new LoanApprovedEvent(
                Guid.NewGuid(),
                loanId,
                "TEST123456",
                100000m,
                DateTime.UtcNow);

            // Act
            await publisher.PublishAsync(@event);

            // Assert
            producer.Verify(
                x => x.ProduceAsync(
                    "loan-approved",
                    It.IsAny<Message<Null, string>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            Assert.NotNull(publishedMessage.Value);
            Assert.Contains(loanId.ToString(), publishedMessage.Value);
            Assert.Contains("TEST123456", publishedMessage.Value);
            Assert.Contains("100000", publishedMessage.Value);
        }
    }
}