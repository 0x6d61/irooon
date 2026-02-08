using Irooon.Core.Runtime;
using System.Text;
using Xunit;

namespace Irooon.Tests.Runtime;

/// <summary>
/// CLRインスタンス操作のテスト
/// </summary>
public class CLRInstanceTests
{
    [Fact]
    public void IsCLRObject_WithCLRInstance_ReturnsTrue()
    {
        // Arrange
        var sb = new StringBuilder();

        // Act
        var result = RuntimeHelpers.IsCLRObject(sb);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsCLRObject_WithNull_ReturnsFalse()
    {
        // Arrange
        object obj = null;

        // Act
        var result = RuntimeHelpers.IsCLRObject(obj);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsCLRObject_WithPrimitive_ReturnsFalse()
    {
        // Arrange
        var obj = 42.0;

        // Act
        var result = RuntimeHelpers.IsCLRObject(obj);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsCLRObject_WithString_ReturnsFalse()
    {
        // Arrange
        var obj = "Hello";

        // Act
        var result = RuntimeHelpers.IsCLRObject(obj);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsCLRObject_WithList_ReturnsTrue()
    {
        // Arrange
        var obj = new List<object>();

        // Act
        var result = RuntimeHelpers.IsCLRObject(obj);

        // Assert
        // List<object> はCLR相互運用でも作成されるため、True
        Assert.True(result);
    }

    [Fact]
    public void IsCLRObject_WithDictionary_ReturnsTrue()
    {
        // Arrange
        var obj = new Dictionary<string, object>();

        // Act
        var result = RuntimeHelpers.IsCLRObject(obj);

        // Assert
        // Dictionary<string, object> もCLRオブジェクトとして扱う
        Assert.True(result);
    }

    [Fact]
    public void CreateCLRInstance_StringBuilder_ReturnsInstance()
    {
        // Arrange
        var type = typeof(StringBuilder);
        var args = Array.Empty<object>();

        // Act
        var result = RuntimeHelpers.CreateCLRInstance(type, args);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<StringBuilder>(result);
    }

    [Fact]
    public void CreateCLRInstance_StringBuilderWithCapacity_ReturnsInstance()
    {
        // Arrange
        var type = typeof(StringBuilder);
        var args = new object[] { 100 };

        // Act
        var result = RuntimeHelpers.CreateCLRInstance(type, args);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<StringBuilder>(result);
    }

    [Fact]
    public void CreateCLRInstance_ListOfInt_ReturnsInstance()
    {
        // Arrange
        var type = typeof(List<int>);
        var args = Array.Empty<object>();

        // Act
        var result = RuntimeHelpers.CreateCLRInstance(type, args);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<int>>(result);
    }

    [Fact]
    public void InvokeCLRInstanceMethod_StringBuilderAppend_AppendsText()
    {
        // Arrange
        var sb = new StringBuilder();
        var methodName = "Append";
        var args = new object[] { "Hello" };

        // Act
        var result = RuntimeHelpers.InvokeCLRInstanceMethod(sb, methodName, args);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<StringBuilder>(result);
        Assert.Equal("Hello", sb.ToString());
    }

    [Fact]
    public void InvokeCLRInstanceMethod_StringBuilderToString_ReturnsString()
    {
        // Arrange
        var sb = new StringBuilder("Hello");
        var methodName = "ToString";
        var args = Array.Empty<object>();

        // Act
        var result = RuntimeHelpers.InvokeCLRInstanceMethod(sb, methodName, args);

        // Assert
        Assert.Equal("Hello", result);
    }

    [Fact]
    public void InvokeCLRInstanceMethod_ListAdd_AddsItem()
    {
        // Arrange
        var list = new List<int>();
        var methodName = "Add";
        var args = new object[] { 42 };

        // Act
        RuntimeHelpers.InvokeCLRInstanceMethod(list, methodName, args);

        // Assert
        Assert.Single(list);
        Assert.Equal(42, list[0]);
    }

    [Fact]
    public void InvokeCLRInstanceMethod_NullInstance_ThrowsException()
    {
        // Arrange
        object instance = null;
        var methodName = "ToString";
        var args = Array.Empty<object>();

        // Act & Assert
        Assert.Throws<RuntimeException>(() =>
            RuntimeHelpers.InvokeCLRInstanceMethod(instance, methodName, args));
    }

    [Fact]
    public void InvokeCLRInstanceMethod_InvalidMethod_ThrowsException()
    {
        // Arrange
        var sb = new StringBuilder();
        var methodName = "InvalidMethod";
        var args = Array.Empty<object>();

        // Act & Assert
        Assert.Throws<RuntimeException>(() =>
            RuntimeHelpers.InvokeCLRInstanceMethod(sb, methodName, args));
    }

    [Fact]
    public void GetCLRInstanceProperty_StringBuilderLength_ReturnsLength()
    {
        // Arrange
        var sb = new StringBuilder("Hello");
        var propertyName = "Length";

        // Act
        var result = RuntimeHelpers.GetCLRInstanceProperty(sb, propertyName);

        // Assert
        // irooonの型システムでは数値はdoubleに変換される
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void GetCLRInstanceProperty_ListCount_ReturnsCount()
    {
        // Arrange
        var list = new List<int> { 1, 2, 3 };
        var propertyName = "Count";

        // Act
        var result = RuntimeHelpers.GetCLRInstanceProperty(list, propertyName);

        // Assert
        // irooonの型システムでは数値はdoubleに変換される
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void GetCLRInstanceProperty_NullInstance_ThrowsException()
    {
        // Arrange
        object instance = null;
        var propertyName = "Length";

        // Act & Assert
        Assert.Throws<RuntimeException>(() =>
            RuntimeHelpers.GetCLRInstanceProperty(instance, propertyName));
    }

    [Fact]
    public void GetCLRInstanceProperty_InvalidProperty_ThrowsException()
    {
        // Arrange
        var sb = new StringBuilder();
        var propertyName = "InvalidProperty";

        // Act & Assert
        Assert.Throws<RuntimeException>(() =>
            RuntimeHelpers.GetCLRInstanceProperty(sb, propertyName));
    }

    [Fact]
    public void SetCLRInstanceProperty_StringBuilderLength_SetsLength()
    {
        // Arrange
        var sb = new StringBuilder("Hello World");
        var propertyName = "Length";
        var value = 5;

        // Act
        var result = RuntimeHelpers.SetCLRInstanceProperty(sb, propertyName, value);

        // Assert
        Assert.Equal(5, result);
        Assert.Equal(5, sb.Length);
        Assert.Equal("Hello", sb.ToString());
    }

    [Fact]
    public void SetCLRInstanceProperty_NullInstance_ThrowsException()
    {
        // Arrange
        object instance = null;
        var propertyName = "Length";
        var value = 5;

        // Act & Assert
        Assert.Throws<RuntimeException>(() =>
            RuntimeHelpers.SetCLRInstanceProperty(instance, propertyName, value));
    }

    [Fact]
    public void SetCLRInstanceProperty_InvalidProperty_ThrowsException()
    {
        // Arrange
        var sb = new StringBuilder();
        var propertyName = "InvalidProperty";
        var value = 5;

        // Act & Assert
        Assert.Throws<RuntimeException>(() =>
            RuntimeHelpers.SetCLRInstanceProperty(sb, propertyName, value));
    }

    [Fact]
    public void CLRMethodWrapper_Invoke_CallsMethod()
    {
        // Arrange
        var sb = new StringBuilder();
        var wrapper = new CLRMethodWrapper(sb, "Append");
        var ctx = new ScriptContext();
        var args = new object[] { "Hello" };

        // Act
        var result = wrapper.Invoke(ctx, args);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<StringBuilder>(result);
        Assert.Equal("Hello", sb.ToString());
    }

    [Fact]
    public void CLRMethodWrapper_ToString_ReturnsString()
    {
        // Arrange
        var sb = new StringBuilder("World");
        var wrapper = new CLRMethodWrapper(sb, "ToString");
        var ctx = new ScriptContext();
        var args = Array.Empty<object>();

        // Act
        var result = wrapper.Invoke(ctx, args);

        // Assert
        Assert.Equal("World", result);
    }
}
