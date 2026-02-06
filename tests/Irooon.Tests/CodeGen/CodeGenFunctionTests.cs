using Xunit;
using Irooon.Core.CodeGen;
using Irooon.Core.Runtime;
using Irooon.Core.Ast.Expressions;

namespace Irooon.Tests.CodeGen;

/// <summary>
/// Task #16: CodeGen関数・クロージャのテスト
/// 仕様: docs/expression-tree-mapping.md セクション9
/// </summary>
public class CodeGenFunctionTests
{
    #region 関数定義のテスト

    [Fact]
    public void TestFunctionDef_NoArgs()
    {
        // 引数なしの関数定義と呼び出し
        var source = @"
fn greet() {
    ""hello""
}
greet()
";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);
        var ctx = new ScriptContext();
        var result = compiled(ctx);

        Assert.Equal("hello", result);
    }

    [Fact]
    public void TestFunctionDef_WithArgs()
    {
        // 引数ありの関数定義と呼び出し
        var source = @"
fn add(a, b) {
    a + b
}
add(3, 5)
";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);
        var ctx = new ScriptContext();
        var result = compiled(ctx);

        Assert.Equal(8.0, result);
    }

    [Fact]
    public void TestFunctionDef_MultipleArgs()
    {
        // 複数引数の関数
        var source = @"
fn multiply(x, y, z) {
    x * y * z
}
multiply(2, 3, 4)
";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);
        var ctx = new ScriptContext();
        var result = compiled(ctx);

        Assert.Equal(24.0, result);
    }

    [Fact]
    public void TestFunctionDef_ReturnsNull()
    {
        // 明示的にnullを返す関数
        var source = @"
fn returnNull() {
    null
}
returnNull()
";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);
        var ctx = new ScriptContext();
        var result = compiled(ctx);

        Assert.Null(result);
    }

    [Fact]
    public void TestFunctionDef_MultipleCalls()
    {
        // 同じ関数を複数回呼び出し
        var source = @"
fn double(x) {
    x * 2
}
let a = double(5)
let b = double(10)
a + b
";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);
        var ctx = new ScriptContext();
        var result = compiled(ctx);

        Assert.Equal(30.0, result);
    }

    #endregion

    #region ラムダ式のテスト

    [Fact]
    public void TestLambdaExpr_Simple()
    {
        // シンプルなラムダ式
        var source = @"
let double = fn (x) { x * 2 }
double(5)
";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);
        var ctx = new ScriptContext();
        var result = compiled(ctx);

        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestLambdaExpr_NoArgs()
    {
        // 引数なしのラムダ式
        var source = @"
let greet = fn () { ""hi"" }
greet()
";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);
        var ctx = new ScriptContext();
        var result = compiled(ctx);

        Assert.Equal("hi", result);
    }

    [Fact]
    public void TestLambdaExpr_MultipleArgs()
    {
        // 複数引数のラムダ式
        var source = @"
let add = fn (a, b, c) { a + b + c }
add(1, 2, 3)
";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);
        var ctx = new ScriptContext();
        var result = compiled(ctx);

        Assert.Equal(6.0, result);
    }

    #endregion

    #region クロージャのテスト（グローバル変数参照）

    [Fact]
    public void TestClosure_GlobalVariable()
    {
        // グローバル変数を参照するクロージャ
        var source = @"
let x = 10
fn getX() {
    x
}
getX()
";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);
        var ctx = new ScriptContext();
        var result = compiled(ctx);

        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestClosure_ModifyGlobalVariable()
    {
        // グローバル変数を変更するクロージャ
        var source = @"
var counter = 0
fn increment() {
    counter = counter + 1
}
increment()
increment()
counter
";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);
        var ctx = new ScriptContext();
        var result = compiled(ctx);

        Assert.Equal(2.0, result);
    }

    [Fact]
    public void TestClosure_LambdaReferencesGlobal()
    {
        // ラムダ式でグローバル変数を参照
        var source = @"
let multiplier = 3
let triple = fn (x) { x * multiplier }
triple(5)
";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);
        var ctx = new ScriptContext();
        var result = compiled(ctx);

        Assert.Equal(15.0, result);
    }

    #endregion

    #region 関数を値として扱うテスト

    [Fact]
    public void TestFunction_AsValue()
    {
        // 関数を変数に代入
        var source = @"
fn add(a, b) {
    a + b
}
let f = add
f(3, 7)
";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);
        var ctx = new ScriptContext();
        var result = compiled(ctx);

        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestFunction_PassAsArgument()
    {
        // 関数を引数として渡す（高階関数）
        var source = @"
fn apply(f, x) {
    f(x)
}
fn double(n) {
    n * 2
}
apply(double, 5)
";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);
        var ctx = new ScriptContext();
        var result = compiled(ctx);

        Assert.Equal(10.0, result);
    }

    #endregion

    #region 即時実行関数のテスト

    [Fact]
    public void TestIIFE_Immediately_Invoked_Function()
    {
        // 即時実行関数式（IIFE）
        var source = @"
(fn (x) { x + 1 })(5)
";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);
        var ctx = new ScriptContext();
        var result = compiled(ctx);

        Assert.Equal(6.0, result);
    }

    #endregion

    #region ネストした関数呼び出しのテスト

    [Fact]
    public void TestFunction_NestedCalls()
    {
        // ネストした関数呼び出し
        var source = @"
fn add(a, b) {
    a + b
}
fn multiply(x, y) {
    x * y
}
multiply(add(2, 3), add(4, 6))
";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);
        var ctx = new ScriptContext();
        var result = compiled(ctx);

        Assert.Equal(50.0, result);
    }

    #endregion

    #region エラーケースのテスト

    [Fact]
    public void TestFunction_CallNonCallable_ThrowsException()
    {
        // 呼び出し可能でないオブジェクトの呼び出し
        var source = @"
let notAFunction = 42
notAFunction()
";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);
        var ctx = new ScriptContext();

        Assert.Throws<InvalidOperationException>(() => compiled(ctx));
    }

    #endregion
}
