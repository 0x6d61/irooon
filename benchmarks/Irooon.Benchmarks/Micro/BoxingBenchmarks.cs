using BenchmarkDotNet.Attributes;

namespace Irooon.Benchmarks.Micro;

/// <summary>
/// double→object ボクシングコスト分離計測。
/// </summary>
[MemoryDiagnoser]
public class BoxingBenchmarks
{
    private object _boxedA = null!;
    private object _boxedB = null!;

    [GlobalSetup]
    public void Setup()
    {
        _boxedA = (object)10.0;
        _boxedB = (object)5.0;
    }

    [Benchmark(Baseline = true)]
    public double NativeArithmetic()
    {
        double a = 10.0, b = 5.0;
        return a - b;
    }

    [Benchmark]
    public object BoxedArithmetic()
    {
        return (object)((double)_boxedA - (double)_boxedB);
    }

    [Benchmark]
    public object ConvertToDoubleArithmetic()
    {
        return (object)(Convert.ToDouble(_boxedA) - Convert.ToDouble(_boxedB));
    }
}
