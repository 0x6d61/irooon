namespace Irooon.Core.Lexer;

/// <summary>
/// irooon言語のトークンを表します。
/// </summary>
public class Token
{
    /// <summary>
    /// トークンのタイプ
    /// </summary>
    public TokenType Type { get; }

    /// <summary>
    /// ソースコード上の字句（元のテキスト）
    /// </summary>
    public string Lexeme { get; }

    /// <summary>
    /// トークンの値（数値リテラルや文字列リテラルの場合）
    /// </summary>
    public object? Value { get; }

    /// <summary>
    /// ソースコード上の行番号（1始まり）
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// ソースコード上の列番号（1始まり）
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// Tokenを構築します。
    /// </summary>
    /// <param name="type">トークンタイプ</param>
    /// <param name="lexeme">字句</param>
    /// <param name="value">値（オプション）</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public Token(TokenType type, string lexeme, object? value, int line, int column)
    {
        Type = type;
        Lexeme = lexeme;
        Value = value;
        Line = line;
        Column = column;
    }

    /// <summary>
    /// デバッグ用の文字列表現
    /// </summary>
    public override string ToString()
    {
        var valueStr = Value != null ? $" ({Value})" : "";
        return $"[{Line}:{Column}] {Type} '{Lexeme}'{valueStr}";
    }
}
