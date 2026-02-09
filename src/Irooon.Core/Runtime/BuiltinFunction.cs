namespace Irooon.Core.Runtime;

/// <summary>
/// ビルトイン関数を表すクラス
/// RuntimeHelpersの静的メソッドをラップして、IroCallableとして扱えるようにする
/// </summary>
public class BuiltinFunction : IroCallable
{
    private readonly string _name;
    private readonly Func<object[], object>? _func;
    private readonly Func<ScriptContext, object[], object>? _ctxFunc;

    /// <summary>
    /// ビルトイン関数を作成する
    /// </summary>
    public BuiltinFunction(string name, Func<object[], object> func)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
        _func = func ?? throw new ArgumentNullException(nameof(func));
    }

    /// <summary>
    /// コンテキスト対応のビルトイン関数を作成する
    /// </summary>
    public BuiltinFunction(string name, Func<ScriptContext, object[], object> ctxFunc)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
        _ctxFunc = ctxFunc ?? throw new ArgumentNullException(nameof(ctxFunc));
    }

    /// <summary>
    /// 関数を呼び出す
    /// </summary>
    public object Invoke(ScriptContext ctx, object[] args)
    {
        if (_ctxFunc != null) return _ctxFunc(ctx, args);
        return _func!(args);
    }

    /// <summary>
    /// ビルトイン関数の文字列表現
    /// </summary>
    public override string ToString() => $"<builtin function {_name}>";
}
