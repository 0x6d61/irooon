using Irooon.Core.Lexer;
using Xunit;
using System.Linq;

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
        var lexer = new Core.Lexer.Lexer("if else for return");
        var tokens = lexer.ScanTokens();

        Assert.Equal(5, tokens.Count);
        Assert.Equal(TokenType.If, tokens[0].Type);
        Assert.Equal(TokenType.Else, tokens[1].Type);
        Assert.Equal(TokenType.For, tokens[2].Type);
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
    public void TestKeywords_Inheritance()
    {
        var lexer = new Core.Lexer.Lexer("extends super");
        var tokens = lexer.ScanTokens();

        Assert.Equal(3, tokens.Count); // extends, super, Eof
        Assert.Equal(TokenType.Extends, tokens[0].Type);
        Assert.Equal("extends", tokens[0].Lexeme);
        Assert.Equal(TokenType.Super, tokens[1].Type);
        Assert.Equal("super", tokens[1].Lexeme);
    }

    [Fact]
    public void TestKeywords_ClassInheritance()
    {
        var lexer = new Core.Lexer.Lexer("class Dog extends Animal");
        var tokens = lexer.ScanTokens();

        Assert.Equal(5, tokens.Count); // class, Dog, extends, Animal, Eof
        Assert.Equal(TokenType.Class, tokens[0].Type);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal("Dog", tokens[1].Lexeme);
        Assert.Equal(TokenType.Extends, tokens[2].Type);
        Assert.Equal(TokenType.Identifier, tokens[3].Type);
        Assert.Equal("Animal", tokens[3].Lexeme);
    }

    [Fact]
    public void TestKeywords_SuperMethodCall()
    {
        var lexer = new Core.Lexer.Lexer("super.speak()");
        var tokens = lexer.ScanTokens();

        Assert.Equal(6, tokens.Count); // super, ., speak, (, ), Eof
        Assert.Equal(TokenType.Super, tokens[0].Type);
        Assert.Equal(TokenType.Dot, tokens[1].Type);
        Assert.Equal(TokenType.Identifier, tokens[2].Type);
        Assert.Equal("speak", tokens[2].Lexeme);
        Assert.Equal(TokenType.LeftParen, tokens[3].Type);
        Assert.Equal(TokenType.RightParen, tokens[4].Type);
    }

    [Fact]
    public void TestKeywords_Exception()
    {
        var lexer = new Core.Lexer.Lexer("try catch finally throw");
        var tokens = lexer.ScanTokens();

        Assert.Equal(5, tokens.Count);
        Assert.Equal(TokenType.Try, tokens[0].Type);
        Assert.Equal(TokenType.Catch, tokens[1].Type);
        Assert.Equal(TokenType.Finally, tokens[2].Type);
        Assert.Equal(TokenType.Throw, tokens[3].Type);
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

    [Fact]
    public void TestKeywords_Module()
    {
        var lexer = new Core.Lexer.Lexer("import export from");
        var tokens = lexer.ScanTokens();

        Assert.Equal(4, tokens.Count);
        Assert.Equal(TokenType.Import, tokens[0].Type);
        Assert.Equal(TokenType.Export, tokens[1].Type);
        Assert.Equal(TokenType.From, tokens[2].Type);
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

    [Fact]
    public void TestDelimiters_Colon()
    {
        var lexer = new Core.Lexer.Lexer(":");
        var tokens = lexer.ScanTokens();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Colon, tokens[0].Type);
        Assert.Equal(":", tokens[0].Lexeme);
    }

    [Fact]
    public void TestDelimiters_ColonInHash()
    {
        var lexer = new Core.Lexer.Lexer("{ \"key\" : \"value\" }");
        var tokens = lexer.ScanTokens();

        Assert.Equal(6, tokens.Count); // {, "key", :, "value", }, Eof
        Assert.Equal(TokenType.LeftBrace, tokens[0].Type);
        Assert.Equal(TokenType.String, tokens[1].Type);
        Assert.Equal(TokenType.Colon, tokens[2].Type);
        Assert.Equal(TokenType.String, tokens[3].Type);
        Assert.Equal(TokenType.RightBrace, tokens[4].Type);
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

    #region バッククォート文字列テスト

    [Fact]
    public void TestBacktickString_Simple()
    {
        var lexer = new Core.Lexer.Lexer("`echo Hello`");
        var tokens = lexer.ScanTokens();

        Assert.Equal(2, tokens.Count); // Backtick + Eof
        Assert.Equal(TokenType.Backtick, tokens[0].Type);
        Assert.Equal("echo Hello", tokens[0].Value);
    }

    [Fact]
    public void TestBacktickString_Empty()
    {
        var lexer = new Core.Lexer.Lexer("``");
        var tokens = lexer.ScanTokens();

        Assert.Equal(2, tokens.Count); // Backtick + Eof
        Assert.Equal(TokenType.Backtick, tokens[0].Type);
        Assert.Equal("", tokens[0].Value);
    }

    [Fact]
    public void TestBacktickString_Multiline()
    {
        var source = @"`
git add .
git commit -m ""Update""
git push
`";
        var lexer = new Core.Lexer.Lexer(source);
        var tokens = lexer.ScanTokens();

        Assert.Equal(2, tokens.Count); // Backtick + Eof
        Assert.Equal(TokenType.Backtick, tokens[0].Type);
        var value = tokens[0].Value?.ToString() ?? "";
        Assert.Contains("git add", value);
        Assert.Contains("git commit", value);
        Assert.Contains("git push", value);
    }

    [Fact]
    public void TestBacktickString_Unterminated_ReturnsErrorToken()
    {
        var lexer = new Core.Lexer.Lexer("`echo Hello");
        var tokens = lexer.ScanTokens();

        // Errorトークンが含まれていることを確認
        Assert.Contains(tokens, t => t.Type == TokenType.Error);
        var errorToken = tokens.First(t => t.Type == TokenType.Error);
        Assert.Contains("Unterminated backtick string", errorToken.Value?.ToString() ?? "");
    }

    [Fact]
    public void TestDollarBacktick_ShellCommand()
    {
        var lexer = new Core.Lexer.Lexer("$`echo test`");
        var tokens = lexer.ScanTokens();

        Assert.Equal(3, tokens.Count); // Dollar + Backtick + Eof
        Assert.Equal(TokenType.Dollar, tokens[0].Type);
        Assert.Equal(TokenType.Backtick, tokens[1].Type);
        Assert.Equal("echo test", tokens[1].Value);
    }

    [Fact]
    public void TestDollarBacktick_WithVariableAssignment()
    {
        var lexer = new Core.Lexer.Lexer("let output = $`pwd`");
        var tokens = lexer.ScanTokens();

        // let, output, =, $, `pwd`, Eof
        Assert.Equal(6, tokens.Count);
        Assert.Equal(TokenType.Let, tokens[0].Type);
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal(TokenType.Equal, tokens[2].Type);
        Assert.Equal(TokenType.Dollar, tokens[3].Type);
        Assert.Equal(TokenType.Backtick, tokens[4].Type);
        Assert.Equal("pwd", tokens[4].Value);
    }

    #endregion

    #region 便利な演算子テスト（v0.5.6）

    [Fact]
    public void TestOperators_Ternary()
    {
        var lexer = new Core.Lexer.Lexer("x > 5 ? 10 : 20");
        var tokens = lexer.ScanTokens();

        Assert.Equal(8, tokens.Count); // x, >, 5, ?, 10, :, 20, Eof
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal(TokenType.Greater, tokens[1].Type);
        Assert.Equal(TokenType.Number, tokens[2].Type);
        Assert.Equal(TokenType.Question, tokens[3].Type);
        Assert.Equal(TokenType.Number, tokens[4].Type);
        Assert.Equal(TokenType.Colon, tokens[5].Type);
        Assert.Equal(TokenType.Number, tokens[6].Type);
    }

    [Fact]
    public void TestOperators_NullCoalescing()
    {
        var lexer = new Core.Lexer.Lexer("x ?? 0");
        var tokens = lexer.ScanTokens();

        Assert.Equal(4, tokens.Count); // x, ??, 0, Eof
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal(TokenType.QuestionQuestion, tokens[1].Type);
        Assert.Equal(TokenType.Number, tokens[2].Type);
    }

    [Fact]
    public void TestOperators_IncrementDecrement()
    {
        var lexer = new Core.Lexer.Lexer("x++ y--");
        var tokens = lexer.ScanTokens();

        Assert.Equal(5, tokens.Count); // x, ++, y, --, Eof
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal(TokenType.PlusPlus, tokens[1].Type);
        Assert.Equal(TokenType.Identifier, tokens[2].Type);
        Assert.Equal(TokenType.MinusMinus, tokens[3].Type);
    }

    [Fact]
    public void TestOperators_SafeNavigation()
    {
        var lexer = new Core.Lexer.Lexer("obj?.method");
        var tokens = lexer.ScanTokens();

        Assert.Equal(4, tokens.Count); // obj, ?., method, Eof
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal(TokenType.QuestionDot, tokens[1].Type);
        Assert.Equal(TokenType.Identifier, tokens[2].Type);
    }

    [Fact]
    public void TestOperators_Disambiguation_PlusVsPlusPlusVsPlusEqual()
    {
        var lexer = new Core.Lexer.Lexer("+ ++ +=");
        var tokens = lexer.ScanTokens();

        Assert.Equal(4, tokens.Count); // +, ++, +=, Eof
        Assert.Equal(TokenType.Plus, tokens[0].Type);
        Assert.Equal(TokenType.PlusPlus, tokens[1].Type);
        Assert.Equal(TokenType.PlusEqual, tokens[2].Type);
    }

    [Fact]
    public void TestOperators_Disambiguation_MinusVsMinusMinusVsMinusEqual()
    {
        var lexer = new Core.Lexer.Lexer("- -- -=");
        var tokens = lexer.ScanTokens();

        Assert.Equal(4, tokens.Count); // -, --, -=, Eof
        Assert.Equal(TokenType.Minus, tokens[0].Type);
        Assert.Equal(TokenType.MinusMinus, tokens[1].Type);
        Assert.Equal(TokenType.MinusEqual, tokens[2].Type);
    }

    [Fact]
    public void TestOperators_Disambiguation_QuestionVsQuestionQuestionVsQuestionDot()
    {
        var lexer = new Core.Lexer.Lexer("? ?? ?.");
        var tokens = lexer.ScanTokens();

        Assert.Equal(4, tokens.Count); // ?, ??, ?., Eof
        Assert.Equal(TokenType.Question, tokens[0].Type);
        Assert.Equal(TokenType.QuestionQuestion, tokens[1].Type);
        Assert.Equal(TokenType.QuestionDot, tokens[2].Type);
    }

    [Fact]
    public void TestOperators_ComplexExpression_WithNewOperators()
    {
        var lexer = new Core.Lexer.Lexer("x++ ?? y?.value");
        var tokens = lexer.ScanTokens();

        Assert.Equal(7, tokens.Count); // x, ++, ??, y, ?., value, Eof
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("x", tokens[0].Lexeme);
        Assert.Equal(TokenType.PlusPlus, tokens[1].Type);
        Assert.Equal(TokenType.QuestionQuestion, tokens[2].Type);
        Assert.Equal(TokenType.Identifier, tokens[3].Type);
        Assert.Equal("y", tokens[3].Lexeme);
        Assert.Equal(TokenType.QuestionDot, tokens[4].Type);
        Assert.Equal(TokenType.Identifier, tokens[5].Type);
        Assert.Equal("value", tokens[5].Lexeme);
    }

    [Fact]
    public void TestOperators_EdgeCase_TriplePlus()
    {
        var lexer = new Core.Lexer.Lexer("+++");
        var tokens = lexer.ScanTokens();

        // ++ と + に分割される
        Assert.Equal(3, tokens.Count); // ++, +, Eof
        Assert.Equal(TokenType.PlusPlus, tokens[0].Type);
        Assert.Equal(TokenType.Plus, tokens[1].Type);
    }

    [Fact]
    public void TestOperators_EdgeCase_TripleMinus()
    {
        var lexer = new Core.Lexer.Lexer("---");
        var tokens = lexer.ScanTokens();

        // -- と - に分割される
        Assert.Equal(3, tokens.Count); // --, -, Eof
        Assert.Equal(TokenType.MinusMinus, tokens[0].Type);
        Assert.Equal(TokenType.Minus, tokens[1].Type);
    }

    [Fact]
    public void TestOperators_EdgeCase_TripleQuestion()
    {
        var lexer = new Core.Lexer.Lexer("???");
        var tokens = lexer.ScanTokens();

        // ?? と ? に分割される
        Assert.Equal(3, tokens.Count); // ??, ?, Eof
        Assert.Equal(TokenType.QuestionQuestion, tokens[0].Type);
        Assert.Equal(TokenType.Question, tokens[1].Type);
    }

    #endregion

    #region 16進数・科学的記数法テスト

    [Fact]
    public void TestHexNumber()
    {
        var lexer = new Core.Lexer.Lexer("0xFF");
        var tokens = lexer.ScanTokens();
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal(255.0, tokens[0].Value);
    }

    [Fact]
    public void TestHexNumber_Lowercase()
    {
        var lexer = new Core.Lexer.Lexer("0x1a");
        var tokens = lexer.ScanTokens();
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal(26.0, tokens[0].Value);
    }

    [Fact]
    public void TestScientificNotation()
    {
        var lexer = new Core.Lexer.Lexer("1.5e10");
        var tokens = lexer.ScanTokens();
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal(1.5e10, tokens[0].Value);
    }

    [Fact]
    public void TestScientificNotation_Negative()
    {
        var lexer = new Core.Lexer.Lexer("2.3e-5");
        var tokens = lexer.ScanTokens();
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal(2.3e-5, tokens[0].Value);
    }

    [Fact]
    public void TestScientificNotation_UpperE()
    {
        var lexer = new Core.Lexer.Lexer("1E3");
        var tokens = lexer.ScanTokens();
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal(1000.0, tokens[0].Value);
    }

    #endregion

    #region 複数行コメントテスト

    [Fact]
    public void TestBlockComment_Simple()
    {
        var lexer = new Core.Lexer.Lexer("/* comment */ 42");
        var tokens = lexer.ScanTokens();
        Assert.Equal(2, tokens.Count); // 42, Eof
        Assert.Equal(TokenType.Number, tokens[0].Type);
    }

    [Fact]
    public void TestBlockComment_Multiline()
    {
        var lexer = new Core.Lexer.Lexer("/* line1\nline2\nline3 */ 42");
        var tokens = lexer.ScanTokens();
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
    }

    [Fact]
    public void TestBlockComment_Empty()
    {
        var lexer = new Core.Lexer.Lexer("/**/ 42");
        var tokens = lexer.ScanTokens();
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
    }

    [Fact]
    public void TestBlockComment_Unterminated()
    {
        var lexer = new Core.Lexer.Lexer("/* unterminated");
        var tokens = lexer.ScanTokens();
        Assert.Contains(tokens, t => t.Type == TokenType.Error);
    }

    [Fact]
    public void TestBlockComment_BetweenTokens()
    {
        var lexer = new Core.Lexer.Lexer("1 /* + 2 */ + 3");
        var tokens = lexer.ScanTokens();
        Assert.Equal(4, tokens.Count); // 1, +, 3, Eof
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal(TokenType.Plus, tokens[1].Type);
        Assert.Equal(TokenType.Number, tokens[2].Type);
    }

    #endregion

    #region エスケープシーケンステスト

    [Fact]
    public void TestEscapeSequence_DoubleQuote()
    {
        // irooonソース: "say \"hi\""
        var lexer = new Core.Lexer.Lexer("\"say \\\"hi\\\"\"");
        var tokens = lexer.ScanTokens();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.String, tokens[0].Type);
        Assert.Equal("say \"hi\"", tokens[0].Value);
    }

    [Fact]
    public void TestEscapeSequence_Backslash()
    {
        // irooonソース: "path\\file"
        var lexer = new Core.Lexer.Lexer("\"path\\\\file\"");
        var tokens = lexer.ScanTokens();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.String, tokens[0].Type);
        Assert.Equal("path\\file", tokens[0].Value);
    }

    [Fact]
    public void TestEscapeSequence_Newline()
    {
        // irooonソース: "line1\nline2"
        var lexer = new Core.Lexer.Lexer("\"line1\\nline2\"");
        var tokens = lexer.ScanTokens();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.String, tokens[0].Type);
        Assert.Equal("line1\nline2", tokens[0].Value);
    }

    [Fact]
    public void TestEscapeSequence_Tab()
    {
        // irooonソース: "col1\tcol2"
        var lexer = new Core.Lexer.Lexer("\"col1\\tcol2\"");
        var tokens = lexer.ScanTokens();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.String, tokens[0].Type);
        Assert.Equal("col1\tcol2", tokens[0].Value);
    }

    [Fact]
    public void TestEscapeSequence_CarriageReturn()
    {
        // irooonソース: "a\rb"
        var lexer = new Core.Lexer.Lexer("\"a\\rb\"");
        var tokens = lexer.ScanTokens();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.String, tokens[0].Type);
        Assert.Equal("a\rb", tokens[0].Value);
    }

    [Fact]
    public void TestEscapeSequence_NullChar()
    {
        // irooonソース: "a\0b"
        var lexer = new Core.Lexer.Lexer("\"a\\0b\"");
        var tokens = lexer.ScanTokens();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.String, tokens[0].Type);
        Assert.Equal("a\0b", tokens[0].Value);
    }

    [Fact]
    public void TestEscapeSequence_Dollar()
    {
        // irooonソース: "\${name}" → 文字列 "${name}"（補間ではない）
        var lexer = new Core.Lexer.Lexer("\"\\${name}\"");
        var tokens = lexer.ScanTokens();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.String, tokens[0].Type);
        // \$ はプレースホルダ \uE000 に変換され、Parser段階で $ に戻る
        // Lexer段階では \uE000 が入っている
        Assert.Contains('\uE000', (string)tokens[0].Value);
    }

    [Fact]
    public void TestEscapeSequence_Multiple()
    {
        // irooonソース: "a\tb\nc"
        var lexer = new Core.Lexer.Lexer("\"a\\tb\\nc\"");
        var tokens = lexer.ScanTokens();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.String, tokens[0].Type);
        Assert.Equal("a\tb\nc", tokens[0].Value);
    }

    [Fact]
    public void TestEscapeSequence_BackslashAtEnd()
    {
        // irooonソース: "abc\" → バックスラッシュ+引用符でエスケープ済み → 未終端文字列
        // 実際は "abc" + エスケープされた " → 未終端
        var lexer = new Core.Lexer.Lexer("\"abc\\\"");
        var tokens = lexer.ScanTokens();

        // \" がエスケープされて文字列が閉じない
        Assert.Contains(tokens, t => t.Type == TokenType.Error);
    }

    [Fact]
    public void TestEscapeSequence_Invalid()
    {
        // irooonソース: "hello\q" → 無効なエスケープ
        var lexer = new Core.Lexer.Lexer("\"hello\\q\"");
        var tokens = lexer.ScanTokens();

        Assert.Contains(tokens, t => t.Type == TokenType.Error);
    }

    [Fact]
    public void TestEscapeSequence_OnlyEscapes()
    {
        // irooonソース: "\\\"\n\t"
        var lexer = new Core.Lexer.Lexer("\"\\\\\\\"\\n\\t\"");
        var tokens = lexer.ScanTokens();

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.String, tokens[0].Type);
        Assert.Equal("\\\"\n\t", tokens[0].Value);
    }

    #endregion
}
