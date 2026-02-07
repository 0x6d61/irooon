namespace Irooon.Core.Runtime;

/// <summary>
/// クロージャを表すクラス
/// 関数定義とラムダ式をIroCallableとして扱います
/// </summary>
public class Closure : IroCallable
{
    /// <summary>
    /// 関数名（ラムダの場合は"&lt;lambda&gt;"）
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// パラメータ名のリスト（再帰呼び出し時の保存/復元に使用）
    /// </summary>
    public List<string> ParameterNames { get; }

    /// <summary>
    /// 関数本体
    /// </summary>
    private readonly Func<ScriptContext, object[], object> _body;

    /// <summary>
    /// Closureの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="name">関数名</param>
    /// <param name="body">関数本体</param>
    /// <param name="parameterNames">パラメータ名のリスト（省略可能）</param>
    public Closure(string name, Func<ScriptContext, object[], object> body, List<string>? parameterNames = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        _body = body ?? throw new ArgumentNullException(nameof(body));
        ParameterNames = parameterNames ?? new List<string>();
    }

    /// <summary>
    /// クロージャを呼び出します。
    /// </summary>
    public object Invoke(ScriptContext ctx, object[] args)
    {
        return _body(ctx, args);
    }

    public override string ToString()
    {
        return $"<closure: {Name}>";
    }
}
