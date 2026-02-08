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

    /// <summary>
    /// 親クラス（継承する場合）
    /// </summary>
    public IroClass? Parent { get; }

    public IroClass(string name, FieldDef[] fields, MethodDef[] methods, IroClass? parent = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Fields = new List<FieldDef>(fields ?? Array.Empty<FieldDef>());
        Methods = new Dictionary<string, IroCallable>();
        StaticMethods = new Dictionary<string, IroCallable>();
        Parent = parent;

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

    /// <summary>
    /// フィールドを取得します（親クラスも検索）
    /// </summary>
    /// <param name="name">フィールド名</param>
    /// <returns>フィールド定義（見つからない場合はnull）</returns>
    public FieldDef? GetField(string name)
    {
        // 自クラスのフィールドから検索
        var field = Fields.FirstOrDefault(f => f.Name == name);
        if (field != null)
        {
            return field;
        }

        // 親クラスから検索
        return Parent?.GetField(name);
    }

    /// <summary>
    /// メソッドを取得します（親クラスも検索）
    /// </summary>
    /// <param name="name">メソッド名</param>
    /// <returns>メソッド（見つからない場合はnull）</returns>
    public IroCallable? GetMethod(string name)
    {
        // 自クラスのメソッドから検索
        if (Methods.TryGetValue(name, out var method))
        {
            return method;
        }

        // 親クラスから検索
        return Parent?.GetMethod(name);
    }
}
