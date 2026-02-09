using Irooon.Core;
using Xunit;

namespace Irooon.Tests.Runtime;

/// <summary>
/// List追加プロトタイプメソッドのテスト
/// </summary>
public class ListExtMethodsTests
{
    private ScriptEngine CreateEngine() => new ScriptEngine();

    #region push() Tests

    [Fact]
    public void List_Push_AddsElement()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let arr = [1, 2]
            arr.push(3)
            arr
        ");

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Equal(3, list.Count);
        Assert.Equal(3.0, list[2]);
    }

    #endregion

    #region pop() Tests

    [Fact]
    public void List_Pop_RemovesAndReturnsLast()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let arr = [1, 2, 3]
            arr.pop()
        ");

        Assert.Equal(3.0, result);
    }

    [Fact]
    public void List_Pop_ModifiesList()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let arr = [1, 2, 3]
            arr.pop()
            arr.length()
        ");

        Assert.Equal(2.0, result);
    }

    #endregion

    #region indexOf() Tests

    [Fact]
    public void List_IndexOf_Found()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let arr = [10, 20, 30, 40]
            arr.indexOf(30)
        ");

        Assert.Equal(2.0, result);
    }

    [Fact]
    public void List_IndexOf_NotFound()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let arr = [10, 20, 30]
            arr.indexOf(99)
        ");

        Assert.Equal(-1.0, result);
    }

    #endregion

    #region includes() Tests

    [Fact]
    public void List_Includes_True()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let arr = [""a"", ""b"", ""c""]
            arr.includes(""b"")
        ");

        Assert.Equal(true, result);
    }

    [Fact]
    public void List_Includes_False()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let arr = [""a"", ""b""]
            arr.includes(""z"")
        ");

        Assert.Equal(false, result);
    }

    #endregion

    #region find() Tests

    [Fact]
    public void List_Find_Found()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let arr = [1, 2, 3, 4, 5]
            arr.find(fn(x) { x > 3 })
        ");

        Assert.Equal(4.0, result);
    }

    [Fact]
    public void List_Find_NotFound()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let arr = [1, 2, 3]
            arr.find(fn(x) { x > 10 })
        ");

        Assert.Null(result);
    }

    #endregion

    #region join() Tests

    [Fact]
    public void List_Join_WithSeparator()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let arr = [""a"", ""b"", ""c""]
            arr.join(""-"")
        ");

        Assert.Equal("a-b-c", result);
    }

    [Fact]
    public void List_Join_Numbers()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let arr = [1, 2, 3]
            arr.join("", "")
        ");

        Assert.Equal("1, 2, 3", result);
    }

    #endregion

    #region concat() Tests

    [Fact]
    public void List_Concat_TwoLists()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let a = [1, 2]
            let b = [3, 4]
            a.concat(b)
        ");

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Equal(4, list.Count);
        Assert.Equal(1.0, list[0]);
        Assert.Equal(4.0, list[3]);
    }

    [Fact]
    public void List_Concat_DoesNotModifyOriginal()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let a = [1, 2]
            let b = [3, 4]
            a.concat(b)
            a.length()
        ");

        Assert.Equal(2.0, result);
    }

    #endregion

    #region reverse() Tests

    [Fact]
    public void List_Reverse()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let arr = [1, 2, 3]
            arr.reverse()
        ");

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Equal(3.0, list[0]);
        Assert.Equal(2.0, list[1]);
        Assert.Equal(1.0, list[2]);
    }

    #endregion

    #region sort() Tests

    [Fact]
    public void List_Sort_Numbers()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let arr = [3, 1, 4, 1, 5]
            arr.sort()
        ");

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Equal(1.0, list[0]);
        Assert.Equal(1.0, list[1]);
        Assert.Equal(3.0, list[2]);
        Assert.Equal(4.0, list[3]);
        Assert.Equal(5.0, list[4]);
    }

    [Fact]
    public void List_Sort_Strings()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let arr = [""banana"", ""apple"", ""cherry""]
            arr.sort()
        ");

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Equal("apple", list[0]);
        Assert.Equal("banana", list[1]);
        Assert.Equal("cherry", list[2]);
    }

    #endregion

    #region slice() Tests

    [Fact]
    public void List_Slice_WithStartAndEnd()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let arr = [10, 20, 30, 40, 50]
            arr.slice(1, 4)
        ");

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Equal(3, list.Count);
        Assert.Equal(20.0, list[0]);
        Assert.Equal(30.0, list[1]);
        Assert.Equal(40.0, list[2]);
    }

    [Fact]
    public void List_Slice_OnlyStart()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let arr = [10, 20, 30, 40, 50]
            arr.slice(2, null)
        ");

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Equal(3, list.Count);
        Assert.Equal(30.0, list[0]);
        Assert.Equal(40.0, list[1]);
        Assert.Equal(50.0, list[2]);
    }

    [Fact]
    public void List_Slice_DoesNotModifyOriginal()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"
            let arr = [10, 20, 30, 40, 50]
            arr.slice(1, 3)
            arr.length()
        ");

        Assert.Equal(5.0, result);
    }

    #endregion
}
