using Irooon.Core;
using Xunit;

namespace Irooon.Tests.Runtime;

/// <summary>
/// Math関数とinput関数のテスト
/// </summary>
public class MathAndInputTests
{
    private ScriptEngine CreateEngine() => new ScriptEngine();

    #region Math.abs Tests

    [Fact]
    public void Math_Abs_Positive()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"Math[""abs""](5)");
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void Math_Abs_Negative()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"Math[""abs""](-5)");
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void Math_Abs_Zero()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"Math[""abs""](0)");
        Assert.Equal(0.0, result);
    }

    #endregion

    #region Math.floor Tests

    [Fact]
    public void Math_Floor()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"Math[""floor""](3.7)");
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void Math_Floor_Negative()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"Math[""floor""](-3.2)");
        Assert.Equal(-4.0, result);
    }

    #endregion

    #region Math.ceil Tests

    [Fact]
    public void Math_Ceil()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"Math[""ceil""](3.2)");
        Assert.Equal(4.0, result);
    }

    [Fact]
    public void Math_Ceil_Negative()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"Math[""ceil""](-3.7)");
        Assert.Equal(-3.0, result);
    }

    #endregion

    #region Math.round Tests

    [Fact]
    public void Math_Round_Up()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"Math[""round""](3.5)");
        Assert.Equal(4.0, result);
    }

    [Fact]
    public void Math_Round_Down()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"Math[""round""](3.4)");
        Assert.Equal(3.0, result);
    }

    #endregion

    #region Math.sqrt Tests

    [Fact]
    public void Math_Sqrt()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"Math[""sqrt""](16)");
        Assert.Equal(4.0, result);
    }

    [Fact]
    public void Math_Sqrt_NonPerfect()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"Math[""sqrt""](2)");
        Assert.Equal(Math.Sqrt(2), result);
    }

    #endregion

    #region Math.min / Math.max Tests

    [Fact]
    public void Math_Min()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"Math[""min""](3, 7)");
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void Math_Max()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"Math[""max""](3, 7)");
        Assert.Equal(7.0, result);
    }

    #endregion

    #region Math.random Tests

    [Fact]
    public void Math_Random_InRange()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"Math[""random""]()");
        Assert.IsType<double>(result);
        var val = (double)result;
        Assert.InRange(val, 0.0, 1.0);
    }

    #endregion

    #region Math.PI / Math.E Tests

    [Fact]
    public void Math_PI()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"Math[""PI""]");
        Assert.Equal(3.141592653589793, result);
    }

    [Fact]
    public void Math_E()
    {
        var engine = CreateEngine();
        var result = engine.Execute(@"Math[""E""]");
        Assert.Equal(2.718281828459045, result);
    }

    #endregion

    #region input Tests

    [Fact]
    public void Input_FunctionExists()
    {
        // input関数がグローバルに登録されていることを確認
        var engine = CreateEngine();
        var result = engine.Execute(@"__typeOf(input)");
        Assert.Equal("Function", result);
    }

    #endregion
}
