namespace Irooon.Core.Resolver;

/// <summary>
/// 解析エラーを表します。
/// </summary>
public class ResolveException : Exception
{
    /// <summary>
    /// エラーが発生した行番号
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// エラーが発生した列番号
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// ResolveExceptionの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="message">エラーメッセージ</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public ResolveException(string message, int line, int column)
        : base($"[Line {line}, Col {column}] Resolve error: {message}")
    {
        Line = line;
        Column = column;
    }
}
