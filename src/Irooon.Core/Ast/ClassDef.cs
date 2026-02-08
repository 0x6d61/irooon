namespace Irooon.Core.Ast;

/// <summary>
/// クラス定義を表します。
/// </summary>
public class ClassDef : Statement
{
    /// <summary>
    /// クラス名
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// フィールドのリスト
    /// </summary>
    public List<FieldDef> Fields { get; }

    /// <summary>
    /// メソッドのリスト
    /// </summary>
    public List<MethodDef> Methods { get; }

    /// <summary>
    /// 親クラス名（継承する場合）
    /// </summary>
    public string? ParentClass { get; }

    /// <summary>
    /// ClassDefの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="name">クラス名</param>
    /// <param name="fields">フィールドのリスト</param>
    /// <param name="methods">メソッドのリスト</param>
    /// <param name="parentClass">親クラス名（nullの場合は継承なし）</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public ClassDef(string name, List<FieldDef> fields, List<MethodDef> methods, string? parentClass, int line, int column)
        : base(line, column)
    {
        Name = name;
        Fields = fields;
        Methods = methods;
        ParentClass = parentClass;
    }
}
