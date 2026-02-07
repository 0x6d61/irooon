using Irooon.Core.Ast;
using Irooon.Core.Ast.Expressions;
using Irooon.Core.Ast.Statements;
using Irooon.Core.Lexer;
using Irooon.Core.Parser;
using Xunit;

namespace Irooon.Tests.Parser;

/// <summary>
/// Parser の制御構造（文、if、while、return、block）のテスト
/// </summary>
public class ParserStmtTests
{
    [Fact]
    public void TestParseBlockExpr()
    {
        var source = @"
{
    let x = 10
    x + 5
}";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        // トップレベルはBlockExpr
        Assert.IsType<BlockExpr>(ast);
        var block = (BlockExpr)ast;

        // ブロック内のブロック（最初の式）を取得
        Assert.NotNull(block.Expression);
        Assert.IsType<BlockExpr>(block.Expression);
        var innerBlock = (BlockExpr)block.Expression;

        // Statements が1つ（LetStmt）
        Assert.Single(innerBlock.Statements);
        Assert.IsType<LetStmt>(innerBlock.Statements[0]);

        // Expression が x + 5
        Assert.NotNull(innerBlock.Expression);
        Assert.IsType<BinaryExpr>(innerBlock.Expression);
        var binaryExpr = (BinaryExpr)innerBlock.Expression;
        Assert.Equal(TokenType.Plus, binaryExpr.Operator);
    }

    [Fact]
    public void TestParseEmptyBlock()
    {
        var source = "{}";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.IsType<BlockExpr>(ast);
        var block = (BlockExpr)ast;
        Assert.NotNull(block.Expression);
        Assert.IsType<BlockExpr>(block.Expression);
        var innerBlock = (BlockExpr)block.Expression;
        Assert.Empty(innerBlock.Statements);
        Assert.Null(innerBlock.Expression);
    }

    [Fact]
    public void TestParseBlockWithOnlyStatements()
    {
        var source = @"
{
    let x = 1
    let y = 2
}";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.IsType<BlockExpr>(ast);
        var block = (BlockExpr)ast;
        Assert.NotNull(block.Expression);
        Assert.IsType<BlockExpr>(block.Expression);
        var innerBlock = (BlockExpr)block.Expression;

        // 2つの文がある
        Assert.Equal(2, innerBlock.Statements.Count);
        Assert.IsType<LetStmt>(innerBlock.Statements[0]);
        Assert.IsType<LetStmt>(innerBlock.Statements[1]);

        // 最後の式はない
        Assert.Null(innerBlock.Expression);
    }

    [Fact]
    public void TestParseIfExpr()
    {
        var source = @"
if (x > 0) {
    ""positive""
} else {
    ""non-positive""
}";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        // トップレベルはBlockExpr
        Assert.IsType<BlockExpr>(ast);
        var block = (BlockExpr)ast;

        // if式が最後の式
        Assert.NotNull(block.Expression);
        Assert.IsType<IfExpr>(block.Expression);
        var ifExpr = (IfExpr)block.Expression;

        // Condition が x > 0
        Assert.IsType<BinaryExpr>(ifExpr.Condition);
        var condition = (BinaryExpr)ifExpr.Condition;
        Assert.Equal(TokenType.Greater, condition.Operator);

        // ThenBranch がブロック
        Assert.IsType<BlockExpr>(ifExpr.ThenBranch);
        var thenBranch = (BlockExpr)ifExpr.ThenBranch;
        Assert.NotNull(thenBranch.Expression);
        Assert.IsType<LiteralExpr>(thenBranch.Expression);

        // ElseBranch がブロック
        Assert.IsType<BlockExpr>(ifExpr.ElseBranch);
        var elseBranch = (BlockExpr)ifExpr.ElseBranch;
        Assert.NotNull(elseBranch.Expression);
        Assert.IsType<LiteralExpr>(elseBranch.Expression);
    }

    [Fact]
    public void TestParseIfExprWithoutElse_ShouldThrowError()
    {
        var source = @"
if (x > 0) {
    ""positive""
}";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);

        // else が必須なので、ParseException が投げられるはず
        Assert.Throws<ParseException>(() => parser.Parse());
    }

    [Fact]
    public void TestParseWhileStmt()
    {
        var source = @"
while (x < 10) {
    x = x + 1
}";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        // トップレベルはBlockExpr
        Assert.IsType<BlockExpr>(ast);
        var block = (BlockExpr)ast;

        // while文が最初の文
        Assert.Single(block.Statements);
        Assert.IsType<WhileStmt>(block.Statements[0]);
        var whileStmt = (WhileStmt)block.Statements[0];

        // Condition が x < 10
        Assert.IsType<BinaryExpr>(whileStmt.Condition);
        var condition = (BinaryExpr)whileStmt.Condition;
        Assert.Equal(TokenType.Less, condition.Operator);

        // Body が ExprStmt でラップされたブロック
        Assert.IsType<ExprStmt>(whileStmt.Body);
        var bodyStmt = (ExprStmt)whileStmt.Body;
        Assert.IsType<BlockExpr>(bodyStmt.Expression);
        var body = (BlockExpr)bodyStmt.Expression;
        Assert.NotNull(body.Expression);
        Assert.IsType<AssignExpr>(body.Expression);
    }

    [Fact]
    public void TestParseReturnStmt()
    {
        var source = "return 42";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        // トップレベルはBlockExpr
        Assert.IsType<BlockExpr>(ast);
        var block = (BlockExpr)ast;

        // return文が最初の文
        Assert.Single(block.Statements);
        Assert.IsType<ReturnStmt>(block.Statements[0]);
        var returnStmt = (ReturnStmt)block.Statements[0];

        // Value が 42
        Assert.NotNull(returnStmt.Value);
        Assert.IsType<LiteralExpr>(returnStmt.Value);
        var literal = (LiteralExpr)returnStmt.Value;
        Assert.Equal(42.0, literal.Value);
    }

    [Fact]
    public void TestParseReturnStmtWithoutValue()
    {
        var source = "return";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        // トップレベルはBlockExpr
        Assert.IsType<BlockExpr>(ast);
        var block = (BlockExpr)ast;

        // return文が最初の文
        Assert.Single(block.Statements);
        Assert.IsType<ReturnStmt>(block.Statements[0]);
        var returnStmt = (ReturnStmt)block.Statements[0];

        // Value が null
        Assert.Null(returnStmt.Value);
    }

    [Fact]
    public void TestParseNestedBlocks()
    {
        var source = @"
{
    let x = 1
    {
        let y = 2
        x + y
    }
}";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        // トップレベルはBlockExpr
        Assert.IsType<BlockExpr>(ast);
        var block = (BlockExpr)ast;

        // 外側のブロックを取得
        Assert.NotNull(block.Expression);
        Assert.IsType<BlockExpr>(block.Expression);
        var outerBlock = (BlockExpr)block.Expression;

        // 外側のブロックには1つの文（let x = 1）
        Assert.Single(outerBlock.Statements);
        Assert.IsType<LetStmt>(outerBlock.Statements[0]);

        // 外側のブロックの最後の式は内側のブロック
        Assert.NotNull(outerBlock.Expression);
        Assert.IsType<BlockExpr>(outerBlock.Expression);
        var innerBlock = (BlockExpr)outerBlock.Expression;

        // 内側のブロックには1つの文（let y = 2）
        Assert.Single(innerBlock.Statements);
        Assert.IsType<LetStmt>(innerBlock.Statements[0]);

        // 内側のブロックの最後の式は x + y
        Assert.NotNull(innerBlock.Expression);
        Assert.IsType<BinaryExpr>(innerBlock.Expression);
    }

    [Fact]
    public void TestParseIfInsideWhile()
    {
        var source = @"
while (x < 10) {
    if (x > 5) {
        x = x + 2
    } else {
        x = x + 1
    }
}";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        // トップレベルはBlockExpr
        Assert.IsType<BlockExpr>(ast);
        var block = (BlockExpr)ast;

        // while文が最初の文
        Assert.Single(block.Statements);
        Assert.IsType<WhileStmt>(block.Statements[0]);
        var whileStmt = (WhileStmt)block.Statements[0];

        // while の body の中に if式
        Assert.IsType<ExprStmt>(whileStmt.Body);
        var bodyStmt = (ExprStmt)whileStmt.Body;
        Assert.IsType<BlockExpr>(bodyStmt.Expression);
        var body = (BlockExpr)bodyStmt.Expression;
        Assert.NotNull(body.Expression);
        Assert.IsType<IfExpr>(body.Expression);
    }

    [Fact]
    public void TestParseReturnInsideIf()
    {
        var source = @"
if (x < 0) {
    return -1
} else {
    return 1
}";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        // トップレベルはBlockExpr
        Assert.IsType<BlockExpr>(ast);
        var block = (BlockExpr)ast;

        // if式が最後の式
        Assert.NotNull(block.Expression);
        Assert.IsType<IfExpr>(block.Expression);
        var ifExpr = (IfExpr)block.Expression;

        // thenブランチにreturn文
        var thenBranch = (BlockExpr)ifExpr.ThenBranch;
        Assert.Single(thenBranch.Statements);
        Assert.IsType<ReturnStmt>(thenBranch.Statements[0]);

        // elseブランチにreturn文
        var elseBranch = (BlockExpr)ifExpr.ElseBranch;
        Assert.Single(elseBranch.Statements);
        Assert.IsType<ReturnStmt>(elseBranch.Statements[0]);
    }

    [Fact]
    public void TestParseBlockAsExpression()
    {
        var source = @"let result = { 1 + 2 }";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        // トップレベルはBlockExpr
        Assert.IsType<BlockExpr>(ast);
        var block = (BlockExpr)ast;

        // let文が1つ
        Assert.Single(block.Statements);
        Assert.IsType<LetStmt>(block.Statements[0]);
        var letStmt = (LetStmt)block.Statements[0];

        // 初期化式がブロック
        Assert.IsType<BlockExpr>(letStmt.Initializer);
        var blockExpr = (BlockExpr)letStmt.Initializer;

        // ブロックの最後の式が 1 + 2
        Assert.NotNull(blockExpr.Expression);
        Assert.IsType<BinaryExpr>(blockExpr.Expression);
    }

    [Fact(Skip = "Task #39 で実装予定")]
    public void TestParseThrowStmt()
    {
        // throw "error message"
        var source = "throw \"error message\"";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        // トップレベルはBlockExpr
        Assert.IsType<BlockExpr>(ast);
        var block = (BlockExpr)ast;

        // throw文が最初の文
        Assert.Single(block.Statements);
        Assert.IsType<ThrowStmt>(block.Statements[0]);
        var throwStmt = (ThrowStmt)block.Statements[0];

        // Value が文字列リテラル
        Assert.NotNull(throwStmt.Value);
        Assert.IsType<LiteralExpr>(throwStmt.Value);
        var literal = (LiteralExpr)throwStmt.Value;
        Assert.Equal("error message", literal.Value);
    }

    [Fact(Skip = "Task #39 で実装予定")]
    public void TestParseThrowStmt_Expression()
    {
        // throw x + 1
        var source = "throw x + 1";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        // トップレベルはBlockExpr
        Assert.IsType<BlockExpr>(ast);
        var block = (BlockExpr)ast;

        // throw文が最初の文
        Assert.Single(block.Statements);
        Assert.IsType<ThrowStmt>(block.Statements[0]);
        var throwStmt = (ThrowStmt)block.Statements[0];

        // Value が x + 1
        Assert.NotNull(throwStmt.Value);
        Assert.IsType<BinaryExpr>(throwStmt.Value);
    }
}
