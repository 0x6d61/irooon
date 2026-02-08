using Xunit;
using Irooon.Core;
using Irooon.Core.Runtime;

namespace Irooon.Tests.Integration;

/// <summary>
/// 複合代入演算子（+=, -=, *=, /=, %=）のテスト
/// </summary>
public class CompoundAssignmentTests
{
    [Fact]
    public void TestPlusEqual_Variable()
    {
        var source = @"
var x = 10
x += 5
x
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);
        Assert.Equal(15.0, result);
    }

    [Fact]
    public void TestMinusEqual_Variable()
    {
        var source = @"
var x = 20
x -= 7
x
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);
        Assert.Equal(13.0, result);
    }

    [Fact]
    public void TestStarEqual_Variable()
    {
        var source = @"
var x = 5
x *= 3
x
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);
        Assert.Equal(15.0, result);
    }

    [Fact]
    public void TestSlashEqual_Variable()
    {
        var source = @"
var x = 20
x /= 4
x
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void TestPercentEqual_Variable()
    {
        var source = @"
var x = 17
x %= 5
x
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);
        Assert.Equal(2.0, result);
    }

    [Fact]
    public void TestPlusEqual_ArrayElement()
    {
        var source = @"
var arr = [1, 2, 3]
arr[1] += 10
arr[1]
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);
        Assert.Equal(12.0, result);
    }

    [Fact]
    public void TestMinusEqual_ArrayElement()
    {
        var source = @"
var arr = [100, 50, 25]
arr[0] -= 30
arr[0]
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);
        Assert.Equal(70.0, result);
    }

    [Fact]
    public void TestStarEqual_ArrayElement()
    {
        var source = @"
var arr = [2, 4, 6]
arr[2] *= 5
arr[2]
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);
        Assert.Equal(30.0, result);
    }

    [Fact]
    public void TestCompoundAssignment_InLoop()
    {
        var source = @"
var sum = 0
foreach (i in [1, 2, 3, 4, 5]) {
    sum += i
}
sum
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);
        Assert.Equal(15.0, result);
    }

    [Fact]
    public void TestCompoundAssignment_MultipleOperations()
    {
        var source = @"
var x = 10
x += 5    // 15
x *= 2    // 30
x -= 10   // 20
x /= 4    // 5
x %= 3    // 2
x
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);
        Assert.Equal(2.0, result);
    }

    [Fact]
    public void TestCompoundAssignment_WithExpression()
    {
        var source = @"
var x = 10
var y = 5
x += y * 2
x
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);
        Assert.Equal(20.0, result);
    }

    [Fact]
    public void TestCompoundAssignment_InFunction()
    {
        var source = @"
fn increment(n) {
    n += 1
    n
}
increment(42)
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);
        Assert.Equal(43.0, result);
    }

    [Fact]
    public void TestCompoundAssignment_ClassField()
    {
        var source = @"
class Counter {
    public var value = 0

    public fn add(n) {
        value += n
        value
    }
}

var counter = Counter()
counter.add(10)
counter.add(5)
counter.value
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);
        Assert.Equal(15.0, result);
    }
}
