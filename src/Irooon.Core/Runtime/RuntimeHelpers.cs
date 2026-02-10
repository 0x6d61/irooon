namespace Irooon.Core.Runtime;

/// <summary>
/// Runtime実行時のヘルパーメソッド集
/// 算術演算、比較演算、論理演算、関数呼び出し、メンバアクセスなどを提供
/// </summary>
public static class RuntimeHelpers
{
    // CLR型解決のキャッシュ
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Type?> _typeCache = new();

    /// <summary>
    /// キャッシュされた boxed true（比較演算の boxing 回避用）
    /// </summary>
    public static readonly object BoxedTrue = true;

    /// <summary>
    /// キャッシュされた boxed false（比較演算の boxing 回避用）
    /// </summary>
    public static readonly object BoxedFalse = false;

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

    #region Operator Overloading Helper

    /// <summary>
    /// IroInstanceのマジックメソッドを呼び出す
    /// </summary>
    private static bool TryCallMagicMethod(object target, string methodName, object[] args, ScriptContext ctx, out object result)
    {
        if (target is IroInstance instance)
        {
            var method = instance.Class.GetMethod(methodName);
            if (method != null)
            {
                result = Invoke(method, ctx, args, instance);
                return true;
            }
        }
        result = null!;
        return false;
    }

    #endregion

    #region Arithmetic Operations

    /// <summary>
    /// 加算演算
    /// </summary>
    public static object Add(object a, object b, ScriptContext? ctx = null)
    {
        if (a is double da && b is double db)
            return da + db;

        if (a == null || b == null)
            throw new InvalidOperationException("Cannot add null values");

        if (ctx != null && TryCallMagicMethod(a, "__add__", new[] { b }, ctx, out var result))
            return result;

        // 片方が文字列なら文字列連結
        if (a is string || b is string)
            return a.ToString() + b.ToString();

        return Convert.ToDouble(a) + Convert.ToDouble(b);
    }

    /// <summary>
    /// 減算演算
    /// </summary>
    public static object Sub(object a, object b, ScriptContext? ctx = null)
    {
        if (a is double da && b is double db)
            return da - db;

        if (a == null || b == null)
            throw new InvalidOperationException("Cannot subtract null values");

        if (ctx != null && TryCallMagicMethod(a, "__sub__", new[] { b }, ctx, out var result))
            return result;

        return Convert.ToDouble(a) - Convert.ToDouble(b);
    }

    /// <summary>
    /// 乗算演算
    /// </summary>
    public static object Mul(object a, object b, ScriptContext? ctx = null)
    {
        if (a is double da && b is double db)
            return da * db;

        if (a == null || b == null)
            throw new InvalidOperationException("Cannot multiply null values");

        if (ctx != null && TryCallMagicMethod(a, "__mul__", new[] { b }, ctx, out var result))
            return result;

        return Convert.ToDouble(a) * Convert.ToDouble(b);
    }

    /// <summary>
    /// 除算演算
    /// </summary>
    public static object Div(object a, object b, ScriptContext? ctx = null)
    {
        if (a is double da && b is double db)
        {
            if (db == 0.0)
                throw new DivideByZeroException("Division by zero");
            return da / db;
        }

        if (a == null || b == null)
            throw new InvalidOperationException("Cannot divide null values");

        if (ctx != null && TryCallMagicMethod(a, "__div__", new[] { b }, ctx, out var result))
            return result;

        double da2 = Convert.ToDouble(a);
        double db2 = Convert.ToDouble(b);

        if (db2 == 0.0)
            throw new DivideByZeroException("Division by zero");

        return da2 / db2;
    }

    /// <summary>
    /// 剰余演算
    /// </summary>
    public static object Mod(object a, object b, ScriptContext? ctx = null)
    {
        if (a is double da && b is double db)
            return da % db;

        if (a == null || b == null)
            throw new InvalidOperationException("Cannot modulo null values");

        if (ctx != null && TryCallMagicMethod(a, "__mod__", new[] { b }, ctx, out var result))
            return result;

        return Convert.ToDouble(a) % Convert.ToDouble(b);
    }

    public static object Power(object a, object b, ScriptContext? ctx = null)
    {
        if (a is double da && b is double db)
            return Math.Pow(da, db);

        if (a == null || b == null)
            throw new InvalidOperationException("Cannot exponentiate null values");

        if (ctx != null && TryCallMagicMethod(a, "__pow__", new[] { b }, ctx, out var result))
            return result;

        return Math.Pow(Convert.ToDouble(a), Convert.ToDouble(b));
    }

    public static object BitwiseAnd(object a, object b, ScriptContext? ctx = null)
    {
        if (ctx != null && TryCallMagicMethod(a, "__band__", new[] { b }, ctx, out var result))
            return result;
        return (double)((long)Convert.ToDouble(a) & (long)Convert.ToDouble(b));
    }

    public static object BitwiseOr(object a, object b, ScriptContext? ctx = null)
    {
        if (ctx != null && TryCallMagicMethod(a, "__bor__", new[] { b }, ctx, out var result))
            return result;
        return (double)((long)Convert.ToDouble(a) | (long)Convert.ToDouble(b));
    }

    public static object BitwiseXor(object a, object b, ScriptContext? ctx = null)
    {
        if (ctx != null && TryCallMagicMethod(a, "__bxor__", new[] { b }, ctx, out var result))
            return result;
        return (double)((long)Convert.ToDouble(a) ^ (long)Convert.ToDouble(b));
    }

    public static object ShiftLeft(object a, object b, ScriptContext? ctx = null)
    {
        if (ctx != null && TryCallMagicMethod(a, "__lshift__", new[] { b }, ctx, out var result))
            return result;
        return (double)((long)Convert.ToDouble(a) << (int)Convert.ToDouble(b));
    }

    public static object ShiftRight(object a, object b, ScriptContext? ctx = null)
    {
        if (ctx != null && TryCallMagicMethod(a, "__rshift__", new[] { b }, ctx, out var result))
            return result;
        return (double)((long)Convert.ToDouble(a) >> (int)Convert.ToDouble(b));
    }

    public static object BitwiseNot(object value, ScriptContext? ctx = null)
    {
        if (ctx != null && TryCallMagicMethod(value, "__bnot__", Array.Empty<object>(), ctx, out var result))
            return result;
        return (double)(~(long)Convert.ToDouble(value));
    }

    /// <summary>
    /// 単項マイナス演算
    /// </summary>
    public static object Negate(object value, ScriptContext? ctx = null)
    {
        if (value is double d)
            return -d;

        if (value == null)
            throw new InvalidOperationException("Cannot negate null value");

        if (ctx != null && TryCallMagicMethod(value, "__neg__", Array.Empty<object>(), ctx, out var result))
            return result;

        return -Convert.ToDouble(value);
    }

    /// <summary>
    /// インクリメント演算（値に1を加算）
    /// </summary>
    public static object Increment(object value)
    {
        if (value is double d)
            return d + 1.0;

        if (value == null)
            throw new InvalidOperationException("Cannot increment null value");

        return Convert.ToDouble(value) + 1.0;
    }

    /// <summary>
    /// デクリメント演算（値から1を減算）
    /// </summary>
    public static object Decrement(object value)
    {
        if (value is double d)
            return d - 1.0;

        if (value == null)
            throw new InvalidOperationException("Cannot decrement null value");

        return Convert.ToDouble(value) - 1.0;
    }

    #endregion

    #region Comparison Operations

    /// <summary>
    /// 等価演算 (==)
    /// </summary>
    public static object Eq(object a, object b, ScriptContext? ctx = null)
    {
        if (a is double da && b is double db)
            return da == db ? BoxedTrue : BoxedFalse;

        if (ctx != null && a is IroInstance)
        {
            if (TryCallMagicMethod(a, "__eq__", new[] { b }, ctx, out var result))
                return result;
        }

        if (a == null && b == null)
            return BoxedTrue;

        if (a == null || b == null)
            return BoxedFalse;

        // 同一型の場合は型固有の比較
        if (a is string sa && b is string sb)
            return sa == sb ? BoxedTrue : BoxedFalse;

        if (a is bool ba && b is bool bb)
            return ba == bb ? BoxedTrue : BoxedFalse;

        // 数値比較（IConvertible実装型のみ）
        if (a is IConvertible && b is IConvertible)
        {
            try
            {
                double da2 = Convert.ToDouble(a);
                double db2 = Convert.ToDouble(b);
                return da2 == db2 ? BoxedTrue : BoxedFalse;
            }
            catch (FormatException)
            {
                return a.Equals(b) ? BoxedTrue : BoxedFalse;
            }
        }

        return a.Equals(b) ? BoxedTrue : BoxedFalse;
    }

    /// <summary>
    /// 非等価演算 (!=)
    /// </summary>
    public static object Ne(object a, object b, ScriptContext? ctx = null)
    {
        if (a is double da && b is double db)
            return da != db ? BoxedTrue : BoxedFalse;

        if (ctx != null && TryCallMagicMethod(a, "__ne__", new[] { b }, ctx, out var result))
            return result;
        return !(bool)Eq(a, b, ctx) ? BoxedTrue : BoxedFalse;
    }

    /// <summary>
    /// 小なり演算 (&lt;)
    /// </summary>
    public static object Lt(object a, object b, ScriptContext? ctx = null)
    {
        if (a is double da && b is double db)
            return da < db ? BoxedTrue : BoxedFalse;

        if (a == null || b == null)
            throw new InvalidOperationException("Cannot compare null values");

        if (ctx != null && TryCallMagicMethod(a, "__lt__", new[] { b }, ctx, out var result))
            return result;

        if (a is string sa && b is string sb)
            return string.Compare(sa, sb, StringComparison.Ordinal) < 0 ? BoxedTrue : BoxedFalse;

        if (a is not IConvertible || b is not IConvertible)
            throw new RuntimeException($"Cannot compare {a.GetType().Name} and {b.GetType().Name}");

        return Convert.ToDouble(a) < Convert.ToDouble(b) ? BoxedTrue : BoxedFalse;
    }

    /// <summary>
    /// 小なりイコール演算 (&lt;=)
    /// </summary>
    public static object Le(object a, object b, ScriptContext? ctx = null)
    {
        if (a is double da && b is double db)
            return da <= db ? BoxedTrue : BoxedFalse;

        if (a == null || b == null)
            throw new InvalidOperationException("Cannot compare null values");

        if (ctx != null && TryCallMagicMethod(a, "__le__", new[] { b }, ctx, out var result))
            return result;

        if (a is string sa && b is string sb)
            return string.Compare(sa, sb, StringComparison.Ordinal) <= 0 ? BoxedTrue : BoxedFalse;

        if (a is not IConvertible || b is not IConvertible)
            throw new RuntimeException($"Cannot compare {a.GetType().Name} and {b.GetType().Name}");

        return Convert.ToDouble(a) <= Convert.ToDouble(b) ? BoxedTrue : BoxedFalse;
    }

    /// <summary>
    /// 大なり演算 (&gt;)
    /// </summary>
    public static object Gt(object a, object b, ScriptContext? ctx = null)
    {
        if (a is double da && b is double db)
            return da > db ? BoxedTrue : BoxedFalse;

        if (a == null || b == null)
            throw new InvalidOperationException("Cannot compare null values");

        if (ctx != null && TryCallMagicMethod(a, "__gt__", new[] { b }, ctx, out var result))
            return result;

        if (a is string sa && b is string sb)
            return string.Compare(sa, sb, StringComparison.Ordinal) > 0 ? BoxedTrue : BoxedFalse;

        if (a is not IConvertible || b is not IConvertible)
            throw new RuntimeException($"Cannot compare {a.GetType().Name} and {b.GetType().Name}");

        return Convert.ToDouble(a) > Convert.ToDouble(b) ? BoxedTrue : BoxedFalse;
    }

    /// <summary>
    /// 大なりイコール演算 (&gt;=)
    /// </summary>
    public static object Ge(object a, object b, ScriptContext? ctx = null)
    {
        if (a is double da && b is double db)
            return da >= db ? BoxedTrue : BoxedFalse;

        if (a == null || b == null)
            throw new InvalidOperationException("Cannot compare null values");

        if (ctx != null && TryCallMagicMethod(a, "__ge__", new[] { b }, ctx, out var result))
            return result;

        if (a is string sa && b is string sb)
            return string.Compare(sa, sb, StringComparison.Ordinal) >= 0 ? BoxedTrue : BoxedFalse;

        if (a is not IConvertible || b is not IConvertible)
            throw new RuntimeException($"Cannot compare {a.GetType().Name} and {b.GetType().Name}");

        return Convert.ToDouble(a) >= Convert.ToDouble(b) ? BoxedTrue : BoxedFalse;
    }

    #endregion

    #region Logical Operations

    /// <summary>
    /// 論理否定演算 (not)
    /// </summary>
    public static object Not(object? v)
    {
        return IsTruthy(v) ? BoxedFalse : BoxedTrue;
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
            // async 関数の場合: コンテキストをクローンして Task.Run で並行実行
            if (callable is Closure asyncClosure && asyncClosure.IsAsync)
            {
                var clonedCtx = ctx.Clone();
                var task = System.Threading.Tasks.Task.Run<object>(() => asyncClosure.Invoke(clonedCtx, args));
                return task;
            }

            // 呼び出し先が ctx.Locals を上書きする可能性があるため、常に保存する
            // (BoundMethod 経由の Closure 呼び出しでも ctx.Locals が破壊される)
            Dictionary<string, object>? savedParams = null;
            List<string>? paramNames = null;
            Dictionary<string, object>? savedLocals = null;
            List<string>? localNames = null;
            object?[]? savedLocalsArray = ctx.Locals;
            bool useArrayScope = false;

            if (callable is Closure closure)
            {
                if (closure.SlotCount > 0)
                {
                    // 配列スコープ: ctx.Locals は上で既に保存済み
                    useArrayScope = true;
                }
                else
                {
                    // Dictionary スコープ: 従来の保存/復元
                    if (closure.ParameterNames.Count > 0)
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

                    if (closure.LocalNames.Count > 0)
                    {
                        localNames = closure.LocalNames;
                        savedLocals = new Dictionary<string, object>();
                        foreach (var name in localNames)
                        {
                            if (ctx.Globals.TryGetValue(name, out var value))
                            {
                                savedLocals[name] = value;
                            }
                        }
                    }
                }
            }

            // thisArgがIroInstanceの場合、フィールドと"this"をGlobalsに展開
            Dictionary<string, object>? savedFields = null;
            Dictionary<string, object>? initialFieldValues = null;
            object? savedThis = null;
            bool hadThis = false;
            IroInstance? instance = thisArg as IroInstance;

            if (instance != null)
            {
                // 同一インスタンスの再帰呼び出しかチェック
                bool isSameInstanceRecursion = ctx.Globals.TryGetValue("this", out var currentThis)
                    && ReferenceEquals(currentThis, instance);

                // A-1: フィールドの初期値スナップショットを保存
                savedFields = new Dictionary<string, object>();
                initialFieldValues = new Dictionary<string, object>();

                foreach (var field in instance.Fields)
                {
                    if (ctx.Globals.TryGetValue(field.Key, out var savedValue))
                    {
                        savedFields[field.Key] = savedValue;

                        if (isSameInstanceRecursion)
                        {
                            // 同一インスタンス再帰: Globals の方が最新なので保持
                            initialFieldValues[field.Key] = savedValue;
                            continue;
                        }
                    }
                    initialFieldValues[field.Key] = field.Value;
                    ctx.Globals[field.Key] = field.Value;
                }

                // "this"を保存・登録
                if (ctx.Globals.TryGetValue("this", out savedThis))
                {
                    hadThis = true;
                }
                ctx.Globals["this"] = instance;
            }

            try
            {
                var result = callable.Invoke(ctx, args);

                // A-2: スマート sync-back
                // Globals が body で直接変更されたフィールドのみ反映し、
                // 内部メソッド呼び出しによる Fields の変更を上書きしない
                if (instance != null && initialFieldValues != null)
                {
                    foreach (var fieldName in instance.Fields.Keys.ToList())
                    {
                        if (ctx.Globals.TryGetValue(fieldName, out var globalValue)
                            && initialFieldValues.TryGetValue(fieldName, out var initialValue)
                            && !Object.Equals(globalValue, initialValue))
                        {
                            // Globals が変更された → body が直接変更 → Globals を信頼
                            instance.Fields[fieldName] = globalValue;
                        }
                        // else: Globals 未変更 → Fields の現在値を維持（内部呼び出しが更新済み）
                    }
                }

                return result;
            }
            finally
            {
                // ctx.Locals を常に復元（BoundMethod 経由の呼び出しでも保護される）
                ctx.Locals = savedLocalsArray;

                if (!useArrayScope)
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

                    // ローカル変数を元の値に復元（または削除）
                    if (savedLocals != null && localNames != null)
                    {
                        foreach (var name in localNames)
                        {
                            if (savedLocals.TryGetValue(name, out var savedValue))
                            {
                                ctx.Globals[name] = savedValue;
                            }
                            else
                            {
                                ctx.Globals.Remove(name);
                            }
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

                    // "this"を元に戻す
                    if (hadThis)
                    {
                        ctx.Globals["this"] = savedThis!;

                        // A-3: 同一インスタンスの場合、内部呼び出しで変更されたフィールドを Globals に再ロード
                        if (savedThis is IroInstance outerInstance
                            && ReferenceEquals(outerInstance, instance)
                            && initialFieldValues != null)
                        {
                            foreach (var fieldName in instance.Fields.Keys)
                            {
                                if (initialFieldValues.TryGetValue(fieldName, out var initialValue))
                                {
                                    var currentFieldValue = instance.Fields[fieldName];
                                    if (!Object.Equals(currentFieldValue, initialValue))
                                    {
                                        ctx.Globals[fieldName] = currentFieldValue;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        ctx.Globals.Remove("this");
                    }
                }
            }
        }

        throw new InvalidOperationException($"Object of type {callee.GetType().Name} is not callable");
    }

    /// <summary>
    /// メンバを取得する（プロトタイプ検索なし、内部呼び出し用）
    /// </summary>
    public static object GetMember(object target, string name)
        => GetMember(null!, target, name);

    /// <summary>
    /// メンバを取得する
    /// </summary>
    public static object GetMember(ScriptContext ctx, object target, string name)
    {
        if (target == null)
            throw new InvalidOperationException("Cannot get member of null");

        // プロトタイプ検索（全型共通、ctxがある場合のみ）
        var typeName = target switch
        {
            string => "String",
            double => "Number",
            bool => "Boolean",
            List<object> => "List",
            Dictionary<string, object> => "Hash",
            _ => null
        };

        if (typeName != null && TryGetPrototypeMethod(ctx, target, typeName, name, out var boundMethod))
        {
            return boundMethod!;
        }

        // 文字列のメンバ（プロトタイプで見つからなかった場合）
        if (target is string)
        {
            throw new InvalidOperationException($"Member '{name}' not found on String");
        }

        // リストのメンバ（プロトタイプで見つからなかった場合、CLRメソッドにフォールバック）
        if (target is List<object>)
        {
            var type = target.GetType();

            // プロパティを試す
            var property = type.GetProperty(name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            if (property != null)
            {
                return GetCLRInstanceProperty(target, name);
            }

            // メソッドを試す（CLRMethodWrapperを返す）
            try
            {
                var method = type.GetMethod(name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                if (method != null)
                {
                    return new CLRMethodWrapper(target, name);
                }
            }
            catch (System.Reflection.AmbiguousMatchException)
            {
                return new CLRMethodWrapper(target, name);
            }

            throw new InvalidOperationException($"Member '{name}' not found on List");
        }

        // ハッシュの場合（プロトタイプで見つからなかった場合はキーアクセス）
        if (target is Dictionary<string, object> hash)
        {
            if (hash.TryGetValue(name, out var value))
            {
                return value;
            }
            throw new InvalidOperationException($"Key '{name}' not found on Hash");
        }

        // CLRオブジェクトの場合
        if (IsCLRObject(target))
        {
            var type = target.GetType();

            // プロパティを試す
            var property = type.GetProperty(name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            if (property != null)
            {
                return GetCLRInstanceProperty(target, name);
            }

            // メソッドを試す（CLRMethodWrapperを返す）
            try
            {
                var method = type.GetMethod(name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                if (method != null)
                {
                    return new CLRMethodWrapper(target, name);
                }
            }
            catch (System.Reflection.AmbiguousMatchException)
            {
                // オーバーロードがある場合、とりあえずCLRMethodWrapperを返す
                // 実際の呼び出し時に引数で判定する
                return new CLRMethodWrapper(target, name);
            }

            throw new InvalidOperationException($"Member '{name}' not found on CLR type {type.Name}");
        }

        if (target is IroClass iroClass)
        {
            var staticMethod = iroClass.GetStaticMethod(name);
            if (staticMethod != null) return staticMethod;
            throw new RuntimeException($"Static member '{name}' not found on class '{iroClass.Name}'");
        }

        if (target is IroInstance instance)
        {
            // フィールドを優先
            if (instance.Fields.TryGetValue(name, out var fieldValue))
            {
                return fieldValue;
            }

            // メソッドを検索（親クラスも含む）
            var method = instance.Class.GetMethod(name);
            if (method != null)
            {
                return method;
            }

            throw new InvalidOperationException($"Member '{name}' not found on instance of {instance.Class.Name}");
        }

        throw new InvalidOperationException($"Cannot get member '{name}' on object of type {target.GetType().Name}");
    }

    /// <summary>
    /// プロトタイプからメソッドを検索し、BoundMethodとして返す
    /// </summary>
    private static bool TryGetPrototypeMethod(ScriptContext? ctx, object target, string typeName, string methodName, out object? result)
    {
        if (ctx != null &&
            ctx.Prototypes.TryGetValue(typeName, out var methods) &&
            methods.TryGetValue(methodName, out var method) &&
            method is IroCallable callable)
        {
            result = new BoundMethod(callable, target);
            return true;
        }
        result = null;
        return false;
    }

    /// <summary>
    /// メンバを設定する
    /// </summary>
    public static object SetMember(object target, string name, object value, ScriptContext? ctx = null)
    {
        if (target == null)
            throw new InvalidOperationException("Cannot set member of null");

        if (target is IroInstance instance)
        {
            instance.Fields[name] = value;

            // this.field = value の場合、ctx.Globalsも同期する
            // （Invokeのフィールド復元で上書きされるのを防ぐ）
            if (ctx != null && ctx.Globals.TryGetValue("this", out var currentThis) && ReferenceEquals(currentThis, instance))
            {
                ctx.Globals[name] = value;
            }

            return value;
        }

        throw new InvalidOperationException($"Cannot set member '{name}' on object of type {target.GetType().Name}");
    }

    /// <summary>
    /// 多段継承対応のsuper メソッド呼び出し。
    /// __callingClass__ を追跡して、正しい親クラスのメソッドを呼び出す。
    /// </summary>
    public static object CallSuperMethod(ScriptContext ctx, string methodName, object[] args)
    {
        if (!ctx.Globals.TryGetValue("this", out var thisObj) || thisObj is not IroInstance instance)
            throw new RuntimeException("super can only be used inside a class method");

        // 現在の呼び出しクラスを取得（未設定なら this.Class）
        IroClass callingClass;
        if (ctx.Globals.TryGetValue("__callingClass__", out var cc) && cc is IroClass c)
            callingClass = c;
        else
            callingClass = instance.Class;

        var parentClass = callingClass.Parent
            ?? throw new RuntimeException("No parent class for super call");
        var method = parentClass.GetMethod(methodName)
            ?? throw new RuntimeException($"Method '{methodName}' not found in parent class");

        // __callingClass__ を親に設定して呼び出し → 復元
        var saved = ctx.Globals.ContainsKey("__callingClass__") ? ctx.Globals["__callingClass__"] : null;
        ctx.Globals["__callingClass__"] = parentClass;
        try
        {
            return Invoke(method, ctx, args, instance);
        }
        finally
        {
            if (saved != null)
                ctx.Globals["__callingClass__"] = saved;
            else
                ctx.Globals.Remove("__callingClass__");
        }
    }

    /// <summary>
    /// 安全なナビゲーション処理（null安全なメンバアクセス）
    /// </summary>
    public static object? SafeNavigation(object? obj, string memberName)
    {
        if (obj == null)
            return null;

        return GetMember(obj, memberName);
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
        if (target is string str)
        {
            int idx = Convert.ToInt32(index);
            if (idx < 0 || idx >= str.Length)
                throw new RuntimeException($"String index out of range: {idx}");
            return str[idx].ToString();
        }
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

    #region Range Operations

    /// <summary>
    /// 範囲リテラルを生成します。
    /// </summary>
    /// <param name="start">開始値</param>
    /// <param name="end">終端値</param>
    /// <param name="inclusive">終端を含むかどうか</param>
    /// <returns>範囲の要素を含むリスト</returns>
    public static object CreateRange(object start, object end, bool inclusive)
    {
        int startInt = Convert.ToInt32(start);
        int endInt = Convert.ToInt32(end);
        var list = new List<object>();
        int limit = inclusive ? endInt + 1 : endInt;
        for (int i = startInt; i < limit; i++)
        {
            list.Add((double)i);
        }
        return list;
    }

    #endregion

    #region CLR Interop

    /// <summary>
    /// CLR型を解決します。
    /// </summary>
    /// <param name="typeName">型名（完全修飾名）</param>
    /// <returns>解決された型（見つからない場合はnull）</returns>
    public static Type? ResolveCLRType(string typeName)
    {
        return _typeCache.GetOrAdd(typeName, ResolveCLRTypeInternal);
    }

    private static Type? ResolveCLRTypeInternal(string typeName)
    {
        // Generic型の特別処理（例: System.Collections.Generic.List → System.Collections.Generic.List`1[System.Object]）
        if (typeName == "System.Collections.Generic.List")
        {
            return typeof(List<object>);
        }

        // まず通常の方法で試す
        var type = Type.GetType(typeName);
        if (type != null) return type;

        // アセンブリ修飾名で試す（mscorlib, System.Runtime, System.Collections など）
        var assemblies = new[]
        {
            "System.Runtime",
            "System.Collections",
            "mscorlib",
            "System.Private.CoreLib",
            "System.Text.RegularExpressions"
        };

        foreach (var assembly in assemblies)
        {
            type = Type.GetType($"{typeName}, {assembly}");
            if (type != null) return type;
        }

        // すべてのロード済みアセンブリから検索
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = assembly.GetType(typeName);
            if (type != null) return type;
        }

        return null;
    }

    /// <summary>
    /// 型キャッシュをクリアします（アセンブリ読み込み後に使用）。
    /// </summary>
    public static void ClearTypeCache()
    {
        _typeCache.Clear();
    }

    /// <summary>
    /// 外部アセンブリをロードします。
    /// #r "path/to/assembly.dll" ディレクティブから呼び出されます。
    /// </summary>
    public static object LoadAssembly(string path)
    {
        var fullPath = Path.GetFullPath(path);
        if (!File.Exists(fullPath))
            throw new RuntimeException($"Assembly not found: {fullPath}");
        System.Reflection.Assembly.LoadFrom(fullPath);
        _typeCache.Clear(); // キャッシュを無効化して新しいアセンブリの型を解決可能にする
        return null!;
    }

    /// <summary>
    /// CLR静的メソッドを呼び出します。
    /// </summary>
    /// <param name="type">型</param>
    /// <param name="methodName">メソッド名</param>
    /// <param name="args">引数</param>
    /// <returns>メソッドの戻り値</returns>
    public static object InvokeCLRStaticMethod(Type type, string methodName, object[] args)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        System.Reflection.MethodInfo? method = null;

        // 引数の型を取得
        var argTypes = args.Select(a => a?.GetType() ?? typeof(object)).ToArray();

        // 引数の型が一致するメソッドを検索
        try
        {
            method = type.GetMethod(
                methodName,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
                null,
                argTypes,
                null);
        }
        catch (System.Reflection.AmbiguousMatchException)
        {
            // 曖昧な場合は、すべてのメソッドを取得して手動で検索
            var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(m => m.Name == methodName && m.GetParameters().Length == args.Length)
                .ToList();

            if (methods.Count > 0)
            {
                // 最初に見つかったメソッドを使用
                method = methods[0];
            }
        }

        if (method == null)
        {
            // プロパティのgetterを試す
            var property = type.GetProperty(methodName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (property != null && property.CanRead)
            {
                method = property.GetGetMethod();
            }
        }

        if (method == null)
            throw new RuntimeException($"Method '{methodName}' not found on type {type.Name}");

        try
        {
            return method.Invoke(null, args) ?? null!;
        }
        catch (Exception ex)
        {
            throw new RuntimeException($"Error invoking CLR method '{methodName}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// オブジェクトがCLR型のインスタンスかどうかを判定する
    /// （irooon独自の型を除外）
    /// </summary>
    public static bool IsCLRObject(object? obj)
    {
        if (obj == null) return false;

        var type = obj.GetType();

        // irooon独自の型を除外
        if (obj is IroCallable || obj is IroInstance || obj is IroClass)
            return false;

        // プリミティブ型、文字列を除外
        if (type.IsPrimitive || obj is string)
            return false;

        // irooonの内部リスト・辞書を除外（irooonスクリプトから作成されたもの）
        // ただし、CLR相互運用で作成されたList<object>はCLRオブジェクトとして扱う
        // 判定: System.Collections.Generic.List<object> は CLRオブジェクト
        // それ以外の List<object> や Dictionary<string, object> は内部型

        // それ以外はCLRオブジェクトとみなす
        return true;
    }

    /// <summary>
    /// CLR型のインスタンスを作成する
    /// </summary>
    public static object CreateCLRInstance(Type type, object[] args)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        try
        {
            // コンストラクタを取得
            var constructors = type.GetConstructors(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            if (constructors.Length == 0)
                throw new RuntimeException($"No public constructors found for type {type.Name}");

            // 引数の数が一致するコンストラクタを探す
            var candidates = constructors.Where(c => c.GetParameters().Length == args.Length).ToList();

            if (candidates.Count == 0)
            {
                throw new RuntimeException($"No constructor found with {args.Length} parameters for type {type.Name}");
            }

            // 引数の型が一致するコンストラクタを探す
            foreach (var constructor in candidates)
            {
                var parameters = constructor.GetParameters();
                bool match = true;

                for (int i = 0; i < parameters.Length; i++)
                {
                    var paramType = parameters[i].ParameterType;
                    var argType = args[i]?.GetType();

                    // null は任意の参照型に変換可能
                    if (args[i] == null && !paramType.IsValueType)
                        continue;

                    // 型が一致するか、または変換可能か
                    if (argType != null && !paramType.IsAssignableFrom(argType))
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    return constructor.Invoke(args);
                }
            }

            // 一致するコンストラクタが見つからない場合、最初の候補を試す
            return candidates[0].Invoke(args);
        }
        catch (Exception ex)
        {
            throw new RuntimeException($"Failed to create instance of {type.Name}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// CLRインスタンスのメソッドを呼び出す
    /// </summary>
    public static object InvokeCLRInstanceMethod(object instance, string methodName, object[] args)
    {
        if (instance == null)
            throw new RuntimeException("Cannot invoke method on null instance");

        var type = instance.GetType();

        System.Reflection.MethodInfo? method = null;

        // 引数の型を取得
        var argTypes = args.Select(a => a?.GetType() ?? typeof(object)).ToArray();

        // 引数の型が一致するメソッドを検索
        try
        {
            method = type.GetMethod(
                methodName,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
                null,
                argTypes,
                null);
        }
        catch (System.Reflection.AmbiguousMatchException)
        {
            // 曖昧な場合は、すべてのメソッドを取得して手動で検索
            var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(m => m.Name == methodName && m.GetParameters().Length == args.Length)
                .ToList();

            if (methods.Count > 0)
            {
                // 最初に見つかったメソッドを使用
                method = methods[0];
            }
        }

        if (method == null)
            throw new RuntimeException($"Method '{methodName}' not found on type {type.Name}");

        try
        {
            // メソッドを呼び出し
            var result = method.Invoke(instance, args);

            // void メソッドの場合は null を返す
            return result ?? null!;
        }
        catch (System.Reflection.TargetInvocationException ex)
        {
            throw new RuntimeException($"Error invoking {type.Name}.{methodName}: {ex.InnerException?.Message ?? ex.Message}", ex);
        }
    }

    /// <summary>
    /// CLRインスタンスのプロパティを取得する
    /// </summary>
    public static object GetCLRInstanceProperty(object instance, string propertyName)
    {
        if (instance == null)
            throw new RuntimeException("Cannot get property on null instance");

        var type = instance.GetType();

        var property = type.GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (property == null)
            throw new RuntimeException($"Property '{propertyName}' not found on type {type.Name}");

        var value = property.GetValue(instance);

        // irooonの型システムに合わせて、数値はdoubleに変換
        if (value is int intValue)
            return (double)intValue;
        if (value is long longValue)
            return (double)longValue;
        if (value is float floatValue)
            return (double)floatValue;
        if (value is decimal decimalValue)
            return (double)decimalValue;

        return value ?? null!;
    }

    /// <summary>
    /// CLRインスタンスのプロパティを設定する
    /// </summary>
    public static object SetCLRInstanceProperty(object instance, string propertyName, object value)
    {
        if (instance == null)
            throw new RuntimeException("Cannot set property on null instance");

        var type = instance.GetType();

        var property = type.GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (property == null)
            throw new RuntimeException($"Property '{propertyName}' not found on type {type.Name}");

        property.SetValue(instance, value);
        return value;
    }

    #endregion

    #region Shell Command Execution

    /// <summary>
    /// シェルコマンドを実行して結果を返します
    /// </summary>
    /// <param name="command">実行するシェルコマンド</param>
    /// <returns>コマンドの標準出力（末尾の改行は削除）</returns>
    public static object ExecuteShellCommand(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
            return "";

        var process = new System.Diagnostics.Process();

        // プラットフォーム判定
        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
            System.Runtime.InteropServices.OSPlatform.Windows))
        {
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c {command}";
        }
        else
        {
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = $"-c \"{command.Replace("\"", "\\\"")}\"";
        }

        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        try
        {
            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new RuntimeException($"Shell command failed (exit code {process.ExitCode}): {error}");
            }

            // 末尾の改行を削除して返す
            return output.TrimEnd('\r', '\n');
        }
        catch (Exception ex) when (ex is not RuntimeException)
        {
            throw new RuntimeException($"Failed to execute shell command: {ex.Message}");
        }
    }

    #endregion

    #region Standard Library Functions

    /// <summary>
    /// ファイルの内容を文字列として読み込む
    /// </summary>
    public static object ReadFile(params object[] args)
    {
        if (args.Length != 1 || args[0] is not string path)
            throw new RuntimeException("readFile requires a file path string argument");

        try
        {
            return System.IO.File.ReadAllText(path);
        }
        catch (System.IO.FileNotFoundException)
        {
            throw new RuntimeException($"File not found: {path}");
        }
        catch (System.UnauthorizedAccessException)
        {
            throw new RuntimeException($"Permission denied: {path}");
        }
        catch (Exception ex)
        {
            throw new RuntimeException($"Error reading file: {ex.Message}");
        }
    }

    /// <summary>
    /// ファイルに文字列を書き込む
    /// </summary>
    public static object WriteFile(params object[] args)
    {
        if (args.Length != 2 || args[0] is not string path || args[1] is not string content)
            throw new RuntimeException("writeFile requires a file path and content string arguments");

        try
        {
            System.IO.File.WriteAllText(path, content);
            return null;
        }
        catch (System.UnauthorizedAccessException)
        {
            throw new RuntimeException($"Permission denied: {path}");
        }
        catch (Exception ex)
        {
            throw new RuntimeException($"Error writing file: {ex.Message}");
        }
    }

    /// <summary>
    /// ファイルに文字列を追記する
    /// </summary>
    public static object AppendFile(params object[] args)
    {
        if (args.Length != 2 || args[0] is not string path || args[1] is not string content)
            throw new RuntimeException("appendFile requires a file path and content string arguments");

        try
        {
            System.IO.File.AppendAllText(path, content);
            return null;
        }
        catch (System.UnauthorizedAccessException)
        {
            throw new RuntimeException($"Permission denied: {path}");
        }
        catch (Exception ex)
        {
            throw new RuntimeException($"Error appending to file: {ex.Message}");
        }
    }

    /// <summary>
    /// ファイルまたはディレクトリの存在をチェックする
    /// </summary>
    public static object Exists(params object[] args)
    {
        if (args.Length != 1 || args[0] is not string path)
            throw new RuntimeException("exists requires a file path string argument");

        return System.IO.File.Exists(path) || System.IO.Directory.Exists(path);
    }

    /// <summary>
    /// ファイルを削除する
    /// </summary>
    public static object DeleteFile(params object[] args)
    {
        if (args.Length != 1 || args[0] is not string path)
            throw new RuntimeException("deleteFile requires a file path string argument");

        try
        {
            System.IO.File.Delete(path);
            return null;
        }
        catch (System.IO.FileNotFoundException)
        {
            throw new RuntimeException($"File not found: {path}");
        }
        catch (System.UnauthorizedAccessException)
        {
            throw new RuntimeException($"Permission denied: {path}");
        }
        catch (Exception ex)
        {
            throw new RuntimeException($"Error deleting file: {ex.Message}");
        }
    }

    /// <summary>
    /// ディレクトリ内のファイルとディレクトリの一覧を取得する
    /// </summary>
    public static object ListDir(params object[] args)
    {
        if (args.Length != 1 || args[0] is not string path)
            throw new RuntimeException("listDir requires a directory path string argument");

        try
        {
            var entries = System.IO.Directory.GetFileSystemEntries(path);
            return new List<object>(entries.Cast<object>());
        }
        catch (System.IO.DirectoryNotFoundException)
        {
            throw new RuntimeException($"Directory not found: {path}");
        }
        catch (System.UnauthorizedAccessException)
        {
            throw new RuntimeException($"Permission denied: {path}");
        }
        catch (Exception ex)
        {
            throw new RuntimeException($"Error listing directory: {ex.Message}");
        }
    }

    /// <summary>
    /// JSON文字列をパースしてオブジェクトに変換する
    /// </summary>
    public static object JsonParse(params object[] args)
    {
        if (args.Length != 1 || args[0] is not string json)
            throw new RuntimeException("jsonParse requires a JSON string argument");

        try
        {
            var element = System.Text.Json.JsonDocument.Parse(json).RootElement;
            return ConvertJsonElement(element);
        }
        catch (System.Text.Json.JsonException ex)
        {
            throw new RuntimeException($"Invalid JSON: {ex.Message}");
        }
    }

    /// <summary>
    /// オブジェクトをJSON文字列に変換する
    /// </summary>
    public static object JsonStringify(params object[] args)
    {
        if (args.Length != 1)
            throw new RuntimeException("jsonStringify requires an object argument");

        try
        {
            var options = new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = false
            };
            return System.Text.Json.JsonSerializer.Serialize(args[0], options);
        }
        catch (Exception ex)
        {
            throw new RuntimeException($"Error serializing to JSON: {ex.Message}");
        }
    }

    /// <summary>
    /// JsonElementをirooonのデータ構造に変換する
    /// </summary>
    private static object ConvertJsonElement(System.Text.Json.JsonElement element)
    {
        return element.ValueKind switch
        {
            System.Text.Json.JsonValueKind.Null => null,
            System.Text.Json.JsonValueKind.True => true,
            System.Text.Json.JsonValueKind.False => false,
            System.Text.Json.JsonValueKind.Number => element.GetDouble(),
            System.Text.Json.JsonValueKind.String => element.GetString(),
            System.Text.Json.JsonValueKind.Array => new List<object>(
                element.EnumerateArray().Select(ConvertJsonElement)
            ),
            System.Text.Json.JsonValueKind.Object => new Dictionary<string, object>(
                element.EnumerateObject().Select(p =>
                    new KeyValuePair<string, object>(p.Name, ConvertJsonElement(p.Value))
                )
            ),
            _ => throw new RuntimeException($"Unsupported JSON value kind: {element.ValueKind}")
        };
    }

    /// <summary>
    /// 現在時刻をISO 8601形式の文字列で取得する
    /// </summary>
    public static object Now(params object[] args)
    {
        return DateTime.Now.ToString("o"); // ISO 8601形式
    }

    #endregion

    #region Async/Await Support

    /// <summary>
    /// Taskをラップして結果を返す
    /// </summary>
    public static object AwaitTask(object taskObj)
    {
        if (taskObj == null)
            throw new RuntimeException("Cannot await null");

        // Task<object> の場合（irooon async fn の戻り値）
        if (taskObj is System.Threading.Tasks.Task<object> taskOfObject)
        {
            return taskOfObject.GetAwaiter().GetResult();
        }

        // CLR Task<T> または void Task の場合
        if (taskObj is System.Threading.Tasks.Task task)
        {
            var taskType = task.GetType();
            if (taskType.IsGenericType)
            {
                // Task<T> — リフレクションで Result を取得
                task.GetAwaiter().GetResult();
                var resultProp = taskType.GetProperty("Result");
                return resultProp?.GetValue(task)!;
            }
            // void Task
            task.GetAwaiter().GetResult();
            return null!;
        }

        // Task でない場合はそのまま返す（同期的な値）
        return taskObj;
    }

    /// <summary>
    /// 指定ミリ秒後に完了する Task を返す
    /// </summary>
    public static object Delay(ScriptContext ctx, object[] args)
    {
        var ms = Convert.ToInt32(args.Length > 0 ? args[0] : 0);
        return System.Threading.Tasks.Task.Run<object>(async () =>
        {
            await System.Threading.Tasks.Task.Delay(ms);
            return null!;
        });
    }

    /// <summary>
    /// 全 Task の完了を待ち、結果リストを返す
    /// </summary>
    public static object AwaitAll(ScriptContext ctx, object[] args)
    {
        var taskList = args.Length > 0 ? args[0] as List<object> : null;
        if (taskList == null)
            throw new RuntimeException("awaitAll expects a list of tasks");

        var tasks = taskList.Select(t =>
        {
            if (t is System.Threading.Tasks.Task<object> typed)
                return typed;
            if (t is System.Threading.Tasks.Task untyped)
            {
                var tType = untyped.GetType();
                if (tType.IsGenericType)
                {
                    return System.Threading.Tasks.Task.Run<object>(() =>
                    {
                        untyped.GetAwaiter().GetResult();
                        var resultProp = tType.GetProperty("Result");
                        return resultProp?.GetValue(untyped)!;
                    });
                }
                return System.Threading.Tasks.Task.Run<object>(() =>
                {
                    untyped.GetAwaiter().GetResult();
                    return null!;
                });
            }
            return System.Threading.Tasks.Task.FromResult(t);
        }).ToArray();

        System.Threading.Tasks.Task.WaitAll(tasks);
        return new List<object>(tasks.Select(t => t.Result!));
    }

    /// <summary>
    /// レストパラメータ用: args配列のstartIndex以降をList&lt;object&gt;として返す
    /// </summary>
    /// <summary>
    /// スプレッドマーカー付きの要素からリストを構築
    /// </summary>
    public static object CreateListWithSpread(object[] items)
    {
        var list = new List<object>();
        foreach (var item in items)
        {
            if (item is SpreadMarker marker && marker.Value is IList<object> spreadList)
            {
                list.AddRange(spreadList);
            }
            else if (item is SpreadMarker marker2 && marker2.Value is System.Collections.IList ilist)
            {
                foreach (var elem in ilist) list.Add(elem!);
            }
            else
            {
                list.Add(item);
            }
        }
        return list;
    }

    public static object MarkSpread(object value) => new SpreadMarker(value);

    /// <summary>
    /// スプレッド付き引数を展開してobject[]に変換
    /// </summary>
    public static object[] ExpandSpreadArgs(object[] args)
    {
        var result = new List<object>();
        foreach (var arg in args)
        {
            if (arg is SpreadMarker marker && marker.Value is IList<object> list)
            {
                result.AddRange(list);
            }
            else if (arg is SpreadMarker marker2 && marker2.Value is System.Collections.IList ilist)
            {
                foreach (var elem in ilist) result.Add(elem!);
            }
            else
            {
                result.Add(arg);
            }
        }
        return result.ToArray();
    }

    public static object CollectRestArgs(object[] args, int startIndex)
    {
        var list = new List<object>();
        for (int i = startIndex; i < args.Length; i++)
        {
            if (args[i] != null)
                list.Add(args[i]);
        }
        return list;
    }

    #endregion

    #region Module Import

    /// <summary>
    /// モジュールをインポートし、指定された名前をctx.Globalsに登録する
    /// </summary>
    public static object ImportModule(ScriptContext ctx, string[] names, string modulePath)
    {
        if (ctx.ModuleLoader == null)
            throw new RuntimeException("Module system not initialized. Use ScriptEngine to run scripts with import.");

        var exports = ctx.ModuleLoader.LoadModule(modulePath, ctx.ModuleBaseDir);

        foreach (var name in names)
        {
            if (exports.TryGetValue(name, out var value))
            {
                ctx.Globals[name] = value!;

                // クラスの場合は ctx.Classes にも登録
                if (value is IroClass iroClass)
                {
                    ctx.Classes[name] = iroClass;
                }
            }
            else
            {
                throw new RuntimeException($"Module '{modulePath}' does not export '{name}'");
            }
        }

        return null!;
    }

    #endregion

    #region Stdlib Primitives

    /// <summary>
    /// 文字列の長さを返す
    /// </summary>
    public static object __stringLength(params object[] args)
    {
        if (args.Length != 1 || args[0] is not string str)
            throw new RuntimeException("__stringLength requires a string argument");
        return (double)str.Length;
    }

    /// <summary>
    /// 指定位置の文字を返す
    /// </summary>
    public static object __charAt(params object[] args)
    {
        if (args.Length != 2 || args[0] is not string str)
            throw new RuntimeException("__charAt requires a string and index");
        int index = Convert.ToInt32(args[1]);
        if (index < 0 || index >= str.Length)
            throw new RuntimeException($"String index out of range: {index}");
        return str[index].ToString();
    }

    /// <summary>
    /// 文字のUnicode値を返す
    /// </summary>
    public static object __charCodeAt(params object[] args)
    {
        if (args.Length != 2 || args[0] is not string str)
            throw new RuntimeException("__charCodeAt requires a string and index");
        int index = Convert.ToInt32(args[1]);
        if (index < 0 || index >= str.Length)
            throw new RuntimeException($"String index out of range: {index}");
        return (double)str[index];
    }

    /// <summary>
    /// Unicode値から文字を生成する
    /// </summary>
    public static object __fromCharCode(params object[] args)
    {
        if (args.Length != 1)
            throw new RuntimeException("__fromCharCode requires a character code");
        int code = Convert.ToInt32(args[0]);
        return ((char)code).ToString();
    }

    /// <summary>
    /// 部分文字列を取得する
    /// </summary>
    public static object __substring(params object[] args)
    {
        if (args.Length < 2 || args[0] is not string str)
            throw new RuntimeException("__substring requires a string and start index");
        int start = Convert.ToInt32(args[1]);
        if (args.Length >= 3)
        {
            int length = Convert.ToInt32(args[2]);
            return str.Substring(start, length);
        }
        return str.Substring(start);
    }

    /// <summary>
    /// リストの長さを返す
    /// </summary>
    public static object __listLength(params object[] args)
    {
        if (args.Length != 1 || args[0] is not List<object> list)
            throw new RuntimeException("__listLength requires a list argument");
        return (double)list.Count;
    }

    /// <summary>
    /// リストに要素を追加する（破壊的操作）
    /// </summary>
    public static object __listPush(params object[] args)
    {
        if (args.Length != 2 || args[0] is not List<object> list)
            throw new RuntimeException("__listPush requires a list and value");
        list.Add(args[1]);
        return args[1];
    }

    /// <summary>
    /// StringBuilderを作成する（高速文字列構築用）
    /// </summary>
    public static object __stringBuilder(params object[] args)
    {
        return new System.Text.StringBuilder();
    }

    /// <summary>
    /// StringBuilderに文字列を追加する
    /// </summary>
    public static object __sbAppend(params object[] args)
    {
        if (args.Length != 2 || args[0] is not System.Text.StringBuilder sb)
            throw new RuntimeException("__sbAppend requires a StringBuilder and string");
        sb.Append(args[1]?.ToString() ?? "");
        return null!;
    }

    /// <summary>
    /// StringBuilderの内容を文字列として返す
    /// </summary>
    public static object __sbToString(params object[] args)
    {
        if (args.Length != 1 || args[0] is not System.Text.StringBuilder sb)
            throw new RuntimeException("__sbToString requires a StringBuilder");
        return sb.ToString();
    }

    /// <summary>
    /// 文字列内の部分文字列の位置を返す（見つからない場合は-1）
    /// </summary>
    public static object __indexOf(params object[] args)
    {
        if (args.Length < 2 || args[0] is not string str || args[1] is not string search)
            throw new RuntimeException("__indexOf requires a string and search string");
        int startIndex = args.Length >= 3 ? Convert.ToInt32(args[2]) : 0;
        return (double)str.IndexOf(search, startIndex, StringComparison.Ordinal);
    }

    /// <summary>
    /// 文字列を数値に変換する
    /// </summary>
    public static object __toNumber(params object[] args)
    {
        if (args.Length != 1)
            throw new RuntimeException("__toNumber requires one argument");
        try
        {
            return Convert.ToDouble(args[0]);
        }
        catch (Exception ex) when (ex is FormatException or InvalidCastException)
        {
            return null!;
        }
    }

    /// <summary>
    /// 値を文字列に変換する
    /// </summary>
    public static object __toString(params object[] args)
    {
        if (args.Length != 1)
            throw new RuntimeException("__toString requires one argument");
        if (args[0] == null) return "null";
        if (args[0] is bool b) return b ? "true" : "false";
        return args[0].ToString() ?? "";
    }

    /// <summary>
    /// オブジェクトの型名を返す
    /// </summary>
    /// <summary>
    /// instanceof 演算子の実装。継承チェーンを辿ってクラスの一致を確認する。
    /// </summary>
    public static object IsInstanceOf(object obj, string className, ScriptContext ctx)
    {
        if (obj is not IroInstance instance) return false;
        if (!ctx.Classes.TryGetValue(className, out var targetClass))
            throw new RuntimeException($"Class '{className}' not found");

        var current = instance.Class;
        while (current != null)
        {
            if (current.Name == targetClass.Name) return true;
            current = current.Parent;
        }
        return false;
    }

    /// <summary>
    /// オブジェクトの型名を文字列で返します。
    /// __typeOf と CheckType/CheckReturnType で共有されるロジック。
    /// </summary>
    public static string GetTypeName(object? value)
    {
        return value switch
        {
            null => "Null",
            string => "String",
            double => "Number",
            bool => "Boolean",
            List<object> => "List",
            Dictionary<string, object> => "Hash",
            IroInstance inst => inst.Class.Name,
            IroClass => "Class",
            IroCallable => "Function",
            _ => value.GetType().Name
        };
    }

    public static object __typeOf(params object[] args)
    {
        if (args.Length != 1)
            throw new RuntimeException("typeof requires one argument");
        return GetTypeName(args[0]);
    }

    /// <summary>
    /// 引数の型を実行時にチェックします。型が一致すれば値をそのまま返し、
    /// 一致しなければ RuntimeException をスローします。
    /// </summary>
    public static object CheckType(object value, string expectedType,
        string paramName, string funcName, int line, int column)
    {
        var actualType = GetTypeName(value);
        if (actualType != expectedType)
        {
            throw new RuntimeException(
                $"Type error: parameter '{paramName}' in function '{funcName}' expected {expectedType}, got {actualType}",
                line, column);
        }
        return value;
    }

    /// <summary>
    /// 戻り値の型を実行時にチェックします。型が一致すれば値をそのまま返し、
    /// 一致しなければ RuntimeException をスローします。
    /// </summary>
    public static object CheckReturnType(object value, string expectedType,
        string funcName, int line, int column)
    {
        var actualType = GetTypeName(value);
        if (actualType != expectedType)
        {
            throw new RuntimeException(
                $"Type error: function '{funcName}' expected to return {expectedType}, but returned {actualType}",
                line, column);
        }
        return value;
    }

    /// <summary>
    /// 空のハッシュを作成する
    /// </summary>
    public static object __hashNew(params object[] args)
    {
        return new Dictionary<string, object>();
    }

    /// <summary>
    /// ハッシュのキーをリストとして返す
    /// </summary>
    public static object __hashKeys(params object[] args)
    {
        if (args.Length != 1 || args[0] is not Dictionary<string, object> hash)
            throw new RuntimeException("__hashKeys requires a Hash argument");
        return new List<object>(hash.Keys);
    }

    // ========================================
    // Hash追加プリミティブ
    // ========================================

    /// <summary>
    /// ハッシュの値をリストとして返す
    /// </summary>
    public static object __hashValues(params object[] args)
    {
        if (args.Length != 1 || args[0] is not Dictionary<string, object> hash)
            throw new RuntimeException("__hashValues requires a Hash argument");
        return new List<object>(hash.Values);
    }

    /// <summary>
    /// ハッシュにキーが存在するか判定する
    /// </summary>
    public static object __hashHas(params object[] args)
    {
        if (args.Length != 2 || args[0] is not Dictionary<string, object> hash)
            throw new RuntimeException("__hashHas requires a Hash and key argument");
        var key = args[1]?.ToString() ?? "";
        return (object)hash.ContainsKey(key);
    }

    /// <summary>
    /// ハッシュからキーを削除する
    /// </summary>
    public static object __hashDelete(params object[] args)
    {
        if (args.Length != 2 || args[0] is not Dictionary<string, object> hash)
            throw new RuntimeException("__hashDelete requires a Hash and key argument");
        var key = args[1]?.ToString() ?? "";
        return (object)hash.Remove(key);
    }

    /// <summary>
    /// ハッシュのサイズを返す
    /// </summary>
    public static object __hashSize(params object[] args)
    {
        if (args.Length != 1 || args[0] is not Dictionary<string, object> hash)
            throw new RuntimeException("__hashSize requires a Hash argument");
        return (double)hash.Count;
    }

    // ========================================
    // List追加プリミティブ
    // ========================================

    /// <summary>
    /// リストの末尾要素を削除して返す
    /// </summary>
    public static object __listPop(params object[] args)
    {
        if (args.Length != 1 || args[0] is not List<object> list)
            throw new RuntimeException("__listPop requires a list argument");
        if (list.Count == 0)
            throw new RuntimeException("Cannot pop from empty list");
        var item = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);
        return item;
    }

    /// <summary>
    /// リストの部分リストを返す（非破壊的）
    /// </summary>
    public static object __listSlice(params object[] args)
    {
        if (args.Length < 2 || args[0] is not List<object> list)
            throw new RuntimeException("__listSlice requires a list and start index");
        var start = (int)Convert.ToDouble(args[1]);
        var count = list.Count;
        if (start < 0) start = Math.Max(0, count + start);
        if (start > count) start = count;

        int end;
        if (args.Length >= 3 && args[2] != null)
        {
            end = (int)Convert.ToDouble(args[2]);
            if (end < 0) end = Math.Max(0, count + end);
            if (end > count) end = count;
        }
        else
        {
            end = count;
        }

        var result = new List<object>();
        for (var i = start; i < end; i++)
        {
            result.Add(list[i]);
        }
        return result;
    }

    /// <summary>
    /// リスト内の要素のインデックスを返す（見つからない場合は-1）
    /// </summary>
    public static object __listIndexOf(params object[] args)
    {
        if (args.Length != 2 || args[0] is not List<object> list)
            throw new RuntimeException("__listIndexOf requires a list and value");
        var target = args[1];
        for (var i = 0; i < list.Count; i++)
        {
            if (Equals(list[i], target)) return (double)i;
            // double比較
            if (list[i] is double d1 && target is double d2 && d1 == d2) return (double)i;
        }
        return -1.0;
    }

    /// <summary>
    /// リストの要素を文字列で結合する
    /// </summary>
    public static object __listJoin(params object[] args)
    {
        if (args.Length < 1 || args[0] is not List<object> list)
            throw new RuntimeException("__listJoin requires a list argument");
        var separator = args.Length >= 2 ? args[1]?.ToString() ?? "" : ",";
        return string.Join(separator, list.Select(x => x?.ToString() ?? "null"));
    }

    /// <summary>
    /// 2つのリストを連結した新しいリストを返す
    /// </summary>
    public static object __listConcat(params object[] args)
    {
        if (args.Length != 2 || args[0] is not List<object> list1 || args[1] is not List<object> list2)
            throw new RuntimeException("__listConcat requires two list arguments");
        var result = new List<object>(list1);
        result.AddRange(list2);
        return result;
    }

    /// <summary>
    /// リストを反転する（破壊的）
    /// </summary>
    public static object __listReverse(params object[] args)
    {
        if (args.Length != 1 || args[0] is not List<object> list)
            throw new RuntimeException("__listReverse requires a list argument");
        list.Reverse();
        return list;
    }

    /// <summary>
    /// リストをソートする（破壊的）
    /// </summary>
    public static object __listSort(params object[] args)
    {
        if (args.Length < 1 || args[0] is not List<object> list)
            throw new RuntimeException("__listSort requires a list argument");
        list.Sort((a, b) =>
        {
            if (a is double d1 && b is double d2) return d1.CompareTo(d2);
            return (a?.ToString() ?? "").CompareTo(b?.ToString() ?? "");
        });
        return list;
    }

    // ========================================
    // Math プリミティブ
    // ========================================

    /// <summary>
    /// 絶対値を返す
    /// </summary>
    public static object __mathAbs(params object[] args)
    {
        if (args.Length != 1) throw new RuntimeException("__mathAbs requires one argument");
        return Math.Abs(Convert.ToDouble(args[0]));
    }

    /// <summary>
    /// 切り捨てを返す
    /// </summary>
    public static object __mathFloor(params object[] args)
    {
        if (args.Length != 1) throw new RuntimeException("__mathFloor requires one argument");
        return Math.Floor(Convert.ToDouble(args[0]));
    }

    /// <summary>
    /// 切り上げを返す
    /// </summary>
    public static object __mathCeil(params object[] args)
    {
        if (args.Length != 1) throw new RuntimeException("__mathCeil requires one argument");
        return Math.Ceiling(Convert.ToDouble(args[0]));
    }

    /// <summary>
    /// 四捨五入を返す
    /// </summary>
    public static object __mathRound(params object[] args)
    {
        if (args.Length != 1) throw new RuntimeException("__mathRound requires one argument");
        return Math.Round(Convert.ToDouble(args[0]), MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// 平方根を返す
    /// </summary>
    public static object __mathSqrt(params object[] args)
    {
        if (args.Length != 1) throw new RuntimeException("__mathSqrt requires one argument");
        return Math.Sqrt(Convert.ToDouble(args[0]));
    }

    /// <summary>
    /// 最小値を返す
    /// </summary>
    public static object __mathMin(params object[] args)
    {
        if (args.Length == 0) throw new RuntimeException("__mathMin requires at least one argument");
        // リストが渡された場合はその中の最小値
        if (args.Length == 1 && args[0] is List<object> list)
        {
            if (list.Count == 0) throw new RuntimeException("__mathMin: empty list");
            return list.Select(x => Convert.ToDouble(x)).Min();
        }
        return args.Select(x => Convert.ToDouble(x)).Min();
    }

    /// <summary>
    /// 最大値を返す
    /// </summary>
    public static object __mathMax(params object[] args)
    {
        if (args.Length == 0) throw new RuntimeException("__mathMax requires at least one argument");
        if (args.Length == 1 && args[0] is List<object> list)
        {
            if (list.Count == 0) throw new RuntimeException("__mathMax: empty list");
            return list.Select(x => Convert.ToDouble(x)).Max();
        }
        return args.Select(x => Convert.ToDouble(x)).Max();
    }

    private static readonly Random _random = new();

    /// <summary>
    /// 0以上1未満のランダムな数を返す
    /// </summary>
    public static object __mathRandom(params object[] args)
    {
        return _random.NextDouble();
    }

    // ========================================
    // Input 関数
    // ========================================

    /// <summary>
    /// 標準入力から1行読み取る。プロンプト引数がある場合は先に表示する。
    /// </summary>
    public static object Input(params object[] args)
    {
        if (args.Length > 0)
            Console.Write(args[0]?.ToString() ?? "");
        return Console.ReadLine() ?? "";
    }

    /// <summary>
    /// プロトタイプにメソッドを登録する
    /// </summary>
    public static object __registerPrototype(ScriptContext ctx, object[] args)
    {
        if (args.Length != 3 || args[0] is not string typeName || args[1] is not string methodName)
            throw new RuntimeException("__registerPrototype requires typeName, methodName, function");

        if (!ctx.Prototypes.ContainsKey(typeName))
            ctx.Prototypes[typeName] = new Dictionary<string, object>();

        ctx.Prototypes[typeName][methodName] = args[2];
        return null!;
    }

    #endregion
}
