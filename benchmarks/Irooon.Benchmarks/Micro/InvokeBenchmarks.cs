using BenchmarkDotNet.Attributes;
using Irooon.Core.Runtime;

namespace Irooon.Benchmarks.Micro;

/// <summary>
/// RuntimeHelpers.Invoke() のパラメータ保存/復元オーバーヘッド計測。
/// 最重要ベンチマーク: Dictionary 生成コストを定量化する。
/// </summary>
[MemoryDiagnoser]
public class InvokeBenchmarks
{
    private ScriptContext _ctx = null!;
    private Closure _simpleClosure = null!;
    private Closure _paramsClosure = null!;
    private Closure _paramsLocalsClosure = null!;
    private Func<ScriptContext, object[], object> _rawBody = null!;
    private object[] _args = null!;

    [GlobalSetup]
    public void Setup()
    {
        _ctx = new ScriptContext();
        _args = new object[] { 10.0, 5.0, 0.0 };

        // Identity 関数 — body は何もしない。純粋なオーバーヘッドを計測。
        Func<ScriptContext, object[], object> identity = (ctx, args) => args[0];
        _rawBody = identity;

        _simpleClosure = new Closure("noop", identity);

        _paramsClosure = new Closure("noop3", identity,
            parameterNames: new List<string> { "x", "y", "z" });

        _paramsLocalsClosure = new Closure("noop3loc", identity,
            parameterNames: new List<string> { "x", "y", "z" },
            localNames: new List<string> { "a", "b" });
    }

    [Benchmark(Baseline = true)]
    public object DirectCall()
    {
        return _rawBody(_ctx, _args);
    }

    [Benchmark]
    public object Invoke_NoClosure()
    {
        return RuntimeHelpers.Invoke(_simpleClosure, _ctx, _args);
    }

    [Benchmark]
    public object Invoke_WithParams()
    {
        return RuntimeHelpers.Invoke(_paramsClosure, _ctx, _args);
    }

    [Benchmark]
    public object Invoke_WithParamsAndLocals()
    {
        return RuntimeHelpers.Invoke(_paramsLocalsClosure, _ctx, _args);
    }

    [Benchmark]
    public object ClosureInvoke_Direct()
    {
        // RuntimeHelpers.Invoke をバイパスし、Closure.Invoke 直接呼び出し
        // （CallStack.Push/Pop のみのオーバーヘッド）
        return _paramsClosure.Invoke(_ctx, _args);
    }

    [Benchmark]
    public Dictionary<string, object> DictionaryAllocation()
    {
        var dict = new Dictionary<string, object>();
        dict["x"] = 10.0;
        dict["y"] = 5.0;
        dict["z"] = 0.0;
        return dict;
    }
}
