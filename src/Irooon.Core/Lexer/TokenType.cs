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
    For,
    Foreach,
    In,
    Break,
    Continue,
    Return,
    Class,
    Public,
    Private,
    Static,
    Init,
    Try,
    Catch,
    Finally,
    Throw,
    Import,
    Export,
    From,

    // 演算子
    Plus,           // +
    Minus,          // -
    Star,           // *
    Slash,          // /
    Percent,        // %
    PlusEqual,      // +=
    MinusEqual,     // -=
    StarEqual,      // *=
    SlashEqual,     // /=
    PercentEqual,   // %=
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
    DotDot,         // ..
    DotDotDot,      // ...
    Colon,          // :

    // 特殊
    Newline,        // 改行（パーサーで必要な場合）
    Eof,            // End of File
    Error           // エラートークン
}
