using LoanPlatform.LoanCore.Application.Loans.ApproveLoan;
using LoanPlatform.LoanCore.Application.Loans.CreateLoan;
using LoanPlatform.LoanCore.Application.Loans.GetLoan;
using LoanPlatform.LoanCore.Application.Loans.RejectLoan;
using LoanPlatform.LoanCore.Application.Loans.ActivateLoan;
using LoanPlatform.LoanCore.Application.Loans.CloseLoan;
using LoanPlatform.LoanCore.Application.Loans.Schedule;
using LoanPlatform.LoanCore.Domain.Loans;

namespace LoanPlatform.LoanCore.Api.Endpoints;

public static class LoanEndpoints
{
    public static IEndpointRouteBuilder MapLoanEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/loans", async (
            CreateLoanCommand command,
            CreateLoanHandler handler,
            CancellationToken cancellationToken) =>
        {
            Guid loanId = await handler.HandleAsync(command, cancellationToken);

            return Results.Created($"/api/loans/{loanId}", new
            {
                Id = loanId
            });
        });

        endpoints.MapGet("/api/loans/{id:guid}", async (
            Guid id,
            GetLoanHandler handler,
            CancellationToken cancellationToken) =>
        {
            GetLoanQuery query = new GetLoanQuery(id);

            Loan? loan = await handler.HandleAsync(
                query,
                cancellationToken);

            return loan is null
                ? Results.NotFound()
                : Results.Ok(loan);
        });

        endpoints.MapPost("/api/loans/{id:guid}/approve", async (
            Guid id,
            ApproveLoanHandler handler,
            CancellationToken cancellationToken) =>
        {
            ApproveLoanCommand command = new ApproveLoanCommand(id);

            try
            {
                bool approved = await handler.HandleAsync(command, cancellationToken);

                return approved 
                    ? Results.Ok(new { Id = id, Status = "Approved" }) 
                    : Results.NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { Message = ex.Message });
            }
        });

        endpoints.MapPost("/api/loans/{id:guid}/reject", async (
            Guid id,
            RejectLoanHandler handler,
            CancellationToken cancellationToken) =>
        {
            RejectLoanCommand command = new RejectLoanCommand(id);

            try
            {
                bool rejected = await handler.HandleAsync(command, cancellationToken);

                return rejected 
                    ? Results.Ok(new { Id = id, Status = "Rejected" }) 
                    : Results.NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { Message = ex.Message });
            }
        });
        
        endpoints.MapPost("/api/loans/{id:guid}/activate", async (
            Guid id,
            ActivateLoanHandler handler,
            CancellationToken cancellationToken) =>
        {
            ActivateLoanCommand command = new ActivateLoanCommand(id);

            try
            {
                bool activated = await handler.HandleAsync(command, cancellationToken);

                return activated
                    ? Results.Ok(new { Id = id, Status = "Active" })
                    : Results.NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { Message = ex.Message });
            }
        });
        
        endpoints.MapPost("/api/loans/{id:guid}/close", async (
            Guid id,
            CloseLoanHandler handler,
            CancellationToken cancellationToken) =>
        {
            CloseLoanCommand command = new CloseLoanCommand(id);

            try
            {
                bool closed = await handler.HandleAsync(command, cancellationToken);

                return closed
                    ? Results.Ok(new { Id = id, Status = "Closed" })
                    : Results.NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { Message = ex.Message });
            }
        });

        endpoints.MapGet("/api/loans/{id:guid}/schedule", async (
            Guid id,
            DateTime firstPaymentDate,
            GetLoanHandler getLoanHandler,
            LoanScheduleCalculator scheduleCalculator,
            CancellationToken cancellationToken) =>
        {
            GetLoanQuery query = new GetLoanQuery(id);

            Loan? loan = await getLoanHandler.HandleAsync(query, cancellationToken);

            if (loan is null)
            {
                return Results.NotFound();
            }

            IReadOnlyList<PaymentScheduleItem> schedule =
                scheduleCalculator.Calculate(
                    loan.PrincipalAmount,
                    loan.InterestRate,
                    loan.TermMonths,
                    loan.PaymentType,
                    firstPaymentDate);

            return Results.Ok(schedule);
        });
        
        return endpoints;
    }
}