using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanPlatform.Scoring.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialScoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "credit_applications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicantIdentifier = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    ApplicantType = table.Column<int>(type: "integer", nullable: false),
                    RequestedAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    RequestedTermMonths = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Decision = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_credit_applications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CreditScore",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Decision = table.Column<int>(type: "integer", nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditScore", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_credit_applications_applicant_identifier",
                table: "credit_applications",
                column: "ApplicantIdentifier");

            migrationBuilder.CreateIndex(
                name: "ix_credit_scores_application_id",
                table: "CreditScore",
                column: "ApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "credit_applications");

            migrationBuilder.DropTable(
                name: "CreditScore");
        }
    }
}
