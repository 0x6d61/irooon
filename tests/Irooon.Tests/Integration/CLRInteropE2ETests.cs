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

    [Fact]
    public void TestCLRInterop_StringBuilderInstance()
    {
        // Arrange
        var source = @"
            let sb = System.Text.StringBuilder()
            sb.Append(""Hello"")
            sb.Append("" "")
            sb.Append(""World"")
            let text = sb.ToString()
            text
        ";

        // Act
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // Assert
        Assert.Equal("Hello World", result);
    }

    [Fact]
    public void TestCLRInterop_StringBuilderWithInitialValue()
    {
        // Arrange
        var source = @"
            let sb = System.Text.StringBuilder(""Initial"")
            sb.Append("" Value"")
            sb.ToString()
        ";

        // Act
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // Assert
        Assert.Equal("Initial Value", result);
    }

    [Fact]
    public void TestCLRInterop_StringBuilderProperty()
    {
        // Arrange
        var source = @"
            let sb = System.Text.StringBuilder(""Hello"")
            let length = sb.Length
            length
        ";

        // Act
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // Assert
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void TestCLRInterop_GenericListInstance()
    {
        // Arrange
        var source = @"
            let list = System.Collections.Generic.List()
            list.Add(1)
            list.Add(2)
            list.Add(3)
            let count = list.Count
            count
        ";

        // Act
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // Assert
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void TestCLRInterop_MethodChaining()
    {
        // Arrange
        var source = @"
            let result = System.Text.StringBuilder()
                .Append(""Method"")
                .Append("" "")
                .Append(""Chaining"")
                .ToString()
            result
        ";

        // Act
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // Assert
        Assert.Equal("Method Chaining", result);
    }

    [Fact]
    public void TestCLRInterop_ComplexInstanceUsage()
    {
        // Arrange
        var source = @"
            let sb1 = System.Text.StringBuilder()
            sb1.Append(""First"")

            let sb2 = System.Text.StringBuilder()
            sb2.Append(""Second"")

            let text1 = sb1.ToString()
            let text2 = sb2.ToString()

            let combined = System.Text.StringBuilder()
            combined.Append(text1)
            combined.Append("" and "")
            combined.Append(text2)
            combined.ToString()
        ";

        // Act
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // Assert
        Assert.Equal("First and Second", result);
    }
}
