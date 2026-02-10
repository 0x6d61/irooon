using Irooon.Core.Ast;
using Irooon.Core.Ast.Statements;
using Irooon.Core.Lexer;
using Irooon.Core.Parser;
using Xunit;

namespace Irooon.Tests.Parser;

/// <summary>
/// import/export文のパーサーテスト
/// </summary>
public class ParserImportExportTests
{
    #region Export Tests

    [Fact]
    public void TestExportLet()
    {
        // export let x = 10
        var tokens = new List<Token>
        {
            new(TokenType.Export, "export", null, 1, 1),
            new(TokenType.Let, "let", null, 1, 8),
            new(TokenType.Identifier, "x", null, 1, 12),
            new(TokenType.Equal, "=", null, 1, 14),
            new(TokenType.Number, "10", 10.0, 1, 16),
            new(TokenType.Eof, "", null, 1, 18)
        };

        var parser = new Core.Parser.Parser(tokens);
        var program = parser.Parse();

        Assert.Single(program.Statements);
        var exportStmt = Assert.IsType<ExportStmt>(program.Statements[0]);
        var letStmt = Assert.IsType<LetStmt>(exportStmt.Declaration);
        Assert.Equal("x", letStmt.Name);
    }

    [Fact]
    public void TestExportFunction()
    {
        // export fn add(a, b) { return a + b }
        var tokens = new List<Token>
        {
            new(TokenType.Export, "export", null, 1, 1),
            new(TokenType.Fn, "fn", null, 1, 8),
            new(TokenType.Identifier, "add", null, 1, 11),
            new(TokenType.LeftParen, "(", null, 1, 14),
            new(TokenType.Identifier, "a", null, 1, 15),
            new(TokenType.Comma, ",", null, 1, 16),
            new(TokenType.Identifier, "b", null, 1, 18),
            new(TokenType.RightParen, ")", null, 1, 19),
            new(TokenType.LeftBrace, "{", null, 1, 21),
            new(TokenType.Return, "return", null, 1, 23),
            new(TokenType.Identifier, "a", null, 1, 30),
            new(TokenType.Plus, "+", null, 1, 32),
            new(TokenType.Identifier, "b", null, 1, 34),
            new(TokenType.RightBrace, "}", null, 1, 36),
            new(TokenType.Eof, "", null, 1, 37)
        };

        var parser = new Core.Parser.Parser(tokens);
        var program = parser.Parse();

        Assert.Single(program.Statements);
        var exportStmt = Assert.IsType<ExportStmt>(program.Statements[0]);
        var funcDef = Assert.IsType<FunctionDef>(exportStmt.Declaration);
        Assert.Equal("add", funcDef.Name);
        Assert.Equal(2, funcDef.Parameters.Count);
    }

    [Fact]
    public void TestExportVar()
    {
        // export var x = 10
        var tokens = new List<Token>
        {
            new(TokenType.Export, "export", null, 1, 1),
            new(TokenType.Var, "var", null, 1, 8),
            new(TokenType.Identifier, "x", null, 1, 12),
            new(TokenType.Equal, "=", null, 1, 14),
            new(TokenType.Number, "10", 10.0, 1, 16),
            new(TokenType.Eof, "", null, 1, 18)
        };

        var parser = new Core.Parser.Parser(tokens);
        var program = parser.Parse();

        Assert.Single(program.Statements);
        var exportStmt = Assert.IsType<ExportStmt>(program.Statements[0]);
        var varStmt = Assert.IsType<VarStmt>(exportStmt.Declaration);
        Assert.Equal("x", varStmt.Name);
    }

    [Fact]
    public void TestExportClass()
    {
        // export class Point { var x = 0 }
        var tokens = new List<Token>
        {
            new(TokenType.Export, "export", null, 1, 1),
            new(TokenType.Class, "class", null, 1, 8),
            new(TokenType.Identifier, "Point", null, 1, 14),
            new(TokenType.LeftBrace, "{", null, 1, 20),
            new(TokenType.Var, "var", null, 1, 22),
            new(TokenType.Identifier, "x", null, 1, 26),
            new(TokenType.Equal, "=", null, 1, 28),
            new(TokenType.Number, "0", 0.0, 1, 30),
            new(TokenType.RightBrace, "}", null, 1, 32),
            new(TokenType.Eof, "", null, 1, 33)
        };

        var parser = new Core.Parser.Parser(tokens);
        var program = parser.Parse();

        Assert.Single(program.Statements);
        var exportStmt = Assert.IsType<ExportStmt>(program.Statements[0]);
        var classDef = Assert.IsType<ClassDef>(exportStmt.Declaration);
        Assert.Equal("Point", classDef.Name);
    }

    #endregion

    #region Import Tests

    [Fact]
    public void TestImportSingleName()
    {
        // import {add} from "./math.iro"
        var tokens = new List<Token>
        {
            new(TokenType.Import, "import", null, 1, 1),
            new(TokenType.LeftBrace, "{", null, 1, 8),
            new(TokenType.Identifier, "add", null, 1, 9),
            new(TokenType.RightBrace, "}", null, 1, 12),
            new(TokenType.From, "from", null, 1, 14),
            new(TokenType.String, "\"./math.iro\"", "./math.iro", 1, 19),
            new(TokenType.Eof, "", null, 1, 31)
        };

        var parser = new Core.Parser.Parser(tokens);
        var program = parser.Parse();

        Assert.Single(program.Statements);
        var importStmt = Assert.IsType<ImportStmt>(program.Statements[0]);
        Assert.Single(importStmt.Names);
        Assert.Equal("add", importStmt.Names[0]);
        Assert.Equal("./math.iro", importStmt.ModulePath);
    }

    [Fact]
    public void TestImportMultipleNames()
    {
        // import {add, subtract, multiply} from "./math.iro"
        var tokens = new List<Token>
        {
            new(TokenType.Import, "import", null, 1, 1),
            new(TokenType.LeftBrace, "{", null, 1, 8),
            new(TokenType.Identifier, "add", null, 1, 9),
            new(TokenType.Comma, ",", null, 1, 12),
            new(TokenType.Identifier, "subtract", null, 1, 14),
            new(TokenType.Comma, ",", null, 1, 22),
            new(TokenType.Identifier, "multiply", null, 1, 24),
            new(TokenType.RightBrace, "}", null, 1, 32),
            new(TokenType.From, "from", null, 1, 34),
            new(TokenType.String, "\"./math.iro\"", "./math.iro", 1, 39),
            new(TokenType.Eof, "", null, 1, 51)
        };

        var parser = new Core.Parser.Parser(tokens);
        var program = parser.Parse();

        Assert.Single(program.Statements);
        var importStmt = Assert.IsType<ImportStmt>(program.Statements[0]);
        Assert.Equal(3, importStmt.Names.Count);
        Assert.Equal("add", importStmt.Names[0]);
        Assert.Equal("subtract", importStmt.Names[1]);
        Assert.Equal("multiply", importStmt.Names[2]);
        Assert.Equal("./math.iro", importStmt.ModulePath);
    }

    #endregion

    #region Error Tests

    [Fact]
    public void TestExportWithoutDeclaration_ThrowsException()
    {
        // export (何も続かない)
        var tokens = new List<Token>
        {
            new(TokenType.Export, "export", null, 1, 1),
            new(TokenType.Eof, "", null, 1, 8)
        };

        var parser = new Core.Parser.Parser(tokens);
        Assert.Throws<ParseException>(() => parser.Parse());
    }

    [Fact]
    public void TestImportWithoutBrace_ThrowsException()
    {
        // import add from "./math.iro" (波括弧がない)
        var tokens = new List<Token>
        {
            new(TokenType.Import, "import", null, 1, 1),
            new(TokenType.Identifier, "add", null, 1, 8),
            new(TokenType.From, "from", null, 1, 12),
            new(TokenType.String, "\"./math.iro\"", "./math.iro", 1, 17),
            new(TokenType.Eof, "", null, 1, 29)
        };

        var parser = new Core.Parser.Parser(tokens);
        Assert.Throws<ParseException>(() => parser.Parse());
    }

    [Fact]
    public void TestImportWithoutFrom_ThrowsException()
    {
        // import {add} (fromがない)
        var tokens = new List<Token>
        {
            new(TokenType.Import, "import", null, 1, 1),
            new(TokenType.LeftBrace, "{", null, 1, 8),
            new(TokenType.Identifier, "add", null, 1, 9),
            new(TokenType.RightBrace, "}", null, 1, 12),
            new(TokenType.Eof, "", null, 1, 14)
        };

        var parser = new Core.Parser.Parser(tokens);
        Assert.Throws<ParseException>(() => parser.Parse());
    }

    #endregion

    #region アセンブリ参照 (#r) パーステスト

    [Fact]
    public void TestParseAssemblyRef()
    {
        var tokens = new List<Token>
        {
            new(TokenType.AssemblyRef, "#r \"test.dll\"", "test.dll", 1, 1),
            new(TokenType.Eof, "", null, 1, 20)
        };

        var parser = new Core.Parser.Parser(tokens);
        var block = parser.Parse();

        Assert.Single(block.Statements);
        var stmt = Assert.IsType<AssemblyRefStmt>(block.Statements[0]);
        Assert.Equal("test.dll", stmt.AssemblyPath);
    }

    [Fact]
    public void TestParseAssemblyRef_WithOtherStatements()
    {
        var tokens = new List<Token>
        {
            new(TokenType.AssemblyRef, "#r \"lib.dll\"", "lib.dll", 1, 1),
            new(TokenType.Newline, "\n", null, 1, 18),
            new(TokenType.Let, "let", null, 2, 1),
            new(TokenType.Identifier, "x", null, 2, 5),
            new(TokenType.Equal, "=", null, 2, 7),
            new(TokenType.Number, "42", 42.0, 2, 9),
            new(TokenType.Eof, "", null, 2, 11)
        };

        var parser = new Core.Parser.Parser(tokens);
        var block = parser.Parse();

        Assert.Equal(2, block.Statements.Count);
        Assert.IsType<AssemblyRefStmt>(block.Statements[0]);
        Assert.IsType<LetStmt>(block.Statements[1]);
    }

    #endregion
}
