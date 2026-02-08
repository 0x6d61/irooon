namespace Irooon.Core.Runtime;

/// <summary>
/// リストメソッドのラッパークラス
/// リストのコレクション操作メソッドをIroCallableとして呼び出せるようにする
/// </summary>
public class ListMethodWrapper : IroCallable
{
    private readonly List<object> _list;
    private readonly string _methodName;

    public ListMethodWrapper(List<object> list, string methodName)
    {
        _list = list;
        _methodName = methodName;
    }

    /// <summary>
    /// リストメソッドを呼び出す
    /// </summary>
    public object Invoke(ScriptContext ctx, object[] args)
    {
        return _methodName switch
        {
            "map" => Map(ctx, args),
            "filter" => Filter(ctx, args),
            "reduce" => Reduce(ctx, args),
            "forEach" => ForEach(ctx, args),
            "first" => First(args),
            "last" => Last(args),
            "length" => Length(args),
            "isEmpty" => IsEmpty(args),
            _ => throw new RuntimeException($"Unknown list method: {_methodName}")
        };
    }

    /// <summary>
    /// map - 各要素に関数を適用して新しいリストを返す
    /// </summary>
    private object Map(ScriptContext ctx, object[] args)
    {
        if (args.Length != 1 || args[0] is not IroCallable fn)
            throw new RuntimeException("map requires a function argument");

        var result = new List<object>();
        foreach (var item in _list)
        {
            result.Add(RuntimeHelpers.Invoke(fn, ctx, new[] { item }));
        }
        return result;
    }

    /// <summary>
    /// filter - 条件に合う要素を抽出して新しいリストを返す
    /// </summary>
    private object Filter(ScriptContext ctx, object[] args)
    {
        if (args.Length != 1 || args[0] is not IroCallable fn)
            throw new RuntimeException("filter requires a function argument");

        var result = new List<object>();
        foreach (var item in _list)
        {
            var predicate = RuntimeHelpers.Invoke(fn, ctx, new[] { item });
            if (RuntimeHelpers.IsTruthy(predicate))
            {
                result.Add(item);
            }
        }
        return result;
    }

    /// <summary>
    /// reduce - 畳み込み演算を行う
    /// </summary>
    private object Reduce(ScriptContext ctx, object[] args)
    {
        if (args.Length != 2 || args[1] is not IroCallable fn)
            throw new RuntimeException("reduce requires an initial value and a function");

        var accumulator = args[0];
        foreach (var item in _list)
        {
            accumulator = RuntimeHelpers.Invoke(fn, ctx, new[] { accumulator, item });
        }
        return accumulator;
    }

    /// <summary>
    /// forEach - 副作用のある反復処理を行う
    /// </summary>
    private object ForEach(ScriptContext ctx, object[] args)
    {
        if (args.Length != 1 || args[0] is not IroCallable fn)
            throw new RuntimeException("forEach requires a function argument");

        foreach (var item in _list)
        {
            RuntimeHelpers.Invoke(fn, ctx, new[] { item });
        }
        return null;
    }

    /// <summary>
    /// first - 最初の要素を返す
    /// </summary>
    private object First(object[] args)
    {
        if (_list.Count == 0)
            throw new RuntimeException("Cannot get first element of empty list");
        return _list[0];
    }

    /// <summary>
    /// last - 最後の要素を返す
    /// </summary>
    private object Last(object[] args)
    {
        if (_list.Count == 0)
            throw new RuntimeException("Cannot get last element of empty list");
        return _list[_list.Count - 1];
    }

    /// <summary>
    /// length - 要素数を返す
    /// </summary>
    private object Length(object[] args)
    {
        return (double)_list.Count;
    }

    /// <summary>
    /// isEmpty - 空かどうかを返す
    /// </summary>
    private object IsEmpty(object[] args)
    {
        return _list.Count == 0;
    }
}
