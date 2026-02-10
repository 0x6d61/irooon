using BenchmarkDotNet.Attributes;

namespace Irooon.Benchmarks.Micro;

/// <summary>
/// Dictionary&lt;string,object&gt; vs object[] アクセス速度比較。
/// 配列ベーススコープ移行の根拠データ。
/// </summary>
[MemoryDiagnoser]
public class DictionaryAccessBenchmarks
{
    private Dictionary<string, object> _globals = null!;
    private object[] _array = null!;
    private const int N = 1000;

    [GlobalSetup]
    public void Setup()
    {
        _globals = new Dictionary<string, object>
        {
            ["x"] = 10.0, ["y"] = 5.0, ["z"] = 0.0,
            ["a"] = 1.0, ["b"] = 2.0
        };
        _array = new object[] { 10.0, 5.0, 0.0, 1.0, 2.0 };
    }

    [Benchmark(Baseline = true)]
    public object? DictionaryLookup()
    {
        object? result = null;
        for (int i = 0; i < N; i++)
        {
            result = _globals["x"];
            _globals["x"] = result!;
        }
        return result;
    }

    [Benchmark]
    public object? DictionaryTryGetValue()
    {
        object? result = null;
        for (int i = 0; i < N; i++)
        {
            _globals.TryGetValue("x", out result);
            _globals["x"] = result!;
        }
        return result;
    }

    [Benchmark]
    public object? ArrayAccess()
    {
        object? result = null;
        for (int i = 0; i < N; i++)
        {
            result = _array[0];
            _array[0] = result!;
        }
        return result;
    }
}
