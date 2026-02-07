using Xunit;
using Irooon.Core.CodeGen;
using Irooon.Core.Runtime;

namespace Irooon.Tests.CodeGen;

/// <summary>
/// CodeGenerator基本テスト（リテラル、変数、代入、ブロック）
/// Task #13: CodeGen基本式実装
/// </summary>
public class CodeGenBasicTests
{
    /// <summary>
    /// テストヘルパー: ソースコードをコンパイルして実行
    /// </summary>
    private object? ExecuteScript(string source)
    {
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);
        var ctx = new ScriptContext();
        return compiled(ctx);
    }

    #region リテラル式のテスト

    [Fact]
    public void TestGenerateLiteral_Number()
    {
        var result = ExecuteScript("123");
        Assert.Equal(123.0, result);
    }

    [Fact]
    public void TestGenerateLiteral_Number_Float()
    {
        var result = ExecuteScript("123.456");
        Assert.Equal(123.456, result);
    }

    [Fact]
    public void TestGenerateLiteral_String()
    {
        var result = ExecuteScript("\"hello\"");
        Assert.Equal("hello", result);
    }

    [Fact]
    public void TestGenerateLiteral_String_Empty()
    {
        var result = ExecuteScript("\"\"");
        Assert.Equal("", result);
    }

    [Fact]
    public void TestGenerateLiteral_True()
    {
        var result = ExecuteScript("true");
        Assert.Equal(true, result);
    }

    [Fact]
    public void TestGenerateLiteral_False()
    {
        var result = ExecuteScript("false");
        Assert.Equal(false, result);
    }

    [Fact]
    public void TestGenerateLiteral_Null()
    {
        var result = ExecuteScript("null");
        Assert.Null(result);
    }

    #endregion

    #region 変数宣言と参照のテスト

    [Fact]
    public void TestGenerateVariable_Let()
    {
        var result = ExecuteScript(@"
            let x = 10
            x
        ");
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestGenerateVariable_Var()
    {
        var result = ExecuteScript(@"
            var x = 20
            x
        ");
        Assert.Equal(20.0, result);
    }

    [Fact]
    public void TestGenerateVariable_Let_String()
    {
        var result = ExecuteScript(@"
            let message = ""hello""
            message
        ");
        Assert.Equal("hello", result);
    }

    [Fact]
    public void TestGenerateVariable_Multiple()
    {
        var result = ExecuteScript(@"
            let x = 10
            let y = 20
            y
        ");
        Assert.Equal(20.0, result);
    }

    #endregion

    #region 代入式のテスト

    [Fact]
    public void TestGenerateAssignment()
    {
        var result = ExecuteScript(@"
            var x = 10
            x = 20
            x
        ");
        Assert.Equal(20.0, result);
    }

    [Fact]
    public void TestGenerateAssignment_ReturnsValue()
    {
        var result = ExecuteScript(@"
            var x = 10
            x = 20
        ");
        // 代入式は代入した値を返す
        Assert.Equal(20.0, result);
    }

    [Fact]
    public void TestGenerateAssignment_Multiple()
    {
        var result = ExecuteScript(@"
            var x = 10
            var y = 20
            x = 30
            y = 40
            x
        ");
        Assert.Equal(30.0, result);
    }

    #endregion

    #region ブロック式のテスト

    [Fact]
    public void TestGenerateBlock_SingleExpression()
    {
        var result = ExecuteScript(@"
            {
                10
            }
        ");
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestGenerateBlock_StatementAndExpression()
    {
        var result = ExecuteScript(@"
            {
                let x = 10
                x
            }
        ");
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestGenerateBlock_MultipleStatements()
    {
        var result = ExecuteScript(@"
            {
                let x = 10
                let y = 20
                y
            }
        ");
        Assert.Equal(20.0, result);
    }

    [Fact]
    public void TestGenerateBlock_OnlyStatements()
    {
        var result = ExecuteScript(@"
            {
                let x = 10
            }
        ");
        // 最後が文のみの場合はnull
        Assert.Null(result);
    }

    [Fact]
    public void TestGenerateBlock_NestedBlocks()
    {
        var result = ExecuteScript(@"
            {
                let x = 10
                {
                    let y = 20
                    y
                }
            }
        ");
        Assert.Equal(20.0, result);
    }

    #endregion

    #region 式文のテスト

    [Fact]
    public void TestGenerateExprStmt_LiteralOnly()
    {
        var result = ExecuteScript(@"
            42
        ");
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void TestGenerateExprStmt_Multiple()
    {
        var result = ExecuteScript(@"
            10
            20
            30
        ");
        // 最後の式の値が返る
        Assert.Equal(30.0, result);
    }

    #endregion

    #region 統合テスト

    [Fact]
    public void TestIntegration_ComplexBlock()
    {
        var result = ExecuteScript(@"
            let a = 10
            var b = 20
            {
                let c = 30
                b = 40
                c
            }
        ");
        Assert.Equal(30.0, result);
    }

    [Fact]
    public void TestIntegration_AssignmentChain()
    {
        var result = ExecuteScript(@"
            var x = 1
            var y = 2
            var z = 3
            x = 10
            y = 20
            z = 30
            z
        ");
        Assert.Equal(30.0, result);
    }

    #endregion

    #region 文字列補間のテスト

    [Fact]
    public void TestStringInterpolation_Simple()
    {
        var result = ExecuteScript(@"
            let name = ""Alice""
            ""Hello, ${name}!""
        ");
        Assert.Equal("Hello, Alice!", result);
    }

    [Fact]
    public void TestStringInterpolation_MultipleVariables()
    {
        var result = ExecuteScript(@"
            let x = 10
            let y = 20
            ""${x} + ${y} = ${x + y}""
        ");
        Assert.Equal("10 + 20 = 30", result);
    }

    [Fact]
    public void TestStringInterpolation_ComplexExpression()
    {
        var result = ExecuteScript(@"
            let value = 42
            ""The answer is ${value * 2}""
        ");
        Assert.Equal("The answer is 84", result);
    }

    [Fact]
    public void TestStringInterpolation_MethodCall()
    {
        var result = ExecuteScript(@"
            let str = ""hello""
            ""Upper: ${str.toUpper()}""
        ");
        Assert.Equal("Upper: HELLO", result);
    }

    [Fact]
    public void TestStringInterpolation_OnlyExpression()
    {
        var result = ExecuteScript(@"
            let value = 123
            ""${value}""
        ");
        Assert.Equal("123", result);
    }

    [Fact]
    public void TestStringInterpolation_EmptyString()
    {
        var result = ExecuteScript(@"
            let empty = """"
            ""Text: ${empty}End""
        ");
        Assert.Equal("Text: End", result);
    }

    #endregion
}
