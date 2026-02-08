using Irooon.Core.Runtime;
using Xunit;

namespace Irooon.Tests.Runtime;

/// <summary>
/// CLR相互運用のテスト
/// </summary>
public class CLRInteropTests
{
    [Fact]
    public void ResolveCLRType_SystemMath_ReturnsType()
    {
        // Arrange
        var typeName = "System.Math";

        // Act
        var type = RuntimeHelpers.ResolveCLRType(typeName);

        // Assert
        Assert.NotNull(type);
        Assert.Equal(typeof(Math), type);
    }

    [Fact]
    public void ResolveCLRType_SystemDateTime_ReturnsType()
    {
        // Arrange
        var typeName = "System.DateTime";

        // Act
        var type = RuntimeHelpers.ResolveCLRType(typeName);

        // Assert
        Assert.NotNull(type);
        Assert.Equal(typeof(DateTime), type);
    }

    [Fact]
    public void ResolveCLRType_SystemString_ReturnsType()
    {
        // Arrange
        var typeName = "System.String";

        // Act
        var type = RuntimeHelpers.ResolveCLRType(typeName);

        // Assert
        Assert.NotNull(type);
        Assert.Equal(typeof(string), type);
    }

    [Fact]
    public void ResolveCLRType_InvalidType_ReturnsNull()
    {
        // Arrange
        var typeName = "System.Invalid.Type";

        // Act
        var type = RuntimeHelpers.ResolveCLRType(typeName);

        // Assert
        Assert.Null(type);
    }

    [Fact]
    public void InvokeCLRStaticMethod_MathAbs_ReturnsAbsoluteValue()
    {
        // Arrange
        var type = typeof(Math);
        var methodName = "Abs";
        var args = new object[] { -42.0 };

        // Act
        var result = RuntimeHelpers.InvokeCLRStaticMethod(type, methodName, args);

        // Assert
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void InvokeCLRStaticMethod_MathMax_ReturnsMaxValue()
    {
        // Arrange
        var type = typeof(Math);
        var methodName = "Max";
        var args = new object[] { 10.0, 20.0 };

        // Act
        var result = RuntimeHelpers.InvokeCLRStaticMethod(type, methodName, args);

        // Assert
        Assert.Equal(20.0, result);
    }

    [Fact]
    public void InvokeCLRStaticMethod_MathMin_ReturnsMinValue()
    {
        // Arrange
        var type = typeof(Math);
        var methodName = "Min";
        var args = new object[] { 10.0, 20.0 };

        // Act
        var result = RuntimeHelpers.InvokeCLRStaticMethod(type, methodName, args);

        // Assert
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void InvokeCLRStaticMethod_MathSqrt_ReturnsSqrt()
    {
        // Arrange
        var type = typeof(Math);
        var methodName = "Sqrt";
        var args = new object[] { 16.0 };

        // Act
        var result = RuntimeHelpers.InvokeCLRStaticMethod(type, methodName, args);

        // Assert
        Assert.Equal(4.0, result);
    }

    [Fact]
    public void InvokeCLRStaticMethod_MathPow_ReturnsPower()
    {
        // Arrange
        var type = typeof(Math);
        var methodName = "Pow";
        var args = new object[] { 2.0, 3.0 };

        // Act
        var result = RuntimeHelpers.InvokeCLRStaticMethod(type, methodName, args);

        // Assert
        Assert.Equal(8.0, result);
    }

    [Fact]
    public void InvokeCLRStaticMethod_InvalidMethod_ThrowsException()
    {
        // Arrange
        var type = typeof(Math);
        var methodName = "InvalidMethod";
        var args = new object[] { 42.0 };

        // Act & Assert
        Assert.Throws<RuntimeException>(() =>
            RuntimeHelpers.InvokeCLRStaticMethod(type, methodName, args));
    }

    [Fact]
    public void InvokeCLRStaticMethod_DateTimeNow_ReturnsDateTime()
    {
        // Arrange
        var type = typeof(DateTime);
        var methodName = "get_Now";
        var args = Array.Empty<object>();

        // Act
        var result = RuntimeHelpers.InvokeCLRStaticMethod(type, methodName, args);

        // Assert
        Assert.IsType<DateTime>(result);
    }
}
