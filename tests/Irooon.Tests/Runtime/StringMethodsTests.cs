using Irooon.Core;
using Irooon.Core.Runtime;
using Xunit;

namespace Irooon.Tests.Runtime;

/// <summary>
/// 文字列メソッドのテスト（ScriptEngine経由のE2Eテスト）
/// stdlib.iroのプロトタイプ実装をテストする
/// </summary>
public class StringMethodsTests
{
    private ScriptEngine CreateEngine()
    {
        return new ScriptEngine();
    }

    #region length Tests

    [Fact]
    public void Length_ReturnsStringLength()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = ""Hello""
            str.length()
        ");

        Assert.Equal(5.0, result);
    }

    [Fact]
    public void Length_ReturnsZeroForEmptyString()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = """"
            str.length()
        ");

        Assert.Equal(0.0, result);
    }

    #endregion

    #region toUpper Tests

    [Fact]
    public void ToUpper_ConvertsToUpperCase()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = ""hello world""
            str.toUpper()
        ");

        Assert.Equal("HELLO WORLD", result);
    }

    [Fact]
    public void ToUpper_AlreadyUpperCase()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = ""HELLO""
            str.toUpper()
        ");

        Assert.Equal("HELLO", result);
    }

    #endregion

    #region toLower Tests

    [Fact]
    public void ToLower_ConvertsToLowerCase()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = ""HELLO WORLD""
            str.toLower()
        ");

        Assert.Equal("hello world", result);
    }

    [Fact]
    public void ToLower_AlreadyLowerCase()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = ""hello""
            str.toLower()
        ");

        Assert.Equal("hello", result);
    }

    #endregion

    #region trim Tests

    [Fact]
    public void Trim_RemovesLeadingAndTrailingSpaces()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = ""  hello  ""
            str.trim()
        ");

        Assert.Equal("hello", result);
    }

    [Fact]
    public void Trim_EmptyString()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = """"
            str.trim()
        ");

        Assert.Equal("", result);
    }

    [Fact]
    public void Trim_OnlySpaces()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = ""   ""
            str.trim()
        ");

        Assert.Equal("", result);
    }

    #endregion

    #region substring Tests

    [Fact]
    public void Substring_WithStartAndLength()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = ""Hello World""
            str.substring(0, 5)
        ");

        Assert.Equal("Hello", result);
    }

    [Fact]
    public void Substring_WithStartOnly()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = ""Hello World""
            str.substring(6)
        ");

        Assert.Equal("World", result);
    }

    #endregion

    #region split Tests

    [Fact]
    public void Split_ByComma()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = ""apple,banana,cherry""
            str.split("","")
        ");

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Equal(3, list.Count);
        Assert.Equal("apple", list[0]);
        Assert.Equal("banana", list[1]);
        Assert.Equal("cherry", list[2]);
    }

    [Fact]
    public void Split_BySpace()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = ""Hello World Test""
            str.split("" "")
        ");

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Equal(3, list.Count);
        Assert.Equal("Hello", list[0]);
        Assert.Equal("World", list[1]);
        Assert.Equal("Test", list[2]);
    }

    #endregion

    #region contains Tests

    [Fact]
    public void Contains_ReturnsTrue_WhenSubstringExists()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = ""Hello World""
            str.contains(""World"")
        ");

        Assert.True((bool)result);
    }

    [Fact]
    public void Contains_ReturnsFalse_WhenSubstringNotExists()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = ""Hello World""
            str.contains(""Test"")
        ");

        Assert.False((bool)result);
    }

    #endregion

    #region startsWith Tests

    [Fact]
    public void StartsWith_ReturnsTrue_WhenMatches()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = ""Hello World""
            str.startsWith(""Hello"")
        ");

        Assert.True((bool)result);
    }

    [Fact]
    public void StartsWith_ReturnsFalse_WhenNoMatch()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = ""Hello World""
            str.startsWith(""World"")
        ");

        Assert.False((bool)result);
    }

    #endregion

    #region endsWith Tests

    [Fact]
    public void EndsWith_ReturnsTrue_WhenMatches()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = ""Hello World""
            str.endsWith(""World"")
        ");

        Assert.True((bool)result);
    }

    [Fact]
    public void EndsWith_ReturnsFalse_WhenNoMatch()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = ""Hello World""
            str.endsWith(""Hello"")
        ");

        Assert.False((bool)result);
    }

    #endregion

    #region replace Tests

    [Fact]
    public void Replace_ReplacesSubstring()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = ""Hello World""
            str.replace(""World"", ""Universe"")
        ");

        Assert.Equal("Hello Universe", result);
    }

    [Fact]
    public void Replace_ReplacesAllOccurrences()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = ""test test test""
            str.replace(""test"", ""demo"")
        ");

        Assert.Equal("demo demo demo", result);
    }

    #endregion

    #region E2E Tests

    [Fact]
    public void E2E_StringMethodChaining()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = ""  Hello World  ""
            str.trim().toUpper()
        ");

        Assert.Equal("HELLO WORLD", result);
    }

    [Fact]
    public void E2E_SplitAndJoin()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = ""a,b,c""
            let parts = str.split("","")
            parts.length()
        ");

        Assert.Equal(3.0, result);
    }

    [Fact]
    public void E2E_ContainsWithReplace()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = ""Hello World""
            if (str.contains(""World"")) {
                str.replace(""World"", ""irooon"")
            } else {
                str
            }
        ");

        Assert.Equal("Hello irooon", result);
    }

    #endregion
}
