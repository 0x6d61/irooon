using Xunit;
using Irooon.Core;
using Irooon.Core.Runtime;

namespace Irooon.Repl.Tests;

public class SimpleReplTest
{
    [Fact]
    public void TestScriptEngineWithContext()
    {
        // Arrange
        var engine = new ScriptEngine();
        var context = new ScriptContext();

        // Act & Assert - Test 1: let x = 10
        var result1 = engine.Execute("let x = 10", context);
        Assert.Null(result1); // let文はnullを返す

        // Check that x is in context
        Assert.True(context.Globals.ContainsKey("x"));
        Assert.Equal(10, Convert.ToInt32(context.Globals["x"]));

        // Act & Assert - Test 2: x + 5
        var result2 = engine.Execute("x + 5", context);
        Assert.NotNull(result2);
        Assert.Equal(15, Convert.ToInt32(result2));
    }

    [Fact]
    public void TestMultipleStatementsWithContext()
    {
        // Arrange
        var engine = new ScriptEngine();
        var context = new ScriptContext();

        // Act
        engine.Execute("let a = 1", context);
        engine.Execute("let b = 2", context);
        var result = engine.Execute("a + b", context);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, Convert.ToInt32(result));
    }
}
