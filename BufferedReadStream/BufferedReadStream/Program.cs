using BenchmarkDotNet.Running;
using System.Reflection;

namespace Benchmarks.IO
{
    class Program
    {
        static void Main(string[] args)
        {
            new BenchmarkSwitcher(typeof(Program).GetTypeInfo().Assembly).Run(args);
        }
    }
}
