using Xunit;
using Irooon.Core.Ast;
using Irooon.Core.Ast.Expressions;
using Irooon.Core.Ast.Statements;
using Irooon.Core.Lexer;

namespace Irooon.Tests.Ast;

/// <summary>
/// ASTノードの基本的なテストクラス
/// </summary>
public class AstNodeTests
{
    #region 基底クラステスト

    [Fact]
    public void AstNode_保持する位置情報が正しいこと()
    {
        // Arrange & Act
        var expr = new LiteralExpr(123.0, 5, 10);

        // Assert
        Assert.Equal(5, expr.Line);
        Assert.Equal(10, expr.Column);
    }

    #endregion

    #region LiteralExpr テスト

    [Fact]
    public void LiteralExpr_数値リテラルを保持できること()
    {
        // Arrange & Act
        var expr = new LiteralExpr(123.0, 1, 1);

        // Assert
        Assert.Equal(123.0, expr.Value);
        Assert.Equal(1, expr.Line);
        Assert.Equal(1, expr.Column);
    }

    [Fact]
    public void LiteralExpr_文字列リテラルを保持できること()
    {
        // Arrange & Act
        var expr = new LiteralExpr("hello", 2, 5);

        // Assert
        Assert.Equal("hello", expr.Value);
        Assert.Equal(2, expr.Line);
    }

    [Fact]
    public void LiteralExpr_nullリテラルを保持できること()
    {
        // Arrange & Act
        var expr = new LiteralExpr(null, 3, 1);

        // Assert
        Assert.Null(expr.Value);
    }

    #endregion

    #region BinaryExpr テスト

    [Fact]
    public void BinaryExpr_加算式を正しく構築できること()
    {
        // Arrange
        var left = new LiteralExpr(1.0, 1, 1);
        var right = new LiteralExpr(2.0, 1, 5);

        // Act
        var expr = new BinaryExpr(left, TokenType.Plus, right, 1, 1);

        // Assert
        Assert.NotNull(expr.Left);
        Assert.NotNull(expr.Right);
        Assert.Equal(TokenType.Plus, expr.Operator);
        Assert.Equal(1.0, ((LiteralExpr)expr.Left).Value);
        Assert.Equal(2.0, ((LiteralExpr)expr.Right).Value);
    }

    [Fact]
    public void BinaryExpr_比較式を正しく構築できること()
    {
        // Arrange
        var left = new IdentifierExpr("x", 1, 1);
        var right = new LiteralExpr(10.0, 1, 5);

        // Act
        var expr = new BinaryExpr(left, TokenType.Less, right, 1, 1);

        // Assert
        Assert.Equal(TokenType.Less, expr.Operator);
        Assert.IsType<IdentifierExpr>(expr.Left);
        Assert.IsType<LiteralExpr>(expr.Right);
    }

    #endregion

    #region UnaryExpr テスト

    [Fact]
    public void UnaryExpr_単項マイナスを正しく構築できること()
    {
        // Arrange
        var operand = new LiteralExpr(5.0, 1, 2);

        // Act
        var expr = new UnaryExpr(TokenType.Minus, operand, 1, 1);

        // Assert
        Assert.Equal(TokenType.Minus, expr.Operator);
        Assert.NotNull(expr.Operand);
        Assert.Equal(5.0, ((LiteralExpr)expr.Operand).Value);
    }

    [Fact]
    public void UnaryExpr_not演算子を正しく構築できること()
    {
        // Arrange
        var operand = new IdentifierExpr("flag", 1, 5);

        // Act
        var expr = new UnaryExpr(TokenType.Not, operand, 1, 1);

        // Assert
        Assert.Equal(TokenType.Not, expr.Operator);
        Assert.IsType<IdentifierExpr>(expr.Operand);
    }

    #endregion

    #region IdentifierExpr テスト

    [Fact]
    public void IdentifierExpr_識別子名を正しく保持できること()
    {
        // Arrange & Act
        var expr = new IdentifierExpr("myVariable", 3, 5);

        // Assert
        Assert.Equal("myVariable", expr.Name);
        Assert.Equal(3, expr.Line);
        Assert.Equal(5, expr.Column);
    }

    #endregion

    #region AssignExpr テスト

    [Fact]
    public void AssignExpr_代入式を正しく構築できること()
    {
        // Arrange
        var value = new LiteralExpr(42.0, 1, 7);

        // Act
        var expr = new AssignExpr("x", value, 1, 1);

        // Assert
        Assert.Equal("x", expr.Name);
        Assert.NotNull(expr.Value);
        Assert.Equal(42.0, ((LiteralExpr)expr.Value).Value);
    }

    #endregion

    #region CallExpr テスト

    [Fact]
    public void CallExpr_引数なしの関数呼び出しを構築できること()
    {
        // Arrange
        var callee = new IdentifierExpr("print", 1, 1);
        var arguments = new List<Expression>();

        // Act
        var expr = new CallExpr(callee, arguments, 1, 1);

        // Assert
        Assert.NotNull(expr.Callee);
        Assert.Empty(expr.Arguments);
    }

    [Fact]
    public void CallExpr_引数ありの関数呼び出しを構築できること()
    {
        // Arrange
        var callee = new IdentifierExpr("add", 1, 1);
        var arguments = new List<Expression>
        {
            new LiteralExpr(1.0, 1, 5),
            new LiteralExpr(2.0, 1, 8)
        };

        // Act
        var expr = new CallExpr(callee, arguments, 1, 1);

        // Assert
        Assert.Equal(2, expr.Arguments.Count);
    }

    #endregion

    #region MemberExpr テスト

    [Fact]
    public void MemberExpr_メンバアクセスを正しく構築できること()
    {
        // Arrange
        var target = new IdentifierExpr("obj", 1, 1);

        // Act
        var expr = new MemberExpr(target, "field", 1, 1);

        // Assert
        Assert.NotNull(expr.Target);
        Assert.Equal("field", expr.Name);
        Assert.IsType<IdentifierExpr>(expr.Target);
    }

    #endregion

    #region IndexExpr テスト

    [Fact]
    public void IndexExpr_インデックスアクセスを正しく構築できること()
    {
        // Arrange
        var target = new IdentifierExpr("arr", 1, 1);
        var index = new LiteralExpr(0.0, 1, 5);

        // Act
        var expr = new IndexExpr(target, index, 1, 1);

        // Assert
        Assert.NotNull(expr.Target);
        Assert.NotNull(expr.Index);
        Assert.IsType<IdentifierExpr>(expr.Target);
        Assert.IsType<LiteralExpr>(expr.Index);
    }

    #endregion

    #region LetStmt テスト

    [Fact]
    public void LetStmt_let文を正しく構築できること()
    {
        // Arrange
        var initializer = new LiteralExpr(100.0, 1, 10);

        // Act
        var stmt = new LetStmt("x", initializer, 1, 1);

        // Assert
        Assert.Equal("x", stmt.Name);
        Assert.NotNull(stmt.Initializer);
        Assert.Equal(100.0, ((LiteralExpr)stmt.Initializer).Value);
    }

    #endregion

    #region VarStmt テスト

    [Fact]
    public void VarStmt_var文を正しく構築できること()
    {
        // Arrange
        var initializer = new LiteralExpr(50.0, 1, 10);

        // Act
        var stmt = new VarStmt("y", initializer, 1, 1);

        // Assert
        Assert.Equal("y", stmt.Name);
        Assert.NotNull(stmt.Initializer);
    }

    #endregion

    #region ExprStmt テスト

    [Fact]
    public void ExprStmt_式文を正しく構築できること()
    {
        // Arrange
        var expression = new CallExpr(
            new IdentifierExpr("print", 1, 1),
            new List<Expression> { new LiteralExpr("test", 1, 7) },
            1, 1
        );

        // Act
        var stmt = new ExprStmt(expression, 1, 1);

        // Assert
        Assert.NotNull(stmt.Expression);
        Assert.IsType<CallExpr>(stmt.Expression);
    }

    #endregion

    #region ReturnStmt テスト

    [Fact]
    public void ReturnStmt_値ありのreturn文を構築できること()
    {
        // Arrange
        var value = new LiteralExpr(42.0, 1, 8);

        // Act
        var stmt = new ReturnStmt(value, 1, 1);

        // Assert
        Assert.NotNull(stmt.Value);
        Assert.Equal(42.0, ((LiteralExpr)stmt.Value).Value);
    }

    [Fact]
    public void ReturnStmt_値なしのreturn文を構築できること()
    {
        // Arrange & Act
        var stmt = new ReturnStmt(null, 1, 1);

        // Assert
        Assert.Null(stmt.Value);
    }

    #endregion

    #region ForStmt テスト

    [Fact]
    public void ForStmt_条件ループを正しく構築できること()
    {
        // Arrange
        var condition = new BinaryExpr(
            new IdentifierExpr("i", 1, 7),
            TokenType.Less,
            new LiteralExpr(10.0, 1, 11),
            1, 7
        );
        var bodyExpr = new BlockExpr(
            new List<Statement>(),
            new AssignExpr("i", new BinaryExpr(
                new IdentifierExpr("i", 2, 5),
                TokenType.Plus,
                new LiteralExpr(1.0, 2, 9),
                2, 5
            ), 2, 1),
            2, 1
        );

        // Act
        var stmt = new ForStmt(condition, bodyExpr, 1, 1);

        // Assert
        Assert.Equal(ForStmtKind.Condition, stmt.Kind);
        Assert.NotNull(stmt.Condition);
        Assert.NotNull(stmt.Body);
        Assert.IsType<BinaryExpr>(stmt.Condition);
        Assert.IsType<BlockExpr>(stmt.Body);
    }

    #endregion
}
