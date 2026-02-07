namespace Irooon.Core.Runtime;

/// <summary>
/// Runtime実行時の例外
/// </summary>
public class RuntimeException : Exception
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
    /// スタックトレース文字列
    /// </summary>
    public string StackTraceString { get; }

    /// <summary>
    /// RuntimeExceptionの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="message">エラーメッセージ</param>
    public RuntimeException(string message) : base(message)
    {
        Line = 0;
        Column = 0;
        StackTraceString = string.Empty;
    }

    /// <summary>
    /// RuntimeExceptionの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="message">エラーメッセージ</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public RuntimeException(string message, int line, int column) : base(message)
    {
        Line = line;
        Column = column;
        StackTraceString = CallStack.GetStackTrace();
    }

    /// <summary>
    /// RuntimeExceptionの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="message">エラーメッセージ</param>
    /// <param name="inner">内部例外</param>
    public RuntimeException(string message, Exception inner) : base(message, inner)
    {
        Line = 0;
        Column = 0;
        StackTraceString = string.Empty;
    }

    /// <summary>
    /// RuntimeExceptionの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="message">エラーメッセージ</param>
    /// <param name="inner">内部例外</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public RuntimeException(string message, Exception inner, int line, int column) : base(message, inner)
    {
        Line = line;
        Column = column;
        StackTraceString = CallStack.GetStackTrace();
    }

    /// <summary>
    /// 例外の文字列表現を取得します。
    /// </summary>
    /// <returns>例外の詳細情報</returns>
    public override string ToString()
    {
        var result = $"RuntimeError at line {Line}, column {Column}:\n  {Message}";

        if (!string.IsNullOrEmpty(StackTraceString))
        {
            result += $"\n\nStack trace:\n{StackTraceString}";
        }

        return result;
    }
}
