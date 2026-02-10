using BenchmarkDotNet.Attributes;
using Irooon.Core.Runtime;

namespace Irooon.Benchmarks.Micro;

/// <summary>
/// 算術演算のオーバーヘッド計測。
/// RuntimeHelpers.Add/Sub/Le vs ネイティブ C# 演算。
/// </summary>
[MemoryDiagnoser]
public class ArithmeticBenchmarks
{
    private ScriptContext _ctx = null!;
    private object _a = null!;
    private object _b = null!;
    private double _da;
    private double _db;

    [GlobalSetup]
    public void Setup()
    {
        _ctx = new ScriptContext();
        _a = (object)10.0;
        _b = (object)5.0;
        _da = 10.0;
        _db = 5.0;
    }

    // --- Baseline: ネイティブ C# ---

    [Benchmark(Baseline = true)]
    public double NativeAdd() => _da + _db;

    [Benchmark]
    public double NativeSub() => _da - _db;

    [Benchmark]
    public bool NativeLe() => _da <= _db;

    // --- irooon RuntimeHelpers ---

    [Benchmark]
    public object IrooonAdd() => RuntimeHelpers.Add(_a, _b, _ctx);

    [Benchmark]
    public object IrooonSub() => RuntimeHelpers.Sub(_a, _b, _ctx);

    [Benchmark]
    public object IrooonLe() => RuntimeHelpers.Le(_a, _b, _ctx);

    // --- 分解計測 ---

    [Benchmark]
    public object IrooonAdd_NoCtx() => RuntimeHelpers.Add(_a, _b);

    [Benchmark]
    public object BoxingRoundtrip()
    {
        double result = (double)_a + (double)_b;
        return (object)result;
    }
}
