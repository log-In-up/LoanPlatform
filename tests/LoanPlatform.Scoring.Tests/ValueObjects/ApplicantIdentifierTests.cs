using LoanPlatform.Scoring.Domain.ValueObjects;

namespace LoanPlatform.Scoring.Tests.ValueObjects
{
    public class ApplicantIdentifierTests
    {
        [Fact]
        public void Constructor_WithValidIdentifier_CreatesValueObject()
        {
            // Arrange
            const string value = "123456789012";

            // Act
            ApplicantIdentifier identifier = new ApplicantIdentifier(value);

            // Assert
            Assert.Equal(value, identifier.Value);
        }
        
        [Fact]
        public void Constructor_WithEmptyIdentifier_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new ApplicantIdentifier(string.Empty));
        }
        
        [Fact]
        public void Constructor_WithWhitespaceIdentifier_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ApplicantIdentifier("            "));
        }
        
        [Theory]
        [InlineData("12345678901")]
        [InlineData("1234567890123")]
        public void Constructor_WithInvalidLength_ThrowsArgumentException(string value)
        {
            Assert.Throws<ArgumentException>(() => new ApplicantIdentifier(value));
        }
        
        [Theory]
        [InlineData("12345678901A")]
        [InlineData("ABCDEFGHIJKL")]
        [InlineData("1234-5678-9012")]
        public void Constructor_WithNonDigitCharacters_ThrowsArgumentException(string value)
        {
            Assert.Throws<ArgumentException>(() => new ApplicantIdentifier(value));
        }
    }
}