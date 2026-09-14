using LoanPlatform.Notification.Api.DependencyInjection;

namespace LoanPlatform.Notification.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthorization();

            builder.Services.AddNotificationServices(builder.Configuration);

            builder.Services.AddOpenApi();

            WebApplication app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.Run();
        }
    }
}