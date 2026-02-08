using Irooon.Core.Runtime;
using Xunit;

namespace Irooon.Tests.Runtime;

/// <summary>
/// 範囲リテラルの実行時テスト。
/// </summary>
public class RangeTests
{
    [Fact]
    public void TestCreateRange_Exclusive()
    {
        // 排他的範囲: 1..5 → [1, 2, 3, 4]
        var result = RuntimeHelpers.CreateRange(1.0, 5.0, false);

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Equal(4, list.Count);
        Assert.Equal(1.0, list[0]);
        Assert.Equal(2.0, list[1]);
        Assert.Equal(3.0, list[2]);
        Assert.Equal(4.0, list[3]);
    }

    [Fact]
    public void TestCreateRange_Inclusive()
    {
        // 包括的範囲: 1...5 → [1, 2, 3, 4, 5]
        var result = RuntimeHelpers.CreateRange(1.0, 5.0, true);

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Equal(5, list.Count);
        Assert.Equal(1.0, list[0]);
        Assert.Equal(2.0, list[1]);
        Assert.Equal(3.0, list[2]);
        Assert.Equal(4.0, list[3]);
        Assert.Equal(5.0, list[4]);
    }

    [Fact]
    public void TestCreateRange_Empty()
    {
        // 空の範囲: 5..5 → []
        var result = RuntimeHelpers.CreateRange(5.0, 5.0, false);

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Empty(list);
    }

    [Fact]
    public void TestCreateRange_SingleElement()
    {
        // 単一要素の範囲: 5...5 → [5]
        var result = RuntimeHelpers.CreateRange(5.0, 5.0, true);

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Single(list);
        Assert.Equal(5.0, list[0]);
    }

    [Fact]
    public void TestCreateRange_LargeRange()
    {
        // 大きな範囲: 1..100
        var result = RuntimeHelpers.CreateRange(1.0, 100.0, false);

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Equal(99, list.Count);
        Assert.Equal(1.0, list[0]);
        Assert.Equal(99.0, list[98]);
    }

    [Fact]
    public void TestCreateRange_Negative()
    {
        // 負の範囲: -5..0
        var result = RuntimeHelpers.CreateRange(-5.0, 0.0, false);

        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Equal(5, list.Count);
        Assert.Equal(-5.0, list[0]);
        Assert.Equal(-4.0, list[1]);
        Assert.Equal(-3.0, list[2]);
        Assert.Equal(-2.0, list[3]);
        Assert.Equal(-1.0, list[4]);
    }
}
