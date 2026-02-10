using BenchmarkDotNet.Attributes;
using Irooon.Core;

namespace Irooon.Benchmarks.Macro;

/// <summary>
/// fibonacci(30) ベンチマーク — 中程度の再帰（約270万回）。
/// </summary>
[MemoryDiagnoser]
public class FibonacciBenchmark
{
    private ScriptEngine _engine = null!;

    private const string FibScript = @"
        fn fib(n) {
            if (n <= 1) { n }
            else { fib(n - 1) + fib(n - 2) }
        }
        fib(30)
    ";

    [GlobalSetup]
    public void Setup()
    {
        _engine = new ScriptEngine();
    }

    [Benchmark(Baseline = true)]
    public double CSharp_Fib30()
    {
        return Fib(30);
    }

    [Benchmark]
    public object? Irooon_Fib30()
    {
        return _engine.Execute(FibScript);
    }

    private static double Fib(double n)
    {
        if (n <= 1) return n;
        return Fib(n - 1) + Fib(n - 2);
    }
}
