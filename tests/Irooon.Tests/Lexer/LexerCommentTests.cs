using Irooon.Core.Lexer;
using Xunit;

namespace Irooon.Tests.Lexer;

/// <summary>
/// Lexerのコメント機能テストクラス
/// </summary>
public class LexerCommentTests
{
    #region 一行コメントテスト

    [Fact]
    public void TestSingleLineComment_OnlyComment()
    {
        var lexer = new Core.Lexer.Lexer("// this is a comment");
        var tokens = lexer.ScanTokens();

        Assert.Single(tokens);
        Assert.Equal(TokenType.Eof, tokens[0].Type);
    }

    [Fact]
    public void TestSingleLineComment_AfterCode()
    {
        var lexer = new Core.Lexer.Lexer("let x = 5 // this is a comment");
        var tokens = lexer.ScanTokens();

        // コメントはスキップされる
        Assert.Equal(5, tokens.Count); // let, x, =, 5, Eof
        Assert.Equal(TokenType.Let, tokens[0].Type);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal(TokenType.Equal, tokens[2].Type);
        Assert.Equal(TokenType.Number, tokens[3].Type);
        Assert.Equal(TokenType.Eof, tokens[4].Type);
    }

    [Fact]
    public void TestSingleLineComment_MultipleLines()
    {
        var source = @"let x = 5 // first comment
// second comment
var y = 10 // third comment";
        var lexer = new Core.Lexer.Lexer(source);
        var tokens = lexer.ScanTokens();

        // コメントは全てスキップされる
        Assert.Equal(9, tokens.Count); // let, x, =, 5, var, y, =, 10, Eof
        Assert.Equal(TokenType.Let, tokens[0].Type);
        Assert.Equal(TokenType.Var, tokens[4].Type);
    }

    #endregion

    #region 複数行コメントテスト

    [Fact]
    public void TestMultiLineComment_Simple()
    {
        var lexer = new Core.Lexer.Lexer("/* this is a comment */");
        var tokens = lexer.ScanTokens();

        Assert.Single(tokens);
        Assert.Equal(TokenType.Eof, tokens[0].Type);
    }

    [Fact]
    public void TestMultiLineComment_MultipleLines()
    {
        var source = @"/* this is a
multi-line
comment */";
        var lexer = new Core.Lexer.Lexer(source);
        var tokens = lexer.ScanTokens();

        Assert.Single(tokens);
        Assert.Equal(TokenType.Eof, tokens[0].Type);
    }

    [Fact]
    public void TestMultiLineComment_BeforeCode()
    {
        var source = "/* comment */ let x = 5";
        var lexer = new Core.Lexer.Lexer(source);
        var tokens = lexer.ScanTokens();

        Assert.Equal(5, tokens.Count); // let, x, =, 5, Eof
        Assert.Equal(TokenType.Let, tokens[0].Type);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal("x", tokens[1].Lexeme);
    }

    [Fact]
    public void TestMultiLineComment_AfterCode()
    {
        var source = "let x = 5 /* comment */";
        var lexer = new Core.Lexer.Lexer(source);
        var tokens = lexer.ScanTokens();

        Assert.Equal(5, tokens.Count); // let, x, =, 5, Eof
        Assert.Equal(TokenType.Let, tokens[0].Type);
    }

    [Fact]
    public void TestMultiLineComment_BetweenCode()
    {
        var source = "let /* comment */ x = 5";
        var lexer = new Core.Lexer.Lexer(source);
        var tokens = lexer.ScanTokens();

        Assert.Equal(5, tokens.Count); // let, x, =, 5, Eof
        Assert.Equal(TokenType.Let, tokens[0].Type);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal("x", tokens[1].Lexeme);
    }

    [Fact]
    public void TestMultiLineComment_Unterminated()
    {
        var source = "let x = 5 /* unterminated comment";
        var lexer = new Core.Lexer.Lexer(source);
        var tokens = lexer.ScanTokens();

        // 未終了の複数行コメントはEOFまでスキップ
        Assert.Equal(5, tokens.Count); // let, x, =, 5, Eof
    }

    #endregion

    #region コメントとコードの混在テスト

    [Fact]
    public void TestMixedComments_SingleAndMultiLine()
    {
        var source = @"let x = 5 // single line comment
/* multi-line
   comment */
var y = 10";
        var lexer = new Core.Lexer.Lexer(source);
        var tokens = lexer.ScanTokens();

        // コメントは全てスキップされる
        Assert.Equal(9, tokens.Count); // let, x, =, 5, var, y, =, 10, Eof
        Assert.Equal(TokenType.Let, tokens[0].Type);
        Assert.Equal(TokenType.Var, tokens[4].Type);
    }

    [Fact]
    public void TestMixedComments_ComplexCode()
    {
        var source = @"// Function definition
fn fibonacci(n) {
    /* Base cases */
    if (n <= 1) {
        return n // Return n directly
    }
    // Recursive case
    return fibonacci(n - 1) + fibonacci(n - 2)
}";
        var lexer = new Core.Lexer.Lexer(source);
        var tokens = lexer.ScanTokens();

        // コメントは全てスキップされる
        // fn, fibonacci, (, n, ), {, if, (, n, <=, 1, ), {, return, n, }, return, fibonacci, (, n, -, 1, ), +, fibonacci, (, n, -, 2, ), }, Eof
        Assert.Contains(tokens, t => t.Type == TokenType.Fn);
        Assert.Contains(tokens, t => t.Type == TokenType.If);
        Assert.Contains(tokens, t => t.Type == TokenType.Return);
    }

    [Fact]
    public void TestComment_SlashInString()
    {
        var source = "let x = \"http://example.com\"";
        var lexer = new Core.Lexer.Lexer(source);
        var tokens = lexer.ScanTokens();

        // 文字列内の // はコメントとして扱わない
        Assert.Equal(5, tokens.Count); // let, x, =, "http://example.com", Eof
        Assert.Equal(TokenType.String, tokens[3].Type);
        Assert.Equal("http://example.com", tokens[3].Value);
    }

    [Fact]
    public void TestComment_StarSlashInString()
    {
        var source = "let x = \"a /* comment */ b\"";
        var lexer = new Core.Lexer.Lexer(source);
        var tokens = lexer.ScanTokens();

        // 文字列内の /* */ はコメントとして扱わない
        Assert.Equal(5, tokens.Count); // let, x, =, "a /* comment */ b", Eof
        Assert.Equal(TokenType.String, tokens[3].Type);
        Assert.Equal("a /* comment */ b", tokens[3].Value);
    }

    [Fact]
    public void TestComment_LineTracking()
    {
        var source = @"let x = 5
// comment on line 2
/* multi-line comment
   on line 3
   and line 4 */
var y = 10";
        var lexer = new Core.Lexer.Lexer(source);
        var tokens = lexer.ScanTokens();

        // let x = 5 は1行目
        Assert.Equal(1, tokens[0].Line); // let

        // var y = 10 は6行目
        var varToken = tokens.First(t => t.Type == TokenType.Var);
        Assert.Equal(6, varToken.Line);
    }

    #endregion
}
