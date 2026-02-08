using Irooon.Core;
using Xunit;

namespace Irooon.Tests.Integration;

/// <summary>
/// CLR相互運用のE2Eテスト
/// </summary>
public class CLRInteropE2ETests
{
    [Fact]
    public void TestCLRInterop_MathAbs()
    {
        // Arrange
        var source = @"
            let abs = System.Math.Abs(-42)
            abs
        ";

        // Act
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // Assert
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void TestCLRInterop_MathMax()
    {
        // Arrange
        var source = @"
            let max = System.Math.Max(10, 20)
            max
        ";

        // Act
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // Assert
        Assert.Equal(20.0, result);
    }

    [Fact]
    public void TestCLRInterop_MathMin()
    {
        // Arrange
        var source = @"
            let min = System.Math.Min(10, 20)
            min
        ";

        // Act
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // Assert
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestCLRInterop_MathSqrt()
    {
        // Arrange
        var source = @"
            let sqrt = System.Math.Sqrt(16)
            sqrt
        ";

        // Act
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // Assert
        Assert.Equal(4.0, result);
    }

    [Fact]
    public void TestCLRInterop_MathPow()
    {
        // Arrange
        var source = @"
            let pow = System.Math.Pow(2, 3)
            pow
        ";

        // Act
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // Assert
        Assert.Equal(8.0, result);
    }

    [Fact]
    public void TestCLRInterop_DateTimeNow()
    {
        // Arrange
        var source = @"
            let now = System.DateTime.Now
            now
        ";

        // Act
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // Assert
        Assert.IsType<DateTime>(result);
    }

    [Fact]
    public void TestCLRInterop_MultipleOperations()
    {
        // Arrange
        var source = @"
            let abs = System.Math.Abs(-42)
            let max = System.Math.Max(abs, 50)
            let pow = System.Math.Pow(max, 2)
            pow
        ";

        // Act
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // Assert
        Assert.Equal(2500.0, result); // 50^2
    }
}
