using Xunit;
using Irooon.Core;

namespace Irooon.Tests.Integration;

/// <summary>
/// エラーフォーマットの E2E テスト。
/// ScriptEngine 経由で各種エラーが Rust 風フォーマットで表示されることを検証する。
/// </summary>
public class ErrorFormatE2ETests
{
    private readonly ScriptEngine _engine = new();

    #region Parse エラー

    [Fact]
    public void ParseError_DetailedMessage_ContainsSourceAndPointer()
    {
        var ex = Assert.Throws<ScriptException>(() => _engine.Execute("1 +"));
        Assert.NotNull(ex.DetailedMessage);
        Assert.Contains("error[E", ex.DetailedMessage);
        Assert.Contains("-->", ex.DetailedMessage);
        // 従来の Message も維持
        Assert.Contains("Parse error", ex.Message);
    }

    [Fact]
    public void ParseError_WithFilePath_ShowsFilePath()
    {
        var ex = Assert.Throws<ScriptException>(() => _engine.Execute("1 +", filePath: "test.iro"));
        Assert.NotNull(ex.DetailedMessage);
        Assert.Contains("--> test.iro:", ex.DetailedMessage);
    }

    [Fact]
    public void ParseError_WithoutFilePath_ShowsRepl()
    {
        var ex = Assert.Throws<ScriptException>(() => _engine.Execute("1 +"));
        Assert.NotNull(ex.DetailedMessage);
        Assert.Contains("--> <repl>:", ex.DetailedMessage);
    }

    #endregion

    #region Resolve エラー

    [Fact]
    public void ResolveError_UndefinedVariable_DetailedMessage()
    {
        var ex = Assert.Throws<ScriptException>(() => _engine.Execute("x"));
        Assert.NotNull(ex.DetailedMessage);
        Assert.Contains("error[E202]", ex.DetailedMessage);
        Assert.Contains("Undefined variable", ex.DetailedMessage);
        // 従来の Message も維持
        Assert.Contains("Resolve errors", ex.Message);
        Assert.Contains("Undefined variable", ex.Message);
    }

    [Fact]
    public void ResolveError_CannotAssignToLet_WithSuggestion()
    {
        var ex = Assert.Throws<ScriptException>(() => _engine.Execute("let x = 10\nx = 20"));
        Assert.NotNull(ex.DetailedMessage);
        Assert.Contains("error[E201]", ex.DetailedMessage);
        Assert.Contains("= help:", ex.DetailedMessage);
        Assert.Contains("var", ex.DetailedMessage);
    }

    [Fact]
    public void ResolveError_ShowsSourceLine()
    {
        var source = "let x = 10\ny + 1";
        var ex = Assert.Throws<ScriptException>(() => _engine.Execute(source));
        Assert.NotNull(ex.DetailedMessage);
        // ソースコード行が表示される
        Assert.Contains("y + 1", ex.DetailedMessage);
        Assert.Contains("^", ex.DetailedMessage);
    }

    #endregion

    #region Lex エラー

    [Fact]
    public void LexError_UnexpectedCharacter_DetailedMessage()
    {
        var ex = Assert.Throws<ScriptException>(() => _engine.Execute("let x = @"));
        Assert.NotNull(ex.DetailedMessage);
        Assert.Contains("error[E001]", ex.DetailedMessage);
        Assert.Contains("let x = @", ex.DetailedMessage);
        Assert.Contains("^", ex.DetailedMessage);
    }

    [Fact]
    public void LexError_UnterminatedString_DetailedMessage()
    {
        var ex = Assert.Throws<ScriptException>(() => _engine.Execute("let x = \"hello"));
        Assert.NotNull(ex.DetailedMessage);
        Assert.Contains("error[E002]", ex.DetailedMessage);
        Assert.Contains("Unterminated string", ex.DetailedMessage);
    }

    #endregion

    #region Runtime エラー

    [Fact]
    public void RuntimeError_DivisionByZero_DetailedMessage()
    {
        var ex = Assert.Throws<ScriptException>(() => _engine.Execute("1 / 0"));
        Assert.NotNull(ex.DetailedMessage);
        Assert.Contains("error[E", ex.DetailedMessage);
        // 従来の Message も維持
        Assert.Contains("Runtime error", ex.Message);
    }

    #endregion

    #region 後方互換

    [Fact]
    public void BackwardCompat_ParseError_MessageContainsParseError()
    {
        var ex = Assert.Throws<ScriptException>(() => _engine.Execute("(1 + 2"));
        Assert.Contains("Parse error", ex.Message);
    }

    [Fact]
    public void BackwardCompat_ResolveError_MessageContainsUndefinedVariable()
    {
        var ex = Assert.Throws<ScriptException>(() => _engine.Execute("x"));
        Assert.Contains("Undefined variable", ex.Message);
    }

    [Fact]
    public void BackwardCompat_ResolveError_MessageContainsCannotAssign()
    {
        var ex = Assert.Throws<ScriptException>(() => _engine.Execute("let x = 10\nx = 20"));
        Assert.Contains("Cannot assign to 'let'", ex.Message);
    }

    [Fact]
    public void BackwardCompat_RuntimeError_MessageContainsRuntimeError()
    {
        var ex = Assert.Throws<ScriptException>(() => _engine.Execute("1 / 0"));
        Assert.Contains("Runtime error", ex.Message);
    }

    #endregion
}
