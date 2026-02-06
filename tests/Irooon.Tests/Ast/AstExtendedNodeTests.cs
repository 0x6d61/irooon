using Xunit;
using Irooon.Core.Ast;
using Irooon.Core.Ast.Expressions;
using Irooon.Core.Ast.Statements;
using Irooon.Core.Lexer;

namespace Irooon.Tests.Ast;

/// <summary>
/// AST拡張ノード（制御構造・関数・クラス）のテストクラス
/// </summary>
public class AstExtendedNodeTests
{
    #region IfExpr テスト

    [Fact]
    public void IfExpr_if式を正しく構築できること()
    {
        // Arrange
        var condition = new BinaryExpr(
            new IdentifierExpr("x", 1, 5),
            TokenType.Greater,
            new LiteralExpr(0.0, 1, 9),
            1, 5
        );
        var thenBranch = new LiteralExpr(1.0, 1, 14);
        var elseBranch = new LiteralExpr(0.0, 1, 23);

        // Act
        var expr = new IfExpr(condition, thenBranch, elseBranch, 1, 1);

        // Assert
        Assert.NotNull(expr.Condition);
        Assert.NotNull(expr.ThenBranch);
        Assert.NotNull(expr.ElseBranch);
        Assert.IsType<BinaryExpr>(expr.Condition);
    }

    #endregion

    #region BlockExpr テスト

    [Fact]
    public void BlockExpr_ステートメントのみのブロックを構築できること()
    {
        // Arrange
        var statements = new List<Statement>
        {
            new LetStmt("x", new LiteralExpr(1.0, 1, 9), 1, 1),
            new VarStmt("y", new LiteralExpr(2.0, 2, 9), 2, 1)
        };

        // Act
        var expr = new BlockExpr(statements, null, 1, 1);

        // Assert
        Assert.Equal(2, expr.Statements.Count);
        Assert.Null(expr.Expression);
    }

    [Fact]
    public void BlockExpr_最終式を持つブロックを構築できること()
    {
        // Arrange
        var statements = new List<Statement>
        {
            new LetStmt("x", new LiteralExpr(1.0, 1, 9), 1, 1)
        };
        var finalExpr = new IdentifierExpr("x", 2, 1);

        // Act
        var expr = new BlockExpr(statements, finalExpr, 1, 1);

        // Assert
        Assert.Single(expr.Statements);
        Assert.NotNull(expr.Expression);
        Assert.Equal("x", ((IdentifierExpr)expr.Expression).Name);
    }

    [Fact]
    public void BlockExpr_空のブロックを構築できること()
    {
        // Arrange
        var statements = new List<Statement>();

        // Act
        var expr = new BlockExpr(statements, null, 1, 1);

        // Assert
        Assert.Empty(expr.Statements);
        Assert.Null(expr.Expression);
    }

    #endregion

    #region LambdaExpr テスト

    [Fact]
    public void LambdaExpr_パラメータなしのラムダ式を構築できること()
    {
        // Arrange
        var parameters = new List<Parameter>();
        var body = new LiteralExpr(42.0, 1, 10);

        // Act
        var expr = new LambdaExpr(parameters, body, 1, 1);

        // Assert
        Assert.Empty(expr.Parameters);
        Assert.NotNull(expr.Body);
    }

    [Fact]
    public void LambdaExpr_パラメータありのラムダ式を構築できること()
    {
        // Arrange
        var parameters = new List<Parameter>
        {
            new Parameter("a", 1, 5),
            new Parameter("b", 1, 8)
        };
        var body = new BinaryExpr(
            new IdentifierExpr("a", 1, 14),
            TokenType.Plus,
            new IdentifierExpr("b", 1, 18),
            1, 14
        );

        // Act
        var expr = new LambdaExpr(parameters, body, 1, 1);

        // Assert
        Assert.Equal(2, expr.Parameters.Count);
        Assert.Equal("a", expr.Parameters[0].Name);
        Assert.Equal("b", expr.Parameters[1].Name);
        Assert.IsType<BinaryExpr>(expr.Body);
    }

    #endregion

    #region NewExpr テスト

    [Fact]
    public void NewExpr_引数なしのインスタンス生成式を構築できること()
    {
        // Arrange
        var arguments = new List<Expression>();

        // Act
        var expr = new NewExpr("MyClass", arguments, 1, 1);

        // Assert
        Assert.Equal("MyClass", expr.ClassName);
        Assert.Empty(expr.Arguments);
    }

    [Fact]
    public void NewExpr_引数ありのインスタンス生成式を構築できること()
    {
        // Arrange
        var arguments = new List<Expression>
        {
            new LiteralExpr(10.0, 1, 10),
            new LiteralExpr("test", 1, 14)
        };

        // Act
        var expr = new NewExpr("Person", arguments, 1, 1);

        // Assert
        Assert.Equal("Person", expr.ClassName);
        Assert.Equal(2, expr.Arguments.Count);
    }

    #endregion

    #region Parameter テスト

    [Fact]
    public void Parameter_パラメータを正しく構築できること()
    {
        // Arrange & Act
        var param = new Parameter("myParam", 1, 5);

        // Assert
        Assert.Equal("myParam", param.Name);
        Assert.Equal(1, param.Line);
        Assert.Equal(5, param.Column);
    }

    #endregion

    #region FunctionDef テスト

    [Fact]
    public void FunctionDef_パラメータなしの関数定義を構築できること()
    {
        // Arrange
        var parameters = new List<Parameter>();
        var body = new LiteralExpr(42.0, 2, 5);

        // Act
        var func = new FunctionDef("getValue", parameters, body, 1, 1);

        // Assert
        Assert.Equal("getValue", func.Name);
        Assert.Empty(func.Parameters);
        Assert.NotNull(func.Body);
    }

    [Fact]
    public void FunctionDef_パラメータありの関数定義を構築できること()
    {
        // Arrange
        var parameters = new List<Parameter>
        {
            new Parameter("x", 1, 11),
            new Parameter("y", 1, 14)
        };
        var body = new BinaryExpr(
            new IdentifierExpr("x", 2, 5),
            TokenType.Plus,
            new IdentifierExpr("y", 2, 9),
            2, 5
        );

        // Act
        var func = new FunctionDef("add", parameters, body, 1, 1);

        // Assert
        Assert.Equal("add", func.Name);
        Assert.Equal(2, func.Parameters.Count);
        Assert.IsType<BinaryExpr>(func.Body);
    }

    #endregion

    #region ClassDef テスト

    [Fact]
    public void ClassDef_空のクラス定義を構築できること()
    {
        // Arrange
        var fields = new List<FieldDef>();
        var methods = new List<MethodDef>();

        // Act
        var classDef = new ClassDef("EmptyClass", fields, methods, 1, 1);

        // Assert
        Assert.Equal("EmptyClass", classDef.Name);
        Assert.Empty(classDef.Fields);
        Assert.Empty(classDef.Methods);
    }

    [Fact]
    public void ClassDef_フィールドとメソッドを持つクラス定義を構築できること()
    {
        // Arrange
        var fields = new List<FieldDef>
        {
            new FieldDef("count", true, new LiteralExpr(0.0, 2, 20), 2, 3)
        };
        var methods = new List<MethodDef>
        {
            new MethodDef(
                "increment",
                true,
                false,
                new List<Parameter>(),
                new AssignExpr("count", new BinaryExpr(
                    new IdentifierExpr("count", 5, 15),
                    TokenType.Plus,
                    new LiteralExpr(1.0, 5, 23),
                    5, 15
                ), 5, 9),
                4, 3
            )
        };

        // Act
        var classDef = new ClassDef("Counter", fields, methods, 1, 1);

        // Assert
        Assert.Equal("Counter", classDef.Name);
        Assert.Single(classDef.Fields);
        Assert.Single(classDef.Methods);
    }

    #endregion

    #region FieldDef テスト

    [Fact]
    public void FieldDef_初期化式なしのフィールド定義を構築できること()
    {
        // Arrange & Act
        var field = new FieldDef("name", true, null, 1, 1);

        // Assert
        Assert.Equal("name", field.Name);
        Assert.True(field.IsPublic);
        Assert.Null(field.Initializer);
    }

    [Fact]
    public void FieldDef_初期化式ありのフィールド定義を構築できること()
    {
        // Arrange
        var initializer = new LiteralExpr("default", 1, 20);

        // Act
        var field = new FieldDef("title", false, initializer, 1, 1);

        // Assert
        Assert.Equal("title", field.Name);
        Assert.False(field.IsPublic);
        Assert.NotNull(field.Initializer);
        Assert.Equal("default", ((LiteralExpr)field.Initializer).Value);
    }

    #endregion

    #region MethodDef テスト

    [Fact]
    public void MethodDef_インスタンスメソッドを構築できること()
    {
        // Arrange
        var parameters = new List<Parameter>
        {
            new Parameter("value", 1, 20)
        };
        var body = new AssignExpr("field", new IdentifierExpr("value", 2, 15), 2, 9);

        // Act
        var method = new MethodDef("setValue", true, false, parameters, body, 1, 1);

        // Assert
        Assert.Equal("setValue", method.Name);
        Assert.True(method.IsPublic);
        Assert.False(method.IsStatic);
        Assert.Single(method.Parameters);
        Assert.NotNull(method.Body);
    }

    [Fact]
    public void MethodDef_静的メソッドを構築できること()
    {
        // Arrange
        var parameters = new List<Parameter>();
        var body = new NewExpr("MyClass", new List<Expression>(), 2, 10);

        // Act
        var method = new MethodDef("create", true, true, parameters, body, 1, 1);

        // Assert
        Assert.Equal("create", method.Name);
        Assert.True(method.IsPublic);
        Assert.True(method.IsStatic);
        Assert.Empty(method.Parameters);
        Assert.IsType<NewExpr>(method.Body);
    }

    [Fact]
    public void MethodDef_privateメソッドを構築できること()
    {
        // Arrange
        var parameters = new List<Parameter>();
        var body = new LiteralExpr(0.0, 2, 5);

        // Act
        var method = new MethodDef("helper", false, false, parameters, body, 1, 1);

        // Assert
        Assert.Equal("helper", method.Name);
        Assert.False(method.IsPublic);
        Assert.False(method.IsStatic);
    }

    #endregion
}
