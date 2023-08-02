using System;
using Microsoft.AspNetCore.SignalR.Client;


namespace SignalRConsoleApp
{
    public class Program
    {
        private static async Task Main(string[] args)
        {

            var connection = new HubConnectionBuilder()
            .WithUrl(new Uri("http://127.0.0.1:8088/chat"))
            .WithAutomaticReconnect(new[]
            { TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromSeconds(10) }
            )
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
        }
    }
}
