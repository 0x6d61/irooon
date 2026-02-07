using Irooon.Core.Runtime;
using Xunit;

namespace Irooon.Tests.Runtime;

/// <summary>
/// RuntimeExceptionのテスト
/// </summary>
public class RuntimeExceptionTests
{
    [Fact]
    public void Constructor_ShouldStoreLineAndColumn()
    {
        // Arrange & Act
        var exception = new RuntimeException("Test error", 10, 5);

        // Assert
        Assert.Equal("Test error", exception.Message);
        Assert.Equal(10, exception.Line);
        Assert.Equal(5, exception.Column);
    }

    [Fact]
    public void Constructor_ShouldCaptureStackTrace()
    {
        // Arrange
        CallStack.Clear();
        CallStack.Push("main", 1, 1);
        CallStack.Push("foo", 5, 10);

        // Act
        var exception = new RuntimeException("Test error", 10, 5);

        // Assert
        Assert.NotNull(exception.StackTraceString);
        Assert.Contains("main", exception.StackTraceString);
        Assert.Contains("foo", exception.StackTraceString);
    }

    [Fact]
    public void ToString_ShouldIncludeAllInformation()
    {
        // Arrange
        CallStack.Clear();
        CallStack.Push("testFunc", 20, 15);
        var exception = new RuntimeException("Division by zero", 25, 10);

        // Act
        var result = exception.ToString();

        // Assert
        Assert.Contains("RuntimeError", result);
        Assert.Contains("line 25", result);
        Assert.Contains("column 10", result);
        Assert.Contains("Division by zero", result);
        Assert.Contains("Stack trace:", result);
        Assert.Contains("testFunc", result);
    }

    [Fact]
    public void Constructor_WithInnerException_ShouldPreserveInnerException()
    {
        // Arrange
        var innerException = new InvalidOperationException("Inner error");
        CallStack.Clear();

        // Act
        var exception = new RuntimeException("Test error", innerException, 10, 5);

        // Assert
        Assert.Equal("Test error", exception.Message);
        Assert.Equal(innerException, exception.InnerException);
        Assert.Equal(10, exception.Line);
        Assert.Equal(5, exception.Column);
    }
}
