using Irooon.Core;

namespace Irooon.Tests.Runtime;

/// <summary>
/// プロトタイプシステムのテスト
/// BoundMethodによるメソッドディスパッチの動作確認
/// </summary>
public class PrototypeSystemTests
{
    private ScriptEngine CreateEngine() => new ScriptEngine();

    #region プロトタイプ登録と呼び出し

    [Fact]
    public void String_プロトタイプメソッドを呼び出せる()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            __registerPrototype(""String"", ""myLen"", fn(self) {
                __stringLength(self)
            })
            ""hello"".myLen()
        ");
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void String_プロトタイプメソッドに引数を渡せる()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            __registerPrototype(""String"", ""repeat"", fn(self, n) {
                let sb = __stringBuilder()
                var i = 0
                for (i < n) {
                    __sbAppend(sb, self)
                    i = i + 1
                }
                __sbToString(sb)
            })
            ""ab"".repeat(3)
        ");
        Assert.Equal("ababab", result);
    }

    [Fact]
    public void List_プロトタイプメソッドを呼び出せる()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            __registerPrototype(""List"", ""mySize"", fn(self) {
                __listLength(self)
            })
            let list = [1, 2, 3]
            list.mySize()
        ");
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void Hash_プロトタイプメソッドを呼び出せる()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            __registerPrototype(""Hash"", ""size"", fn(self) {
                var count = 0
                for (k in self) {
                    count = count + 1
                }
                count
            })
            let obj = {name: ""Alice"", age: 30}
            obj.size()
        ");
        Assert.Equal(2.0, result);
    }

    #endregion

    #region BoundMethodの動作

    [Fact]
    public void BoundMethod_selfにターゲットオブジェクトが渡される()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            __registerPrototype(""String"", ""firstChar"", fn(self) {
                __charAt(self, 0)
            })
            ""world"".firstChar()
        ");
        Assert.Equal("w", result);
    }

    [Fact]
    public void BoundMethod_チェーン呼び出しができる()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            __registerPrototype(""String"", ""addExclaim"", fn(self) {
                ""${self}!""
            })
            __registerPrototype(""String"", ""myLen2"", fn(self) {
                __stringLength(self)
            })
            ""hello"".addExclaim().myLen2()
        ");
        Assert.Equal(6.0, result);
    }

    #endregion

    #region 既存メソッドとの共存

    [Fact]
    public void String_既存メソッドは引き続き動作する()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"""hello"".toUpper()");
        Assert.Equal("HELLO", result);
    }

    [Fact]
    public void List_既存メソッドは引き続き動作する()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let doubled = [1, 2, 3].map(fn(x) { x * 2 })
            doubled[0]
        ");
        Assert.Equal(2.0, result);
    }

    [Fact]
    public void プロトタイプが新規メソッドとして登録できる()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            __registerPrototype(""String"", ""shout"", fn(self) {
                ""${self}!!!""
            })
            ""hi"".shout()
        ");
        Assert.Equal("hi!!!", result);
    }

    #endregion

    #region Number/Boolean のプロトタイプ

    [Fact]
    public void Number_プロトタイプメソッドを呼び出せる()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            __registerPrototype(""Number"", ""isEven"", fn(self) {
                self % 2 == 0
            })
            let a = 4
            a.isEven()
        ");
        Assert.Equal(true, result);
    }

    [Fact]
    public void Boolean_プロトタイプメソッドを呼び出せる()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            __registerPrototype(""Boolean"", ""toStr"", fn(self) {
                if (self) { ""true"" } else { ""false"" }
            })
            true.toStr()
        ");
        Assert.Equal("true", result);
    }

    #endregion
}
