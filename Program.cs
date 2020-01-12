using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;

namespace PerfProber
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new Runner();
            //runner.Sync();
            // Measure(runner.CpuLoad, 1000000);
            Measure("Disk", runner.DiskLoad, 2500);
        }

        public static void Measure(string name, Action<int> action, int iterations)
        {
            action(1); // Warm-up
            var sw = Stopwatch.StartNew();
            action(iterations);
            sw.Stop();
            Console.WriteLine($"<{name}> Elapsed: {sw.Elapsed.TotalSeconds:0.000}");
        }
    }

    class Runner
    {
        public static DateTime GetNistTime()
        {
            using (var response = 
                    WebRequest.Create("http://www.google.com").GetResponse())
                //string todaysDates =  response.Headers["date"];
                return DateTime.ParseExact(response.Headers["date"],
                    "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                    CultureInfo.InvariantCulture.DateTimeFormat,
                    DateTimeStyles.AssumeUniversal);
        }

        private SHA256 sha256 = SHA256.Create();
        private byte[] cpuData, diskLoad;
        public const int N = 100000;

        public Runner()
        {
            cpuData = new byte[N];
            new Random(42).NextBytes(cpuData);
            diskLoad = new byte[64 * 1024 * 1024];
        }
        
        private readonly DateTime waitForDate = new DateTime(2020, 1, 12, 21, 4, 0);

        public void Sync()
        {
            using (WebClient client = new WebClient())
            {
                while (true)
                {
                    var gate = client.DownloadString(
                        "https://raw.githubusercontent.com/AndreyAkinshin/PerfProber/master/gate.txt");
                    if (gate != "1")
                    {
                        Console.WriteLine($"Wait for sync (gate = {gate})");
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            Console.WriteLine("Synced");
        }

        private int holder;

        public void CpuLoad(int m)
        {
            var res = 0;
            for (int i = 0; i < m; i++)
            {
                sha256.ComputeHash(cpuData);
                res += cpuData[0];
            }
            holder = res;
        }

        public void DiskLoad(int m)
        {
            for (int i = 0; i < m; i++)
            {
                var fileName = Path.GetTempFileName();
                File.WriteAllBytes(fileName, diskLoad);
                File.Delete(fileName);
            }
        }
    }
}