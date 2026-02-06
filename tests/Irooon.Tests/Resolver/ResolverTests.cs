using Xunit;
using Irooon.Core.Lexer;
using Irooon.Core.Parser;
using Irooon.Core.Resolver;

namespace Irooon.Tests.Resolver;

/// <summary>
/// Resolverのテスト
/// </summary>
public class ResolverTests
{
    [Fact]
    public void TestResolveSimpleVariable()
    {
        // 単純な変数の宣言と参照
        var source = @"
        let x = 10
        x
        ";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        Assert.Empty(resolver.GetErrors());
    }

    [Fact]
    public void TestUndefinedVariable()
    {
        // 未定義の変数を参照
        var source = "x";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        Assert.NotEmpty(resolver.GetErrors());
        Assert.Contains("Undefined variable", resolver.GetErrors()[0].Message);
    }

    [Fact]
    public void TestLetReassignment()
    {
        // let変数への再代入（エラー）
        var source = @"
        let x = 10
        x = 20
        ";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        Assert.NotEmpty(resolver.GetErrors());
        Assert.Contains("Cannot assign to 'let'", resolver.GetErrors()[0].Message);
    }

    [Fact]
    public void TestVarReassignment()
    {
        // var変数への再代入（OK）
        var source = @"
        var x = 10
        x = 20
        ";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        Assert.Empty(resolver.GetErrors());
    }

    [Fact]
    public void TestDuplicateDeclaration()
    {
        // 同じスコープ内での重複宣言
        var source = @"
        let x = 10
        let x = 20
        ";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        Assert.NotEmpty(resolver.GetErrors());
        Assert.Contains("already declared", resolver.GetErrors()[0].Message);
    }

    [Fact]
    public void TestFunctionScope()
    {
        // 関数スコープ内での変数シャドーイング
        var source = @"
        let x = 10
        fn foo() {
            let x = 20
            x
        }
        ";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        Assert.Empty(resolver.GetErrors());
    }

    [Fact]
    public void TestNestedScope()
    {
        // ネストされたブロックスコープ
        var source = @"
        let x = 1
        {
            let y = 2
            x + y
        }
        ";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        Assert.Empty(resolver.GetErrors());
    }

    [Fact]
    public void TestClosureCapture()
    {
        // クロージャによる変数キャプチャ
        var source = @"
        let x = 10
        fn foo() {
            x
        }
        ";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        Assert.Empty(resolver.GetErrors());
    }

    [Fact]
    public void TestFunctionParameterScope()
    {
        // 関数パラメータのスコープ
        var source = @"
        fn add(a, b) {
            a + b
        }
        ";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        Assert.Empty(resolver.GetErrors());
    }

    [Fact]
    public void TestMultipleStatements()
    {
        // 複数の文
        var source = @"
        let a = 1
        let b = 2
        let c = a + b
        ";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        Assert.Empty(resolver.GetErrors());
    }

    [Fact]
    public void TestWhileLoop()
    {
        // while文のスコープ
        var source = @"
        var i = 0
        while (i < 10) {
            i = i + 1
        }
        ";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        Assert.Empty(resolver.GetErrors());
    }

    [Fact]
    public void TestIfExpression()
    {
        // if式のスコープ
        var source = @"
        let x = 10
        if (x > 5) {
            let y = 20
            y
        } else {
            let z = 30
            z
        }
        ";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        Assert.Empty(resolver.GetErrors());
    }

    [Fact]
    public void TestLambdaExpression()
    {
        // ラムダ式のスコープ
        var source = @"
        let add = fn (a, b) { a + b }
        add(1, 2)
        ";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        Assert.Empty(resolver.GetErrors());
    }

    [Fact]
    public void TestScopeOutOfBlock()
    {
        // ブロック外での変数参照（エラー）
        var source = @"
        {
            let x = 10
        }
        x
        ";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        Assert.NotEmpty(resolver.GetErrors());
        Assert.Contains("Undefined variable", resolver.GetErrors()[0].Message);
    }

    [Fact]
    public void TestReturnStatement()
    {
        // return文
        var source = @"
        fn foo() {
            return 42
        }
        ";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        Assert.Empty(resolver.GetErrors());
    }

    [Fact]
    public void TestComplexExpression()
    {
        // 複雑な式の解析
        var source = @"
        let x = 10
        let y = 20
        let z = x + y * 2
        ";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        Assert.Empty(resolver.GetErrors());
    }
}
