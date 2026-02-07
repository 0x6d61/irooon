using Irooon.Core.Ast;
using Irooon.Core.Ast.Expressions;
using Irooon.Core.Ast.Statements;
using Irooon.Core.Lexer;
using Irooon.Core.Parser;
using Xunit;

namespace Irooon.Tests.Parser;

/// <summary>
/// Parserの式パース機能のテスト。
/// </summary>
public class ParserExprTests
{
    #region リテラルのテスト

    [Fact]
    public void TestParseNumberLiteral()
    {
        // 数値リテラル: 123
        var tokens = new Core.Lexer.Lexer("123").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.Empty(ast.Statements);
        Assert.NotNull(ast.Expression);
        Assert.IsType<LiteralExpr>(ast.Expression);
        var literal = (LiteralExpr)ast.Expression;
        Assert.Equal(123.0, literal.Value);
    }

    [Fact]
    public void TestParseFloatLiteral()
    {
        // 小数リテラル: 45.6
        var tokens = new Core.Lexer.Lexer("45.6").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<LiteralExpr>(ast.Expression);
        var literal = (LiteralExpr)ast.Expression;
        Assert.Equal(45.6, literal.Value);
    }

    [Fact]
    public void TestParseStringLiteral()
    {
        // 文字列リテラル: "hello"
        var tokens = new Core.Lexer.Lexer("\"hello\"").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<LiteralExpr>(ast.Expression);
        var literal = (LiteralExpr)ast.Expression;
        Assert.Equal("hello", literal.Value);
    }

    [Fact]
    public void TestParseTrueLiteral()
    {
        // 真偽値リテラル: true
        var tokens = new Core.Lexer.Lexer("true").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<LiteralExpr>(ast.Expression);
        var literal = (LiteralExpr)ast.Expression;
        Assert.Equal(true, literal.Value);
    }

    [Fact]
    public void TestParseFalseLiteral()
    {
        // 真偽値リテラル: false
        var tokens = new Core.Lexer.Lexer("false").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<LiteralExpr>(ast.Expression);
        var literal = (LiteralExpr)ast.Expression;
        Assert.Equal(false, literal.Value);
    }

    [Fact]
    public void TestParseNullLiteral()
    {
        // nullリテラル: null
        var tokens = new Core.Lexer.Lexer("null").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<LiteralExpr>(ast.Expression);
        var literal = (LiteralExpr)ast.Expression;
        Assert.Null(literal.Value);
    }

    #endregion

    #region 識別子のテスト

    [Fact]
    public void TestParseIdentifier()
    {
        // 識別子: x
        var tokens = new Core.Lexer.Lexer("x").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<IdentifierExpr>(ast.Expression);
        var ident = (IdentifierExpr)ast.Expression;
        Assert.Equal("x", ident.Name);
    }

    #endregion

    #region 括弧式のテスト

    [Fact]
    public void TestParseParentheses()
    {
        // 括弧式: (42)
        var tokens = new Core.Lexer.Lexer("(42)").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<LiteralExpr>(ast.Expression);
        var literal = (LiteralExpr)ast.Expression;
        Assert.Equal(42.0, literal.Value);
    }

    #endregion

    #region 単項演算子のテスト

    [Fact]
    public void TestParseUnaryMinus()
    {
        // 単項マイナス: -10
        var tokens = new Core.Lexer.Lexer("-10").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<UnaryExpr>(ast.Expression);
        var unary = (UnaryExpr)ast.Expression;
        Assert.Equal(TokenType.Minus, unary.Operator);
        Assert.IsType<LiteralExpr>(unary.Operand);
        var literal = (LiteralExpr)unary.Operand;
        Assert.Equal(10.0, literal.Value);
    }

    [Fact]
    public void TestParseUnaryNot()
    {
        // 単項not: not true
        var tokens = new Core.Lexer.Lexer("not true").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<UnaryExpr>(ast.Expression);
        var unary = (UnaryExpr)ast.Expression;
        Assert.Equal(TokenType.Not, unary.Operator);
        Assert.IsType<LiteralExpr>(unary.Operand);
        var literal = (LiteralExpr)unary.Operand;
        Assert.Equal(true, literal.Value);
    }

    #endregion

    #region 二項演算子のテスト

    [Fact]
    public void TestParseBinaryAdd()
    {
        // 加算: 1 + 2
        var tokens = new Core.Lexer.Lexer("1 + 2").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<BinaryExpr>(ast.Expression);
        var binary = (BinaryExpr)ast.Expression;
        Assert.Equal(TokenType.Plus, binary.Operator);

        Assert.IsType<LiteralExpr>(binary.Left);
        var left = (LiteralExpr)binary.Left;
        Assert.Equal(1.0, left.Value);

        Assert.IsType<LiteralExpr>(binary.Right);
        var right = (LiteralExpr)binary.Right;
        Assert.Equal(2.0, right.Value);
    }

    [Fact]
    public void TestParseBinarySubtract()
    {
        // 減算: 5 - 3
        var tokens = new Core.Lexer.Lexer("5 - 3").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<BinaryExpr>(ast.Expression);
        var binary = (BinaryExpr)ast.Expression;
        Assert.Equal(TokenType.Minus, binary.Operator);
    }

    [Fact]
    public void TestParseBinaryMultiply()
    {
        // 乗算: 3 * 4
        var tokens = new Core.Lexer.Lexer("3 * 4").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<BinaryExpr>(ast.Expression);
        var binary = (BinaryExpr)ast.Expression;
        Assert.Equal(TokenType.Star, binary.Operator);
    }

    [Fact]
    public void TestParseBinaryDivide()
    {
        // 除算: 10 / 2
        var tokens = new Core.Lexer.Lexer("10 / 2").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<BinaryExpr>(ast.Expression);
        var binary = (BinaryExpr)ast.Expression;
        Assert.Equal(TokenType.Slash, binary.Operator);
    }

    [Fact]
    public void TestParseBinaryModulo()
    {
        // 剰余: 10 % 3
        var tokens = new Core.Lexer.Lexer("10 % 3").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<BinaryExpr>(ast.Expression);
        var binary = (BinaryExpr)ast.Expression;
        Assert.Equal(TokenType.Percent, binary.Operator);
    }

    #endregion

    #region 比較演算子のテスト

    [Fact]
    public void TestParseComparison_Equal()
    {
        // 等値: 1 == 2
        var tokens = new Core.Lexer.Lexer("1 == 2").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<BinaryExpr>(ast.Expression);
        var binary = (BinaryExpr)ast.Expression;
        Assert.Equal(TokenType.EqualEqual, binary.Operator);
    }

    [Fact]
    public void TestParseComparison_NotEqual()
    {
        // 不等値: 1 != 2
        var tokens = new Core.Lexer.Lexer("1 != 2").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<BinaryExpr>(ast.Expression);
        var binary = (BinaryExpr)ast.Expression;
        Assert.Equal(TokenType.BangEqual, binary.Operator);
    }

    [Fact]
    public void TestParseComparison_Less()
    {
        // 小なり: 1 < 2
        var tokens = new Core.Lexer.Lexer("1 < 2").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<BinaryExpr>(ast.Expression);
        var binary = (BinaryExpr)ast.Expression;
        Assert.Equal(TokenType.Less, binary.Operator);
    }

    [Fact]
    public void TestParseComparison_LessEqual()
    {
        // 小なりイコール: 1 <= 2
        var tokens = new Core.Lexer.Lexer("1 <= 2").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<BinaryExpr>(ast.Expression);
        var binary = (BinaryExpr)ast.Expression;
        Assert.Equal(TokenType.LessEqual, binary.Operator);
    }

    [Fact]
    public void TestParseComparison_Greater()
    {
        // 大なり: 1 > 2
        var tokens = new Core.Lexer.Lexer("1 > 2").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<BinaryExpr>(ast.Expression);
        var binary = (BinaryExpr)ast.Expression;
        Assert.Equal(TokenType.Greater, binary.Operator);
    }

    [Fact]
    public void TestParseComparison_GreaterEqual()
    {
        // 大なりイコール: 1 >= 2
        var tokens = new Core.Lexer.Lexer("1 >= 2").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<BinaryExpr>(ast.Expression);
        var binary = (BinaryExpr)ast.Expression;
        Assert.Equal(TokenType.GreaterEqual, binary.Operator);
    }

    #endregion

    #region 論理演算子のテスト

    [Fact]
    public void TestParseLogicalAnd()
    {
        // 論理積: true and false
        var tokens = new Core.Lexer.Lexer("true and false").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<BinaryExpr>(ast.Expression);
        var binary = (BinaryExpr)ast.Expression;
        Assert.Equal(TokenType.And, binary.Operator);
    }

    [Fact]
    public void TestParseLogicalOr()
    {
        // 論理和: true or false
        var tokens = new Core.Lexer.Lexer("true or false").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<BinaryExpr>(ast.Expression);
        var binary = (BinaryExpr)ast.Expression;
        Assert.Equal(TokenType.Or, binary.Operator);
    }

    #endregion

    #region 演算子の優先順位のテスト

    [Fact]
    public void TestParseOperatorPrecedence_MultiplyBeforeAdd()
    {
        // 1 + 2 * 3 は 1 + (2 * 3) になること
        var tokens = new Core.Lexer.Lexer("1 + 2 * 3").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<BinaryExpr>(ast.Expression);
        var binary = (BinaryExpr)ast.Expression;
        Assert.Equal(TokenType.Plus, binary.Operator);

        // 左は 1
        Assert.IsType<LiteralExpr>(binary.Left);
        var left = (LiteralExpr)binary.Left;
        Assert.Equal(1.0, left.Value);

        // 右は 2 * 3
        Assert.IsType<BinaryExpr>(binary.Right);
        var rightBinary = (BinaryExpr)binary.Right;
        Assert.Equal(TokenType.Star, rightBinary.Operator);

        var rightLeft = (LiteralExpr)rightBinary.Left;
        Assert.Equal(2.0, rightLeft.Value);

        var rightRight = (LiteralExpr)rightBinary.Right;
        Assert.Equal(3.0, rightRight.Value);
    }

    [Fact]
    public void TestParseOperatorPrecedence_ParenthesesOverride()
    {
        // (1 + 2) * 3 は (1 + 2) が先に評価されること
        var tokens = new Core.Lexer.Lexer("(1 + 2) * 3").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<BinaryExpr>(ast.Expression);
        var binary = (BinaryExpr)ast.Expression;
        Assert.Equal(TokenType.Star, binary.Operator);

        // 左は 1 + 2
        Assert.IsType<BinaryExpr>(binary.Left);
        var leftBinary = (BinaryExpr)binary.Left;
        Assert.Equal(TokenType.Plus, leftBinary.Operator);

        // 右は 3
        Assert.IsType<LiteralExpr>(binary.Right);
        var right = (LiteralExpr)binary.Right;
        Assert.Equal(3.0, right.Value);
    }

    [Fact]
    public void TestParseOperatorPrecedence_ComparisonBeforeLogical()
    {
        // 1 < 2 and 3 > 2 は (1 < 2) and (3 > 2) になること
        var tokens = new Core.Lexer.Lexer("1 < 2 and 3 > 2").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<BinaryExpr>(ast.Expression);
        var binary = (BinaryExpr)ast.Expression;
        Assert.Equal(TokenType.And, binary.Operator);

        // 左は 1 < 2
        Assert.IsType<BinaryExpr>(binary.Left);
        var leftBinary = (BinaryExpr)binary.Left;
        Assert.Equal(TokenType.Less, leftBinary.Operator);

        // 右は 3 > 2
        Assert.IsType<BinaryExpr>(binary.Right);
        var rightBinary = (BinaryExpr)binary.Right;
        Assert.Equal(TokenType.Greater, rightBinary.Operator);
    }

    #endregion

    #region 代入式のテスト

    [Fact]
    public void TestParseAssignment()
    {
        // x = 42
        var tokens = new Core.Lexer.Lexer("x = 42").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<AssignExpr>(ast.Expression);
        var assign = (AssignExpr)ast.Expression;
        Assert.Equal("x", assign.Name);

        Assert.IsType<LiteralExpr>(assign.Value);
        var value = (LiteralExpr)assign.Value;
        Assert.Equal(42.0, value.Value);
    }

    [Fact]
    public void TestParseAssignment_ChainedExpression()
    {
        // x = 1 + 2
        var tokens = new Core.Lexer.Lexer("x = 1 + 2").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<AssignExpr>(ast.Expression);
        var assign = (AssignExpr)ast.Expression;
        Assert.Equal("x", assign.Name);

        // 値は 1 + 2
        Assert.IsType<BinaryExpr>(assign.Value);
        var binary = (BinaryExpr)assign.Value;
        Assert.Equal(TokenType.Plus, binary.Operator);
    }

    #endregion

    #region 関数呼び出しのテスト

    [Fact]
    public void TestParseFunctionCall_NoArgs()
    {
        // f()
        var tokens = new Core.Lexer.Lexer("f()").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<CallExpr>(ast.Expression);
        var call = (CallExpr)ast.Expression;

        Assert.IsType<IdentifierExpr>(call.Callee);
        var callee = (IdentifierExpr)call.Callee;
        Assert.Equal("f", callee.Name);

        Assert.Empty(call.Arguments);
    }

    [Fact]
    public void TestParseFunctionCall_OneArg()
    {
        // f(1)
        var tokens = new Core.Lexer.Lexer("f(1)").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<CallExpr>(ast.Expression);
        var call = (CallExpr)ast.Expression;

        Assert.Single(call.Arguments);
        Assert.IsType<LiteralExpr>(call.Arguments[0]);
        var arg = (LiteralExpr)call.Arguments[0];
        Assert.Equal(1.0, arg.Value);
    }

    [Fact]
    public void TestParseFunctionCall_MultipleArgs()
    {
        // f(1, 2, 3)
        var tokens = new Core.Lexer.Lexer("f(1, 2, 3)").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<CallExpr>(ast.Expression);
        var call = (CallExpr)ast.Expression;

        Assert.Equal(3, call.Arguments.Count);

        var arg1 = (LiteralExpr)call.Arguments[0];
        Assert.Equal(1.0, arg1.Value);

        var arg2 = (LiteralExpr)call.Arguments[1];
        Assert.Equal(2.0, arg2.Value);

        var arg3 = (LiteralExpr)call.Arguments[2];
        Assert.Equal(3.0, arg3.Value);
    }

    #endregion

    #region メンバアクセスのテスト

    [Fact]
    public void TestParseMemberAccess()
    {
        // obj.field
        var tokens = new Core.Lexer.Lexer("obj.field").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<MemberExpr>(ast.Expression);
        var member = (MemberExpr)ast.Expression;

        Assert.IsType<IdentifierExpr>(member.Target);
        var target = (IdentifierExpr)member.Target;
        Assert.Equal("obj", target.Name);

        Assert.Equal("field", member.Name);
    }

    [Fact]
    public void TestParseMemberAccess_Chained()
    {
        // obj.field.subfield
        var tokens = new Core.Lexer.Lexer("obj.field.subfield").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<MemberExpr>(ast.Expression);
        var member = (MemberExpr)ast.Expression;
        Assert.Equal("subfield", member.Name);

        Assert.IsType<MemberExpr>(member.Target);
        var target = (MemberExpr)member.Target;
        Assert.Equal("field", target.Name);
    }

    [Fact]
    public void TestParseMethodCall()
    {
        // obj.method()
        var tokens = new Core.Lexer.Lexer("obj.method()").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<CallExpr>(ast.Expression);
        var call = (CallExpr)ast.Expression;

        Assert.IsType<MemberExpr>(call.Callee);
        var member = (MemberExpr)call.Callee;
        Assert.Equal("method", member.Name);
    }

    #endregion

    #region インデックスアクセスのテスト

    [Fact]
    public void TestParseIndexAccess()
    {
        // arr[0]
        var tokens = new Core.Lexer.Lexer("arr[0]").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<IndexExpr>(ast.Expression);
        var index = (IndexExpr)ast.Expression;

        Assert.IsType<IdentifierExpr>(index.Target);
        var target = (IdentifierExpr)index.Target;
        Assert.Equal("arr", target.Name);

        Assert.IsType<LiteralExpr>(index.Index);
        var indexValue = (LiteralExpr)index.Index;
        Assert.Equal(0.0, indexValue.Value);
    }

    [Fact]
    public void TestParseIndexAccess_StringKey()
    {
        // map["key"]
        var tokens = new Core.Lexer.Lexer("map[\"key\"]").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<IndexExpr>(ast.Expression);
        var index = (IndexExpr)ast.Expression;

        Assert.IsType<LiteralExpr>(index.Index);
        var indexValue = (LiteralExpr)index.Index;
        Assert.Equal("key", indexValue.Value);
    }

    #endregion

    #region 文のテスト

    [Fact]
    public void TestParseLetStatement()
    {
        // let x = 10
        var tokens = new Core.Lexer.Lexer("let x = 10").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.Single(ast.Statements);
        Assert.IsType<LetStmt>(ast.Statements[0]);
        var letStmt = (LetStmt)ast.Statements[0];
        Assert.Equal("x", letStmt.Name);

        Assert.IsType<LiteralExpr>(letStmt.Initializer);
        var initializer = (LiteralExpr)letStmt.Initializer;
        Assert.Equal(10.0, initializer.Value);
    }

    [Fact]
    public void TestParseVarStatement()
    {
        // var y = 20
        var tokens = new Core.Lexer.Lexer("var y = 20").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.Single(ast.Statements);
        Assert.IsType<VarStmt>(ast.Statements[0]);
        var varStmt = (VarStmt)ast.Statements[0];
        Assert.Equal("y", varStmt.Name);

        Assert.IsType<LiteralExpr>(varStmt.Initializer);
        var initializer = (LiteralExpr)varStmt.Initializer;
        Assert.Equal(20.0, initializer.Value);
    }

    [Fact]
    public void TestParseExpressionStatement()
    {
        // f()
        var tokens = new Core.Lexer.Lexer("f()\n42").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.Single(ast.Statements);
        Assert.IsType<ExprStmt>(ast.Statements[0]);
        var exprStmt = (ExprStmt)ast.Statements[0];
        Assert.IsType<CallExpr>(exprStmt.Expression);
    }

    [Fact]
    public void TestParseMultipleStatements()
    {
        // let x = 10
        // var y = 20
        // x + y
        var tokens = new Core.Lexer.Lexer("let x = 10\nvar y = 20\nx + y").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.Equal(2, ast.Statements.Count);
        Assert.IsType<LetStmt>(ast.Statements[0]);
        Assert.IsType<VarStmt>(ast.Statements[1]);

        Assert.NotNull(ast.Expression);
        Assert.IsType<BinaryExpr>(ast.Expression);
    }

    #endregion

    #region リストリテラルのテスト

    [Fact]
    public void TestParseListLiteral_Empty()
    {
        // 空のリスト: []
        var tokens = new Core.Lexer.Lexer("[]").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<ListExpr>(ast.Expression);
        var list = (ListExpr)ast.Expression;
        Assert.Empty(list.Elements);
    }

    [Fact]
    public void TestParseListLiteral_SingleElement()
    {
        // リスト: [1]
        var tokens = new Core.Lexer.Lexer("[1]").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<ListExpr>(ast.Expression);
        var list = (ListExpr)ast.Expression;
        Assert.Single(list.Elements);

        Assert.IsType<LiteralExpr>(list.Elements[0]);
        var elem = (LiteralExpr)list.Elements[0];
        Assert.Equal(1.0, elem.Value);
    }

    [Fact]
    public void TestParseListLiteral_MultipleElements()
    {
        // リスト: [1, 2, 3]
        var tokens = new Core.Lexer.Lexer("[1, 2, 3]").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<ListExpr>(ast.Expression);
        var list = (ListExpr)ast.Expression;
        Assert.Equal(3, list.Elements.Count);

        var elem1 = (LiteralExpr)list.Elements[0];
        Assert.Equal(1.0, elem1.Value);

        var elem2 = (LiteralExpr)list.Elements[1];
        Assert.Equal(2.0, elem2.Value);

        var elem3 = (LiteralExpr)list.Elements[2];
        Assert.Equal(3.0, elem3.Value);
    }

    [Fact]
    public void TestParseListLiteral_MixedTypes()
    {
        // リスト: [1, "hello", true]
        var tokens = new Core.Lexer.Lexer("[1, \"hello\", true]").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<ListExpr>(ast.Expression);
        var list = (ListExpr)ast.Expression;
        Assert.Equal(3, list.Elements.Count);

        var elem1 = (LiteralExpr)list.Elements[0];
        Assert.Equal(1.0, elem1.Value);

        var elem2 = (LiteralExpr)list.Elements[1];
        Assert.Equal("hello", elem2.Value);

        var elem3 = (LiteralExpr)list.Elements[2];
        Assert.Equal(true, elem3.Value);
    }

    [Fact]
    public void TestParseListLiteral_Nested()
    {
        // ネストしたリスト: [[1, 2], [3, 4]]
        var tokens = new Core.Lexer.Lexer("[[1, 2], [3, 4]]").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<ListExpr>(ast.Expression);
        var list = (ListExpr)ast.Expression;
        Assert.Equal(2, list.Elements.Count);

        Assert.IsType<ListExpr>(list.Elements[0]);
        var subList1 = (ListExpr)list.Elements[0];
        Assert.Equal(2, subList1.Elements.Count);

        Assert.IsType<ListExpr>(list.Elements[1]);
        var subList2 = (ListExpr)list.Elements[1];
        Assert.Equal(2, subList2.Elements.Count);
    }

    #endregion

    #region ハッシュリテラルのテスト

    [Fact]
    public void TestParseHashLiteral_Empty()
    {
        // 空のハッシュ: {}
        var tokens = new Core.Lexer.Lexer("{}").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        // 空の {} はブロック式として解釈される
        Assert.IsType<BlockExpr>(ast.Expression);
        var block = (BlockExpr)ast.Expression;
        Assert.Empty(block.Statements);
        Assert.Null(block.Expression);
    }

    [Fact]
    public void TestParseHashLiteral_SinglePair()
    {
        // ハッシュ: {name: "Alice"}
        var tokens = new Core.Lexer.Lexer("{name: \"Alice\"}").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<HashExpr>(ast.Expression);
        var hash = (HashExpr)ast.Expression;
        Assert.Single(hash.Pairs);

        Assert.Equal("name", hash.Pairs[0].Key);
        Assert.IsType<LiteralExpr>(hash.Pairs[0].Value);
        var value = (LiteralExpr)hash.Pairs[0].Value;
        Assert.Equal("Alice", value.Value);
    }

    [Fact]
    public void TestParseHashLiteral_MultiplePairs()
    {
        // ハッシュ: {name: "Alice", age: 30, active: true}
        var tokens = new Core.Lexer.Lexer("{name: \"Alice\", age: 30, active: true}").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<HashExpr>(ast.Expression);
        var hash = (HashExpr)ast.Expression;
        Assert.Equal(3, hash.Pairs.Count);

        Assert.Equal("name", hash.Pairs[0].Key);
        var value1 = (LiteralExpr)hash.Pairs[0].Value;
        Assert.Equal("Alice", value1.Value);

        Assert.Equal("age", hash.Pairs[1].Key);
        var value2 = (LiteralExpr)hash.Pairs[1].Value;
        Assert.Equal(30.0, value2.Value);

        Assert.Equal("active", hash.Pairs[2].Key);
        var value3 = (LiteralExpr)hash.Pairs[2].Value;
        Assert.Equal(true, value3.Value);
    }

    [Fact]
    public void TestParseHashLiteral_NestedHash()
    {
        // ネストしたハッシュ: {person: {name: "Bob"}}
        var tokens = new Core.Lexer.Lexer("{person: {name: \"Bob\"}}").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<HashExpr>(ast.Expression);
        var hash = (HashExpr)ast.Expression;
        Assert.Single(hash.Pairs);

        Assert.Equal("person", hash.Pairs[0].Key);
        Assert.IsType<HashExpr>(hash.Pairs[0].Value);
        var nestedHash = (HashExpr)hash.Pairs[0].Value;
        Assert.Single(nestedHash.Pairs);
        Assert.Equal("name", nestedHash.Pairs[0].Key);
    }

    #endregion

    #region インデックス代入のテスト

    [Fact]
    public void TestParseIndexAssignment()
    {
        // arr[0] = 10
        var tokens = new Core.Lexer.Lexer("arr[0] = 10").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<IndexAssignExpr>(ast.Expression);
        var assign = (IndexAssignExpr)ast.Expression;

        Assert.IsType<IdentifierExpr>(assign.Target);
        var target = (IdentifierExpr)assign.Target;
        Assert.Equal("arr", target.Name);

        Assert.IsType<LiteralExpr>(assign.Index);
        var index = (LiteralExpr)assign.Index;
        Assert.Equal(0.0, index.Value);

        Assert.IsType<LiteralExpr>(assign.Value);
        var value = (LiteralExpr)assign.Value;
        Assert.Equal(10.0, value.Value);
    }

    [Fact]
    public void TestParseIndexAssignment_StringKey()
    {
        // map["key"] = "value"
        var tokens = new Core.Lexer.Lexer("map[\"key\"] = \"value\"").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<IndexAssignExpr>(ast.Expression);
        var assign = (IndexAssignExpr)ast.Expression;

        Assert.IsType<LiteralExpr>(assign.Index);
        var index = (LiteralExpr)assign.Index;
        Assert.Equal("key", index.Value);

        Assert.IsType<LiteralExpr>(assign.Value);
        var value = (LiteralExpr)assign.Value;
        Assert.Equal("value", value.Value);
    }

    #endregion

    #region メンバ代入のテスト

    [Fact]
    public void TestParseMemberAssignment()
    {
        // obj.field = 42
        var tokens = new Core.Lexer.Lexer("obj.field = 42").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<MemberAssignExpr>(ast.Expression);
        var assign = (MemberAssignExpr)ast.Expression;

        Assert.IsType<IdentifierExpr>(assign.Target);
        var target = (IdentifierExpr)assign.Target;
        Assert.Equal("obj", target.Name);

        Assert.Equal("field", assign.MemberName);

        Assert.IsType<LiteralExpr>(assign.Value);
        var value = (LiteralExpr)assign.Value;
        Assert.Equal(42.0, value.Value);
    }

    [Fact]
    public void TestParseMemberAssignment_ChainedMember()
    {
        // obj.outer.inner = 100
        var tokens = new Core.Lexer.Lexer("obj.outer.inner = 100").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<MemberAssignExpr>(ast.Expression);
        var assign = (MemberAssignExpr)ast.Expression;

        Assert.Equal("inner", assign.MemberName);

        Assert.IsType<MemberExpr>(assign.Target);
        var target = (MemberExpr)assign.Target;
        Assert.Equal("outer", target.Name);
    }

    #endregion

    #region エラーケースのテスト

    [Fact]
    public void TestParseError_UnexpectedToken()
    {
        // 不正な構文: +
        var tokens = new Core.Lexer.Lexer("+").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);

        Assert.Throws<ParseException>(() => parser.Parse());
    }

    [Fact]
    public void TestParseError_MissingClosingParen()
    {
        // 不正な構文: (1 + 2
        var tokens = new Core.Lexer.Lexer("(1 + 2").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);

        Assert.Throws<ParseException>(() => parser.Parse());
    }

    [Fact]
    public void TestParseError_MissingExpression()
    {
        // 不正な構文: 1 +
        var tokens = new Core.Lexer.Lexer("1 +").ScanTokens();
        var parser = new Core.Parser.Parser(tokens);

        Assert.Throws<ParseException>(() => parser.Parse());
    }

    #endregion
}
