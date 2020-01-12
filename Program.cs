using System;
using System.Diagnostics;
using System.Security.Cryptography;

namespace PerfProber
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new Runner();
            runner.CpuLoad(1); // Warmup

            var sw = Stopwatch.StartNew();
            runner.CpuLoad(100000);
            sw.Stop();
            Console.WriteLine("Elapsed: {0:0.000}", sw.Elapsed.TotalSeconds);
        }
    }

    class Runner
    {
        private SHA256 sha256 = SHA256.Create();
        private byte[] data;
        public const int N = 100000;

        public Runner()
        {
            data = new byte[N];
            new Random(42).NextBytes(data);
        }

        public void Setup()
        {
            
        }
        
        public int CpuLoad(int m)
        {
            var res = 0;
            for (int i = 0; i < m; i++)
            {
                sha256.ComputeHash(data);
                res += data[0];
            }
            return res;
        }
    }
}