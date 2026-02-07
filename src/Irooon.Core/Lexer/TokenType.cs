namespace Irooon.Core.Lexer;

/// <summary>
/// irooon言語のトークンタイプを定義します。
/// </summary>
public enum TokenType
{
    // リテラル
    Number,
    String,
    True,
    False,
    Null,

    // 識別子とキーワード
    Identifier,
    Let,
    Var,
    Fn,
    If,
    Else,
    While,
    Return,
    Class,
    Public,
    Private,
    Static,
    Init,

    // 演算子
    Plus,           // +
    Minus,          // -
    Star,           // *
    Slash,          // /
    Percent,        // %
    EqualEqual,     // ==
    BangEqual,      // !=
    Less,           // <
    LessEqual,      // <=
    Greater,        // >
    GreaterEqual,   // >=
    And,            // and
    Or,             // or
    Not,            // not
    Equal,          // =

    // 区切り文字
    LeftParen,      // (
    RightParen,     // )
    LeftBrace,      // {
    RightBrace,     // }
    LeftBracket,    // [
    RightBracket,   // ]
    Comma,          // ,
    Dot,            // .
    Colon,          // :

    // 特殊
    Newline,        // 改行（パーサーで必要な場合）
    Eof,            // End of File
    Error           // エラートークン
}
