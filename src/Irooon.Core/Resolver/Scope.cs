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
    /// Scopeの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="parent">親スコープ</param>
    public Scope(Scope? parent)
    {
        Parent = parent;
        Depth = parent?.Depth + 1 ?? 0;
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
}
