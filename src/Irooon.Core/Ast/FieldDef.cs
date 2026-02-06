namespace Irooon.Core.Ast;

/// <summary>
/// フィールド定義を表します。
/// </summary>
public class FieldDef : AstNode
{
    /// <summary>
    /// フィールド名
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// publicかどうか
    /// </summary>
    public bool IsPublic { get; }

    /// <summary>
    /// 初期化式（nullの場合はnull初期化）
    /// </summary>
    public Expression? Initializer { get; }

    /// <summary>
    /// FieldDefの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="name">フィールド名</param>
    /// <param name="isPublic">publicかどうか</param>
    /// <param name="initializer">初期化式</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public FieldDef(string name, bool isPublic, Expression? initializer, int line, int column)
        : base(line, column)
    {
        Name = name;
        IsPublic = isPublic;
        Initializer = initializer;
    }
}
