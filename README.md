# LoanPlatform

LoanPlatform is a student web application for processing loan applications using a microservice architecture.

The project demonstrates a distributed loan-processing system built with ASP.NET Core 9, Entity Framework Core, PostgreSQL, Apache Kafka, Redis, YARP, JWT authentication, Serilog, Docker and Docker Compose.

## Architecture

```text
                              ┌───────────────────────┐
                              │      API Gateway      │
                              │       YARP + JWT      │
                              └───────────┬───────────┘
                                          │
                    ┌─────────────────────┼─────────────────────┐
                    │                     │                     │
                    ▼                     ▼                     ▼
          ┌─────────────────┐   ┌─────────────────┐   ┌─────────────────┐
          │ Scoring Service │   │  Loan Core      │   │ Notification    │
          │                 │   │  Service        │   │ Service         │
          └────────┬────────┘   └────────┬────────┘   └────────┬────────┘
                   │                     │                     │
                   ▼                     ▼                     │
          ┌─────────────────┐   ┌─────────────────┐            │
          │  PostgreSQL     │   │  PostgreSQL     │            │
          │  Scoring DB     │   │  Loan Core DB   │            │
          └─────────────────┘   └─────────────────┘            │
                                                               │
                                             ┌─────────────────▼──────────┐
                                             │           Kafka             │
                                             │       loan-approved        │
                                             └────────────────────────────┘

          ┌───────────────────────┐
          │  Fake Tax Authority   │
          │        API            │
          └───────────▲───────────┘
                      │
                      │ HttpClient
                      │
             ┌────────┴─────────┐
             │ Scoring Service │
             └─────────────────┘

                       ┌───────────────┐
                       │     Redis     │
                       │ Scoring Cache │
                       └───────────────┘
```

## Services

### API Gateway

The API Gateway is the single entry point for client requests.

Responsibilities:

- JWT authentication
- JWT token generation for development/testing
- protected API endpoints
- reverse proxy routing using YARP
- routing requests to Scoring and Loan Core services

Configured routes include:

```text
/api/auth/*
/api/scoring/*
/api/loans/*
/api/payments/*
```

### Scoring Service

The Scoring Service is responsible for creating credit applications and calculating credit scores.

Responsibilities:

- create credit applications
- retrieve credit applications
- calculate credit scores
- retrieve calculated scores
- communicate with the Fake Tax Authority API
- store scoring data in PostgreSQL
- cache scoring results in Redis

A score greater than `600` results in a preliminary approval decision.

### Loan Core Service

The Loan Core Service manages the loan lifecycle and financial operations.

Responsibilities:

- create loans
- retrieve loans
- approve loans
- reject loans
- activate loans
- close loans
- calculate repayment schedules
- register payments
- retrieve individual payments
- retrieve payments for a loan
- track paid and outstanding principal
- perform database operations using Entity Framework Core

The service uses PostgreSQL and database transactions for financial operations.

### Notification Service

The Notification Service is responsible for processing loan approval events.

Responsibilities:

- consume loan approval events from Kafka
- process `loan-approved` messages
- log notification information

The Notification Service communicates with Loan Core through Kafka rather than through direct HTTP calls.

### Fake Tax Authority

A lightweight fake external API used by the Scoring Service to simulate integration with an external tax authority.

The service is called through `HttpClient`.

Its purpose is to provide tax/history data required during the scoring process without depending on a real external government API.

## Technologies

| Component | Technology |
|---|---|
| Language | C# 13 |
| Runtime | .NET 9 |
| Web Framework | ASP.NET Core 9 |
| API Gateway | YARP |
| Authentication | JWT Bearer |
| Database | PostgreSQL 18 |
| ORM | Entity Framework Core 9 |
| Architecture | Clean Architecture |
| Application Pattern | CQRS |
| Messaging | Apache Kafka |
| Cache | Redis |
| Logging | Serilog |
| Containers | Docker |
| Orchestration | Docker Compose |
| Testing | xUnit |
| HTTP Integration | HttpClient |
| API Documentation | OpenAPI + Swagger UI |
| Dependency Management | Central Package Management |

## Project Structure

```text
LoanPlatform/
│
├── src/
│   ├── ApiGateway/
│   │   └── LoanPlatform.ApiGateway/
│   │
│   ├── BuildingBlocks/
│   │   └── LoanPlatform.Contracts/
│   │
│   └── Services/
│       │
│       ├── FakeTaxAuthority/
│       │   └── LoanPlatform.FakeTaxAuthority.Api/
│       │
│       ├── LoanCore/
│       │   ├── LoanPlatform.LoanCore.Api/
│       │   ├── LoanPlatform.LoanCore.Application/
│       │   ├── LoanPlatform.LoanCore.Domain/
│       │   └── LoanPlatform.LoanCore.Infrastructure/
│       │
│       ├── Notification/
│       │   ├── LoanPlatform.Notification.Api/
│       │   ├── LoanPlatform.Notification.Application/
│       │   └── LoanPlatform.Notification.Infrastructure/
│       │
│       └── Scoring/
│           ├── LoanPlatform.Scoring.Api/
│           ├── LoanPlatform.Scoring.Application/
│           ├── LoanPlatform.Scoring.Domain/
│           └── LoanPlatform.Scoring.Infrastructure/
│
├── tests/
│   ├── LoanPlatform.LoanCore.Tests/
│   ├── LoanPlatform.Notification.Tests/
│   └── LoanPlatform.Scoring.Tests
│
├── docker-compose.yml
├── .dockerignore
├── .gitignore
├── .env
└── LoanPlatform.sln
```

## Prerequisites

The following tools are required for local development:

- .NET 9 SDK
- Docker Desktop
- Git

The Docker-based setup does not require manually installing PostgreSQL, Kafka or Redis.

## Configuration

Docker Compose uses environment variables for the Gateway JWT configuration.

Create a `.env` file in the repository root:

```env
JWT_KEY=your-secret-key
JWT_ISSUER=LoanPlatform.ApiGateway
JWT_AUDIENCE=LoanPlatform

DEMO_USER_USERNAME=student
DEMO_USER_PASSWORD=your-password
```

The `.env` file is intentionally excluded from Git.

Do not commit real secrets to the repository.

## Running the Application

The complete application stack can be started with Docker Compose.

From the repository root:

```bash
docker compose up -d --build
```

After startup, the API Gateway is available on port `8080`.

Swagger UI for the Scoring API:

```text
http://localhost:8080/api/scoring/swagger/index.html
```

OpenAPI specification:

```text
http://localhost:8080/api/scoring/openapi/v1.json
```

The Swagger/OpenAPI endpoints are publicly accessible through the Gateway, while application endpoints require JWT authentication.

Docker Compose starts:

- API Gateway
- Scoring Service
- Loan Core Service
- Notification Service
- Fake Tax Authority
- Scoring PostgreSQL database
- Loan Core PostgreSQL database
- Redis
- Kafka
- Kafka topic initialization
- database migration containers

The database migrations are executed automatically during startup.

## Stopping the Application

To stop the application:

```bash
docker compose down
```

To completely remove the containers and associated Docker volumes:

```bash
docker compose down -v
```

The `-v` option removes the PostgreSQL and Redis volumes as well.

## Services and Ports

| Service | Host Port |
|---|---:|
| API Gateway | `8080` |
| Kafka | `9092` |
| Redis | `6379` |
| Scoring PostgreSQL | `5433` |
| Loan Core PostgreSQL | `5434` |

Internal Docker service communication uses Docker service names and port `8080` for the ASP.NET Core APIs.

For example:

```text
http://scoring-api:8080
http://loancore-api:8080
http://notification-api:8080
http://fake-tax-authority:8080
```

## Authentication

The API Gateway protects application endpoints using JWT Bearer authentication.

A development token can be requested through:

```http
POST /api/auth/token
Content-Type: application/json
```

Example request:

```json
{
  "username": "student",
  "password": "your-password"
}
```

The response contains an access token:

```json
{
  "accessToken": "..."
}
```

The token must then be supplied in the HTTP `Authorization` header:

```text
Authorization: Bearer <access-token>
```

Requests without a valid JWT token to protected endpoints return:

```text
401 Unauthorized
```

Swagger and OpenAPI documentation endpoints are intentionally excluded from JWT authentication so that the API documentation remains accessible without a token.

Protected API operations are still required to provide a valid JWT.

## API Documentation

The Scoring API provides OpenAPI documentation and Swagger UI.

Swagger UI is available through the API Gateway:

```text
http://localhost:8080/api/scoring/swagger/index.html

The generated OpenAPI document is available at:

http://localhost:8080/api/scoring/openapi/v1.json

The API Gateway exposes these documentation endpoints without requiring JWT authentication.

Application endpoints remain protected by JWT Bearer authentication.

The Swagger UI uses the OpenAPI document exposed through the Gateway and includes the JWT Bearer security scheme. Protected endpoints can therefore be tested directly from Swagger UI after providing a valid access token through the Authorize button.

The documented Scoring endpoints include:

POST /api/scoring/applications
GET  /api/scoring/applications/{id}
POST /api/scoring/applications/{id}/score
GET  /api/scoring/applications/{id}/score

The OpenAPI document is generated by ASP.NET Core and Swagger UI is provided by Swashbuckle.AspNetCore.SwaggerUI.

## Scoring API

All Scoring endpoints are accessed through the API Gateway.

### Create a Credit Application

```http
POST /api/scoring/applications
```

Example:

```json
{
  "applicantIdentifier": "123456789012",
  "applicantType": 1,
  "requestedAmount": 500000,
  "requestedTermMonths": 12
}
```

### Get a Credit Application

```http
GET /api/scoring/applications/{id}
```

### Run Credit Scoring

```http
POST /api/scoring/applications/{id}/score
```

Example response:

```json
{
  "applicationId": "a31f797e-ed44-4c79-9115-8816ca4401e0",
  "score": 841,
  "decision": 2
}
```

### Get Credit Score

```http
GET /api/scoring/applications/{id}/score
```

The Scoring Service stores scoring information in PostgreSQL and uses Redis to cache scoring results.

## Loan Core API

All Loan Core endpoints are accessed through the API Gateway.

### Create Loan

```http
POST /api/loans
```

Example:

```json
{
  "applicantIdentifier": "123456789012",
  "principalAmount": 300000,
  "interestRate": 0.12,
  "termMonths": 12,
  "paymentType": 1
}
```

### Get Loan

```http
GET /api/loans/{id}
```

Example response:

```json
{
  "principalAmount": 300000,
  "paidPrincipalAmount": 0,
  "outstandingPrincipalAmount": 300000,
  "interestRate": 0.12,
  "termMonths": 12,
  "status": "Pending",
  "paymentType": "Differentiated"
}
```

### Approve Loan

```http
POST /api/loans/{id}/approve
```

### Reject Loan

```http
POST /api/loans/{id}/reject
```

### Activate Loan

```http
POST /api/loans/{id}/activate
```

### Close Loan

```http
POST /api/loans/{id}/close
```

### Get Loan Schedule

```http
GET /api/loans/{id}/schedule?firstPaymentDate=2026-10-18
```

The endpoint calculates the repayment schedule based on:

- principal amount
- interest rate
- loan term
- payment type
- first payment date

## Payments API

### Create Payment

```http
POST /api/payments
```

Example:

```json
{
  "loanId": "8255c090-8288-49ee-82c7-764b602317b6",
  "amount": 41729.17,
  "principalAmount": 41666.67,
  "interestAmount": 62.50
}
```

### Get Payment

```http
GET /api/payments/{id}
```

### Get Payments by Loan

```http
GET /api/payments/loan/{loanId}
```

Payments update the loan's paid and outstanding principal amounts.

## Messaging

Loan approval notifications are implemented using Apache Kafka.

The Loan Core Service publishes a loan approval event to:

```text
loan-approved
```

The Notification Service consumes this event.

The services are therefore decoupled:

```text
Loan Core
    │
    │ loan-approved event
    ▼
 Kafka
    │
    ▼
Notification Service
```

Example notification log:

```text
[NOTIFICATION] Loan approved.
LoanId: 9abddf6b-fcaa-4ce2-a3b8-ee330b73b646,
Applicant: 123456789012,
Amount: 300000.00
```

Kafka topic creation is automated by the `kafka-init` Docker Compose service.

## Caching

Redis is used by the Scoring Service to cache scoring results.

The cache reduces repeated database and external API operations when the same scoring information is requested multiple times.

Redis runs as part of the Docker Compose environment.

## Logging

Serilog is configured in the Scoring Service for structured application logging.

The configuration includes:

- console logging
- request logging
- application metadata
- log-level overrides
- Entity Framework Core database command logging

HTTP request logging includes information such as:

```text
HTTP method
endpoint
status code
request duration
```

Entity Framework Core informational logging is enabled to assist with database diagnostics.

## Database Migrations

Both PostgreSQL databases use Entity Framework Core migrations.

The project contains dedicated migration containers:

```text
scoring-migrations
loancore-migrations
```

They run automatically during Docker Compose startup.

The migration containers wait for the corresponding PostgreSQL databases to become healthy before applying migrations.

This allows the complete environment to be started from a clean state without manually running EF Core commands.

## Testing

The solution contains automated tests using xUnit.

Run all tests with:

```bash
dotnet test LoanPlatform.sln
```

Current test result:

```text
Total tests: 76
Failed: 0
Passed: 76
Skipped: 0
```

The solution therefore passes all currently implemented automated tests.

## Build

The complete solution can be built with:

```bash
dotnet build LoanPlatform.sln
```

The final build was successfully completed without warnings or errors.

## Dependency Management

NuGet package versions are managed centrally using Central Package Management.

Individual project files contain package references without explicit version numbers. Package versions are maintained in the central package configuration.

This keeps package versions consistent across the solution and simplifies dependency updates.

The repository also contains a PowerShell script for checking outdated packages:

```powershell
.\scripts\Check-OutdatedPackages.ps1
```

The script checks all projects in the solution against the configured NuGet package sources.

The current dependency check reports no outdated packages for the application projects.

Test projects may report newer patch releases for testing-only dependencies such as xunit and coverlet.collector; these packages do not affect the runtime services.

```text
Application projects → no updates
Test projects        → newer patch versions available
```

## End-to-End Workflow

A typical loan-processing workflow is:

```text
1. Client authenticates with API Gateway
                    │
                    ▼
2. Create credit application
                    │
                    ▼
3. Scoring Service retrieves external tax information
                    │
                    ▼
4. Credit score is calculated
                    │
                    ▼
5. Loan application receives a scoring decision
                    │
                    ▼
6. Loan Core creates the loan
                    │
                    ▼
7. Loan is approved
                    │
                    ▼
8. Loan approval event is published to Kafka
                    │
                    ▼
9. Notification Service consumes the event
                    │
                    ▼
10. Notification is logged
                    │
                    ▼
11. Loan can be activated
                    │
                    ▼
12. Repayment schedule is calculated
                    │
                    ▼
13. Payments are registered
                    │
                    ▼
14. Paid and outstanding principal are updated
```

## Docker Startup Verification

The complete Docker environment was tested from a clean state.

The verification procedure included:

```bash
docker compose down -v
docker compose up -d --build
```

The following components successfully started:

- API Gateway
- Scoring API
- Loan Core API
- Notification API
- Fake Tax Authority API
- Kafka
- Redis
- Scoring PostgreSQL
- Loan Core PostgreSQL
- scoring database migrations
- Loan Core database migrations
- Kafka topic initialization

The following end-to-end operations were verified after the clean startup:

- JWT authentication through the Gateway
- Scoring application creation
- Credit scoring
- Loan creation
- Loan approval
- Kafka event publication
- Kafka message consumption by Notification Service
- Swagger UI availability through the API Gateway
- OpenAPI document availability through the API Gateway
- JWT authorization from Swagger UI
- anonymous access to Swagger/OpenAPI documentation
- rejection of protected API requests without a JWT token

The Notification Service successfully processed the approval event:

```text
[NOTIFICATION] Loan approved.
LoanId: 9abddf6b-fcaa-4ce2-a3b8-ee330b73b646,
Applicant: 123456789012,
Amount: 300000.00
```

## Engineering Notes

### Clean Architecture

The services are separated into layers responsible for different concerns:

```text
API
Application
Domain
Infrastructure
```

This keeps business logic independent from external infrastructure where practical.

### CQRS

Application operations are organized around commands and queries.

Examples include:

```text
CreateLoanCommand
ApproveLoanCommand
RejectLoanCommand
ActivateLoanCommand
CreateCreditApplicationCommand
RunCreditScoringCommand
GetLoanQuery
GetCreditApplicationQuery
GetCreditScoreQuery
```

### Database Isolation

Scoring and Loan Core use separate PostgreSQL databases.

This keeps service persistence isolated and follows the microservice architecture.

### Service Communication

Synchronous communication is used where an immediate response is required, such as the Scoring Service communicating with the Fake Tax Authority API.

Asynchronous communication is used for loan approval notifications through Kafka.

### Containerized Infrastructure

Infrastructure dependencies are included in Docker Compose:

```text
PostgreSQL
Kafka
Redis
```

The application services are also containerized.

The goal is to provide a reproducible development and testing environment.