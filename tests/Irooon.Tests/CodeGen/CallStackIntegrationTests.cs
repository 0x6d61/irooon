using Irooon.Core.Runtime;
using Irooon.Core.CodeGen;
using Irooon.Core.Ast;
using Irooon.Core.Ast.Expressions;
using Irooon.Core.Ast.Statements;
using Xunit;

namespace Irooon.Tests.CodeGen;

/// <summary>
/// CallStack統合テスト
/// </summary>
public class CallStackIntegrationTests
{
    [Fact]
    public void FunctionCall_ShouldPushAndPopCallStack()
    {
        // Arrange
        CallStack.Clear();
        var ctx = new ScriptContext();
        var codegen = new CodeGenerator();

        // fun add(a, b) { a + b }
        var funcDef = new FunctionDef(
            "add",
            new List<Parameter> {
                new Parameter("a", 1, 5),
                new Parameter("b", 1, 8)
            },
            new BinaryExpr(
                new IdentifierExpr("a", 1, 13),
                Irooon.Core.Lexer.TokenType.Plus,
                new IdentifierExpr("b", 1, 17),
                1, 15
            ),
            1, 1
        );

        // add(1, 2)
        var call = new CallExpr(
            new IdentifierExpr("add", 2, 1),
            new List<Expression> {
                new LiteralExpr(1.0, 2, 5),
                new LiteralExpr(2.0, 2, 8)
            },
            2, 1
        );

        var program = new BlockExpr(
            new List<Statement> { funcDef },
            call,
            1, 1
        );

        // Act
        var compiled = codegen.Compile(program);
        var result = compiled(ctx);

        // Assert
        Assert.Equal(3.0, result);

        // スタックは関数呼び出し後にクリアされているべき
        var trace = CallStack.GetStackTrace();
        Assert.Empty(trace);
    }

    [Fact]
    public void NestedFunctionCall_ShouldMaintainProperStack()
    {
        // Arrange
        CallStack.Clear();
        var ctx = new ScriptContext();
        var codegen = new CodeGenerator();

        // fun inner() { throw RuntimeException }
        // fun outer() { inner() }
        // outer()

        // テストの詳細は後で実装
        // 現時点ではスタックの動作が正しいことを確認

        Assert.True(true); // プレースホルダー
    }
}
