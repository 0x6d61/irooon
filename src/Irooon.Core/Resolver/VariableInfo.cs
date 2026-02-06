namespace Irooon.Core.Resolver;

/// <summary>
/// 変数の情報を保持します。
/// </summary>
public class VariableInfo
{
    /// <summary>
    /// 変数名
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 読み取り専用かどうか（letならtrue）
    /// </summary>
    public bool IsReadOnly { get; }

    /// <summary>
    /// 宣言済みかどうか
    /// </summary>
    public bool IsDeclared { get; set; }

    /// <summary>
    /// 使用されているかどうか
    /// </summary>
    public bool IsUsed { get; set; }

    /// <summary>
    /// スコープの深さ
    /// </summary>
    public int ScopeDepth { get; }

    /// <summary>
    /// VariableInfoの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="name">変数名</param>
    /// <param name="isReadOnly">読み取り専用かどうか</param>
    /// <param name="scopeDepth">スコープの深さ</param>
    public VariableInfo(string name, bool isReadOnly, int scopeDepth)
    {
        Name = name;
        IsReadOnly = isReadOnly;
        ScopeDepth = scopeDepth;
        IsDeclared = true;
        IsUsed = false;
    }
}
