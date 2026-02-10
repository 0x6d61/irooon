using Xunit;
using Irooon.Core.CodeGen;
using Irooon.Core.Runtime;

namespace Irooon.Tests.CodeGen;

/// <summary>
/// 型アノテーション E2E テスト
/// v0.12.2: 実行時型チェック
/// </summary>
public class TypeAnnotationTests
{
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

    #region パラメータ型チェック

    [Fact]
    public void TestTypeCheck_NumberParam_Pass()
    {
        var result = ExecuteScript(@"
            fn add(a: Number, b: Number): Number { a + b }
            add(1, 2)
        ");
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void TestTypeCheck_NumberParam_Fail()
    {
        Assert.Throws<RuntimeException>(() => ExecuteScript(@"
            fn add(a: Number, b: Number): Number { a + b }
            add(""hello"", 2)
        "));
    }

    [Fact]
    public void TestTypeCheck_StringParam()
    {
        var result = ExecuteScript(@"
            fn greet(name: String): String { ""Hello, "" + name }
            greet(""World"")
        ");
        Assert.Equal("Hello, World", result);
    }

    [Fact]
    public void TestTypeCheck_BooleanParam()
    {
        var result = ExecuteScript(@"
            fn check(flag: Boolean): Boolean { flag }
            check(true)
        ");
        Assert.Equal(true, result);
    }

    [Fact]
    public void TestTypeCheck_ListParam()
    {
        var result = ExecuteScript(@"
            fn first(items: List): Number { items[0] }
            first([10, 20, 30])
        ");
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestTypeCheck_HashParam()
    {
        var result = ExecuteScript(@"
            fn getName(data: Hash): String { data[""name""] }
            getName({name: ""Alice""})
        ");
        Assert.Equal("Alice", result);
    }

    [Fact]
    public void TestTypeCheck_FunctionParam()
    {
        var result = ExecuteScript(@"
            fn apply(f: Function, x: Number): Number { f(x) }
            apply(fn (n) { n * 2 }, 5)
        ");
        Assert.Equal(10.0, result);
    }

    #endregion

    #region 戻り値型チェック

    [Fact]
    public void TestTypeCheck_ReturnType_Pass()
    {
        var result = ExecuteScript(@"
            fn double(x: Number): Number { x * 2 }
            double(5)
        ");
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestTypeCheck_ReturnType_Fail()
    {
        Assert.Throws<RuntimeException>(() => ExecuteScript(@"
            fn bad(): Number { ""hello"" }
            bad()
        "));
    }

    #endregion

    #region 部分的アノテーション・デフォルト値

    [Fact]
    public void TestTypeCheck_PartialAnnotation()
    {
        // data は型注釈なし、limit は Number 型
        var result = ExecuteScript(@"
            fn process(data, limit: Number) { data }
            process(""anything"", 10)
        ");
        Assert.Equal("anything", result);
    }

    [Fact]
    public void TestTypeCheck_DefaultValue()
    {
        // デフォルト値を使用した場合も型チェックが通る
        var result = ExecuteScript(@"
            fn greet(name: String = ""World""): String { ""Hello, "" + name }
            greet()
        ");
        Assert.Equal("Hello, World", result);
    }

    #endregion

    #region ユーザー定義クラス

    [Fact]
    public void TestTypeCheck_UserDefinedClass()
    {
        var result = ExecuteScript(@"
            class Person {
                public var name = """"
                init(n) { name = n }
                public fn getName(): String { name }
            }
            fn greetPerson(p: Person): String { p.getName() }
            let alice = Person(""Alice"")
            greetPerson(alice)
        ");
        Assert.Equal("Alice", result);
    }

    [Fact]
    public void TestTypeCheck_UserDefinedClass_Fail()
    {
        Assert.Throws<RuntimeException>(() => ExecuteScript(@"
            class Person {
                public var name = """"
            }
            fn greetPerson(p: Person) { p }
            greetPerson(""not a person"")
        "));
    }

    #endregion

    #region ラムダ・アロー関数

    [Fact]
    public void TestTypeCheck_Lambda()
    {
        var result = ExecuteScript(@"
            let f = fn (x: Number): Number { x * 2 }
            f(5)
        ");
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestTypeCheck_Arrow()
    {
        var result = ExecuteScript(@"
            let g = (x: Number) => x * 2
            g(5)
        ");
        Assert.Equal(10.0, result);
    }

    #endregion

    #region クラスメソッド

    [Fact]
    public void TestTypeCheck_Method()
    {
        var result = ExecuteScript(@"
            class Calculator {
                public fn add(a: Number, b: Number): Number { a + b }
            }
            let calc = Calculator()
            calc.add(3, 4)
        ");
        Assert.Equal(7.0, result);
    }

    #endregion

    #region return 文

    [Fact]
    public void TestTypeCheck_ReturnStmt()
    {
        // 明示的 return でも戻り値型チェック
        var result = ExecuteScript(@"
            fn double(x: Number): Number {
                return x * 2
            }
            double(5)
        ");
        Assert.Equal(10.0, result);
    }

    #endregion

    #region 後方互換

    [Fact]
    public void TestTypeCheck_NoAnnotation()
    {
        // 型注釈なしの場合はチェックなし
        var result = ExecuteScript(@"
            fn add(a, b) { a + b }
            add(1, 2)
        ");
        Assert.Equal(3.0, result);
    }

    #endregion
}
