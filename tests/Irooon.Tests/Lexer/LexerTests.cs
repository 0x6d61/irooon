using Irooon.Core.Lexer;
using Xunit;

namespace Irooon.Tests.Lexer;

/// <summary>
/// Lexerのテストクラス
/// </summary>
public class LexerTests
{
    #region 基本テスト

    [Fact]
    public void TestEmptySource()
    {
        var lexer = new Core.Lexer.Lexer("");
        var tokens = lexer.ScanTokens();

        Assert.Single(tokens);
        Assert.Equal(TokenType.Eof, tokens[0].Type);
    }

    [Fact]
    public void TestWhitespaceOnly()
    {
        var lexer = new Core.Lexer.Lexer("   \t  \n  ");
        var tokens = lexer.ScanTokens();

        Assert.Single(tokens);
        Assert.Equal(TokenType.Eof, tokens[0].Type);
    }

    #endregion

    #region 数値リテラルテスト

    [Fact]
    public void TestNumberLiteral_Integer()
    {
        var lexer = new Core.Lexer.Lexer("123");
        var tokens = lexer.ScanTokens();

        Assert.Equal(2, tokens.Count); // Number + Eof
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal("123", tokens[0].Lexeme);
        Assert.Equal(123.0, tokens[0].Value);
    }

    [Fact]
    public void TestNumberLiteral_Decimal()
    {
        var lexer = new Core.Lexer.Lexer("123.456");
        var tokens = lexer.ScanTokens();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal("123.456", tokens[0].Lexeme);
        Assert.Equal(123.456, tokens[0].Value);
    }

    [Fact]
    public void TestNumberLiteral_Zero()
    {
        var lexer = new Core.Lexer.Lexer("0");
        var tokens = lexer.ScanTokens();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal(0.0, tokens[0].Value);
    }

    [Fact]
    public void TestNumberLiteral_DecimalStartWithZero()
    {
        var lexer = new Core.Lexer.Lexer("0.5");
        var tokens = lexer.ScanTokens();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal(0.5, tokens[0].Value);
    }

    #endregion

    #region 文字列リテラルテスト

    [Fact]
    public void TestStringLiteral_Simple()
    {
        var lexer = new Core.Lexer.Lexer("\"hello world\"");
        var tokens = lexer.ScanTokens();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.String, tokens[0].Type);
        Assert.Equal("\"hello world\"", tokens[0].Lexeme);
        Assert.Equal("hello world", tokens[0].Value);
    }

    [Fact]
    public void TestStringLiteral_Empty()
    {
        var lexer = new Core.Lexer.Lexer("\"\"");
        var tokens = lexer.ScanTokens();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.String, tokens[0].Type);
        Assert.Equal("", tokens[0].Value);
    }

    [Fact]
    public void TestStringLiteral_WithSpaces()
    {
        var lexer = new Core.Lexer.Lexer("\"  spaces  \"");
        var tokens = lexer.ScanTokens();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.String, tokens[0].Type);
        Assert.Equal("  spaces  ", tokens[0].Value);
    }

    [Fact]
    public void TestStringLiteral_Unterminated()
    {
        var lexer = new Core.Lexer.Lexer("\"unterminated");
        var tokens = lexer.ScanTokens();

        // エラートークンを返す
        Assert.Contains(tokens, t => t.Type == TokenType.Error);
    }

    #endregion

    #region キーワードテスト

    [Fact]
    public void TestKeywords_LetVar()
    {
        var lexer = new Core.Lexer.Lexer("let var");
        var tokens = lexer.ScanTokens();

        Assert.Equal(3, tokens.Count); // let, var, Eof
        Assert.Equal(TokenType.Let, tokens[0].Type);
        Assert.Equal(TokenType.Var, tokens[1].Type);
    }

    [Fact]
    public void TestKeywords_Function()
    {
        var lexer = new Core.Lexer.Lexer("fn");
        var tokens = lexer.ScanTokens();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Fn, tokens[0].Type);
    }

    [Fact]
    public void TestKeywords_Control()
    {
        var lexer = new Core.Lexer.Lexer("if else while return");
        var tokens = lexer.ScanTokens();

        Assert.Equal(5, tokens.Count);
        Assert.Equal(TokenType.If, tokens[0].Type);
        Assert.Equal(TokenType.Else, tokens[1].Type);
        Assert.Equal(TokenType.While, tokens[2].Type);
        Assert.Equal(TokenType.Return, tokens[3].Type);
    }

    [Fact]
    public void TestKeywords_Class()
    {
        var lexer = new Core.Lexer.Lexer("class public private static init");
        var tokens = lexer.ScanTokens();

        Assert.Equal(6, tokens.Count);
        Assert.Equal(TokenType.Class, tokens[0].Type);
        Assert.Equal(TokenType.Public, tokens[1].Type);
        Assert.Equal(TokenType.Private, tokens[2].Type);
        Assert.Equal(TokenType.Static, tokens[3].Type);
        Assert.Equal(TokenType.Init, tokens[4].Type);
    }

    [Fact]
    public void TestKeywords_Boolean()
    {
        var lexer = new Core.Lexer.Lexer("true false null");
        var tokens = lexer.ScanTokens();

        Assert.Equal(4, tokens.Count);
        Assert.Equal(TokenType.True, tokens[0].Type);
        Assert.Equal(TokenType.False, tokens[1].Type);
        Assert.Equal(TokenType.Null, tokens[2].Type);
    }

    [Fact]
    public void TestKeywords_Logical()
    {
        var lexer = new Core.Lexer.Lexer("and or not");
        var tokens = lexer.ScanTokens();

        Assert.Equal(4, tokens.Count);
        Assert.Equal(TokenType.And, tokens[0].Type);
        Assert.Equal(TokenType.Or, tokens[1].Type);
        Assert.Equal(TokenType.Not, tokens[2].Type);
    }

    #endregion

    #region 識別子テスト

    [Fact]
    public void TestIdentifier_Simple()
    {
        var lexer = new Core.Lexer.Lexer("hello");
        var tokens = lexer.ScanTokens();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("hello", tokens[0].Lexeme);
    }

    [Fact]
    public void TestIdentifier_WithUnderscore()
    {
        var lexer = new Core.Lexer.Lexer("_private __internal");
        var tokens = lexer.ScanTokens();

        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("_private", tokens[0].Lexeme);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal("__internal", tokens[1].Lexeme);
    }

    [Fact]
    public void TestIdentifier_WithNumbers()
    {
        var lexer = new Core.Lexer.Lexer("var1 test2var");
        var tokens = lexer.ScanTokens();

        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("var1", tokens[0].Lexeme);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal("test2var", tokens[1].Lexeme);
    }

    #endregion

    #region 演算子テスト

    [Fact]
    public void TestOperators_Arithmetic()
    {
        var lexer = new Core.Lexer.Lexer("+ - * / %");
        var tokens = lexer.ScanTokens();

        Assert.Equal(6, tokens.Count);
        Assert.Equal(TokenType.Plus, tokens[0].Type);
        Assert.Equal(TokenType.Minus, tokens[1].Type);
        Assert.Equal(TokenType.Star, tokens[2].Type);
        Assert.Equal(TokenType.Slash, tokens[3].Type);
        Assert.Equal(TokenType.Percent, tokens[4].Type);
    }

    [Fact]
    public void TestOperators_Comparison()
    {
        var lexer = new Core.Lexer.Lexer("== != < > <= >=");
        var tokens = lexer.ScanTokens();

        Assert.Equal(7, tokens.Count);
        Assert.Equal(TokenType.EqualEqual, tokens[0].Type);
        Assert.Equal(TokenType.BangEqual, tokens[1].Type);
        Assert.Equal(TokenType.Less, tokens[2].Type);
        Assert.Equal(TokenType.Greater, tokens[3].Type);
        Assert.Equal(TokenType.LessEqual, tokens[4].Type);
        Assert.Equal(TokenType.GreaterEqual, tokens[5].Type);
    }

    [Fact]
    public void TestOperators_Assignment()
    {
        var lexer = new Core.Lexer.Lexer("=");
        var tokens = lexer.ScanTokens();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Equal, tokens[0].Type);
    }

    [Fact]
    public void TestOperators_EqualVsEqualEqual()
    {
        var lexer = new Core.Lexer.Lexer("= ==");
        var tokens = lexer.ScanTokens();

        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Equal, tokens[0].Type);
        Assert.Equal(TokenType.EqualEqual, tokens[1].Type);
    }

    #endregion

    #region 区切り文字テスト

    [Fact]
    public void TestDelimiters_Parentheses()
    {
        var lexer = new Core.Lexer.Lexer("( )");
        var tokens = lexer.ScanTokens();

        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.LeftParen, tokens[0].Type);
        Assert.Equal(TokenType.RightParen, tokens[1].Type);
    }

    [Fact]
    public void TestDelimiters_Braces()
    {
        var lexer = new Core.Lexer.Lexer("{ }");
        var tokens = lexer.ScanTokens();

        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.LeftBrace, tokens[0].Type);
        Assert.Equal(TokenType.RightBrace, tokens[1].Type);
    }

    [Fact]
    public void TestDelimiters_Brackets()
    {
        var lexer = new Core.Lexer.Lexer("[ ]");
        var tokens = lexer.ScanTokens();

        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.LeftBracket, tokens[0].Type);
        Assert.Equal(TokenType.RightBracket, tokens[1].Type);
    }

    [Fact]
    public void TestDelimiters_CommaDot()
    {
        var lexer = new Core.Lexer.Lexer(", .");
        var tokens = lexer.ScanTokens();

        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Comma, tokens[0].Type);
        Assert.Equal(TokenType.Dot, tokens[1].Type);
    }

    #endregion

    #region コメントテスト

    [Fact]
    public void TestComment_SingleLine()
    {
        var lexer = new Core.Lexer.Lexer("let x = 5 // this is a comment");
        var tokens = lexer.ScanTokens();

        // コメントはスキップされる
        Assert.Equal(5, tokens.Count); // let, x, =, 5, Eof
        Assert.Equal(TokenType.Let, tokens[0].Type);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal(TokenType.Equal, tokens[2].Type);
        Assert.Equal(TokenType.Number, tokens[3].Type);
    }

    [Fact]
    public void TestComment_OnlyComment()
    {
        var lexer = new Core.Lexer.Lexer("// this is a comment");
        var tokens = lexer.ScanTokens();

        Assert.Single(tokens);
        Assert.Equal(TokenType.Eof, tokens[0].Type);
    }

    #endregion

    #region 位置情報テスト

    [Fact]
    public void TestPosition_LineAndColumn()
    {
        var lexer = new Core.Lexer.Lexer("let x = 5");
        var tokens = lexer.ScanTokens();

        Assert.Equal(1, tokens[0].Line);
        Assert.Equal(1, tokens[0].Column);

        Assert.Equal(1, tokens[1].Line);
        Assert.Equal(5, tokens[1].Column);
    }

    [Fact]
    public void TestPosition_Multiline()
    {
        var source = @"let x = 5
var y = 10";
        var lexer = new Core.Lexer.Lexer(source);
        var tokens = lexer.ScanTokens();

        // 1行目のトークン
        Assert.Equal(1, tokens[0].Line); // let
        Assert.Equal(1, tokens[1].Line); // x

        // 2行目のトークン
        Assert.Equal(2, tokens[4].Line); // var
        Assert.Equal(2, tokens[5].Line); // y
    }

    #endregion

    #region 複合テスト

    [Fact]
    public void TestComplex_VariableDeclaration()
    {
        var lexer = new Core.Lexer.Lexer("let x = 10");
        var tokens = lexer.ScanTokens();

        Assert.Equal(5, tokens.Count); // let, x, =, 10, Eof
        Assert.Equal(TokenType.Let, tokens[0].Type);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal("x", tokens[1].Lexeme);
        Assert.Equal(TokenType.Equal, tokens[2].Type);
        Assert.Equal(TokenType.Number, tokens[3].Type);
        Assert.Equal(10.0, tokens[3].Value);
        Assert.Equal(TokenType.Eof, tokens[4].Type);
    }

    [Fact]
    public void TestComplex_FunctionCall()
    {
        var lexer = new Core.Lexer.Lexer("print(\"hello\")");
        var tokens = lexer.ScanTokens();

        Assert.Equal(5, tokens.Count); // print, (, "hello", ), Eof
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("print", tokens[0].Lexeme);
        Assert.Equal(TokenType.LeftParen, tokens[1].Type);
        Assert.Equal(TokenType.String, tokens[2].Type);
        Assert.Equal("hello", tokens[2].Value);
        Assert.Equal(TokenType.RightParen, tokens[3].Type);
    }

    [Fact]
    public void TestComplex_Expression()
    {
        var lexer = new Core.Lexer.Lexer("x + y * 2");
        var tokens = lexer.ScanTokens();

        Assert.Equal(6, tokens.Count); // x, +, y, *, 2, Eof
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal(TokenType.Plus, tokens[1].Type);
        Assert.Equal(TokenType.Identifier, tokens[2].Type);
        Assert.Equal(TokenType.Star, tokens[3].Type);
        Assert.Equal(TokenType.Number, tokens[4].Type);
    }

    [Fact]
    public void TestComplex_IfStatement()
    {
        var source = "if (x > 5) { return true } else { return false }";
        var lexer = new Core.Lexer.Lexer(source);
        var tokens = lexer.ScanTokens();

        Assert.Equal(TokenType.If, tokens[0].Type);
        Assert.Equal(TokenType.LeftParen, tokens[1].Type);
        Assert.Equal(TokenType.Identifier, tokens[2].Type);
        Assert.Equal(TokenType.Greater, tokens[3].Type);
        Assert.Equal(TokenType.Number, tokens[4].Type);
        Assert.Equal(TokenType.RightParen, tokens[5].Type);
        Assert.Equal(TokenType.LeftBrace, tokens[6].Type);
        Assert.Equal(TokenType.Return, tokens[7].Type);
        Assert.Equal(TokenType.True, tokens[8].Type);
        Assert.Equal(TokenType.RightBrace, tokens[9].Type);
        Assert.Equal(TokenType.Else, tokens[10].Type);
    }

    [Fact]
    public void TestComplex_MemberAccess()
    {
        var lexer = new Core.Lexer.Lexer("obj.method()");
        var tokens = lexer.ScanTokens();

        Assert.Equal(6, tokens.Count); // obj, ., method, (, ), Eof
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal(TokenType.Dot, tokens[1].Type);
        Assert.Equal(TokenType.Identifier, tokens[2].Type);
        Assert.Equal(TokenType.LeftParen, tokens[3].Type);
        Assert.Equal(TokenType.RightParen, tokens[4].Type);
    }

    #endregion
}
