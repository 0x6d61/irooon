using Irooon.Core;

namespace Irooon.Tests.Runtime;

/// <summary>
/// 低レベルプリミティブ関数のテスト
/// stdlib.iro（irooonで書かれた標準ライブラリ）の基盤となる関数群
/// </summary>
public class PrimitiveTests
{
    private ScriptEngine CreateEngine() => new ScriptEngine();

    #region 文字列プリミティブ

    [Fact]
    public void StringLength_文字列の長さを返す()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"__stringLength(""hello"")");
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void StringLength_空文字列は0()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"__stringLength("""")");
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void CharAt_指定位置の文字を返す()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"__charAt(""hello"", 0)");
        Assert.Equal("h", result);
    }

    [Fact]
    public void CharAt_末尾の文字を返す()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"__charAt(""hello"", 4)");
        Assert.Equal("o", result);
    }

    [Fact]
    public void CharCodeAt_文字のUnicode値を返す()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"__charCodeAt(""A"", 0)");
        Assert.Equal(65.0, result);
    }

    [Fact]
    public void CharCodeAt_小文字aのコードは97()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"__charCodeAt(""a"", 0)");
        Assert.Equal(97.0, result);
    }

    [Fact]
    public void FromCharCode_Unicode値から文字を生成()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"__fromCharCode(65)");
        Assert.Equal("A", result);
    }

    [Fact]
    public void FromCharCode_小文字を生成()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"__fromCharCode(97)");
        Assert.Equal("a", result);
    }

    [Fact]
    public void Substring_開始位置から末尾まで()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"__substring(""hello world"", 6)");
        Assert.Equal("world", result);
    }

    [Fact]
    public void Substring_開始位置と長さを指定()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"__substring(""hello world"", 0, 5)");
        Assert.Equal("hello", result);
    }

    #endregion

    #region リストプリミティブ

    [Fact]
    public void ListLength_リストの長さを返す()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"__listLength([1, 2, 3])");
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void ListLength_空リストは0()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"__listLength([])");
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void ListPush_リストに要素を追加()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            var list = [1, 2]
            __listPush(list, 3)
            __listLength(list)
        ");
        Assert.Equal(3.0, result);
    }

    #endregion

    #region StringBuilder プリミティブ

    [Fact]
    public void StringBuilder_文字列を効率的に構築()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let sb = __stringBuilder()
            __sbAppend(sb, ""hello"")
            __sbAppend(sb, "" "")
            __sbAppend(sb, ""world"")
            __sbToString(sb)
        ");
        Assert.Equal("hello world", result);
    }

    #endregion

    #region 型判定プリミティブ

    [Fact]
    public void TypeOf_文字列の型名を返す()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"__typeOf(""hello"")");
        Assert.Equal("String", result);
    }

    [Fact]
    public void TypeOf_数値の型名を返す()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"__typeOf(42)");
        Assert.Equal("Number", result);
    }

    [Fact]
    public void TypeOf_リストの型名を返す()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"__typeOf([1, 2])");
        Assert.Equal("List", result);
    }

    [Fact]
    public void TypeOf_ハッシュの型名を返す()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"__typeOf({name: ""Alice""})");
        Assert.Equal("Hash", result);
    }

    [Fact]
    public void TypeOf_真偽値の型名を返す()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"__typeOf(true)");
        Assert.Equal("Boolean", result);
    }

    [Fact]
    public void TypeOf_nullの型名を返す()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"__typeOf(null)");
        Assert.Equal("Null", result);
    }

    #endregion

    #region プロトタイプ登録プリミティブ

    [Fact]
    public void RegisterPrototype_関数を登録して呼び出せる()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            __registerPrototype(""String"", ""myMethod"", fn(self) {
                __stringLength(self)
            })
            ""hello"".myMethod()
        ");
        Assert.Equal(5.0, result);
    }

    #endregion

    #region 組み合わせテスト

    [Fact]
    public void 文字列プリミティブの組み合わせ_大文字変換()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let str = ""abc""
            let len = __stringLength(str)
            let sb = __stringBuilder()
            var i = 0
            for (i < len) {
                let code = __charCodeAt(str, i)
                if (code >= 97 and code <= 122) {
                    __sbAppend(sb, __fromCharCode(code - 32))
                } else {
                    __sbAppend(sb, __charAt(str, i))
                }
                i = i + 1
            }
            __sbToString(sb)
        ");
        Assert.Equal("ABC", result);
    }

    #endregion
}
