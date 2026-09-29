using LoanPlatform.Scoring.Api.BuildingBlocks.Logging;
using LoanPlatform.Scoring.Api.DependencyInjection;
using LoanPlatform.Scoring.Api.OpenApi;
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
            
            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });
            
            builder.Services.AddScoringServices(builder.Configuration);
            builder.Services.AddAuthorization();

            WebApplication application = builder.Build();
            
            application.UseSerilogRequestLogging();
            
            if (builder.Configuration.GetValue<bool>("OpenApi:Enabled"))
            {
                application.MapOpenApi();

                application.UseSwaggerUI(options =>
                {
                    const string url = "/api/scoring/openapi/v1.json";
                    const string name = "LoanPlatform Scoring API v1";

                    options.SwaggerEndpoint(url, name);
                });
            }

            application.UseHttpsRedirection();
            application.UseAuthorization();
            application.MapControllers();

            application.Run();
        }
    }
}