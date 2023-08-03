using Client;
using Client.Metrix;
using Microsoft.AspNetCore.SignalR.Client;
using System.Runtime.InteropServices;

WMetrix wMetrix = new WMetrix();
LinuxMetrix linuxMetrix = new LinuxMetrix();
short time = 5000;

var connection = new HubConnectionBuilder()
           .WithUrl(new Uri("http://127.0.0.1:8088/chat"))
           .WithAutomaticReconnect(new[]
           { TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromSeconds(10)})
           .Build();

connection.On<string, string>("ReceiveMessage", (user, message) =>
{
    Console.WriteLine(user + message);
});

try
{
    await connection.StartAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}

Console.ReadLine();
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    while (true)
    {
        wMetrix.GetMetrix();
        Console.WriteLine();
        Thread.Sleep(time);
        Console.Clear();

        // Отправляем метрики RAM и CPU на сервер SignalR
        await connection.SendAsync("ReceiveMessage", "RAM", wMetrix.GetRamMetrics());
        await connection.SendAsync("ReceiveMessage", "CPU", wMetrix.GetCpuMetrics());

        Console.WriteLine("Windows");
    }
}

else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    while (true)
    {
        linuxMetrix.GetMetrix();
        Console.WriteLine();
        Thread.Sleep(time);
        Console.Clear();

        // Отправляем метрики RAM и CPU на сервер SignalR
       // await connection.SendAsync("ReceiveMessage", "RAM", linuxMetrix.GetRamMetrics());
       // await connection.SendAsync("ReceiveMessage", "CPU", linuxMetrix.GetCpuMetrics());

        Console.WriteLine("Linux");
    }
}
