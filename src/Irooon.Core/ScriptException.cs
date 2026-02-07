namespace Irooon.Core;

/// <summary>
/// スクリプト実行中に発生したエラーを表す例外。
/// </summary>
public class ScriptException : Exception
{
    /// <summary>
    /// ScriptExceptionの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="message">エラーメッセージ</param>
    public ScriptException(string message) : base(message)
    {
    }

    /// <summary>
    /// ScriptExceptionの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="message">エラーメッセージ</param>
    /// <param name="innerException">内部例外</param>
    public ScriptException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
