namespace Irooon.Core.Runtime;

/// <summary>
/// 文字列メソッドのラッパークラス
/// .NETのStringメソッドをIroCallableとして呼び出せるようにする
/// </summary>
public class StringMethodWrapper : IroCallable
{
    private readonly string _str;
    private readonly string _methodName;

    public StringMethodWrapper(string str, string methodName)
    {
        _str = str;
        _methodName = methodName;
    }

    /// <summary>
    /// 文字列メソッドを呼び出す
    /// </summary>
    public object Invoke(ScriptContext ctx, object[] args)
    {
        return _methodName switch
        {
            "length" => (double)_str.Length,
            "toUpper" => _str.ToUpper(),
            "toLower" => _str.ToLower(),
            "trim" => _str.Trim(),
            "substring" => InvokeSubstring(args),
            "split" => InvokeSplit(args),
            "contains" => _str.Contains(args[0]?.ToString() ?? ""),
            "startsWith" => _str.StartsWith(args[0]?.ToString() ?? ""),
            "endsWith" => _str.EndsWith(args[0]?.ToString() ?? ""),
            "replace" => _str.Replace(args[0]?.ToString() ?? "", args[1]?.ToString() ?? ""),
            _ => throw new RuntimeException($"Unknown string method: {_methodName}")
        };
    }

    /// <summary>
    /// substring メソッドの実装
    /// 引数1個の場合: substring(start)
    /// 引数2個の場合: substring(start, length)
    /// </summary>
    private object InvokeSubstring(object[] args)
    {
        if (args.Length == 0)
            throw new RuntimeException("substring requires at least 1 argument");

        int start = Convert.ToInt32(args[0]);

        if (args.Length == 1)
        {
            // substring(start) - 開始位置から末尾まで
            return _str.Substring(start);
        }
        else
        {
            // substring(start, length) - 開始位置から指定長さ
            int length = Convert.ToInt32(args[1]);
            return _str.Substring(start, length);
        }
    }

    /// <summary>
    /// split メソッドの実装
    /// 引数: セパレータ文字列
    /// 戻り値: List&lt;object&gt;
    /// </summary>
    private object InvokeSplit(object[] args)
    {
        if (args.Length == 0)
            throw new RuntimeException("split requires 1 argument");

        string separator = args[0]?.ToString() ?? "";
        var parts = _str.Split(separator);
        return new List<object>(parts.Cast<object>());
    }
}
