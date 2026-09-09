using LoanPlatform.Scoring.Api.Models;
using LoanPlatform.Scoring.Application.Commands.CreateCreditApplication;
using LoanPlatform.Scoring.Application.Commands.RunCreditScoring;
using LoanPlatform.Scoring.Application.Queries.GetCreditApplication;
using Microsoft.AspNetCore.Mvc;
using LoanPlatform.Scoring.Application.Queries.GetCreditScore;

namespace LoanPlatform.Scoring.Api.Controllers
{
    [ApiController]
    [Route("api/scoring/applications")]
    public sealed class CreditApplicationsController : ControllerBase
    {
        private readonly CreateCreditApplicationHandler _createHandler;
        private readonly GetCreditApplicationHandler _getHandler;
        private readonly RunCreditScoringHandler _runScoringHandler;
        private readonly GetCreditScoreHandler _getScoreHandler;

        public CreditApplicationsController(
            CreateCreditApplicationHandler createHandler,
            GetCreditApplicationHandler getHandler,
            RunCreditScoringHandler runScoringHandler,
            GetCreditScoreHandler getScoreHandler)
        {
            _createHandler = createHandler;
            _getHandler = getHandler;
            _runScoringHandler = runScoringHandler;
            _getScoreHandler = getScoreHandler;
        }

        [HttpPost]
        public async Task<ActionResult<CreateCreditApplicationResult>> Create(
            [FromBody] CreateCreditApplicationRequest request,
            CancellationToken cancellationToken)
        {
            CreateCreditApplicationCommand command = new(
                request.ApplicantIdentifier,
                request.ApplicantType,
                request.RequestedAmount,
                request.RequestedTermMonths);

            CreateCreditApplicationResult result =
                await _createHandler.HandleAsync(
                    command,
                    cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.ApplicationId },
                result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<GetCreditApplicationResult>> GetById(Guid id, CancellationToken cancellationToken)
        {
            GetCreditApplicationQuery query = new(id);

            GetCreditApplicationResult? result =
                await _getHandler.HandleAsync(
                    query,
                    cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        
        [HttpPost("{id:guid}/score")]
        public async Task<ActionResult<RunCreditScoringResult>> RunScoring(Guid id, CancellationToken cancellationToken)
        {
            RunCreditScoringCommand command = new(id);

            RunCreditScoringResult result = await _runScoringHandler.HandleAsync(command, cancellationToken);

            return Ok(result);
        }
        
        [HttpGet("{id:guid}/score")]
        public async Task<ActionResult<GetCreditScoreResult>> GetScore(Guid id, CancellationToken cancellationToken)
        {
            GetCreditScoreQuery query = new(id);

            GetCreditScoreResult? result = await _getScoreHandler.HandleAsync(query, cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}