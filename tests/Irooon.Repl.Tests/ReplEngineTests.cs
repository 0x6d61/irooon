using Xunit;
using Irooon.Repl;

namespace Irooon.Repl.Tests;

public class ReplEngineTests
{
    [Fact]
    public void TestBasicEvaluation()
    {
        // Arrange
        var repl = new ReplEngine();

        // Act
        var result = repl.Evaluate("1 + 2");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, Convert.ToInt32(result));
    }

    [Fact]
    public void TestVariablePersistence()
    {
        // Arrange
        var repl = new ReplEngine();

        // Act
        repl.Evaluate("let x = 10");
        var result = repl.Evaluate("x + 5");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(15, Convert.ToInt32(result));
    }

    [Fact]
    public void TestMultipleStatements()
    {
        // Arrange
        var repl = new ReplEngine();

        // Act
        repl.Evaluate("let a = 1");
        repl.Evaluate("let b = 2");
        var result = repl.Evaluate("a + b");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, Convert.ToInt32(result));
    }

    [Fact]
    public void TestFunctionDefinition()
    {
        // Arrange
        var repl = new ReplEngine();

        // Act
        repl.Evaluate("let add = fn(x, y) { x + y }");
        var result = repl.Evaluate("add(3, 4)");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, Convert.ToInt32(result));
    }

    [Fact]
    public void TestErrorHandling()
    {
        // Arrange
        var repl = new ReplEngine();

        // Act
        var result = repl.Evaluate("undefined_variable");

        // Assert
        Assert.Null(result); // エラー時はnullを返す
    }

    [Fact]
    public void TestContextIsolation()
    {
        // Arrange
        var repl1 = new ReplEngine();
        var repl2 = new ReplEngine();

        // Act
        repl1.Evaluate("let x = 10");
        var result = repl2.Evaluate("x"); // repl2では未定義

        // Assert
        Assert.Null(result); // エラーでnull
    }

    [Fact]
    public void TestReassignment()
    {
        // Arrange
        var repl = new ReplEngine();

        // Act
        repl.Evaluate("var x = 10");
        repl.Evaluate("x = 20");
        var result = repl.Evaluate("x");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(20, Convert.ToInt32(result));
    }

    [Fact]
    public void TestClearContext()
    {
        // Arrange
        var repl = new ReplEngine();
        repl.Evaluate("let x = 10");

        // Act
        repl.Clear();
        var result = repl.Evaluate("x"); // クリア後は未定義

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void TestGetVariables()
    {
        // Arrange
        var repl = new ReplEngine();
        repl.Evaluate("let x = 10");
        repl.Evaluate("let y = 20");

        // Act
        var variables = repl.GetVariables();

        // Assert
        Assert.Contains("x", variables.Keys);
        Assert.Contains("y", variables.Keys);
        Assert.NotNull(variables["x"]);
        Assert.NotNull(variables["y"]);
        Assert.Equal(10, Convert.ToInt32(variables["x"]));
        Assert.Equal(20, Convert.ToInt32(variables["y"]));
    }

    [Fact]
    public void TestExpressionResult()
    {
        // Arrange
        var repl = new ReplEngine();

        // Act
        var result = repl.Evaluate("2 * 3 + 4");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, Convert.ToInt32(result));
    }
}
