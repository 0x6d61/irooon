namespace Irooon.Core.Runtime;

/// <summary>
/// ビルトイン関数を表すクラス
/// RuntimeHelpersの静的メソッドをラップして、IroCallableとして扱えるようにする
/// </summary>
public class BuiltinFunction : IroCallable
{
    private readonly string _name;
    private readonly Func<object[], object> _func;

    /// <summary>
    /// ビルトイン関数を作成する
    /// </summary>
    /// <param name="name">関数名</param>
    /// <param name="func">実行する関数</param>
    public BuiltinFunction(string name, Func<object[], object> func)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
        _func = func ?? throw new ArgumentNullException(nameof(func));
    }

    /// <summary>
    /// 関数を呼び出す
    /// </summary>
    /// <param name="ctx">スクリプトコンテキスト（使用しない）</param>
    /// <param name="args">引数の配列</param>
    /// <returns>実行結果</returns>
    public object Invoke(ScriptContext ctx, object[] args)
    {
        return _func(args);
    }

    /// <summary>
    /// ビルトイン関数の文字列表現
    /// </summary>
    public override string ToString() => $"<builtin function {_name}>";
}
