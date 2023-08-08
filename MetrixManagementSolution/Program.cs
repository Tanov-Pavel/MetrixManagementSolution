using Microsoft.OpenApi.Models;
using Repository.Repositories;
using Repository.Repositories.Interfaces;

namespace TestTask
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Metrics API", Version = "v1" });
            });

            builder.Services.AddSignalR();
            builder.Services.AddScoped<IMetricsRepository, MetricsRepository>();
            builder.Services.AddScoped<IDiskRepository, DiskRepository>();

            var app = builder.Build();

            app.MapHub<SignalRChat>("/chat");
            app.MapControllers();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Metrix API V1");
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
