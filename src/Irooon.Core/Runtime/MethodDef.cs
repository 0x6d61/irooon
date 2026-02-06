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
    /// メソッド本体（IroCallableとして）
    /// </summary>
    public IroCallable Body { get; }

    public MethodDef(string name, bool isPublic, bool isStatic, IroCallable body)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        IsPublic = isPublic;
        IsStatic = isStatic;
        Body = body ?? throw new ArgumentNullException(nameof(body));
    }
}
