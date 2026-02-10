using Xunit;
using Irooon.Core;
using Irooon.Core.CodeGen;
using Irooon.Core.Runtime;

namespace Irooon.Tests.Optimization;

/// <summary>
/// v0.12.6 トップレベルスコープ Array 最適化テスト。
/// optimizeTopLevel=true の場合、トップレベル let/var が ctx.Locals に格納される。
/// </summary>
public class TopLevelScopeTests
{
    private readonly ScriptEngine _engine = new();

    #region Basic top-level optimization

    [Fact]
    public void TopLevel_SimpleVarAssignment()
    {
        var result = _engine.Execute(@"
            var x = 10
            x
        ");
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TopLevel_MultipleVars()
    {
        var result = _engine.Execute(@"
            var a = 1
            var b = 2
            var c = 3
            a + b + c
        ");
        Assert.Equal(6.0, result);
    }

    [Fact]
    public void TopLevel_LetAndVar()
    {
        var result = _engine.Execute(@"
            let x = 10
            var y = 20
            x + y
        ");
        Assert.Equal(30.0, result);
    }

    [Fact]
    public void TopLevel_VarReassignment()
    {
        var result = _engine.Execute(@"
            var sum = 0
            sum = sum + 10
            sum = sum + 20
            sum
        ");
        Assert.Equal(30.0, result);
    }

    #endregion

    #region Loop optimization (primary target)

    [Fact]
    public void TopLevel_ForLoop()
    {
        var result = _engine.Execute(@"
            var sum = 0
            var i = 0
            for (i < 100) {
                sum = sum + i
                i = i + 1
            }
            sum
        ");
        Assert.Equal(4950.0, result);
    }

    [Fact]
    public void TopLevel_ForLoop_Increment()
    {
        var result = _engine.Execute(@"
            var sum = 0
            var i = 0
            for (i < 100) {
                sum = sum + i
                i++
            }
            sum
        ");
        Assert.Equal(4950.0, result);
    }

    [Fact]
    public void TopLevel_NestedForLoop()
    {
        var result = _engine.Execute(@"
            var sum = 0
            var i = 0
            for (i < 10) {
                var j = 0
                for (j < 10) {
                    sum = sum + 1
                    j++
                }
                i++
            }
            sum
        ");
        Assert.Equal(100.0, result);
    }

    #endregion

    #region Fallback to Dictionary when inner functions exist

    [Fact]
    public void TopLevel_WithFunction_StillWorks()
    {
        var result = _engine.Execute(@"
            fn add(a, b) { a + b }
            add(3, 4)
        ");
        Assert.Equal(7.0, result);
    }

    [Fact]
    public void TopLevel_WithFunctionAndVars_StillWorks()
    {
        var result = _engine.Execute(@"
            var x = 10
            fn double(n) { n * 2 }
            double(x)
        ");
        Assert.Equal(20.0, result);
    }

    [Fact]
    public void TopLevel_WithLambda_StillWorks()
    {
        var result = _engine.Execute(@"
            var x = 10
            let f = (n) => n * 2
            f(x)
        ");
        Assert.Equal(20.0, result);
    }

    [Fact]
    public void TopLevel_WithClass_StillWorks()
    {
        var result = _engine.Execute(@"
            var x = 5
            class Counter {
                var count = 0
                fn increment() { count = count + 1 }
                fn getCount() { count }
            }
            let c = Counter()
            c.increment()
            c.increment()
            c.getCount() + x
        ");
        Assert.Equal(7.0, result);
    }

    #endregion

    #region Destructuring at top-level

    [Fact]
    public void TopLevel_ListDestructuring()
    {
        var result = _engine.Execute(@"
            let [a, b, c] = [10, 20, 30]
            a + b + c
        ");
        Assert.Equal(60.0, result);
    }

    [Fact]
    public void TopLevel_HashDestructuring()
    {
        var result = _engine.Execute(@"
            let {x, y} = {x: 10, y: 20}
            x + y
        ");
        Assert.Equal(30.0, result);
    }

    #endregion

    #region Complex scenarios

    [Fact]
    public void TopLevel_IfExpression()
    {
        var result = _engine.Execute(@"
            var x = 10
            var result = if (x > 5) { ""big"" } else { ""small"" }
            result
        ");
        Assert.Equal("big", result);
    }

    [Fact]
    public void TopLevel_StringInterpolation()
    {
        var result = _engine.Execute(@"
            var name = ""world""
            ""hello ${name}""
        ");
        Assert.Equal("hello world", result);
    }

    [Fact]
    public void TopLevel_TryCatch()
    {
        var result = _engine.Execute(@"
            var result = 0
            try {
                result = 42
            } catch (e) {
                result = -1
            }
            result
        ");
        Assert.Equal(42.0, result);
    }

    #endregion

    #region Compile API with optimizeTopLevel

    [Fact]
    public void Compile_OptimizeTopLevel_UsesLocals()
    {
        var source = @"
            var x = 10
            var y = 20
            x + y
        ";
        var tokens = new Irooon.Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Irooon.Core.Parser.Parser(tokens).Parse();
        var resolver = new Irooon.Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast, optimizeTopLevel: true);

        var ctx = new ScriptContext();
        var result = compiled(ctx);
        Assert.Equal(30.0, result);
        // Locals should have been allocated
        Assert.NotNull(ctx.Locals);
    }

    [Fact]
    public void Compile_NoOptimize_UsesGlobals()
    {
        var source = @"
            var x = 10
            x
        ";
        var tokens = new Irooon.Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Irooon.Core.Parser.Parser(tokens).Parse();
        var resolver = new Irooon.Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast, optimizeTopLevel: false);

        var ctx = new ScriptContext();
        var result = compiled(ctx);
        Assert.Equal(10.0, result);
        // Globals should have the variable
        Assert.True(ctx.Globals.ContainsKey("x"));
    }

    [Fact]
    public void Compile_OptimizeTopLevel_FallsBackWithInnerFunction()
    {
        var source = @"
            fn add(a, b) { a + b }
            add(3, 4)
        ";
        var tokens = new Irooon.Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Irooon.Core.Parser.Parser(tokens).Parse();
        var resolver = new Irooon.Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast, optimizeTopLevel: true);

        var ctx = new ScriptContext();
        var result = compiled(ctx);
        Assert.Equal(7.0, result);
        // Falls back to Globals because FunctionDef exists
        Assert.True(ctx.Globals.ContainsKey("add"));
    }

    #endregion
}
