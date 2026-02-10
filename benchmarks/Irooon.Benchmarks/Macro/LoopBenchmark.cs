using BenchmarkDotNet.Attributes;
using Irooon.Core;

namespace Irooon.Benchmarks.Macro;

/// <summary>
/// ループ重視ベンチマーク（再帰なし）。
/// 変数アクセス + 算術のみのオーバーヘッドを計測。
/// </summary>
[MemoryDiagnoser]
public class LoopBenchmark
{
    private ScriptEngine _engine = null!;

    private const string LoopScript = @"
        var sum = 0
        var i = 0
        for (i < 10000) {
            sum = sum + i
            i = i + 1
        }
        sum
    ";

    [GlobalSetup]
    public void Setup()
    {
        _engine = new ScriptEngine();
    }

    [Benchmark(Baseline = true)]
    public double CSharp_Loop10K()
    {
        double sum = 0;
        for (int i = 0; i < 10000; i++)
            sum += i;
        return sum;
    }

    [Benchmark]
    public object? Irooon_Loop10K()
    {
        return _engine.Execute(LoopScript);
    }
}
