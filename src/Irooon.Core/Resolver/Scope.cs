namespace Irooon.Core.Resolver;

/// <summary>
/// スコープを表現します。
/// </summary>
public class Scope
{
    private readonly Dictionary<string, VariableInfo> _variables = new();

    /// <summary>
    /// 親スコープ
    /// </summary>
    public Scope? Parent { get; }

    /// <summary>
    /// スコープの深さ
    /// </summary>
    public int Depth { get; }

    /// <summary>
    /// 関数/ラムダ/メソッドスコープかどうか
    /// </summary>
    public bool IsFunctionScope { get; }

    /// <summary>
    /// 関数スコープのスロットカウンター（パラメータ＋ローカル変数のインデックス割り当て用）
    /// </summary>
    public int FunctionSlotCounter { get; set; }

    /// <summary>
    /// Scopeの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="parent">親スコープ</param>
    /// <param name="isFunctionScope">関数スコープかどうか</param>
    public Scope(Scope? parent, bool isFunctionScope = false)
    {
        Parent = parent;
        Depth = parent?.Depth + 1 ?? 0;
        IsFunctionScope = isFunctionScope;
    }

    /// <summary>
    /// 変数を定義します。
    /// </summary>
    /// <param name="name">変数名</param>
    /// <param name="info">変数情報</param>
    public void Define(string name, VariableInfo info)
    {
        _variables[name] = info;
    }

    /// <summary>
    /// 変数を解決します（現在のスコープと親スコープを検索）。
    /// </summary>
    /// <param name="name">変数名</param>
    /// <returns>変数情報（見つからない場合はnull）</returns>
    public VariableInfo? Resolve(string name)
    {
        if (_variables.TryGetValue(name, out var info))
        {
            return info;
        }

        return Parent?.Resolve(name);
    }

    /// <summary>
    /// 現在のスコープに変数が定義されているか確認します（親スコープは検索しない）。
    /// </summary>
    /// <param name="name">変数名</param>
    /// <returns>定義されている場合はtrue</returns>
    public bool IsDefined(string name)
    {
        return _variables.ContainsKey(name);
    }

    /// <summary>
    /// 親チェーンを辿って最も近い関数スコープを返します。
    /// 自分自身が関数スコープの場合は自分を返します。
    /// </summary>
    /// <returns>関数スコープ（見つからない場合はnull）</returns>
    public Scope? GetEnclosingFunctionScope()
    {
        if (IsFunctionScope) return this;
        return Parent?.GetEnclosingFunctionScope();
    }
}
