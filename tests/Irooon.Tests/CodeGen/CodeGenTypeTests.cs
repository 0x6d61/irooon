using Xunit;
using Irooon.Core.CodeGen;
using Irooon.Core.Runtime;

namespace Irooon.Tests.CodeGen;

/// <summary>
/// typeof / instanceof のテスト
/// Issue #35: v0.11.0
/// </summary>
public class CodeGenTypeTests
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

    #region typeof テスト

    [Fact]
    public void TestTypeOf_Number()
    {
        var result = ExecuteScript("typeof(42)");
        Assert.Equal("Number", result);
    }

    [Fact]
    public void TestTypeOf_String()
    {
        var result = ExecuteScript(@"typeof(""hello"")");
        Assert.Equal("String", result);
    }

    [Fact]
    public void TestTypeOf_Boolean()
    {
        var result = ExecuteScript("typeof(true)");
        Assert.Equal("Boolean", result);
    }

    [Fact]
    public void TestTypeOf_Null()
    {
        var result = ExecuteScript("typeof(null)");
        Assert.Equal("Null", result);
    }

    [Fact]
    public void TestTypeOf_List()
    {
        var result = ExecuteScript("typeof([1, 2, 3])");
        Assert.Equal("List", result);
    }

    [Fact]
    public void TestTypeOf_Hash()
    {
        var result = ExecuteScript("typeof({ a: 1 })");
        Assert.Equal("Hash", result);
    }

    [Fact]
    public void TestTypeOf_Instance()
    {
        var source = @"
        class Dog {
        }
        typeof(Dog())
        ";
        var result = ExecuteScript(source);
        Assert.Equal("Dog", result);
    }

    [Fact]
    public void TestTypeOf_Function()
    {
        var source = @"
        fn add(a, b) { a + b }
        typeof(add)
        ";
        var result = ExecuteScript(source);
        Assert.Equal("Function", result);
    }

    #endregion

    #region instanceof テスト

    [Fact]
    public void TestInstanceOf_True()
    {
        var source = @"
        class Dog {
        }
        let d = Dog()
        d instanceof Dog
        ";
        var result = ExecuteScript(source);
        Assert.Equal(true, result);
    }

    [Fact]
    public void TestInstanceOf_False()
    {
        var source = @"
        class Dog {
        }
        class Cat {
        }
        let d = Dog()
        d instanceof Cat
        ";
        var result = ExecuteScript(source);
        Assert.Equal(false, result);
    }

    [Fact]
    public void TestInstanceOf_Inheritance()
    {
        var source = @"
        class Animal {
        }
        class Dog extends Animal {
        }
        let d = Dog()
        d instanceof Animal
        ";
        var result = ExecuteScript(source);
        Assert.Equal(true, result);
    }

    [Fact]
    public void TestInstanceOf_NonInstance()
    {
        var source = @"
        class Dog {
        }
        42 instanceof Dog
        ";
        var result = ExecuteScript(source);
        Assert.Equal(false, result);
    }

    [Fact]
    public void TestInstanceOf_InExpression()
    {
        var source = @"
        class Dog {
        }
        let d = Dog()
        if (d instanceof Dog) {
            ""yes""
        } else {
            ""no""
        }
        ";
        var result = ExecuteScript(source);
        Assert.Equal("yes", result);
    }

    #endregion
}
