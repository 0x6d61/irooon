using Irooon.Core.Runtime;

namespace Irooon.Tests.Runtime;

public class BuiltinFunctionsTests
{
    #region Print Tests

    [Fact]
    public void Print_単一の値を出力する()
    {
        // Arrange
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        RuntimeHelpers.Print("Hello");

        // Assert
        Assert.Equal("Hello", output.ToString());

        // Cleanup
        Console.SetOut(Console.Out);
    }

    [Fact]
    public void Print_複数の値を出力する()
    {
        // Arrange
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        RuntimeHelpers.Print("Hello", "World", 42.0);

        // Assert
        Assert.Equal("Hello World 42", output.ToString());

        // Cleanup
        Console.SetOut(Console.Out);
    }

    [Fact]
    public void Print_nullを出力する()
    {
        // Arrange
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        RuntimeHelpers.Print(null, "test");

        // Assert
        Assert.Equal("null test", output.ToString());

        // Cleanup
        Console.SetOut(Console.Out);
    }

    [Fact]
    public void Print_引数なしで呼び出せる()
    {
        // Arrange
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        RuntimeHelpers.Print();

        // Assert
        Assert.Equal("", output.ToString());

        // Cleanup
        Console.SetOut(Console.Out);
    }

    #endregion

    #region Println Tests

    [Fact]
    public void Println_単一の値を出力する()
    {
        // Arrange
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        RuntimeHelpers.Println("Hello");

        // Assert
        Assert.Equal("Hello" + Environment.NewLine, output.ToString());

        // Cleanup
        Console.SetOut(Console.Out);
    }

    [Fact]
    public void Println_複数の値を出力する()
    {
        // Arrange
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        RuntimeHelpers.Println("Hello", "World", 42.0);

        // Assert
        Assert.Equal("Hello World 42" + Environment.NewLine, output.ToString());

        // Cleanup
        Console.SetOut(Console.Out);
    }

    [Fact]
    public void Println_nullを出力する()
    {
        // Arrange
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        RuntimeHelpers.Println(null, "test");

        // Assert
        Assert.Equal("null test" + Environment.NewLine, output.ToString());

        // Cleanup
        Console.SetOut(Console.Out);
    }

    [Fact]
    public void Println_引数なしで呼び出せる()
    {
        // Arrange
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        RuntimeHelpers.Println();

        // Assert
        Assert.Equal(Environment.NewLine, output.ToString());

        // Cleanup
        Console.SetOut(Console.Out);
    }

    #endregion
}
