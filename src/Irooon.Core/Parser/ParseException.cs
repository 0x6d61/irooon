using Irooon.Core.Lexer;

namespace Irooon.Core.Parser;

/// <summary>
/// パース中に発生したエラーを表す例外。
/// </summary>
public class ParseException : Exception
{
    /// <summary>
    /// エラーが発生したトークン
    /// </summary>
    public Token Token { get; }

    /// <summary>
    /// ParseExceptionの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="token">エラーが発生したトークン</param>
    /// <param name="message">エラーメッセージ</param>
    public ParseException(Token token, string message)
        : base($"[Line {token.Line}, Col {token.Column}] Parse error: {message}")
    {
        Token = token;
    }
}
