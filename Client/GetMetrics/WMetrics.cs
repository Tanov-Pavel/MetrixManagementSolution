using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using Client.Metrix;
using DTO.DTO;
using System.IO;
using Domain.Domain;
using System.Xml.Linq;

namespace Client.GetMetrix
{
    public class WMetrics : ImetricsRepository
    {
        public static PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        public static PerformanceCounter ramCounterAvailable = new PerformanceCounter("Memory", "Available MBytes");

        public CreateMetricDto GetMetrix()
        {
            
            int processorCount = Environment.ProcessorCount;      
            long totalMemory = GetTotalPhysicalMemory();
            long usedMemory = Process.GetProcesses().Sum(x => x.WorkingSet64);
            int availableMemory = (int)ramCounterAvailable.NextValue();
            long freeMemory = totalMemory - usedMemory;

            Console.WriteLine("Processors count: " + processorCount);
            Console.WriteLine("Total RAM: " + totalMemory / (1024 * 1024) + " MB");
            Console.WriteLine("Used memory: " + usedMemory / (1024 * 1024) + " MB");
            Console.WriteLine("Free memory: " + freeMemory / (1024 * 1024) + " MB");
            Console.WriteLine("Available memory: " + availableMemory + " MB");

            DriveInfo[] drives = DriveInfo.GetDrives();
            List<Disk_spaces> disks = new List<Disk_spaces>();
            string ipAddress = GetIPAddress();

            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady)
                {
                    double freeSpaceGB = drive.TotalFreeSpace / (1024.0 * 1024 * 1024);
                    double totalSizeGB = drive.TotalSize / (1024.0 * 1024 * 1024);
                    Console.WriteLine($"Drive {drive.Name}: Free Space {freeSpaceGB:0.00} GB | Total Size {totalSizeGB:0.00} GB");
                    var disk = new Disk_spaces
                    {
                        ip_address = ipAddress,
                        name = drive.Name,
                        free_disk_space = freeSpaceGB,
                        total_disk_space = totalSizeGB,
                    };
                    disks.Add(disk);
                }
            }
            CreateMetricDto metricDto = new CreateMetricDto(
                ipAddress,
                cpuCounter.NextValue(),
                availableMemory,
                freeMemory,
                disks
            );

            return metricDto;
        }

        private long GetTotalPhysicalMemory()
        {
            ObjectQuery query = new ObjectQuery("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                ManagementObjectCollection results = searcher.Get();
                foreach (ManagementObject result in results)
                {
                    return Convert.ToInt64(result["TotalPhysicalMemory"]);
                }
            }
            return 0;
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
