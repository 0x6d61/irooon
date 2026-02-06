namespace Irooon.Core.Runtime;

/// <summary>
/// irooon言語のクラス定義
/// </summary>
public class IroClass
{
    /// <summary>
    /// クラス名
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// フィールド定義のリスト
    /// </summary>
    public List<FieldDef> Fields { get; }

    /// <summary>
    /// インスタンスメソッドの辞書
    /// </summary>
    public Dictionary<string, IroCallable> Methods { get; }

    /// <summary>
    /// スタティックメソッドの辞書
    /// </summary>
    public Dictionary<string, IroCallable> StaticMethods { get; }

    public IroClass(string name, FieldDef[] fields, MethodDef[] methods)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Fields = new List<FieldDef>(fields ?? Array.Empty<FieldDef>());
        Methods = new Dictionary<string, IroCallable>();
        StaticMethods = new Dictionary<string, IroCallable>();

        // メソッドを辞書に追加
        if (methods != null)
        {
            foreach (var method in methods)
            {
                if (method.IsStatic)
                {
                    StaticMethods[method.Name] = method.Body;
                }
                else
                {
                    Methods[method.Name] = method.Body;
                }
            }
        }
    }
}
