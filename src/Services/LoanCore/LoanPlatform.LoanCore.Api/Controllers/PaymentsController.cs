using LoanPlatform.LoanCore.Application.Payments.MakePayment;
using LoanPlatform.LoanCore.Application.Payments.GetPayment;
using LoanPlatform.LoanCore.Application.Payments.GetPaymentsByLoan;
using LoanPlatform.LoanCore.Domain.Payments;
using Microsoft.AspNetCore.Mvc;

namespace LoanPlatform.LoanCore.Api.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public sealed class PaymentsController : ControllerBase
    {
        private readonly MakePaymentHandler _handler;
        private readonly GetPaymentHandler _getPaymentHandler;
        private readonly GetPaymentsByLoanHandler _getPaymentsByLoanHandler;
        
        public PaymentsController(
            MakePaymentHandler handler,
            GetPaymentHandler getPaymentHandler,
            GetPaymentsByLoanHandler getPaymentsByLoanHandler)
        {
            _handler = handler;
            _getPaymentHandler = getPaymentHandler;
            _getPaymentsByLoanHandler = getPaymentsByLoanHandler;
        }

        [HttpPost]
        public async Task<IActionResult> MakePayment(
            [FromBody] MakePaymentRequest request,
            CancellationToken cancellationToken)
        {
            MakePaymentCommand command = new MakePaymentCommand(
                request.LoanId,
                request.Amount,
                request.PrincipalAmount,
                request.InterestAmount);

            Guid? paymentId = await _handler.HandleAsync(
                command,
                cancellationToken);

            if (paymentId is null)
            {
                return NotFound();
            }

            return CreatedAtAction(nameof(GetPayment), new { id = paymentId }, new { id = paymentId });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPayment(Guid id, CancellationToken cancellationToken)
        {
            Payment? payment = await _getPaymentHandler.HandleAsync(id, cancellationToken);

            if (payment is null)
            {
                return NotFound();
            }

            return Ok(payment);
        }
        
        [HttpGet("loan/{loanId:guid}")]
        public async Task<IActionResult> GetPaymentsByLoan(
            Guid loanId,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<Payment> payments = await _getPaymentsByLoanHandler.HandleAsync(
                loanId,
                cancellationToken);

            return Ok(payments);
        }
    }
    
    public sealed record MakePaymentRequest(
        Guid LoanId,
        decimal Amount,
        decimal PrincipalAmount,
        decimal InterestAmount);
}