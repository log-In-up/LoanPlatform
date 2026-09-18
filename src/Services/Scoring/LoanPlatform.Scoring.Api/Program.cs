using LoanPlatform.Scoring.Api.BuildingBlocks.Logging;
using LoanPlatform.Scoring.Api.DependencyInjection;
using Serilog;

namespace LoanPlatform.Scoring.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Host.AddSerilogLogging();
            
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddScoringServices(builder.Configuration);
            builder.Services.AddAuthorization();

            WebApplication application = builder.Build();
            
            application.UseSerilogRequestLogging();
            
            if (application.Environment.IsDevelopment())
            {
                application.MapOpenApi();
            }

            application.UseHttpsRedirection();
            application.UseAuthorization();
            application.MapControllers();

            application.Run();
        }
    }
}