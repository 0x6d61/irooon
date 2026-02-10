using Irooon.Core.Runtime;

namespace Irooon.Tests.Optimization;

/// <summary>
/// v0.12.5 パフォーマンス最適化テスト
/// - Double Fast Path（算術・比較・インクリメント）
/// - Boolean キャッシング（BoxedTrue/BoxedFalse）
/// - CallFrame struct 化
/// </summary>
public class FastPathTests
{
    #region Double Fast Path - Arithmetic

    [Fact]
    public void Add_DoubleFastPath()
    {
        var result = RuntimeHelpers.Add(3.0, 5.0);
        Assert.Equal(8.0, result);
    }

    [Fact]
    public void Sub_DoubleFastPath()
    {
        var result = RuntimeHelpers.Sub(10.0, 3.0);
        Assert.Equal(7.0, result);
    }

    [Fact]
    public void Mul_DoubleFastPath()
    {
        var result = RuntimeHelpers.Mul(4.0, 5.0);
        Assert.Equal(20.0, result);
    }

    [Fact]
    public void Div_DoubleFastPath()
    {
        var result = RuntimeHelpers.Div(15.0, 3.0);
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void Div_DoubleFastPath_DivByZero()
    {
        Assert.Throws<DivideByZeroException>(() => RuntimeHelpers.Div(1.0, 0.0));
    }

    [Fact]
    public void Mod_DoubleFastPath()
    {
        var result = RuntimeHelpers.Mod(10.0, 3.0);
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void Power_DoubleFastPath()
    {
        var result = RuntimeHelpers.Power(2.0, 10.0);
        Assert.Equal(1024.0, result);
    }

    #endregion

    #region Double Fast Path - Comparison

    [Fact]
    public void Lt_DoubleFastPath_True()
    {
        var result = RuntimeHelpers.Lt(3.0, 5.0);
        Assert.Equal(true, result);
    }

    [Fact]
    public void Lt_DoubleFastPath_False()
    {
        var result = RuntimeHelpers.Lt(5.0, 3.0);
        Assert.Equal(false, result);
    }

    [Fact]
    public void Le_DoubleFastPath_Equal()
    {
        Assert.Equal(true, RuntimeHelpers.Le(5.0, 5.0));
        Assert.Equal(true, RuntimeHelpers.Le(3.0, 5.0));
        Assert.Equal(false, RuntimeHelpers.Le(5.0, 3.0));
    }

    [Fact]
    public void Gt_DoubleFastPath()
    {
        Assert.Equal(true, RuntimeHelpers.Gt(5.0, 3.0));
        Assert.Equal(false, RuntimeHelpers.Gt(3.0, 5.0));
    }

    [Fact]
    public void Ge_DoubleFastPath()
    {
        Assert.Equal(true, RuntimeHelpers.Ge(5.0, 5.0));
        Assert.Equal(true, RuntimeHelpers.Ge(5.0, 3.0));
        Assert.Equal(false, RuntimeHelpers.Ge(3.0, 5.0));
    }

    [Fact]
    public void Eq_DoubleFastPath()
    {
        Assert.Equal(true, RuntimeHelpers.Eq(5.0, 5.0));
        Assert.Equal(false, RuntimeHelpers.Eq(3.0, 5.0));
    }

    [Fact]
    public void Ne_DoubleFastPath()
    {
        Assert.Equal(true, RuntimeHelpers.Ne(3.0, 5.0));
        Assert.Equal(false, RuntimeHelpers.Ne(5.0, 5.0));
    }

    #endregion

    #region Double Fast Path - Unary

    [Fact]
    public void Increment_DoubleFastPath()
    {
        var result = RuntimeHelpers.Increment(5.0);
        Assert.Equal(6.0, result);
    }

    [Fact]
    public void Decrement_DoubleFastPath()
    {
        var result = RuntimeHelpers.Decrement(5.0);
        Assert.Equal(4.0, result);
    }

    [Fact]
    public void Negate_DoubleFastPath()
    {
        var result = RuntimeHelpers.Negate(5.0);
        Assert.Equal(-5.0, result);
    }

    #endregion

    #region Boolean Caching

    [Fact]
    public void BoolCaching_Lt_ReturnsCachedTrue()
    {
        var result = RuntimeHelpers.Lt(3.0, 5.0);
        Assert.Same(RuntimeHelpers.BoxedTrue, result);
    }

    [Fact]
    public void BoolCaching_Lt_ReturnsCachedFalse()
    {
        var result = RuntimeHelpers.Lt(5.0, 3.0);
        Assert.Same(RuntimeHelpers.BoxedFalse, result);
    }

    [Fact]
    public void BoolCaching_Eq_ReturnsCachedTrue()
    {
        var result = RuntimeHelpers.Eq(5.0, 5.0);
        Assert.Same(RuntimeHelpers.BoxedTrue, result);
    }

    [Fact]
    public void BoolCaching_Not_ReturnsCachedBool()
    {
        Assert.Same(RuntimeHelpers.BoxedTrue, RuntimeHelpers.Not(null));
        Assert.Same(RuntimeHelpers.BoxedFalse, RuntimeHelpers.Not(true));
    }

    #endregion

    #region CallFrame struct

    [Fact]
    public void CallFrame_IsValueType()
    {
        Assert.True(typeof(CallFrame).IsValueType);
    }

    [Fact]
    public void CallFrame_PreservesData()
    {
        var frame = new CallFrame("test", 10, 5);
        Assert.Equal("test", frame.FunctionName);
        Assert.Equal(10, frame.Line);
        Assert.Equal(5, frame.Column);
    }

    #endregion

    #region Fallback - Existing behavior preserved

    [Fact]
    public void Add_StringConcat_StillWorks()
    {
        Assert.Equal("ab", RuntimeHelpers.Add("a", "b"));
        Assert.Equal("3hello", RuntimeHelpers.Add(3.0, "hello"));
    }

    [Fact]
    public void Eq_NullHandling_StillWorks()
    {
        Assert.Equal(true, RuntimeHelpers.Eq(null!, null!));
        Assert.Equal(false, RuntimeHelpers.Eq(null!, 5.0));
        Assert.Equal(false, RuntimeHelpers.Eq(5.0, null!));
    }

    [Fact]
    public void Lt_NullThrows_StillWorks()
    {
        Assert.Throws<InvalidOperationException>(() => RuntimeHelpers.Lt(null!, 5.0));
    }

    #endregion
}
