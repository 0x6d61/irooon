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
    /// ローカル変数名のリスト（再帰呼び出し時の保存/復元に使用）
    /// let/varで宣言された変数名を保持
    /// </summary>
    public List<string> LocalNames { get; }

    /// <summary>
    /// 関数が定義された行番号
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// 関数が定義された列番号
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// 非同期関数かどうか
    /// </summary>
    public bool IsAsync { get; }

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
    /// <param name="line">関数定義の行番号</param>
    /// <param name="column">関数定義の列番号</param>
    /// <param name="localNames">ローカル変数名のリスト（省略可能）</param>
    /// <param name="isAsync">非同期関数かどうか</param>
    public Closure(string name, Func<ScriptContext, object[], object> body, List<string>? parameterNames = null, int line = 0, int column = 0, List<string>? localNames = null, bool isAsync = false)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        _body = body ?? throw new ArgumentNullException(nameof(body));
        ParameterNames = parameterNames ?? new List<string>();
        Line = line;
        Column = column;
        LocalNames = localNames ?? new List<string>();
        IsAsync = isAsync;
    }

    /// <summary>
    /// クロージャを呼び出します。
    /// </summary>
    public object Invoke(ScriptContext ctx, object[] args)
    {
        // スタックトレースに関数呼び出しを追加
        CallStack.Push(Name, Line, Column);
        try
        {
            // 引数が足りない場合はnullでパディング（省略可能引数のサポート）
            var actualArgs = args;
            if (ParameterNames.Count > args.Length)
            {
                actualArgs = new object[ParameterNames.Count];
                Array.Copy(args, actualArgs, args.Length);
                // 残りはnull（default(object)）
            }
            return _body(ctx, actualArgs);
        }
        finally
        {
            // 関数から戻る際にスタックから削除
            CallStack.Pop();
        }
    }

    public override string ToString()
    {
        return $"<closure: {Name}>";
    }
}
