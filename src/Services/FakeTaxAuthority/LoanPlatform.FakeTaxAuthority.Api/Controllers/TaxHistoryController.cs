using LoanPlatform.FakeTaxAuthority.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace LoanPlatform.FakeTaxAuthority.Api.Controllers
{
    [ApiController]
    [Route("api/tax-authority")]
    public sealed class TaxHistoryController : ControllerBase
    {
        [HttpGet("history/{applicantIdentifier}")]
        public ActionResult<TaxHistoryResponse> GetHistory(
            string applicantIdentifier)
        {
            if (string.IsNullOrWhiteSpace(applicantIdentifier))
            {
                return BadRequest();
            }

            TaxHistoryResponse response = new(
                applicantIdentifier,
                new List<TaxPaymentResponse>
                {
                    new(2023, 1_200_000m),
                    new(2024, 1_500_000m),
                    new(2025, 1_800_000m)
                });

            return Ok(response);
        }
    }
}