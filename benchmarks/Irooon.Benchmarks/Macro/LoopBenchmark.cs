using BenchmarkDotNet.Attributes;
using Irooon.Core;
using Irooon.Core.Runtime;
using Irooon.Benchmarks.Helpers;

namespace Irooon.Benchmarks.Macro;

/// <summary>
/// ループ重視ベンチマーク（再帰なし）。
/// 変数アクセス + 算術のみのオーバーヘッドを計測。
/// FullPipeline vs PreCompiled でコンパイルコスト vs ランタイムコストを分離。
/// </summary>
[MemoryDiagnoser]
public class LoopBenchmark
{
    private ScriptEngine _engine = null!;
    private Func<ScriptContext, object?> _precompiled = null!;

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
        (_precompiled, _) = BenchmarkHelper.PreCompile(LoopScript);
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

    [Benchmark]
    public object? Irooon_Loop10K_PreCompiled()
    {
        var ctx = new ScriptContext();
        return _precompiled(ctx);
    }
}
