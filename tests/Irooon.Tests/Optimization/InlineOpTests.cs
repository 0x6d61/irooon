using Xunit;
using Irooon.Core;

namespace Irooon.Tests.Optimization;

/// <summary>
/// v0.12.7 比較/算術インライン化 + Invoke ファストパス テスト。
/// CodeGenerator が double-double の場合に ExprTree レベルで演算をインライン化し、
/// RuntimeHelpers.Invoke が array スコープ + thisArg=null の場合にファストパスを取ることを検証。
/// </summary>
public class InlineOpTests
{
    private readonly ScriptEngine _engine = new();

    #region 算術演算インライン化

    [Fact]
    public void InlineAdd_DoubleDouble()
    {
        var result = _engine.Execute("3 + 5");
        Assert.Equal(8.0, result);
    }

    [Fact]
    public void InlineAdd_StringConcat()
    {
        // フォールバック: 文字列連結は RuntimeHelpers.Add が処理
        var result = _engine.Execute(@"""a"" + ""b""");
        Assert.Equal("ab", result);
    }

    [Fact]
    public void InlineSub_DoubleDouble()
    {
        var result = _engine.Execute("10 - 3");
        Assert.Equal(7.0, result);
    }

    [Fact]
    public void InlineMul_DoubleDouble()
    {
        var result = _engine.Execute("4 * 5");
        Assert.Equal(20.0, result);
    }

    #endregion

    #region 比較演算インライン化

    [Fact]
    public void InlineLt_True()
    {
        var result = _engine.Execute("3 < 5");
        Assert.Equal(true, result);
    }

    [Fact]
    public void InlineLt_False()
    {
        var result = _engine.Execute("5 < 3");
        Assert.Equal(false, result);
    }

    [Fact]
    public void InlineLe_Equal()
    {
        var result = _engine.Execute("5 <= 5");
        Assert.Equal(true, result);
    }

    [Fact]
    public void InlineGt_True()
    {
        var result = _engine.Execute("5 > 3");
        Assert.Equal(true, result);
    }

    [Fact]
    public void InlineGe_Equal()
    {
        var result = _engine.Execute("5 >= 5");
        Assert.Equal(true, result);
    }

    [Fact]
    public void InlineLt_StringFallback()
    {
        // フォールバック: 文字列比較は RuntimeHelpers.Lt が処理
        var result = _engine.Execute(@"""a"" < ""b""");
        Assert.Equal(true, result);
    }

    #endregion

    #region ループ内インライン化

    [Fact]
    public void InlineAdd_InLoop()
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

    [Fact]
    public void InlineLt_InLoopCondition()
    {
        var result = _engine.Execute(@"
            var count = 0
            var i = 10
            for (i > 0) {
                count = count + 1
                i = i - 1
            }
            count
        ");
        Assert.Equal(10.0, result);
    }

    #endregion

    #region 複合式

    [Fact]
    public void InlineOps_NestedExpression()
    {
        // (3 + 7) * (10 - 4) = 10 * 6 = 60
        var result = _engine.Execute("(3 + 7) * (10 - 4)");
        Assert.Equal(60.0, result);
    }

    [Fact]
    public void InlineOps_WithFunctionCall()
    {
        // 副作用のある式がオペランドでも正しく動作（tmpL/tmpR で二重評価防止）
        var result = _engine.Execute(@"
            fn add(a, b) { a + b }
            add(1, 2) + 3
        ");
        Assert.Equal(6.0, result);
    }

    #endregion

    #region Invoke ファストパス

    [Fact]
    public void FastPath_ArrayScope_SimpleCall()
    {
        var result = _engine.Execute(@"
            fn double(n) { n * 2 }
            double(5)
        ");
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void FastPath_ArrayScope_Recursion()
    {
        var result = _engine.Execute(@"
            fn fib(n) {
                if (n <= 1) { n }
                else { fib(n - 1) + fib(n - 2) }
            }
            fib(10)
        ");
        Assert.Equal(55.0, result);
    }

    [Fact]
    public void FastPath_BuiltinFunction()
    {
        var result = _engine.Execute(@"typeof(42)");
        Assert.Equal("Number", result);
    }

    [Fact]
    public void FastPath_DictScope_StillWorks()
    {
        // 内部関数を含む関数は Dictionary スコープにフォールバック
        var result = _engine.Execute(@"
            fn outer(x) {
                fn inner(y) { y * 2 }
                inner(x) + 1
            }
            outer(5)
        ");
        Assert.Equal(11.0, result);
    }

    [Fact]
    public void FastPath_InstanceMethod_StillWorks()
    {
        var result = _engine.Execute(@"
            class Counter {
                var count = 0
                fn increment() { count = count + 1 }
                fn getCount() { count }
            }
            let c = Counter()
            c.increment()
            c.increment()
            c.increment()
            c.getCount()
        ");
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void FastPath_PrototypeMethod_StillWorks()
    {
        // BoundMethod 経由の Closure 呼び出しが正しく動作するか
        var result = _engine.Execute(@"
            let parts = ""a,b,c"".split("","")
            parts.length()
        ");
        Assert.Equal(3.0, result);
    }

    #endregion
}
