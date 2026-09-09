using System.Net.Http.Json;
using LoanPlatform.Scoring.Application.Abstractions;
using LoanPlatform.Scoring.Application.Models.TaxAuthority;
using LoanPlatform.Scoring.Domain.Entities;
using LoanPlatform.Scoring.Domain.ValueObjects;

namespace LoanPlatform.Scoring.Infrastructure.TaxAuthority
{
    public sealed class HttpTaxHistoryProvider : ITaxHistoryProvider
    {
        private readonly HttpClient _httpClient;

        public HttpTaxHistoryProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TaxHistory> GetHistoryAsync(
            ApplicantIdentifier applicantIdentifier,
            CancellationToken cancellationToken)
        {
            TaxHistoryResponse? response =
                await _httpClient.GetFromJsonAsync<TaxHistoryResponse>(
                    $"api/tax-authority/history/{applicantIdentifier.Value}",
                    cancellationToken);

            if (response is null)
            {
                throw new InvalidOperationException(
                    "Tax authority returned an empty response.");
            }

            List<TaxPayment> payments = response.Payments
                .Select(payment => new TaxPayment(
                    payment.Year,
                    payment.Amount))
                .ToList();

            return new TaxHistory(applicantIdentifier, payments);
        }
    }
}