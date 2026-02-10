using Irooon.Core.Runtime;
using Xunit;
using System.Collections.Generic;

namespace Irooon.Tests.Runtime;

/// <summary>
/// RuntimeHelpers の型チェック機能テスト
/// v0.12.2: 型アノテーション実装
/// </summary>
public class TypeCheckTests
{
    #region GetTypeName テスト

    [Fact]
    public void TestGetTypeName_AllTypes()
    {
        Assert.Equal("Null", RuntimeHelpers.GetTypeName(null));
        Assert.Equal("Number", RuntimeHelpers.GetTypeName(42.0));
        Assert.Equal("String", RuntimeHelpers.GetTypeName("hello"));
        Assert.Equal("Boolean", RuntimeHelpers.GetTypeName(true));
        Assert.Equal("List", RuntimeHelpers.GetTypeName(new List<object>()));
        Assert.Equal("Hash", RuntimeHelpers.GetTypeName(new Dictionary<string, object>()));
        Assert.Equal("Function", RuntimeHelpers.GetTypeName(
            new BuiltinFunction("test", (ctx, args) => null!)));
    }

    #endregion

    #region CheckType テスト

    [Fact]
    public void TestCheckType_Pass()
    {
        // 型が一致すればそのまま返る
        var result = RuntimeHelpers.CheckType(42.0, "Number", "x", "test", 1, 1);
        Assert.Equal(42.0, result);

        var strResult = RuntimeHelpers.CheckType("hello", "String", "s", "test", 1, 1);
        Assert.Equal("hello", strResult);
    }

    [Fact]
    public void TestCheckType_Fail()
    {
        // 型不一致で RuntimeException
        var ex = Assert.Throws<RuntimeException>(() =>
            RuntimeHelpers.CheckType("hello", "Number", "x", "add", 1, 1));
        Assert.Contains("Type error", ex.Message);
        Assert.Contains("parameter 'x'", ex.Message);
        Assert.Contains("expected Number", ex.Message);
        Assert.Contains("got String", ex.Message);
    }

    #endregion

    #region CheckReturnType テスト

    [Fact]
    public void TestCheckReturnType_Pass()
    {
        var result = RuntimeHelpers.CheckReturnType(42.0, "Number", "add", 1, 1);
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void TestCheckReturnType_Fail()
    {
        var ex = Assert.Throws<RuntimeException>(() =>
            RuntimeHelpers.CheckReturnType("hello", "Number", "add", 1, 1));
        Assert.Contains("Type error", ex.Message);
        Assert.Contains("function 'add'", ex.Message);
        Assert.Contains("expected to return Number", ex.Message);
        Assert.Contains("returned String", ex.Message);
    }

    #endregion
}
