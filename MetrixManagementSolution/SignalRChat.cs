using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace TestTask
{
    public class SignalRChat : Hub
    {
        public async Task SendMetrics(string message)
        {
            await Clients.All.SendAsync("ReceiveMetrics", message);
        }
    }
}

