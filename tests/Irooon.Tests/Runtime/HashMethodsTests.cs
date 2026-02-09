using Irooon.Core;
using Xunit;

namespace Irooon.Tests.Runtime;

/// <summary>
/// Hashプロトタイプメソッドのテスト
/// </summary>
public class HashMethodsTests
{
    private ScriptEngine CreateEngine() => new ScriptEngine();

    #region keys() Tests

    [Fact]
    public void Hash_Keys_ReturnsAllKeys()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let h = { a: 1, b: 2, c: 3 }
            h.keys()
        ");

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Equal(3, list.Count);
        Assert.Contains("a", list);
        Assert.Contains("b", list);
        Assert.Contains("c", list);
    }

    [Fact]
    public void Hash_Keys_EmptyHash()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let h = __hashNew()
            h.keys()
        ");

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Empty(list);
    }

    #endregion

    #region values() Tests

    [Fact]
    public void Hash_Values_ReturnsAllValues()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let h = { x: 10, y: 20, z: 30 }
            h.values()
        ");

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Equal(3, list.Count);
        Assert.Contains(10.0, list);
        Assert.Contains(20.0, list);
        Assert.Contains(30.0, list);
    }

    #endregion

    #region has() Tests

    [Fact]
    public void Hash_Has_ExistingKey_ReturnsTrue()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let h = { name: ""Alice"", age: 30 }
            h.has(""name"")
        ");

        Assert.Equal(true, result);
    }

    [Fact]
    public void Hash_Has_MissingKey_ReturnsFalse()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let h = { name: ""Alice"" }
            h.has(""email"")
        ");

        Assert.Equal(false, result);
    }

    #endregion

    #region delete() Tests

    [Fact]
    public void Hash_Delete_ExistingKey()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let h = { a: 1, b: 2 }
            h.delete(""a"")
            h.size()
        ");

        Assert.Equal(1.0, result);
    }

    [Fact]
    public void Hash_Delete_ReturnsTrue_WhenKeyExists()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let h = { a: 1 }
            h.delete(""a"")
        ");

        Assert.Equal(true, result);
    }

    [Fact]
    public void Hash_Delete_ReturnsFalse_WhenKeyMissing()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let h = { a: 1 }
            h.delete(""b"")
        ");

        Assert.Equal(false, result);
    }

    #endregion

    #region size() Tests

    [Fact]
    public void Hash_Size_ReturnsCount()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let h = { a: 1, b: 2, c: 3 }
            h.size()
        ");

        Assert.Equal(3.0, result);
    }

    [Fact]
    public void Hash_Size_EmptyHash()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let h = __hashNew()
            h.size()
        ");

        Assert.Equal(0.0, result);
    }

    #endregion

    #region isEmpty() Tests

    [Fact]
    public void Hash_IsEmpty_True()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let h = __hashNew()
            h.isEmpty()
        ");

        Assert.Equal(true, result);
    }

    [Fact]
    public void Hash_IsEmpty_False()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let h = { a: 1 }
            h.isEmpty()
        ");

        Assert.Equal(false, result);
    }

    #endregion
}
