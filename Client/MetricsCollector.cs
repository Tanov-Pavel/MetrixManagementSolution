using Client;
using Client.GetMetrix;
using Client.Metrix;
using DTO.DTO;
using Microsoft.AspNetCore.SignalR.Client;
using System.Runtime.InteropServices;
using System.Text.Json;

WMetrics wMetrics = new WMetrics();
LinuxMetrics linuxMetrics = new LinuxMetrics();

short time = 5000;

var connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:8088/chat")
            .Build();
connection.StartAsync().Wait();

CreateMetricDto createMetricDto = new CreateMetricDto();

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    while (true)
    {
        Console.WriteLine("Windows");
        createMetricDto = wMetrics.GetMetrix();
        Console.WriteLine();
        Thread.Sleep(5000);
        var jsonmetric = JsonSerializer.Serialize(createMetricDto);
        Console.WriteLine(jsonmetric);
        connection.SendAsync("ReceiveMetrics", jsonmetric);

        connection.InvokeCoreAsync("SendMetrics", args: new[]
        { jsonmetric });

        connection.On<string>("ReceiveMessage", (message) =>
        {
            Console.WriteLine(message);
        });
        Console.ReadKey();
    }
}
else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    while (true)
    {
        //Console.WriteLine("Linux");
        //createMetricDto.GetMetrix();
        //Console.WriteLine();
        //Thread.Sleep(5000);
        var jsonmetric = JsonSerializer.Serialize(createMetricDto);
        Console.WriteLine(jsonmetric);
        connection.SendAsync("ReceiveMetrics", jsonmetric);

        connection.InvokeCoreAsync("SendMetrics", args: new[]
        { jsonmetric });

        connection.On<string>("ReceiveMessage", (message) =>
        {
            Console.WriteLine(message);
        });
        Console.ReadKey();
    }

    

    
}
