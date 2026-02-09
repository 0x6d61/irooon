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
    Extends,
    Super,
    Async,
    Await,
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
    Match,

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
    PlusPlus,       // ++
    MinusMinus,     // --
    EqualEqual,     // ==
    BangEqual,      // !=
    Less,           // <
    LessEqual,      // <=
    Greater,        // >
    GreaterEqual,   // >=
    Question,       // ?
    QuestionQuestion, // ??
    QuestionDot,    // ?.
    And,            // and
    Or,             // or
    Not,            // not
    Equal,          // =
    Arrow,          // =>
    Ampersand,      // &
    Pipe,           // |
    Caret,          // ^
    Tilde,          // ~
    LessLess,       // <<
    GreaterGreater, // >>
    StarStar,       // **

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
    Dollar,         // $
    Backtick,       // `

    // 特殊
    Newline,        // 改行（パーサーで必要な場合）
    Eof,            // End of File
    Error           // エラートークン
}
