using BenchmarkDotNet.Attributes;
using Irooon.Core.Runtime;

namespace Irooon.Benchmarks.Micro;

/// <summary>
/// CallStack.Push/Pop と配列パディングのコスト計測。
/// </summary>
[MemoryDiagnoser]
public class ClosureBenchmarks
{
    private ScriptContext _ctx = null!;
    private Closure _closure3Params = null!;
    private object[] _fullArgs = null!;
    private object[] _partialArgs = null!;

    [GlobalSetup]
    public void Setup()
    {
        _ctx = new ScriptContext();
        Func<ScriptContext, object[], object> body = (ctx, args) => args[0];

        _closure3Params = new Closure("test", body,
            parameterNames: new List<string> { "x", "y", "z" });

        _fullArgs = new object[] { 1.0, 2.0, 3.0 };
        _partialArgs = new object[] { 1.0 };
    }

    [Benchmark(Baseline = true)]
    public object InvokeFullArgs() => _closure3Params.Invoke(_ctx, _fullArgs);

    [Benchmark]
    public object InvokePartialArgs() => _closure3Params.Invoke(_ctx, _partialArgs);

    [Benchmark]
    public void CallStackPushPop()
    {
        CallStack.Push("test", 1, 1);
        CallStack.Pop();
    }
}
