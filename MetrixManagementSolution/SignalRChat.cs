using Microsoft.AspNetCore.SignalR;
using DTO.DTO;
using Domain.Domain;
using Repository;
using System.Text.Json;
using Repository.Repositories.Interfaces;

namespace TestTask
{
    public class SignalRChat : Hub
    {
        int Time = 0;
        private readonly IMetricsRepository _metricRepository;
        private readonly IDiskRepository _diskRepository;

        public SignalRChat(IMetricsRepository metricRepository, IDiskRepository diskRepository)
        {
            _metricRepository = metricRepository;
            _diskRepository = diskRepository;
        }

        public async Task SendMetrics(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", Time);
            try
            {
                // Deserialize the message to a DTO
                var metrics = JsonSerializer.Deserialize<CreateMetricDto>(message);

                // Validate the DTO before creating the metric
                if (metrics != null && ValidateMetrics(metrics))
                {
                    var metric = new Metrics
                    {
                        ip_address = metrics.ip_address,
                        cpu = metrics.cpu,
                        ram_free = metrics.ram_free,
                        ram_total = metrics.ram_total,
                    };
                    // Create the metric using the repository
                    var objec = _metricRepository.GetAll()
                   .Where(x => x.ip_address == metrics.ip_address)
                   .FirstOrDefault();

                    if (objec == null)
                    {
                        _metricRepository.Create(metric);
                    }
                    else
                    {
                        _metricRepository.Update(metric, objec.ip_address);
                    }

                    var alldisks = _diskRepository.GetAll().ToList()
                        ?? throw new Exception();
                    foreach (var disk in metrics.disk_Spaces)
                    {
                        
                           var check =alldisks.Where(x => x.ip_address == disk.ip_address && x.name == disk.name)
                            .FirstOrDefault();
                        if (check == null)
                        {
                            _diskRepository.Create(disk);
                        }
                        else
                        {
                            _diskRepository.Update(disk, objec.ip_address);
                        }
                    }
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");
            }
        }
        // Example of a simple validation method
        private bool ValidateMetrics(CreateMetricDto metrics)
        {
            // Perform necessary validation checks on the metrics DTO
            return !string.IsNullOrWhiteSpace(metrics?.ip_address) && metrics.cpu >= 0 && metrics.ram_free >= 0 && metrics.ram_total >= 0;
        }

    }
}
