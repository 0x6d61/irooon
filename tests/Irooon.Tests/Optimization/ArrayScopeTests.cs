using Xunit;
using Irooon.Core;

namespace Irooon.Tests.Optimization;

/// <summary>
/// Array-based scope 最適化のテスト。
/// 関数スコープの変数が object[] 配列で正しく動作することを確認する。
/// </summary>
public class ArrayScopeTests
{
    private readonly ScriptEngine _engine = new();

    [Fact]
    public void BasicFunction_UsesLocals()
    {
        var result = _engine.Execute(@"
            fn add(a, b) { a + b }
            add(3, 4)
        ");
        Assert.Equal(7.0, result);
    }

    [Fact]
    public void RecursiveFunction_PreservesLocals()
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

    [Fact]
    public void NestedFunction_CapturedVariable()
    {
        var result = _engine.Execute(@"
            fn outer() {
                var x = 10
                let inner = fn() { x + 5 }
                inner()
            }
            outer()
        ");
        Assert.Equal(15.0, result);
    }

    [Fact]
    public void Lambda_WithLocals()
    {
        var result = _engine.Execute(@"
            let compute = fn(a, b) {
                var temp = a * 2
                temp + b
            }
            compute(3, 4)
        ");
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void ClassMethod_FieldAccess()
    {
        var result = _engine.Execute(@"
            class Counter {
                var count = 0
                fn increment() {
                    count = count + 1
                    count
                }
            }
            let c = Counter()
            c.increment()
            c.increment()
            c.increment()
        ");
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void ForLoop_WithLocals()
    {
        var result = _engine.Execute(@"
            fn sumRange(n) {
                var total = 0
                var i = 0
                for (i < n) {
                    total = total + i
                    i = i + 1
                }
                total
            }
            sumRange(100)
        ");
        Assert.Equal(4950.0, result);
    }

    [Fact]
    public void TryCatch_ExceptionVariable()
    {
        var result = _engine.Execute(@"
            fn safeDivide(a, b) {
                try {
                    if (b == 0) { throw ""division by zero"" }
                    a / b
                } catch (e) {
                    e
                }
            }
            safeDivide(10, 0)
        ");
        Assert.Equal("division by zero", result);
    }

    [Fact]
    public void Destructuring_InFunction()
    {
        var result = _engine.Execute(@"
            fn sumPair(pair) {
                let [a, b] = pair
                a + b
            }
            sumPair([3, 7])
        ");
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void Increment_LocalVariable()
    {
        var result = _engine.Execute(@"
            fn countUp() {
                var i = 0
                i++
                i++
                i++
                i
            }
            countUp()
        ");
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void GlobalVariable_StillWorks()
    {
        var result = _engine.Execute(@"
            var x = 42
            x + 8
        ");
        Assert.Equal(50.0, result);
    }

    [Fact]
    public void BuiltinFunction_StillWorks()
    {
        var result = _engine.Execute(@"
            typeof(42)
        ");
        Assert.Equal("Number", result);
    }

    [Fact]
    public void ClosureCapture_InSameScope()
    {
        // 同一スコープ内でのクロージャ使用（即時実行）
        var result = _engine.Execute(@"
            fn compute() {
                var x = 10
                let adder = fn(y) { x + y }
                adder(5)
            }
            compute()
        ");
        Assert.Equal(15.0, result);
    }

    [Fact]
    public void ForeachLoop_WithLocals()
    {
        var result = _engine.Execute(@"
            fn sumList(items) {
                var total = 0
                foreach (item in items) {
                    total = total + item
                }
                total
            }
            sumList([1, 2, 3, 4, 5])
        ");
        Assert.Equal(15.0, result);
    }

    [Fact]
    public void ForCollection_WithLocals()
    {
        var result = _engine.Execute(@"
            fn sumRange2() {
                var total = 0
                for (i in 1..6) {
                    total = total + i
                }
                total
            }
            sumRange2()
        ");
        Assert.Equal(15.0, result);
    }

    [Fact]
    public void DefaultParameter_WithLocals()
    {
        var result = _engine.Execute(@"
            fn greet(name, greeting = ""Hello"") {
                ""${greeting}, ${name}!""
            }
            greet(""World"")
        ");
        Assert.Equal("Hello, World!", result);
    }

    [Fact]
    public void MultipleRecursion_Fibonacci()
    {
        var result = _engine.Execute(@"
            fn fib(n) {
                if (n <= 1) { n }
                else { fib(n - 1) + fib(n - 2) }
            }
            fib(20)
        ");
        Assert.Equal(6765.0, result);
    }

    [Fact]
    public void NestedClosure_MultipleLevels()
    {
        var result = _engine.Execute(@"
            fn outer() {
                var a = 1
                fn middle() {
                    var b = 2
                    fn inner() {
                        a + b
                    }
                    inner()
                }
                middle()
            }
            outer()
        ");
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void ClassMethod_WithParameters()
    {
        var result = _engine.Execute(@"
            class Calculator {
                var result = 0
                fn add(a, b) {
                    result = a + b
                    result
                }
            }
            let calc = Calculator()
            calc.add(10, 20)
        ");
        Assert.Equal(30.0, result);
    }
}
