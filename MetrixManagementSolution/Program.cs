using Domain.Domain;
using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Models;
using Repository;
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
            var app = builder.Build();

            // void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            //{
            //    if (env.IsDevelopment())
            //    {
            //        app.UseDeveloperExceptionPage();
            //    }
            //    app.UseHttpsRedirection();
            //    app.UseRouting();
            //}

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Metrix API V1");
                });
            }

            app.MapHub<SignalRChat>("/chat");

            app.MapControllers();

            //var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
            //var hubContext = app.Services.GetRequiredService<IHubContext<SignalRChat>>();

            app.Run();
        }
    }
}
