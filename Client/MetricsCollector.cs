using Client;
using Client.Metrix;
using System.Runtime.InteropServices;

WMetrix wMetrix = new WMetrix();
LinuxMetrix linuxMetrix = new LinuxMetrix();
short time = 5000;

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    while (true)
    {
        wMetrix.GetMetrix();
        Console.WriteLine();
        Thread.Sleep(time);
        Console.Clear();

        Console.WriteLine(TestGetMetrix.getUnixCpu());
        // Console.WriteLine(test.getUnixMemory());
        Console.WriteLine("Winsows");
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



        Console.WriteLine(TestGetMetrix.getUnixCpu());
        //Console.WriteLine(test.getUnixMemory());
        //Thread.Sleep(200);
        //Console.Clear();
    }
}