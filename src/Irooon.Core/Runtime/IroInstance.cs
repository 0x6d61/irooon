namespace Irooon.Core.Runtime;

/// <summary>
/// irooon言語のクラスインスタンス
/// </summary>
public class IroInstance
{
    /// <summary>
    /// このインスタンスのクラス定義
    /// </summary>
    public IroClass Class { get; }

    /// <summary>
    /// フィールド値の辞書
    /// </summary>
    public Dictionary<string, object> Fields { get; }

    public IroInstance(IroClass iroClass)
    {
        Class = iroClass ?? throw new ArgumentNullException(nameof(iroClass));
        Fields = new Dictionary<string, object>();
    }
}
