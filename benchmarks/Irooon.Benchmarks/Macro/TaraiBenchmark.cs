using BenchmarkDotNet.Attributes;
using Irooon.Core;
using Irooon.Core.Runtime;
using Irooon.Benchmarks.Helpers;

namespace Irooon.Benchmarks.Macro;

/// <summary>
/// tarai(10,5,0) ベンチマーク — 深い再帰。
/// FullPipeline vs PreCompiled でコンパイルコスト vs ランタイムコストを分離。
/// </summary>
[MemoryDiagnoser]
public class TaraiBenchmark
{
    private ScriptEngine _engine = null!;
    private Func<ScriptContext, object?> _precompiled = null!;

    private const string TaraiScript = @"
        fn tarai(x, y, z) {
            if (x <= y) { y }
            else {
                tarai(tarai(x - 1, y, z), tarai(y - 1, z, x), tarai(z - 1, x, y))
            }
        }
        tarai(10, 5, 0)
    ";

    [GlobalSetup]
    public void Setup()
    {
        _engine = new ScriptEngine();
        (_precompiled, _) = BenchmarkHelper.PreCompile(TaraiScript);
    }

    [Benchmark(Baseline = true)]
    public double CSharp_Tarai()
    {
        return Tarai(10.0, 5.0, 0.0);
    }

    [Benchmark]
    public object? Irooon_Tarai_FullPipeline()
    {
        return _engine.Execute(TaraiScript);
    }

    [Benchmark]
    public object? Irooon_Tarai_PreCompiled()
    {
        var ctx = new ScriptContext();
        return _precompiled(ctx);
    }

    private static double Tarai(double x, double y, double z)
    {
        if (x <= y) return y;
        return Tarai(Tarai(x - 1, y, z), Tarai(y - 1, z, x), Tarai(z - 1, x, y));
    }
}
