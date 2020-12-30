using BenchmarkDotNet.Running;
using System;

namespace ProtobufUUIDBench
{
    class Program
    {
        static void Main(string[] args)
        {
            _ = BenchmarkRunner.Run<StrBinBench>();
        }
    }
}
