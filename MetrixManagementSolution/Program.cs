using Microsoft.AspNetCore.SignalR;
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

            var app = builder.Build();

            void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseHttpsRedirection();
                app.UseRouting();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapHub<SignalRChat>("/chat");
                    endpoints.MapControllers();
                });
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Metrix API V1");
                });
            }

            app.MapControllers();

            var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
            var hubContext = app.Services.GetRequiredService<IHubContext<SignalRChat>>();

            hubContext.Clients.All.SendAsync("ReceiveMetrics", "Сервер SignalR запущен и готов к подключениям.");

            lifetime.ApplicationStopping.Register(() =>
            {
                hubContext.Clients.All.SendAsync("ReceiveMetrics", "Сервер SignalR остановлен.");
            });

            app.Run();
        }
    }
}
