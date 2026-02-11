using Xunit;
using Irooon.Core.Diagnostics;

namespace Irooon.Tests.Diagnostics;

/// <summary>
/// DiagnosticFormatter のテスト。
/// Rust コンパイラ風のエラーフォーマット出力を検証する。
/// </summary>
public class DiagnosticFormatterTests
{
    #region 基本フォーマット

    [Fact]
    public void FormatError_BasicFormat_WithSourceAndPointer()
    {
        var location = new SourceLocation(
            FilePath: "script.iro",
            Source: "x = 20",
            Line: 1,
            Column: 1,
            Length: 6
        );

        var result = DiagnosticFormatter.FormatError(
            ErrorCode.E201_CannotAssignToLet,
            "Cannot assign to 'let' variable 'x'",
            location);

        Assert.Contains("error[E201]", result);
        Assert.Contains("Cannot assign to 'let' variable 'x'", result);
        Assert.Contains("--> script.iro:1:1", result);
        Assert.Contains("x = 20", result);
        Assert.Contains("^^^^^^", result);
    }

    [Fact]
    public void FormatError_WithSuggestion()
    {
        var location = new SourceLocation(
            FilePath: "script.iro",
            Source: "x = 20",
            Line: 1,
            Column: 1,
            Length: 6
        );

        var result = DiagnosticFormatter.FormatError(
            ErrorCode.E201_CannotAssignToLet,
            "Cannot assign to 'let' variable 'x'",
            location,
            suggestion: "Use 'var' instead of 'let' if you need to reassign");

        Assert.Contains("= help:", result);
        Assert.Contains("Use 'var' instead of 'let'", result);
    }

    [Fact]
    public void FormatError_WithoutSuggestion_NoHelpLine()
    {
        var location = new SourceLocation(
            FilePath: "script.iro",
            Source: "x = 20",
            Line: 1,
            Column: 1,
            Length: 1
        );

        var result = DiagnosticFormatter.FormatError(
            ErrorCode.E202_UndefinedVariable,
            "Undefined variable 'x'",
            location);

        Assert.DoesNotContain("= help:", result);
    }

    #endregion

    #region ファイルパス

    [Fact]
    public void FormatError_WithFilePath_ShowsFilePath()
    {
        var location = new SourceLocation(
            FilePath: "examples/hello.iro",
            Source: "let x = @",
            Line: 1,
            Column: 9,
            Length: 1
        );

        var result = DiagnosticFormatter.FormatError(
            ErrorCode.E001_UnexpectedCharacter,
            "Unexpected character '@'",
            location);

        Assert.Contains("--> examples/hello.iro:1:9", result);
    }

    [Fact]
    public void FormatError_WithoutFilePath_ShowsRepl()
    {
        var location = new SourceLocation(
            FilePath: null,
            Source: "x + 1",
            Line: 1,
            Column: 1,
            Length: 1
        );

        var result = DiagnosticFormatter.FormatError(
            ErrorCode.E202_UndefinedVariable,
            "Undefined variable 'x'",
            location);

        Assert.Contains("--> <repl>:1:1", result);
    }

    #endregion

    #region 複数行ソース

    [Fact]
    public void FormatError_MultilineSource_ExtractsCorrectLine()
    {
        var source = "let x = 10\nvar y = 20\nx = 30";
        var location = new SourceLocation(
            FilePath: "test.iro",
            Source: source,
            Line: 3,
            Column: 1,
            Length: 6
        );

        var result = DiagnosticFormatter.FormatError(
            ErrorCode.E201_CannotAssignToLet,
            "Cannot assign to 'let' variable 'x'",
            location);

        // 3行目のソースが表示される
        Assert.Contains("3 | x = 30", result);
        Assert.Contains("^^^^^^", result);
        // 他の行は表示されない
        Assert.DoesNotContain("let x = 10", result);
        Assert.DoesNotContain("var y = 20", result);
    }

    [Fact]
    public void FormatError_ColumnOffset_CorrectPointerPosition()
    {
        var source = "let result = x + y";
        var location = new SourceLocation(
            FilePath: null,
            Source: source,
            Line: 1,
            Column: 14,
            Length: 1
        );

        var result = DiagnosticFormatter.FormatError(
            ErrorCode.E202_UndefinedVariable,
            "Undefined variable 'x'",
            location);

        // ポインタが Column 14 の位置にある
        Assert.Contains("1 | let result = x + y", result);
        // "  |" の後に 13 個のスペース + "^"
        Assert.Contains("  |              ^", result);
    }

    #endregion

    #region Source が null の場合

    [Fact]
    public void FormatError_NullSource_ShowsWithoutSourceLine()
    {
        var location = new SourceLocation(
            FilePath: null,
            Source: null,
            Line: 5,
            Column: 10,
            Length: 1
        );

        var result = DiagnosticFormatter.FormatError(
            ErrorCode.E300_DivisionByZero,
            "Division by zero",
            location);

        Assert.Contains("error[E300]", result);
        Assert.Contains("Division by zero", result);
        Assert.Contains("--> <repl>:5:10", result);
        // ソースコード行は表示されない
        Assert.DoesNotContain(" | ", result);
    }

    #endregion

    #region エラーコードフォーマット

    [Fact]
    public void FormatError_ErrorCodeFormatting_ThreeDigits()
    {
        var location = new SourceLocation(null, "x", 1, 1, 1);

        var result1 = DiagnosticFormatter.FormatError(
            ErrorCode.E001_UnexpectedCharacter, "test", location);
        Assert.Contains("error[E001]", result1);

        var result2 = DiagnosticFormatter.FormatError(
            ErrorCode.E100_ExpectExpression, "test", location);
        Assert.Contains("error[E100]", result2);

        var result3 = DiagnosticFormatter.FormatError(
            ErrorCode.E300_DivisionByZero, "test", location);
        Assert.Contains("error[E300]", result3);
    }

    #endregion

    #region Length のバリエーション

    [Fact]
    public void FormatError_Length1_SingleCaret()
    {
        var location = new SourceLocation(null, "x", 1, 1, 1);

        var result = DiagnosticFormatter.FormatError(
            ErrorCode.E202_UndefinedVariable, "Undefined variable 'x'", location);

        Assert.Contains("  | ^", result);
    }

    [Fact]
    public void FormatError_LengthMultiple_MultipleCarets()
    {
        var location = new SourceLocation(null, "hello", 1, 1, 5);

        var result = DiagnosticFormatter.FormatError(
            ErrorCode.E202_UndefinedVariable, "Undefined variable 'hello'", location);

        Assert.Contains("  | ^^^^^", result);
    }

    #endregion
}
