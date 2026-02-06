namespace Irooon.Core.Runtime;

/// <summary>
/// クラスのメソッド定義
/// </summary>
public class MethodDef
{
    /// <summary>
    /// メソッド名
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// public修飾子
    /// </summary>
    public bool IsPublic { get; }

    /// <summary>
    /// static修飾子
    /// </summary>
    public bool IsStatic { get; }

    /// <summary>
    /// パラメータ名のリスト
    /// </summary>
    public List<string> Parameters { get; }

    /// <summary>
    /// メソッド本体（IroCallableとして）
    /// </summary>
    public IroCallable? Body { get; set; }

    public MethodDef(string name, bool isPublic, bool isStatic, List<string> parameters)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        IsPublic = isPublic;
        IsStatic = isStatic;
        Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
    }
}
