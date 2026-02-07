using Irooon.Core.Runtime;

namespace Irooon.Tests.Runtime;

/// <summary>
/// 文字列メソッドのテスト
/// </summary>
public class StringMethodsTests
{
    private ScriptContext _ctx;

    public StringMethodsTests()
    {
        _ctx = new ScriptContext();
    }

    #region GetMember Tests (String)

    [Fact]
    public void GetMember_文字列のlengthプロパティを取得()
    {
        // Arrange
        var str = "Hello";

        // Act
        var lengthWrapper = RuntimeHelpers.GetMember(str, "length");

        // Assert
        Assert.NotNull(lengthWrapper);
        Assert.IsType<StringMethodWrapper>(lengthWrapper);

        var callable = (IroCallable)lengthWrapper;
        var result = callable.Invoke(_ctx, Array.Empty<object>());
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void GetMember_文字列のtoUpperメソッドを取得()
    {
        // Arrange
        var str = "hello";

        // Act
        var methodWrapper = RuntimeHelpers.GetMember(str, "toUpper");

        // Assert
        Assert.NotNull(methodWrapper);
        Assert.IsType<StringMethodWrapper>(methodWrapper);

        var callable = (IroCallable)methodWrapper;
        var result = callable.Invoke(_ctx, Array.Empty<object>());
        Assert.Equal("HELLO", result);
    }

    [Fact]
    public void GetMember_文字列のtoLowerメソッドを取得()
    {
        // Arrange
        var str = "HELLO";

        // Act
        var methodWrapper = RuntimeHelpers.GetMember(str, "toLower");

        // Assert
        Assert.NotNull(methodWrapper);
        Assert.IsType<StringMethodWrapper>(methodWrapper);

        var callable = (IroCallable)methodWrapper;
        var result = callable.Invoke(_ctx, Array.Empty<object>());
        Assert.Equal("hello", result);
    }

    #endregion

    #region StringMethodWrapper Tests

    [Fact]
    public void StringMethodWrapper_length_文字列の長さを返す()
    {
        // Arrange
        var wrapper = new StringMethodWrapper("Hello", "length");

        // Act
        var result = wrapper.Invoke(_ctx, Array.Empty<object>());

        // Assert
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void StringMethodWrapper_toUpper_大文字に変換()
    {
        // Arrange
        var wrapper = new StringMethodWrapper("hello world", "toUpper");

        // Act
        var result = wrapper.Invoke(_ctx, Array.Empty<object>());

        // Assert
        Assert.Equal("HELLO WORLD", result);
    }

    [Fact]
    public void StringMethodWrapper_toLower_小文字に変換()
    {
        // Arrange
        var wrapper = new StringMethodWrapper("HELLO WORLD", "toLower");

        // Act
        var result = wrapper.Invoke(_ctx, Array.Empty<object>());

        // Assert
        Assert.Equal("hello world", result);
    }

    [Fact]
    public void StringMethodWrapper_trim_前後の空白を削除()
    {
        // Arrange
        var wrapper = new StringMethodWrapper("  hello  ", "trim");

        // Act
        var result = wrapper.Invoke(_ctx, Array.Empty<object>());

        // Assert
        Assert.Equal("hello", result);
    }

    [Fact]
    public void StringMethodWrapper_substring_部分文字列を取得()
    {
        // Arrange
        var wrapper = new StringMethodWrapper("Hello World", "substring");

        // Act
        var result = wrapper.Invoke(_ctx, new object[] { 0.0, 5.0 });

        // Assert
        Assert.Equal("Hello", result);
    }

    [Fact]
    public void StringMethodWrapper_substring_開始位置のみ指定()
    {
        // Arrange
        var wrapper = new StringMethodWrapper("Hello World", "substring");

        // Act
        var result = wrapper.Invoke(_ctx, new object[] { 6.0 });

        // Assert
        Assert.Equal("World", result);
    }

    [Fact]
    public void StringMethodWrapper_split_文字列を分割()
    {
        // Arrange
        var wrapper = new StringMethodWrapper("apple,banana,cherry", "split");

        // Act
        var result = wrapper.Invoke(_ctx, new object[] { "," });

        // Assert
        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Equal(3, list.Count);
        Assert.Equal("apple", list[0]);
        Assert.Equal("banana", list[1]);
        Assert.Equal("cherry", list[2]);
    }

    [Fact]
    public void StringMethodWrapper_split_スペースで分割()
    {
        // Arrange
        var wrapper = new StringMethodWrapper("Hello World Test", "split");

        // Act
        var result = wrapper.Invoke(_ctx, new object[] { " " });

        // Assert
        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Equal(3, list.Count);
        Assert.Equal("Hello", list[0]);
        Assert.Equal("World", list[1]);
        Assert.Equal("Test", list[2]);
    }

    [Fact]
    public void StringMethodWrapper_contains_含まれる文字列を検索()
    {
        // Arrange
        var wrapper = new StringMethodWrapper("Hello World", "contains");

        // Act
        var result = wrapper.Invoke(_ctx, new object[] { "World" });

        // Assert
        Assert.Equal(true, result);
    }

    [Fact]
    public void StringMethodWrapper_contains_含まれない文字列を検索()
    {
        // Arrange
        var wrapper = new StringMethodWrapper("Hello World", "contains");

        // Act
        var result = wrapper.Invoke(_ctx, new object[] { "Test" });

        // Assert
        Assert.Equal(false, result);
    }

    [Fact]
    public void StringMethodWrapper_startsWith_先頭文字列を判定()
    {
        // Arrange
        var wrapper = new StringMethodWrapper("Hello World", "startsWith");

        // Act
        var result = wrapper.Invoke(_ctx, new object[] { "Hello" });

        // Assert
        Assert.Equal(true, result);
    }

    [Fact]
    public void StringMethodWrapper_startsWith_先頭でない文字列を判定()
    {
        // Arrange
        var wrapper = new StringMethodWrapper("Hello World", "startsWith");

        // Act
        var result = wrapper.Invoke(_ctx, new object[] { "World" });

        // Assert
        Assert.Equal(false, result);
    }

    [Fact]
    public void StringMethodWrapper_endsWith_末尾文字列を判定()
    {
        // Arrange
        var wrapper = new StringMethodWrapper("Hello World", "endsWith");

        // Act
        var result = wrapper.Invoke(_ctx, new object[] { "World" });

        // Assert
        Assert.Equal(true, result);
    }

    [Fact]
    public void StringMethodWrapper_endsWith_末尾でない文字列を判定()
    {
        // Arrange
        var wrapper = new StringMethodWrapper("Hello World", "endsWith");

        // Act
        var result = wrapper.Invoke(_ctx, new object[] { "Hello" });

        // Assert
        Assert.Equal(false, result);
    }

    [Fact]
    public void StringMethodWrapper_replace_文字列を置換()
    {
        // Arrange
        var wrapper = new StringMethodWrapper("Hello World", "replace");

        // Act
        var result = wrapper.Invoke(_ctx, new object[] { "World", "Universe" });

        // Assert
        Assert.Equal("Hello Universe", result);
    }

    [Fact]
    public void StringMethodWrapper_replace_複数箇所を置換()
    {
        // Arrange
        var wrapper = new StringMethodWrapper("test test test", "replace");

        // Act
        var result = wrapper.Invoke(_ctx, new object[] { "test", "demo" });

        // Assert
        Assert.Equal("demo demo demo", result);
    }

    [Fact]
    public void StringMethodWrapper_未知のメソッドで例外()
    {
        // Arrange
        var wrapper = new StringMethodWrapper("Hello", "unknownMethod");

        // Act & Assert
        var ex = Assert.Throws<RuntimeException>(() => wrapper.Invoke(_ctx, Array.Empty<object>()));
        Assert.Contains("Unknown string method: unknownMethod", ex.Message);
    }

    #endregion
}
