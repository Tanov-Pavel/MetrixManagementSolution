using Client.Metrix;
using DTO.DTO;
using System.Net;
using System;

namespace Client.GetMetrix
{
    public class LinuxMetrics : ImetricsRepository
    {
        private static string GetProcInfo(string path)
        {
            try
            {
                using (var streamReader = new StreamReader(path))
                {
                    return streamReader.ReadLine();
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public CreateMetricDto GetMetrix()
        {
            Console.WriteLine("Процессор:");
            double cpuUsage = 0;
            string cpuUsagePath = "/proc/stat";
            string cpuUsageLine = GetProcInfo(cpuUsagePath);
            if (!string.IsNullOrEmpty(cpuUsageLine) && cpuUsageLine.StartsWith("cpu "))
            {
                string[] cpuInfo = cpuUsageLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (cpuInfo.Length >= 8)
                {
                    double user = double.Parse(cpuInfo[1]);
                    double nice = double.Parse(cpuInfo[2]);
                    double system = double.Parse(cpuInfo[3]);
                    double idle = double.Parse(cpuInfo[4]);
                    double iowait = double.Parse(cpuInfo[5]);
                    double irq = double.Parse(cpuInfo[6]);
                    double softirq = double.Parse(cpuInfo[7]);
                    double totalCpu = user + nice + system + idle + iowait + irq + softirq;
                    double idleCpu = idle + iowait;
                    cpuUsage = 100.0 * (totalCpu - idleCpu) / totalCpu;
                }
            }
            Console.WriteLine($"Загруженность CPU: {cpuUsage}%");

            Console.WriteLine("\nПамять:");
            double freeMemory = 0;
            double totalMemory = 0;
            string memoryPath = "/proc/meminfo";
            string[] memoryLines = File.ReadAllLines(memoryPath);
            foreach (var line in memoryLines)
            {
                if (line.StartsWith("MemTotal:"))
                {
                    totalMemory = double.Parse(line.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]);
                }
                else if (line.StartsWith("MemAvailable:"))
                {
                    freeMemory = double.Parse(line.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]);
                }
            }
            Console.WriteLine($"Свободная память: {freeMemory / 1024} MB");
            Console.WriteLine($"Общий размер памяти: {totalMemory / 1024} MB");

            Console.WriteLine("\nДиск:");
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in allDrives)
            {
                if (drive.IsReady)
                {
                    double freeSpaceGB = drive.TotalFreeSpace / (1024.0 * 1024 * 1024);
                    double totalSizeGB = drive.TotalSize / (1024.0 * 1024 * 1024);
                    Console.WriteLine($"Состояние диска {drive.Name}: Свободно {freeSpaceGB:0.00} GB | Общий размер {totalSizeGB:0.00} GB");
                }
            }

            CreateMetricDto metricDto = new CreateMetricDto();
          

            return metricDto;
        }
    }
}

