using Irooon.Core.CodeGen;
using Irooon.Core.Lexer;
using Irooon.Core.Parser;
using Irooon.Core.Resolver;
using Irooon.Core.Runtime;
using Xunit;

namespace Irooon.Tests.CodeGen;

/// <summary>
/// CodeGeneratorのクラスとインスタンス実装をテストします。
/// Task #17: クラスとインスタンスの変換
/// </summary>
public class CodeGenClassTests
{
    private object? CompileAndRun(string source)
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

    #region クラス定義テスト

    [Fact]
    public void TestClassDef_Empty()
    {
        var source = @"
        class Counter {
        }
        ";

        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);
        var ctx = new ScriptContext();
        compiled(ctx);

        Assert.True(ctx.Classes.ContainsKey("Counter"));
        Assert.NotNull(ctx.Classes["Counter"]);
    }

    [Fact]
    public void TestClassDef_WithField()
    {
        var source = @"
        class Counter {
            public var value = 0
        }
        ";

        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);
        var ctx = new ScriptContext();
        compiled(ctx);

        Assert.True(ctx.Classes.ContainsKey("Counter"));
        var cls = ctx.Classes["Counter"];
        Assert.Single(cls.Fields);
        Assert.Equal("value", cls.Fields[0].Name);
        Assert.True(cls.Fields[0].IsPublic);
    }

    [Fact]
    public void TestClassDef_WithMethod()
    {
        var source = @"
        class Counter {
            public fn getValue() {
                42
            }
        }
        ";

        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);
        var ctx = new ScriptContext();
        compiled(ctx);

        Assert.True(ctx.Classes.ContainsKey("Counter"));
        var cls = ctx.Classes["Counter"];
        Assert.Single(cls.Methods);
        Assert.True(cls.Methods.ContainsKey("getValue"));
    }

    #endregion

    #region インスタンス生成テスト

    [Fact]
    public void TestNewExpr_EmptyClass()
    {
        var source = @"
        class Counter {
        }
        Counter()
        ";

        var result = CompileAndRun(source);
        Assert.IsType<IroInstance>(result);
        var instance = (IroInstance)result!;
        Assert.Equal("Counter", instance.Class.Name);
    }

    [Fact]
    public void TestNewExpr_WithField()
    {
        var source = @"
        class Counter {
            public var value = 42
        }
        Counter()
        ";

        var result = CompileAndRun(source);
        Assert.IsType<IroInstance>(result);
        var instance = (IroInstance)result!;
        Assert.True(instance.Fields.ContainsKey("value"));
        Assert.Equal(42.0, instance.Fields["value"]);
    }

    #endregion

    #region メンバアクセステスト

    [Fact]
    public void TestMemberAccess_Field()
    {
        var source = @"
        class Counter {
            public var value = 42
        }
        let c = Counter()
        c.value
        ";

        var result = CompileAndRun(source);
        Assert.Equal(42.0, result);
    }

    [Fact(Skip = "Parser does not support member assignment yet")]
    public void TestMemberAssign_Field()
    {
        var source = @"
        class Counter {
            public var value = 0
        }
        let c = Counter()
        c.value = 100
        c.value
        ";

        var result = CompileAndRun(source);
        Assert.Equal(100.0, result);
    }

    [Fact]
    public void TestMemberAccess_Method()
    {
        var source = @"
        class Counter {
            public var value = 42

            public fn getValue() {
                value
            }
        }
        let c = Counter()
        c.getValue()
        ";

        var result = CompileAndRun(source);
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void TestMethodCall_Increment()
    {
        var source = @"
        class Counter {
            public var value = 0

            public fn increment() {
                value = value + 1
            }

            public fn getValue() {
                value
            }
        }
        let c = Counter()
        c.increment()
        c.getValue()
        ";

        var result = CompileAndRun(source);
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void TestMethodCall_MultipleIncrement()
    {
        var source = @"
        class Counter {
            public var value = 0

            public fn increment() {
                value = value + 1
            }

            public fn getValue() {
                value
            }
        }
        let c = Counter()
        c.increment()
        c.increment()
        c.increment()
        c.getValue()
        ";

        var result = CompileAndRun(source);
        Assert.Equal(3.0, result);
    }

    #endregion

    #region initメソッドテスト

    [Fact]
    public void TestInit_NoArgs()
    {
        var source = @"
        class Counter {
            public var value = 0

            init() {
                value = 100
            }
        }
        let c = Counter()
        c.value
        ";

        var result = CompileAndRun(source);
        Assert.Equal(100.0, result);
    }

    [Fact]
    public void TestInit_WithArgs()
    {
        var source = @"
        class Counter {
            public var value = 0

            init(initialValue) {
                value = initialValue
            }
        }
        let c = Counter(10)
        c.value
        ";

        var result = CompileAndRun(source);
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestInit_MultipleArgs()
    {
        var source = @"
        class Point {
            public var x = 0
            public var y = 0

            init(xVal, yVal) {
                x = xVal
                y = yVal
            }
        }
        let p = Point(3, 4)
        p.x + p.y
        ";

        var result = CompileAndRun(source);
        Assert.Equal(7.0, result);
    }

    #endregion

    #region インデックスアクセステスト（簡易実装）

    [Fact]
    public void TestIndexExpr_AsFieldAccess()
    {
        // v0.1ではインデックスアクセスはメンバアクセスとして扱う
        var source = @"
        class Container {
            public var item = 42
        }
        let c = Container()
        c[""item""]
        ";

        var result = CompileAndRun(source);
        Assert.Equal(42.0, result);
    }

    #endregion

    #region エラーケーステスト

    [Fact]
    public void TestNewExpr_ClassNotFound()
    {
        var source = @"
        UnknownClass()
        ";

        Assert.Throws<InvalidOperationException>(() => CompileAndRun(source));
    }

    [Fact]
    public void TestMemberAccess_FieldNotFound()
    {
        var source = @"
        class Counter {
            public var value = 0
        }
        let c = Counter()
        c.unknownField
        ";

        Assert.Throws<InvalidOperationException>(() => CompileAndRun(source));
    }

    [Fact]
    public void TestMemberAccess_OnNull()
    {
        var source = @"
        let c = null
        c.value
        ";

        Assert.Throws<InvalidOperationException>(() => CompileAndRun(source));
    }

    #endregion

    #region 複雑なテストケース

    [Fact]
    public void TestMultipleInstances()
    {
        var source = @"
        class Counter {
            public var value = 0

            init(initialValue) {
                value = initialValue
            }

            public fn increment() {
                value = value + 1
            }
        }
        let c1 = Counter(10)
        let c2 = Counter(20)
        c1.increment()
        c2.increment()
        c2.increment()
        c1.value + c2.value
        ";

        var result = CompileAndRun(source);
        Assert.Equal(33.0, result); // 11 + 22
    }

    [Fact]
    public void TestNestedMemberAccess()
    {
        var source = @"
        class Inner {
            public var value = 42
        }
        class Outer {
            public var inner = null

            init() {
                inner = Inner()
            }
        }
        let o = Outer()
        o.inner.value
        ";

        var result = CompileAndRun(source);
        Assert.Equal(42.0, result);
    }

    #endregion
}
