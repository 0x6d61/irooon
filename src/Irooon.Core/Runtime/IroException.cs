namespace Irooon.Core.Runtime;

/// <summary>
/// irooon言語の throw 文でスローされる例外をラップするクラス。
/// </summary>
public class IroException : Exception
{
    /// <summary>
    /// スローされた値（irooon言語のオブジェクト）。
    /// </summary>
    public object Value { get; } = null!;

    /// <summary>
    /// IroExceptionの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="value">スローされた値</param>
    public IroException(object? value)
        : base(value?.ToString() ?? "null")
    {
        Value = value ?? "null";
    }
}
