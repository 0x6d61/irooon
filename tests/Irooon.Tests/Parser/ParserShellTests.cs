using Xunit;
using Irooon.Core.Lexer;
using Irooon.Core.Parser;
using Irooon.Core.Ast.Expressions;
using Irooon.Core.Ast;
using Irooon.Core.Ast.Statements;

namespace Irooon.Tests.Parser;

/// <summary>
/// シェルコマンド式のパーサーテスト
/// </summary>
public class ParserShellTests
{
    [Fact]
    public void ParseShellExpr_SimpleCommand_ReturnsShellExpr()
    {
        // Arrange
        var lexer = new Core.Lexer.Lexer("$`echo Hello`");
        var tokens = lexer.ScanTokens();
        var parser = new Core.Parser.Parser(tokens);

        // Act
        var block = parser.Parse();

        // Assert
        Assert.NotNull(block.Expression);
        var shellExpr = Assert.IsType<ShellExpr>(block.Expression);
        Assert.Equal("echo Hello", shellExpr.Command);
    }

    [Fact]
    public void ParseShellExpr_WithVariableAssignment_ReturnsLetStmt()
    {
        // Arrange
        var lexer = new Core.Lexer.Lexer("let output = $`pwd`");
        var tokens = lexer.ScanTokens();
        var parser = new Core.Parser.Parser(tokens);

        // Act
        var block = parser.Parse();

        // Assert
        Assert.Single(block.Statements);
        var letStmt = Assert.IsType<LetStmt>(block.Statements[0]);
        Assert.Equal("output", letStmt.Name);
        var shellExpr = Assert.IsType<ShellExpr>(letStmt.Initializer);
        Assert.Equal("pwd", shellExpr.Command);
    }

    [Fact]
    public void ParseShellExpr_EmptyCommand_ReturnsShellExpr()
    {
        // Arrange
        var lexer = new Core.Lexer.Lexer("$``");
        var tokens = lexer.ScanTokens();
        var parser = new Core.Parser.Parser(tokens);

        // Act
        var block = parser.Parse();

        // Assert
        Assert.NotNull(block.Expression);
        var shellExpr = Assert.IsType<ShellExpr>(block.Expression);
        Assert.Equal("", shellExpr.Command);
    }

    [Fact]
    public void ParseShellExpr_MultilineCommand_ReturnsShellExpr()
    {
        // Arrange
        var source = @"$`
git add .
git commit -m ""Update""
`";
        var lexer = new Core.Lexer.Lexer(source);
        var tokens = lexer.ScanTokens();
        var parser = new Core.Parser.Parser(tokens);

        // Act
        var block = parser.Parse();

        // Assert
        Assert.NotNull(block.Expression);
        var shellExpr = Assert.IsType<ShellExpr>(block.Expression);
        Assert.Contains("git add", shellExpr.Command);
        Assert.Contains("git commit", shellExpr.Command);
    }

    [Fact]
    public void ParseShellExpr_DollarWithoutBacktick_ThrowsParseException()
    {
        // Arrange
        var lexer = new Core.Lexer.Lexer("$ echo");
        var tokens = lexer.ScanTokens();
        var parser = new Core.Parser.Parser(tokens);

        // Act & Assert
        var exception = Assert.Throws<ParseException>(() => parser.Parse());
        Assert.Contains("Expected backtick after $", exception.Message);
    }

    [Fact]
    public void ParseShellExpr_MultipleCalls_ReturnsMultipleStatements()
    {
        // Arrange
        var source = @"
let branch = $`git branch --show-current`
let files = $`ls`
";
        var lexer = new Core.Lexer.Lexer(source);
        var tokens = lexer.ScanTokens();
        var parser = new Core.Parser.Parser(tokens);

        // Act
        var block = parser.Parse();

        // Assert
        Assert.Equal(2, block.Statements.Count);

        var letStmt1 = Assert.IsType<LetStmt>(block.Statements[0]);
        Assert.Equal("branch", letStmt1.Name);
        var shellExpr1 = Assert.IsType<ShellExpr>(letStmt1.Initializer);
        Assert.Equal("git branch --show-current", shellExpr1.Command);

        var letStmt2 = Assert.IsType<LetStmt>(block.Statements[1]);
        Assert.Equal("files", letStmt2.Name);
        var shellExpr2 = Assert.IsType<ShellExpr>(letStmt2.Initializer);
        Assert.Equal("ls", shellExpr2.Command);
    }
}
