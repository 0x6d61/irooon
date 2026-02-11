using Xunit;
using Irooon.Core;

namespace Irooon.Tests.Lexer;

/// <summary>
/// Lexer エラーが ScriptEngine 経由で適切に報告されることを検証する。
/// v0.13.0: 以前はサイレントに失われていた Lexer エラーが正式に例外になる。
/// </summary>
public class LexExceptionTests
{
    private readonly ScriptEngine _engine = new();

    [Fact]
    public void UnexpectedCharacter_ThrowsScriptException()
    {
        var ex = Assert.Throws<ScriptException>(() => _engine.Execute("let x = @"));
        Assert.Contains("Unexpected character", ex.Message);
    }

    [Fact]
    public void UnterminatedString_ThrowsScriptException()
    {
        var ex = Assert.Throws<ScriptException>(() => _engine.Execute("let x = \"hello"));
        Assert.Contains("Unterminated string", ex.Message);
    }

    [Fact]
    public void InvalidEscapeSequence_ThrowsScriptException()
    {
        var ex = Assert.Throws<ScriptException>(() => _engine.Execute("let x = \"\\z\""));
        Assert.Contains("Invalid escape sequence", ex.Message);
    }

    [Fact]
    public void UnterminatedBlockComment_ThrowsScriptException()
    {
        var ex = Assert.Throws<ScriptException>(() => _engine.Execute("/* unterminated"));
        Assert.Contains("Unterminated block comment", ex.Message);
    }

    [Fact]
    public void InvalidHexNumber_ThrowsScriptException()
    {
        var ex = Assert.Throws<ScriptException>(() => _engine.Execute("let x = 0xZZ"));
        Assert.Contains("Invalid hex number", ex.Message);
    }

    [Fact]
    public void MultipleErrors_ReportsAll()
    {
        // 複数のエラーがある場合、全て報告される
        var ex = Assert.Throws<ScriptException>(() => _engine.Execute("@ #"));
        Assert.Contains("Lex error", ex.Message);
    }

    [Fact]
    public void DetailedMessage_ContainsSourceDisplay()
    {
        var ex = Assert.Throws<ScriptException>(() => _engine.Execute("let x = @"));
        Assert.NotNull(ex.DetailedMessage);
        Assert.Contains("error[E001]", ex.DetailedMessage);
        Assert.Contains("let x = @", ex.DetailedMessage);
        Assert.Contains("^", ex.DetailedMessage);
    }
}
