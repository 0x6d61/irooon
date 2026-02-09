namespace Irooon.Core.Runtime;

/// <summary>
/// スプレッド演算子でマークされた値を表すラッパー
/// </summary>
public class SpreadMarker
{
    public object Value { get; }

    public SpreadMarker(object value)
    {
        Value = value;
    }
}
