using Xunit;
using Irooon.Core.CodeGen;
using Irooon.Core.Runtime;
using System.Collections.Generic;

namespace Irooon.Tests.CodeGen;

/// <summary>
/// CodeGeneratorリスト・ハッシュテスト
/// Task #25: CodeGen拡張（リスト・ハッシュ生成）
/// </summary>
public class CodeGenListHashTests
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

    #region リストリテラルのテスト

    [Fact]
    public void TestGenerateList_Empty()
    {
        var result = ExecuteScript("[]");
        Assert.IsType<List<object>>(result);
        Assert.Empty((List<object>)result!);
    }

    [Fact]
    public void TestGenerateList_SingleElement()
    {
        var result = ExecuteScript("[42]");
        Assert.IsType<List<object>>(result);
        var list = (List<object>)result!;
        Assert.Single(list);
        Assert.Equal(42.0, list[0]);
    }

    [Fact]
    public void TestGenerateList_MultipleElements()
    {
        var result = ExecuteScript("[1, 2, 3]");
        Assert.IsType<List<object>>(result);
        var list = (List<object>)result!;
        Assert.Equal(3, list.Count);
        Assert.Equal(1.0, list[0]);
        Assert.Equal(2.0, list[1]);
        Assert.Equal(3.0, list[2]);
    }

    [Fact]
    public void TestGenerateList_MixedTypes()
    {
        var result = ExecuteScript("[1, \"hello\", true, null]");
        Assert.IsType<List<object>>(result);
        var list = (List<object>)result!;
        Assert.Equal(4, list.Count);
        Assert.Equal(1.0, list[0]);
        Assert.Equal("hello", list[1]);
        Assert.Equal(true, list[2]);
        Assert.Null(list[3]);
    }

    [Fact]
    public void TestGenerateList_WithVariables()
    {
        var result = ExecuteScript(@"
            let x = 10
            let y = 20
            let z = x + y
            let arr = [x, y, z]
            arr
        ");
        Assert.IsType<List<object>>(result);
        var list = (List<object>)result!;
        Assert.Equal(3, list.Count);
        Assert.Equal(10.0, list[0]);
        Assert.Equal(20.0, list[1]);
        Assert.Equal(30.0, list[2]);
    }

    [Fact]
    public void TestGenerateList_Nested()
    {
        var result = ExecuteScript("[1, [2, 3], 4]");
        Assert.IsType<List<object>>(result);
        var list = (List<object>)result!;
        Assert.Equal(3, list.Count);
        Assert.Equal(1.0, list[0]);
        Assert.IsType<List<object>>(list[1]);
        var nested = (List<object>)list[1];
        Assert.Equal(2, nested.Count);
        Assert.Equal(2.0, nested[0]);
        Assert.Equal(3.0, nested[1]);
        Assert.Equal(4.0, list[2]);
    }

    #endregion

    #region ハッシュリテラルのテスト

    [Fact]
    public void TestGenerateHash_Empty()
    {
        var result = ExecuteScript("{}");
        Assert.IsType<Dictionary<string, object>>(result);
        var hash = (Dictionary<string, object>)result!;
        Assert.Empty(hash);
    }

    [Fact]
    public void TestGenerateHash_SinglePair()
    {
        var result = ExecuteScript("{name: \"Alice\"}");
        Assert.IsType<Dictionary<string, object>>(result);
        var hash = (Dictionary<string, object>)result!;
        Assert.Single(hash);
        Assert.Equal("Alice", hash["name"]);
    }

    [Fact]
    public void TestGenerateHash_MultiplePairs()
    {
        var result = ExecuteScript("{name: \"Alice\", age: 30, active: true}");
        Assert.IsType<Dictionary<string, object>>(result);
        var hash = (Dictionary<string, object>)result!;
        Assert.Equal(3, hash.Count);
        Assert.Equal("Alice", hash["name"]);
        Assert.Equal(30.0, hash["age"]);
        Assert.Equal(true, hash["active"]);
    }

    [Fact]
    public void TestGenerateHash_WithVariables()
    {
        var result = ExecuteScript(@"
            let n = ""Bob""
            let a = 25
            {name: n, age: a}
        ");
        Assert.IsType<Dictionary<string, object>>(result);
        var hash = (Dictionary<string, object>)result!;
        Assert.Equal(2, hash.Count);
        Assert.Equal("Bob", hash["name"]);
        Assert.Equal(25.0, hash["age"]);
    }

    [Fact]
    public void TestGenerateHash_Nested()
    {
        var result = ExecuteScript("{user: {name: \"Alice\", age: 30}, count: 5}");
        Assert.IsType<Dictionary<string, object>>(result);
        var hash = (Dictionary<string, object>)result!;
        Assert.Equal(2, hash.Count);
        Assert.IsType<Dictionary<string, object>>(hash["user"]);
        var nested = (Dictionary<string, object>)hash["user"];
        Assert.Equal("Alice", nested["name"]);
        Assert.Equal(30.0, nested["age"]);
        Assert.Equal(5.0, hash["count"]);
    }

    #endregion

    #region インデックスアクセスのテスト

    [Fact]
    public void TestGenerateIndexAccess_List()
    {
        var result = ExecuteScript(@"
            let arr = [10, 20, 30]
            arr[1]
        ");
        Assert.Equal(20.0, result);
    }

    [Fact]
    public void TestGenerateIndexAccess_Hash()
    {
        var result = ExecuteScript(@"
            let obj = {name: ""Alice"", age: 30}
            obj[""name""]
        ");
        Assert.Equal("Alice", result);
    }

    #endregion

    #region インデックス代入のテスト

    [Fact]
    public void TestGenerateIndexAssign_List()
    {
        var result = ExecuteScript(@"
            let arr = [10, 20, 30]
            arr[1] = 99
            arr[1]
        ");
        Assert.Equal(99.0, result);
    }

    [Fact]
    public void TestGenerateIndexAssign_Hash()
    {
        var result = ExecuteScript(@"
            let obj = {name: ""Alice"", age: 30}
            obj[""age""] = 31
            obj[""age""]
        ");
        Assert.Equal(31.0, result);
    }

    [Fact]
    public void TestGenerateIndexAssign_ReturnsValue()
    {
        var result = ExecuteScript(@"
            let arr = [1, 2, 3]
            arr[0] = 100
        ");
        // 代入式は代入した値を返す
        Assert.Equal(100.0, result);
    }

    #endregion

    #region メンバ代入のテスト

    [Fact]
    public void TestGenerateMemberAssign_Basic()
    {
        var result = ExecuteScript(@"
            class Person {
                var name = null
                var age = null
            }
            let p = Person()
            p.name = ""Alice""
            p.name
        ");
        Assert.Equal("Alice", result);
    }

    [Fact]
    public void TestGenerateMemberAssign_ReturnsValue()
    {
        var result = ExecuteScript(@"
            class Person {
                var name = null
            }
            let p = Person()
            p.name = ""Bob""
        ");
        // 代入式は代入した値を返す
        Assert.Equal("Bob", result);
    }

    #endregion

    #region 統合テスト

    [Fact]
    public void TestIntegration_ListAndHashCombined()
    {
        var result = ExecuteScript(@"
            let users = [
                {name: ""Alice"", age: 30},
                {name: ""Bob"", age: 25}
            ]
            users[0][""name""]
        ");
        Assert.Equal("Alice", result);
    }

    [Fact]
    public void TestIntegration_ComplexNesting()
    {
        var result = ExecuteScript(@"
            let data = {
                users: [
                    {name: ""Alice"", scores: [85, 90, 92]},
                    {name: ""Bob"", scores: [78, 82, 88]}
                ]
            }
            data[""users""][0][""scores""][1]
        ");
        Assert.Equal(90.0, result);
    }

    [Fact]
    public void TestIntegration_ModifyNestedStructure()
    {
        var result = ExecuteScript(@"
            let data = {
                counter: 0,
                items: [1, 2, 3]
            }
            data[""counter""] = 5
            data[""items""][0] = 100
            data[""items""][0]
        ");
        Assert.Equal(100.0, result);
    }

    #endregion
}
