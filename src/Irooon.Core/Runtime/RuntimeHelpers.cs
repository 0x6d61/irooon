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
    /// - 数値0、0.0 → false
    /// - 空文字列 → false
    /// - それ以外 → true
    /// </summary>
    public static bool IsTruthy(object? v)
    {
        if (v == null)
            return false;

        if (v is bool b)
            return b;

        // 数値0は false
        if (v is double d && d == 0.0)
            return false;

        // 空文字列は false
        if (v is string s && s.Length == 0)
            return false;

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
    /// 再帰呼び出しの正確性のため、関数のパラメータのみを保存/復元する
    /// </summary>
    public static object Invoke(object callee, ScriptContext ctx, object[] args, object? thisArg = null)
    {
        if (callee == null)
            throw new InvalidOperationException("Cannot invoke null");

        if (callee is IroCallable callable)
        {
            // Closureの場合、パラメータ名のみを保存/復元する
            // これにより、再帰呼び出しでパラメータが上書きされるのを防ぎつつ、
            // グローバル変数の変更は維持される
            Dictionary<string, object>? savedParams = null;
            List<string>? paramNames = null;

            if (callable is Closure closure && closure.ParameterNames.Count > 0)
            {
                paramNames = closure.ParameterNames;
                savedParams = new Dictionary<string, object>();
                foreach (var paramName in paramNames)
                {
                    if (ctx.Globals.TryGetValue(paramName, out var value))
                    {
                        savedParams[paramName] = value;
                    }
                }
            }

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
                // パラメータを元の値に復元（または削除）
                if (savedParams != null && paramNames != null)
                {
                    foreach (var paramName in paramNames)
                    {
                        if (savedParams.TryGetValue(paramName, out var savedValue))
                        {
                            ctx.Globals[paramName] = savedValue;
                        }
                        else
                        {
                            ctx.Globals.Remove(paramName);
                        }
                    }
                }

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

    #region Builtin Functions

    /// <summary>
    /// 標準出力に値を出力する（改行なし）
    /// </summary>
    public static object Print(params object[] args)
    {
        var output = string.Join(" ", args.Select(a => a?.ToString() ?? "null"));
        Console.Write(output);
        return null;
    }

    /// <summary>
    /// 標準出力に値を出力する（改行あり）
    /// </summary>
    public static object Println(params object[] args)
    {
        var output = string.Join(" ", args.Select(a => a?.ToString() ?? "null"));
        Console.WriteLine(output);
        return null;
    }

    #endregion

    #region List and Hash Operations

    /// <summary>
    /// リストを作成する
    /// </summary>
    public static object CreateList(object[] elements)
    {
        return new List<object>(elements);
    }

    /// <summary>
    /// ハッシュ（辞書）を作成する
    /// </summary>
    public static object CreateHash((string Key, object Value)[] pairs)
    {
        var dict = new Dictionary<string, object>();
        foreach (var (key, value) in pairs)
        {
            dict[key] = value;
        }
        return dict;
    }

    /// <summary>
    /// インデックスアクセス（統一インターフェース）
    /// リスト、ハッシュ、またはインスタンスから要素を取得する
    /// </summary>
    public static object GetIndexed(object target, object index)
    {
        if (target is List<object> list)
        {
            int idx = Convert.ToInt32(index);
            if (idx < 0 || idx >= list.Count)
                throw new RuntimeException($"List index out of range: {idx}");
            return list[idx];
        }
        else if (target is Dictionary<string, object> dict)
        {
            string key = index?.ToString() ?? throw new RuntimeException("Hash key cannot be null");
            if (!dict.TryGetValue(key, out var value))
                throw new RuntimeException($"Hash key not found: {key}");
            return value;
        }
        else if (target is IroInstance)
        {
            // IroInstanceの場合、GetMemberにフォールバック
            string key = index?.ToString() ?? throw new RuntimeException("Member name cannot be null");
            return GetMember(target, key);
        }
        throw new RuntimeException($"Cannot index type: {target?.GetType().Name ?? "null"}");
    }

    /// <summary>
    /// インデックス代入（統一インターフェース）
    /// リスト、ハッシュ、またはインスタンスに要素を設定する
    /// </summary>
    public static object SetIndexed(object target, object index, object value)
    {
        if (target is List<object> list)
        {
            int idx = Convert.ToInt32(index);
            if (idx < 0 || idx >= list.Count)
                throw new RuntimeException($"List index out of range: {idx}");
            list[idx] = value;
            return value;
        }
        else if (target is Dictionary<string, object> dict)
        {
            string key = index?.ToString() ?? throw new RuntimeException("Hash key cannot be null");
            dict[key] = value;
            return value;
        }
        else if (target is IroInstance)
        {
            // IroInstanceの場合、SetMemberにフォールバック
            string key = index?.ToString() ?? throw new RuntimeException("Member name cannot be null");
            return SetMember(target, key, value);
        }
        throw new RuntimeException($"Cannot index type: {target?.GetType().Name ?? "null"}");
    }

    #endregion
}
