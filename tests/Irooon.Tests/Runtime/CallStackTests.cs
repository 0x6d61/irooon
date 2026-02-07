using Irooon.Core.Runtime;
using Xunit;

namespace Irooon.Tests.Runtime;

/// <summary>
/// CallStackのテスト
/// </summary>
public class CallStackTests
{
    [Fact]
    public void PushAndPop_ShouldManageStackFrames()
    {
        // Arrange
        CallStack.Clear();

        // Act
        CallStack.Push("main", 1, 1);
        CallStack.Push("foo", 5, 10);
        CallStack.Push("bar", 10, 5);

        // Assert
        var trace = CallStack.GetStackTrace();
        Assert.Contains("main", trace);
        Assert.Contains("foo", trace);
        Assert.Contains("bar", trace);

        // Act - Pop
        CallStack.Pop();
        trace = CallStack.GetStackTrace();

        // Assert
        Assert.Contains("main", trace);
        Assert.Contains("foo", trace);
        Assert.DoesNotContain("bar", trace);
    }

    [Fact]
    public void GetStackTrace_ShouldReturnEmptyWhenNoFrames()
    {
        // Arrange
        CallStack.Clear();

        // Act
        var trace = CallStack.GetStackTrace();

        // Assert
        Assert.Empty(trace);
    }

    [Fact]
    public void GetStackTrace_ShouldFormatCorrectly()
    {
        // Arrange
        CallStack.Clear();
        CallStack.Push("testFunction", 42, 15);

        // Act
        var trace = CallStack.GetStackTrace();

        // Assert
        Assert.Contains("testFunction", trace);
        Assert.Contains("line 42", trace);
        Assert.Contains("column 15", trace);
    }

    [Fact]
    public void Pop_ShouldNotFailWhenStackIsEmpty()
    {
        // Arrange
        CallStack.Clear();

        // Act & Assert - should not throw
        CallStack.Pop();
        CallStack.Pop();
    }

    [Fact]
    public void Clear_ShouldRemoveAllFrames()
    {
        // Arrange
        CallStack.Push("func1", 1, 1);
        CallStack.Push("func2", 2, 2);

        // Act
        CallStack.Clear();

        // Assert
        var trace = CallStack.GetStackTrace();
        Assert.Empty(trace);
    }
}
