using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using Client.Metrix;

namespace Client
{
    public class WMetrix : ImetricsRepository
    {
        public static PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        public static PerformanceCounter ramCounterAvailable = new PerformanceCounter("Memory", "Available MBytes");
        public void GetMetrix()
        {
           ObjectQuery winQuery = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(winQuery);

            foreach (ManagementObject item in searcher.Get())
            {
                var memoryKb = Convert.ToInt32(item["TotalVisibleMemorySize"].ToString());
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
        }
    }
}