namespace Irooon.Core.Runtime;

/// <summary>
/// Runtime実行時のヘルパーメソッド集
/// 算術演算、比較演算、論理演算、関数呼び出し、メンバアクセスなどを提供
/// </summary>
public static class RuntimeHelpers
{
    #region Truthy Evaluation

    /// <summary>
    /// 値がtruthyかどうかを判定
    /// - null → false
    /// - bool → そのまま
    /// - それ以外 → true
    /// </summary>
    public static bool IsTruthy(object? v)
    {
        if (v == null)
            return false;

        if (v is bool b)
            return b;

        // それ以外は全てtrue
        return true;
    }

    #endregion

    #region Arithmetic Operations

    /// <summary>
    /// 加算演算
    /// </summary>
    public static object Add(object a, object b)
    {
        if (a == null || b == null)
            throw new InvalidOperationException("Cannot add null values");

        double da = Convert.ToDouble(a);
        double db = Convert.ToDouble(b);
        return da + db;
    }

    /// <summary>
    /// 減算演算
    /// </summary>
    public static object Sub(object a, object b)
    {
        if (a == null || b == null)
            throw new InvalidOperationException("Cannot subtract null values");

        double da = Convert.ToDouble(a);
        double db = Convert.ToDouble(b);
        return da - db;
    }

    /// <summary>
    /// 乗算演算
    /// </summary>
    public static object Mul(object a, object b)
    {
        if (a == null || b == null)
            throw new InvalidOperationException("Cannot multiply null values");

        double da = Convert.ToDouble(a);
        double db = Convert.ToDouble(b);
        return da * db;
    }

    /// <summary>
    /// 除算演算
    /// </summary>
    public static object Div(object a, object b)
    {
        if (a == null || b == null)
            throw new InvalidOperationException("Cannot divide null values");

        double da = Convert.ToDouble(a);
        double db = Convert.ToDouble(b);

        if (db == 0.0)
            throw new DivideByZeroException("Division by zero");

        return da / db;
    }

    /// <summary>
    /// 剰余演算
    /// </summary>
    public static object Mod(object a, object b)
    {
        if (a == null || b == null)
            throw new InvalidOperationException("Cannot modulo null values");

        double da = Convert.ToDouble(a);
        double db = Convert.ToDouble(b);
        return da % db;
    }

    #endregion

    #region Comparison Operations

    /// <summary>
    /// 等価演算 (==)
    /// </summary>
    public static object Eq(object a, object b)
    {
        if (a == null && b == null)
            return true;

        if (a == null || b == null)
            return false;

        double da = Convert.ToDouble(a);
        double db = Convert.ToDouble(b);
        return da == db;
    }

    /// <summary>
    /// 非等価演算 (!=)
    /// </summary>
    public static object Ne(object a, object b)
    {
        return !(bool)Eq(a, b);
    }

    /// <summary>
    /// 小なり演算 (&lt;)
    /// </summary>
    public static object Lt(object a, object b)
    {
        if (a == null || b == null)
            throw new InvalidOperationException("Cannot compare null values");

        double da = Convert.ToDouble(a);
        double db = Convert.ToDouble(b);
        return da < db;
    }

    /// <summary>
    /// 小なりイコール演算 (&lt;=)
    /// </summary>
    public static object Le(object a, object b)
    {
        if (a == null || b == null)
            throw new InvalidOperationException("Cannot compare null values");

        double da = Convert.ToDouble(a);
        double db = Convert.ToDouble(b);
        return da <= db;
    }

    /// <summary>
    /// 大なり演算 (&gt;)
    /// </summary>
    public static object Gt(object a, object b)
    {
        if (a == null || b == null)
            throw new InvalidOperationException("Cannot compare null values");

        double da = Convert.ToDouble(a);
        double db = Convert.ToDouble(b);
        return da > db;
    }

    /// <summary>
    /// 大なりイコール演算 (&gt;=)
    /// </summary>
    public static object Ge(object a, object b)
    {
        if (a == null || b == null)
            throw new InvalidOperationException("Cannot compare null values");

        double da = Convert.ToDouble(a);
        double db = Convert.ToDouble(b);
        return da >= db;
    }

    #endregion

    #region Logical Operations

    /// <summary>
    /// 論理否定演算 (not)
    /// </summary>
    public static object Not(object? v)
    {
        return !IsTruthy(v);
    }

    #endregion
}
