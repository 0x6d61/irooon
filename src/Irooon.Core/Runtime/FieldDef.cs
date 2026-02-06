namespace Irooon.Core.Runtime;

/// <summary>
/// クラスのフィールド定義
/// </summary>
public class FieldDef
{
    /// <summary>
    /// フィールド名
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
    /// 初期化式（オプション）
    /// ScriptContextを受け取ってobjectを返す関数
    /// </summary>
    public Func<ScriptContext, object?>? Initializer { get; set; }

    public FieldDef(string name, bool isPublic, Func<ScriptContext, object?>? initializer = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        IsPublic = isPublic;
        IsStatic = false; // v0.1ではstaticフィールド未対応
        Initializer = initializer;
    }
}
