using System;
using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;
using System.Text.Json;
using Client.Metrix;
using DTO.DTO;
using Microsoft.AspNetCore.SignalR.Client;

namespace Client.GetMetrix
{
    public class WMetrics : ImetricsRepository
    {
        public static PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        public static PerformanceCounter ramCounterAvailable = new PerformanceCounter("Memory", "Available MBytes");
        private HubConnection connection;

        public WMetrics(HubConnection hubConnection)
        {
            connection = hubConnection;
        }

        public WMetrics()
        {
        }

        public CreateMetricDto GetMetrix()
        {
            ObjectQuery winQuery = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(winQuery);

            int memoryKb = 0;

            foreach (ManagementObject item in searcher.Get())
            {
                memoryKb = Convert.ToInt32(item["TotalVisibleMemorySize"].ToString());
                Console.WriteLine("Показатели RAM " + "Cвободно " + ramCounterAvailable.NextValue() + " MB" + " | " + "Общий размер " + memoryKb / 1024 + " MB");
            }

            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo Drive in allDrives)
            {
                if (Drive.IsReady == true)
                {
                    Console.WriteLine("Состояния диска " + Drive.Name + " " + "Свободно " + Drive.TotalFreeSpace / 1024 / 1024 / 1024 + " GB" + " | " + "Общий размер " + Drive.TotalSize / 1024 / 1024 / 1024 + " GB");
                }
            }

            Console.WriteLine("Загруженность CPU: " + cpuCounter.NextValue() + " %");

            string ipAddress = GetIPAddress();
            Console.WriteLine("IP-адрес: " + ipAddress);


            CreateMetricDto metricDto = new CreateMetricDto(
                ipAddress,
                cpuCounter.NextValue(),
                ramCounterAvailable.NextValue(),
                ramCounterAvailable.NextValue() + memoryKb / 1024);

            return metricDto;
        }
        private string GetIPAddress()
        {
            string? ipAddress = "";

            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    ipAddress = networkInterface.GetIPProperties().UnicastAddresses
                        .FirstOrDefault(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.Address.ToString();

                    if (!string.IsNullOrEmpty(ipAddress))
                        break;
                }
            }

            return ipAddress;
        }
    }
}
