using Microsoft.AspNetCore.SignalR;
using DTO.DTO;
using Domain.Domain;
using Repository;
using System.Text.Json;


namespace TestTask
{
    public class SignalRChat : Hub
    {
        //private readonly IRepository<Metrics> repository;

        //public SignalRChat(IRepository<Metrics> metricRepository)
        //{
        //    repository = metricRepository;
        //}

        public async Task SendMetrics(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
            Console.WriteLine(message);

            //var metrics = JsonSerializer.Deserialize<CreateMetricDto>(message);

            //var metric = new Metrics
            //{
            //    ip_address = metrics.ip_address,
            //    cpu = metrics.cpu,
            //    ram_free = metrics.ram_free,
            //    ram_total = metrics.ram_total
            //};

            //repository.Create(metric);
            //}
        }
    }
    }

