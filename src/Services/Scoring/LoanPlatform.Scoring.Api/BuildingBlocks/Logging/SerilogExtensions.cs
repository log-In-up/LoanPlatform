using Serilog;

namespace LoanPlatform.Scoring.Api.BuildingBlocks.Logging
{
    public static class SerilogExtensions
    {
        public static IHostBuilder AddSerilogLogging(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseSerilog((context, configuration) =>
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .WriteTo.Console());
        }
    }
}