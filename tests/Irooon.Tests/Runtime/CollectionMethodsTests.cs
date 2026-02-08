using Irooon.Core;
using Irooon.Core.Runtime;
using Xunit;

namespace Irooon.Tests.Runtime;

/// <summary>
/// コレクション操作メソッドのテスト
/// </summary>
public class CollectionMethodsTests
{
    private ScriptEngine CreateEngine()
    {
        return new ScriptEngine();
    }

    #region map Tests

    [Fact]
    public void Map_DoublesEachElement()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let numbers = [1, 2, 3]
            numbers.map(fn(x) { x * 2 })
        ");

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Equal(3, list.Count);
        Assert.Equal(2.0, list[0]);
        Assert.Equal(4.0, list[1]);
        Assert.Equal(6.0, list[2]);
    }

    [Fact]
    public void Map_SquaresEachElement()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let numbers = [1, 2, 3, 4]
            numbers.map(fn(x) { x * x })
        ");

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Equal(4, list.Count);
        Assert.Equal(1.0, list[0]);
        Assert.Equal(4.0, list[1]);
        Assert.Equal(9.0, list[2]);
        Assert.Equal(16.0, list[3]);
    }

    [Fact]
    public void Map_EmptyList_ReturnsEmptyList()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let empty = []
            empty.map(fn(x) { x * 2 })
        ");

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Empty(list);
    }

    [Fact]
    public void Map_ThrowsException_WhenNoFunction()
    {
        var engine = CreateEngine();
        var ex = Assert.Throws<ScriptException>(() =>
        {
            engine.Execute(@"
                let numbers = [1, 2, 3]
                numbers.map(42)
            ");
        });
        Assert.Contains("map requires a function argument", ex.Message);
    }

    #endregion

    #region filter Tests

    [Fact]
    public void Filter_FiltersEvenNumbers()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let numbers = [1, 2, 3, 4, 5, 6]
            numbers.filter(fn(x) { x % 2 == 0 })
        ");

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Equal(3, list.Count);
        Assert.Equal(2.0, list[0]);
        Assert.Equal(4.0, list[1]);
        Assert.Equal(6.0, list[2]);
    }

    [Fact]
    public void Filter_FiltersOddNumbers()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let numbers = [1, 2, 3, 4, 5]
            numbers.filter(fn(x) { x % 2 != 0 })
        ");

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Equal(3, list.Count);
        Assert.Equal(1.0, list[0]);
        Assert.Equal(3.0, list[1]);
        Assert.Equal(5.0, list[2]);
    }

    [Fact]
    public void Filter_EmptyList_ReturnsEmptyList()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let empty = []
            empty.filter(fn(x) { x > 0 })
        ");

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Empty(list);
    }

    [Fact]
    public void Filter_ThrowsException_WhenNoFunction()
    {
        var engine = CreateEngine();
        var ex = Assert.Throws<ScriptException>(() =>
        {
            engine.Execute(@"
                let numbers = [1, 2, 3]
                numbers.filter(42)
            ");
        });
        Assert.Contains("filter requires a function argument", ex.Message);
    }

    #endregion

    #region reduce Tests

    [Fact]
    public void Reduce_SumsAllElements()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let numbers = [1, 2, 3, 4]
            numbers.reduce(0, fn(acc, x) { acc + x })
        ");

        Assert.Equal(10.0, result);
    }

    [Fact]
    public void Reduce_MultipliesAllElements()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let numbers = [2, 3, 4]
            numbers.reduce(1, fn(acc, x) { acc * x })
        ");

        Assert.Equal(24.0, result);
    }

    [Fact]
    public void Reduce_EmptyList_ReturnsInitialValue()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let empty = []
            empty.reduce(42, fn(acc, x) { acc + x })
        ");

        Assert.Equal(42.0, result);
    }

    [Fact]
    public void Reduce_ThrowsException_WhenNoFunction()
    {
        var engine = CreateEngine();
        var ex = Assert.Throws<ScriptException>(() =>
        {
            engine.Execute(@"
                let numbers = [1, 2, 3]
                numbers.reduce(0, 42)
            ");
        });
        Assert.Contains("reduce requires an initial value and a function", ex.Message);
    }

    #endregion

    #region forEach Tests

    [Fact]
    public void ForEach_ExecutesFunctionForEachElement()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
var sum = 0
let numbers = [1, 2, 3]
numbers.forEach(fn(x) { sum = sum + x })
sum
");

        Assert.Equal(6.0, result);
    }

    [Fact]
    public void ForEach_EmptyList_DoesNothing()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
var sum = 0
let empty = []
empty.forEach(fn(x) { sum = sum + x })
sum
");

        Assert.Equal(0.0, result);
    }

    [Fact]
    public void ForEach_ReturnsNull()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let numbers = [1, 2, 3]
            numbers.forEach(fn(x) { x * 2 })
        ");

        Assert.Null(result);
    }

    [Fact]
    public void ForEach_ThrowsException_WhenNoFunction()
    {
        var engine = CreateEngine();
        var ex = Assert.Throws<ScriptException>(() =>
        {
            engine.Execute(@"
                let numbers = [1, 2, 3]
                numbers.forEach(42)
            ");
        });
        Assert.Contains("forEach requires a function argument", ex.Message);
    }

    #endregion

    #region first Tests

    [Fact]
    public void First_ReturnsFirstElement()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let numbers = [1, 2, 3]
            numbers.first()
        ");

        Assert.Equal(1.0, result);
    }

    [Fact]
    public void First_ThrowsException_WhenEmpty()
    {
        var engine = CreateEngine();
        var ex = Assert.Throws<ScriptException>(() =>
        {
            engine.Execute(@"
                let empty = []
                empty.first()
            ");
        });
        Assert.Contains("Cannot get first element of empty list", ex.Message);
    }

    #endregion

    #region last Tests

    [Fact]
    public void Last_ReturnsLastElement()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let numbers = [1, 2, 3]
            numbers.last()
        ");

        Assert.Equal(3.0, result);
    }

    [Fact]
    public void Last_ThrowsException_WhenEmpty()
    {
        var engine = CreateEngine();
        var ex = Assert.Throws<ScriptException>(() =>
        {
            engine.Execute(@"
                let empty = []
                empty.last()
            ");
        });
        Assert.Contains("Cannot get last element of empty list", ex.Message);
    }

    #endregion

    #region length Tests

    [Fact]
    public void Length_ReturnsNumberOfElements()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let numbers = [1, 2, 3, 4, 5]
            numbers.length()
        ");

        Assert.Equal(5.0, result);
    }

    [Fact]
    public void Length_ReturnsZeroForEmptyList()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let empty = []
            empty.length()
        ");

        Assert.Equal(0.0, result);
    }

    #endregion

    #region isEmpty Tests

    [Fact]
    public void IsEmpty_ReturnsTrueForEmptyList()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let empty = []
            empty.isEmpty()
        ");

        Assert.True((bool)result);
    }

    [Fact]
    public void IsEmpty_ReturnsFalseForNonEmptyList()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let numbers = [1, 2, 3]
            numbers.isEmpty()
        ");

        Assert.False((bool)result);
    }

    #endregion

    #region E2E Tests

    [Fact]
    public void E2E_ComplexChaining()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let numbers = [1, 2, 3, 4, 5, 6]
            numbers
                .filter(fn(x) { x % 2 == 0 })
                .map(fn(x) { x * 2 })
                .reduce(0, fn(acc, x) { acc + x })
        ");

        // [2, 4, 6] -> [4, 8, 12] -> 24
        Assert.Equal(24.0, result);
    }

    [Fact]
    public void E2E_MapFilterReduce()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let numbers = [1, 2, 3, 4, 5]

            // 各要素を2倍にしてから、10より小さいものだけを残す
            let doubled = numbers.map(fn(x) { x * 2 })
            let filtered = doubled.filter(fn(x) { x < 10 })
            let sum = filtered.reduce(0, fn(acc, x) { acc + x })

            sum
        ");

        // [1, 2, 3, 4, 5] -> [2, 4, 6, 8, 10] -> [2, 4, 6, 8] -> 20
        Assert.Equal(20.0, result);
    }

    [Fact]
    public void E2E_FirstLastLength()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let numbers = [10, 20, 30, 40, 50]

            let f = numbers.first()
            let l = numbers.last()
            let len = numbers.length()

            f + l + len
        ");

        // 10 + 50 + 5 = 65
        Assert.Equal(65.0, result);
    }

    [Fact]
    public void E2E_ForEachWithMutation()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
let numbers = [1, 2, 3]
var sum = 0

numbers.forEach(fn(x) {
    sum = sum + (x * 10)
})

sum
");

        Assert.Equal(60.0, result); // 10 + 20 + 30 = 60
    }

    #endregion
}
