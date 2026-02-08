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

        // 文字列の場合、StringMethodWrapperを返す
        if (target is string str)
        {
            return new StringMethodWrapper(str, name);
        }

        // リストの場合、ListMethodWrapperを優先する（irooon内部のリストメソッドをサポート）
        if (target is List<object> list)
        {
            // ListMethodWrapperのメソッド名をチェック（大文字小文字を区別しない）
            var listMethods = new[] { "map", "filter", "reduce", "foreach", "first", "last", "length", "isempty" };
            var lowerName = name.ToLower();
            if (listMethods.Contains(lowerName))
            {
                // forEach と isEmpty を正規化
                var normalizedName = lowerName switch
                {
                    "foreach" => "forEach",
                    "isempty" => "isEmpty",
                    _ => lowerName
                };
                return new ListMethodWrapper(list, normalizedName);
            }

            // それ以外はCLRメソッド/プロパティとして扱う
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
}
