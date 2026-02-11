using Irooon.Core.Diagnostics;

namespace Irooon.Core.Lexer;

/// <summary>
/// 字句解析中に発生したエラーを表す例外。
/// </summary>
public class LexException : Exception
{
    public ErrorCode Code { get; }
    public int Line { get; }
    public int Column { get; }
    public string RawMessage { get; }

    public LexException(string message, ErrorCode code, int line, int column)
        : base($"[Line {line}, Col {column}] Lex error: {message}")
    {
        Code = code;
        Line = line;
        Column = column;
        RawMessage = message;
    }
}
