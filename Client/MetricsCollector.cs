using Client;
using Client.GetMetrix;
using Client.Metrix;
using DTO.DTO;
using Microsoft.AspNetCore.SignalR.Client;
using System.Runtime.InteropServices;
using System.Text.Json;

WMetrics wMetrics = new WMetrics();
LinuxMetrics linuxMetrics = new LinuxMetrics();
var Time = 0;
var connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:8088/chat")
            .Build();
connection.StartAsync().Wait();


CreateMetricDto createMetricDto = new CreateMetricDto();

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    

    while (true)
    {
        createMetricDto = wMetrics.GetMetrix();
        var jsonmetric = JsonSerializer.Serialize(createMetricDto);
        await connection.InvokeCoreAsync("SendMetrics", args: new[]
        { jsonmetric });
        connection.On<int>("ReceiveMessage", (time) =>
        {
            Time = time;
        });
        Thread.Sleep(5000);

    }
}
else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    while (true)
    {
        createMetricDto =linuxMetrics.GetMetrix();
        var jsonmetric = JsonSerializer.Serialize(createMetricDto);
        await connection.InvokeCoreAsync("SendMetrics", args: new[]
        { jsonmetric });
        Thread.Sleep(Time);

    }
    
   
}

