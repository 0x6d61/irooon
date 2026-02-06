using Irooon.Core.Runtime;

namespace Irooon.Tests.Runtime;

public class RuntimeHelpersTests
{
    #region IsTruthy Tests

    [Fact]
    public void IsTruthy_nullはfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.IsTruthy(null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsTruthy_bool_trueはtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.IsTruthy(true);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsTruthy_bool_falseはfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.IsTruthy(false);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsTruthy_数値はtrueを返す()
    {
        // Act & Assert
        Assert.True(RuntimeHelpers.IsTruthy(1.0));
        Assert.True(RuntimeHelpers.IsTruthy(0.0));
        Assert.True(RuntimeHelpers.IsTruthy(-5.5));
    }

    [Fact]
    public void IsTruthy_文字列はtrueを返す()
    {
        // Act & Assert
        Assert.True(RuntimeHelpers.IsTruthy("hello"));
        Assert.True(RuntimeHelpers.IsTruthy(""));
    }

    [Fact]
    public void IsTruthy_オブジェクトはtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.IsTruthy(new object());

        // Assert
        Assert.True(result);
    }

    #endregion

    #region Add Tests

    [Fact]
    public void Add_整数同士の加算()
    {
        // Act
        var result = RuntimeHelpers.Add(3.0, 5.0);

        // Assert
        Assert.Equal(8.0, result);
    }

    [Fact]
    public void Add_小数同士の加算()
    {
        // Act
        var result = RuntimeHelpers.Add(3.5, 2.5);

        // Assert
        Assert.Equal(6.0, result);
    }

    [Fact]
    public void Add_負の数の加算()
    {
        // Act
        var result = RuntimeHelpers.Add(-3.0, 5.0);

        // Assert
        Assert.Equal(2.0, result);
    }

    #endregion

    #region Sub Tests

    [Fact]
    public void Sub_整数同士の減算()
    {
        // Act
        var result = RuntimeHelpers.Sub(10.0, 3.0);

        // Assert
        Assert.Equal(7.0, result);
    }

    [Fact]
    public void Sub_小数同士の減算()
    {
        // Act
        var result = RuntimeHelpers.Sub(5.5, 2.5);

        // Assert
        Assert.Equal(3.0, result);
    }

    #endregion

    #region Mul Tests

    [Fact]
    public void Mul_整数同士の乗算()
    {
        // Act
        var result = RuntimeHelpers.Mul(3.0, 4.0);

        // Assert
        Assert.Equal(12.0, result);
    }

    [Fact]
    public void Mul_小数同士の乗算()
    {
        // Act
        var result = RuntimeHelpers.Mul(2.5, 4.0);

        // Assert
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void Mul_ゼロとの乗算()
    {
        // Act
        var result = RuntimeHelpers.Mul(5.0, 0.0);

        // Assert
        Assert.Equal(0.0, result);
    }

    #endregion

    #region Div Tests

    [Fact]
    public void Div_整数同士の除算()
    {
        // Act
        var result = RuntimeHelpers.Div(10.0, 2.0);

        // Assert
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void Div_小数の除算()
    {
        // Act
        var result = RuntimeHelpers.Div(7.5, 2.5);

        // Assert
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void Div_ゼロ除算は例外を投げる()
    {
        // Act & Assert
        Assert.Throws<DivideByZeroException>(() => RuntimeHelpers.Div(10.0, 0.0));
    }

    #endregion

    #region Mod Tests

    [Fact]
    public void Mod_整数の剰余()
    {
        // Act
        var result = RuntimeHelpers.Mod(10.0, 3.0);

        // Assert
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void Mod_小数の剰余()
    {
        // Act
        var result = RuntimeHelpers.Mod(7.5, 2.0);

        // Assert
        Assert.Equal(1.5, result);
    }

    #endregion

    #region Comparison Tests

    [Fact]
    public void Eq_等しい値はtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.Eq(5.0, 5.0);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void Eq_異なる値はfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.Eq(5.0, 3.0);

        // Assert
        Assert.False((bool)result);
    }

    [Fact]
    public void Ne_異なる値はtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.Ne(5.0, 3.0);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void Ne_等しい値はfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.Ne(5.0, 5.0);

        // Assert
        Assert.False((bool)result);
    }

    [Fact]
    public void Lt_小さい値はtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.Lt(3.0, 5.0);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void Lt_大きい値はfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.Lt(5.0, 3.0);

        // Assert
        Assert.False((bool)result);
    }

    [Fact]
    public void Lt_等しい値はfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.Lt(5.0, 5.0);

        // Assert
        Assert.False((bool)result);
    }

    [Fact]
    public void Le_小さい値はtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.Le(3.0, 5.0);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void Le_等しい値はtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.Le(5.0, 5.0);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void Le_大きい値はfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.Le(5.0, 3.0);

        // Assert
        Assert.False((bool)result);
    }

    [Fact]
    public void Gt_大きい値はtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.Gt(5.0, 3.0);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void Gt_小さい値はfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.Gt(3.0, 5.0);

        // Assert
        Assert.False((bool)result);
    }

    [Fact]
    public void Gt_等しい値はfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.Gt(5.0, 5.0);

        // Assert
        Assert.False((bool)result);
    }

    [Fact]
    public void Ge_大きい値はtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.Ge(5.0, 3.0);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void Ge_等しい値はtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.Ge(5.0, 5.0);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void Ge_小さい値はfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.Ge(3.0, 5.0);

        // Assert
        Assert.False((bool)result);
    }

    #endregion

    #region Not Tests

    [Fact]
    public void Not_trueはfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.Not(true);

        // Assert
        Assert.False((bool)result);
    }

    [Fact]
    public void Not_falseはtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.Not(false);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void Not_nullはtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.Not(null);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void Not_オブジェクトはfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.Not(new object());

        // Assert
        Assert.False((bool)result);
    }

    #endregion
}
