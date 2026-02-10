using Xunit;
using Irooon.Core;

namespace Irooon.Tests.Benchmarks;

/// <summary>
/// ベンチマークスクリプトの正確性を確認するサニティテスト。
/// v0.12.3: パフォーマンスプロファイリング基盤
/// </summary>
public class BenchmarkSanityTests
{
    private readonly ScriptEngine _engine = new();

    #region Tarai

    [Fact]
    public void Tarai_CSharp_ReturnsCorrectResult()
    {
        static double Tarai(double x, double y, double z)
        {
            if (x <= y) return y;
            return Tarai(Tarai(x - 1, y, z), Tarai(y - 1, z, x), Tarai(z - 1, x, y));
        }

        Assert.Equal(10.0, Tarai(10.0, 5.0, 0.0));
    }

    [Fact]
    public void Tarai_Irooon_ReturnsCorrectResult()
    {
        var result = _engine.Execute(@"
            fn tarai(x, y, z) {
                if (x <= y) { y }
                else {
                    tarai(tarai(x - 1, y, z), tarai(y - 1, z, x), tarai(z - 1, x, y))
                }
            }
            tarai(10, 5, 0)
        ");
        Assert.Equal(10.0, result);
    }

    #endregion

    #region Fibonacci

    [Fact]
    public void Fibonacci_BothMatch()
    {
        static double Fib(double n)
        {
            if (n <= 1) return n;
            return Fib(n - 1) + Fib(n - 2);
        }

        var iroResult = _engine.Execute(@"
            fn fib(n) {
                if (n <= 1) { n }
                else { fib(n - 1) + fib(n - 2) }
            }
            fib(20)
        ");
        Assert.Equal(Fib(20), (double)iroResult!);
    }

    #endregion

    #region Loop

    [Fact]
    public void Loop_ReturnsCorrectResult()
    {
        var result = _engine.Execute(@"
            var sum = 0
            var i = 0
            for (i < 100) {
                sum = sum + i
                i = i + 1
            }
            sum
        ");
        Assert.Equal(4950.0, result);
    }

    #endregion
}
