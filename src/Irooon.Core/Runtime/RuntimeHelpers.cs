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

    #region Function and Member Operations

    /// <summary>
    /// 関数を呼び出す
    /// インスタンスメソッドの場合、インスタンスのフィールドをGlobalsに展開する
    /// </summary>
    public static object Invoke(object callee, ScriptContext ctx, object[] args, object? thisArg = null)
    {
        if (callee == null)
            throw new InvalidOperationException("Cannot invoke null");

        if (callee is IroCallable callable)
        {
            // thisArgがIroInstanceの場合、フィールドをGlobalsに展開
            Dictionary<string, object>? savedFields = null;
            IroInstance? instance = thisArg as IroInstance;

            if (instance != null)
            {
                // 既存のフィールド名と衝突する場合に備えて保存
                savedFields = new Dictionary<string, object>();
                foreach (var field in instance.Fields)
                {
                    if (ctx.Globals.TryGetValue(field.Key, out var savedValue))
                    {
                        savedFields[field.Key] = savedValue;
                    }
                    ctx.Globals[field.Key] = field.Value;
                }
            }

            try
            {
                var result = callable.Invoke(ctx, args);

                // インスタンスのフィールドをGlobalsから逆反映
                if (instance != null)
                {
                    foreach (var fieldName in instance.Fields.Keys.ToList())
                    {
                        if (ctx.Globals.TryGetValue(fieldName, out var newValue))
                        {
                            instance.Fields[fieldName] = newValue;
                        }
                    }
                }

                return result;
            }
            finally
            {
                // フィールドをGlobalsから削除し、元の値を復元
                if (instance != null && savedFields != null)
                {
                    foreach (var fieldName in instance.Fields.Keys)
                    {
                        if (savedFields.TryGetValue(fieldName, out var savedValue))
                        {
                            ctx.Globals[fieldName] = savedValue;
                        }
                        else
                        {
                            ctx.Globals.Remove(fieldName);
                        }
                    }
                }
            }
        }

        throw new InvalidOperationException($"Object of type {callee.GetType().Name} is not callable");
    }

    /// <summary>
    /// メンバを取得する
    /// </summary>
    public static object GetMember(object target, string name)
    {
        if (target == null)
            throw new InvalidOperationException("Cannot get member of null");

        if (target is IroInstance instance)
        {
            // フィールドを優先
            if (instance.Fields.TryGetValue(name, out var fieldValue))
            {
                return fieldValue;
            }

            // メソッドを検索
            if (instance.Class.Methods.TryGetValue(name, out var method))
            {
                return method;
            }

            throw new InvalidOperationException($"Member '{name}' not found on instance of {instance.Class.Name}");
        }

        throw new InvalidOperationException($"Cannot get member '{name}' on object of type {target.GetType().Name}");
    }

    /// <summary>
    /// メンバを設定する
    /// </summary>
    public static object SetMember(object target, string name, object value)
    {
        if (target == null)
            throw new InvalidOperationException("Cannot set member of null");

        if (target is IroInstance instance)
        {
            instance.Fields[name] = value;
            return value;
        }

        throw new InvalidOperationException($"Cannot set member '{name}' on object of type {target.GetType().Name}");
    }

    /// <summary>
    /// インスタンスを生成する
    /// </summary>
    public static object NewInstance(string className, ScriptContext ctx, object[] args)
    {
        if (ctx == null)
            throw new ArgumentNullException(nameof(ctx));

        if (!ctx.Classes.TryGetValue(className, out var iroClass))
        {
            throw new InvalidOperationException($"Class '{className}' not found");
        }

        // 1. インスタンス生成
        var instance = new IroInstance(iroClass);

        // 2. フィールド初期化
        foreach (var field in iroClass.Fields)
        {
            if (field.Initializer != null)
            {
                // Initializerを実行して値を取得
                var value = field.Initializer(ctx);
                instance.Fields[field.Name] = value ?? null!;
            }
            else
            {
                instance.Fields[field.Name] = null!;
            }
        }

        // 3. init メソッド呼び出し（存在する場合）
        if (iroClass.Methods.TryGetValue("init", out var initMethod))
        {
            Invoke(initMethod, ctx, args, instance);
        }

        return instance;
    }

    #endregion
}
