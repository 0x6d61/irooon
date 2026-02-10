using BenchmarkDotNet.Attributes;
using Irooon.Core;

namespace Irooon.Benchmarks.Macro;

/// <summary>
/// 算術重視ベンチマーク。
/// 複数の算術演算子 (*, +, -, /, %) のスループットを計測。
/// </summary>
[MemoryDiagnoser]
public class ArithmeticScriptBenchmark
{
    private ScriptEngine _engine = null!;

    private const string Script = @"
        var result = 0
        var i = 0
        for (i < 1000) {
            result = (i * 3 + i * 7 - i / 2) % 100
            i = i + 1
        }
        result
    ";

    [GlobalSetup]
    public void Setup()
    {
        _engine = new ScriptEngine();
    }

    [Benchmark(Baseline = true)]
    public double CSharp_ArithHeavy()
    {
        double result = 0;
        for (int i = 0; i < 1000; i++)
            result = (i * 3.0 + i * 7.0 - i / 2.0) % 100;
        return result;
    }

    [Benchmark]
    public object? Irooon_ArithHeavy()
    {
        return _engine.Execute(Script);
    }
}
