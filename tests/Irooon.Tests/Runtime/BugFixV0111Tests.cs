using Xunit;
using Irooon.Core;

namespace Irooon.Tests.Runtime;

/// <summary>
/// v0.11.1 バグ修正テスト
/// Issue #39
/// </summary>
public class BugFixV0111Tests
{
    private object? Execute(string source)
    {
        var engine = new ScriptEngine();
        return engine.Execute(source);
    }

    #region 修正D: __toNumber() が非数値で例外

    [Fact]
    public void ToNumber_NonNumericString_ReturnsNull()
    {
        var result = Execute(@"__toNumber(""hello"")");
        Assert.Null(result);
    }

    [Fact]
    public void ToNumber_NumericString_ReturnsNumber()
    {
        var result = Execute(@"__toNumber(""42"")");
        Assert.Equal(42.0, result);
    }

    #endregion

    #region 修正B: == 演算子の型不一致例外

    [Fact]
    public void Eq_ListAndString_ReturnsFalse()
    {
        var result = Execute(@"[1, 2, 3] == ""hello""");
        Assert.Equal(false, result);
    }

    [Fact]
    public void Eq_HashAndNumber_ReturnsFalse()
    {
        var result = Execute(@"{ a: 1 } == 42");
        Assert.Equal(false, result);
    }

    [Fact]
    public void Ne_ListAndString_ReturnsTrue()
    {
        var result = Execute(@"[1, 2, 3] != ""hello""");
        Assert.Equal(true, result);
    }

    [Fact]
    public void Lt_ListAndNumber_ThrowsException()
    {
        var ex = Assert.ThrowsAny<Exception>(() => Execute(@"[1, 2] < 5"));
        Assert.Contains("Cannot compare", ex.InnerException?.Message ?? ex.Message);
    }

    #endregion

    #region 修正E: if式にelseが必須

    [Fact]
    public void If_WithoutElse_TrueReturnsValue()
    {
        var result = Execute("if (true) { 42 }");
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void If_WithoutElse_FalseReturnsNull()
    {
        var result = Execute("if (false) { 42 }");
        Assert.Null(result);
    }

    [Fact]
    public void If_WithElse_StillWorks()
    {
        var result = Execute("if (true) { 1 } else { 2 }");
        Assert.Equal(1.0, result);
    }

    #endregion

    #region 修正C: returnがfor内で機能しない

    [Fact]
    public void Return_InsideForLoop_ExitsFunction()
    {
        var result = Execute(@"
            fn findFirst(items) {
                for (item in items) {
                    if (item == 3) {
                        return item
                    }
                }
                return -1
            }
            findFirst([1, 2, 3, 4, 5])
        ");
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void Return_InsideForeachLoop_ExitsFunction()
    {
        var result = Execute(@"
            fn contains(items, target) {
                foreach (item in items) {
                    if (item == target) {
                        return true
                    }
                }
                return false
            }
            contains([10, 20, 30], 20)
        ");
        Assert.Equal(true, result);
    }

    [Fact]
    public void Return_WithoutLoop_StillWorks()
    {
        var result = Execute(@"
            fn double(x) {
                return x * 2
            }
            double(21)
        ");
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void Return_InNestedFunction_DoesNotAffectOuter()
    {
        var result = Execute(@"
            fn outer() {
                fn inner() {
                    return 10
                }
                var x = inner()
                return x + 5
            }
            outer()
        ");
        Assert.Equal(15.0, result);
    }

    #endregion

    #region 修正A: this.method()ハング

    [Fact]
    public void ThisMethod_MultipleCallsNoHang()
    {
        // this.method() を複数回呼んでもハングしない
        var result = Execute(@"
            class Counter {
                var count = 0

                fn increment() {
                    count = count + 1
                }

                fn addThree() {
                    this.increment()
                    this.increment()
                    this.increment()
                    return count
                }
            }
            var c = Counter()
            c.addThree()
        ");
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void ThisMethod_FieldChangePropagation()
    {
        // 内部メソッド呼び出しでのフィールド変更が外部に伝播する
        var result = Execute(@"
            class Acc {
                var total = 0

                fn add(n) {
                    total = total + n
                }

                fn addTwice(n) {
                    this.add(n)
                    this.add(n)
                    return total
                }
            }
            var a = Acc()
            a.addTwice(5)
        ");
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void ThisMethod_DifferentInstancesIsolated()
    {
        // 異なるインスタンスのフィールドは干渉しない
        var result = Execute(@"
            class Box {
                var value = 0

                fn set(v) {
                    value = v
                }

                fn get() {
                    return value
                }
            }
            var a = Box()
            var b = Box()
            a.set(10)
            b.set(20)
            a.get() + b.get()
        ");
        Assert.Equal(30.0, result);
    }

    [Fact]
    public void ThisMethod_Recursive()
    {
        // 再帰的な this.method() 呼び出し
        var result = Execute(@"
            class Factorial {
                var result = 1

                fn calc(n) {
                    if (n <= 1) {
                        return result
                    }
                    result = result * n
                    return this.calc(n - 1)
                }
            }
            var f = Factorial()
            f.calc(5)
        ");
        Assert.Equal(120.0, result);
    }

    [Fact]
    public void ThisMethod_ChainedCalls()
    {
        // メソッドチェーン的な連続呼び出し
        var result = Execute(@"
            class Builder {
                var count = 0

                fn addItem() {
                    count = count + 1
                }

                fn build() {
                    this.addItem()
                    this.addItem()
                    this.addItem()
                    return count
                }
            }
            var b = Builder()
            b.build()
        ");
        Assert.Equal(3.0, result);
    }

    #endregion
}
