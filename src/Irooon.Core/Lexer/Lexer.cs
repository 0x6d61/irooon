using System.Globalization;

namespace Irooon.Core.Lexer;

/// <summary>
/// irooon言語のLexer（字句解析器）
/// ソースコードをトークン列に変換します。
/// </summary>
public class Lexer
{
    private readonly string _source;
    private readonly List<Token> _tokens = new();
    private int _start = 0;
    private int _current = 0;
    private int _line = 1;
    private int _column = 1;

    // キーワード辞書
    private static readonly Dictionary<string, TokenType> _keywords = new()
    {
        { "let", TokenType.Let },
        { "var", TokenType.Var },
        { "fn", TokenType.Fn },
        { "if", TokenType.If },
        { "else", TokenType.Else },
        { "for", TokenType.For },
        { "foreach", TokenType.Foreach },
        { "in", TokenType.In },
        { "break", TokenType.Break },
        { "continue", TokenType.Continue },
        { "return", TokenType.Return },
        { "class", TokenType.Class },
        { "public", TokenType.Public },
        { "private", TokenType.Private },
        { "static", TokenType.Static },
        { "init", TokenType.Init },
        { "try", TokenType.Try },
        { "catch", TokenType.Catch },
        { "finally", TokenType.Finally },
        { "throw", TokenType.Throw },
        { "import", TokenType.Import },
        { "export", TokenType.Export },
        { "from", TokenType.From },
        { "true", TokenType.True },
        { "false", TokenType.False },
        { "null", TokenType.Null },
        { "and", TokenType.And },
        { "or", TokenType.Or },
        { "not", TokenType.Not }
    };

    /// <summary>
    /// Lexerを構築します。
    /// </summary>
    /// <param name="source">ソースコード</param>
    public Lexer(string source)
    {
        _source = source ?? "";
    }

    /// <summary>
    /// ソースコードをスキャンしてトークン列を返します。
    /// </summary>
    /// <returns>トークンのリスト</returns>
    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            _start = _current;
            ScanToken();
        }

        // EOFトークンを追加
        _tokens.Add(new Token(TokenType.Eof, "", null, _line, _column));
        return _tokens;
    }

    /// <summary>
    /// 1つのトークンをスキャンします。
    /// </summary>
    private void ScanToken()
    {
        char c = Advance();

        switch (c)
        {
            // 単一文字トークン
            case '(': AddToken(TokenType.LeftParen); break;
            case ')': AddToken(TokenType.RightParen); break;
            case '{': AddToken(TokenType.LeftBrace); break;
            case '}': AddToken(TokenType.RightBrace); break;
            case '[': AddToken(TokenType.LeftBracket); break;
            case ']': AddToken(TokenType.RightBracket); break;
            case ',': AddToken(TokenType.Comma); break;
            case ':': AddToken(TokenType.Colon); break;

            // . または .. または ...
            case '.':
                if (Match('.'))
                {
                    // .. または ...
                    if (Match('.'))
                    {
                        AddToken(TokenType.DotDotDot);
                    }
                    else
                    {
                        AddToken(TokenType.DotDot);
                    }
                }
                else
                {
                    AddToken(TokenType.Dot);
                }
                break;
            case '+':
                AddToken(Match('=') ? TokenType.PlusEqual : TokenType.Plus);
                break;
            case '-':
                AddToken(Match('=') ? TokenType.MinusEqual : TokenType.Minus);
                break;
            case '*':
                AddToken(Match('=') ? TokenType.StarEqual : TokenType.Star);
                break;
            case '%':
                AddToken(Match('=') ? TokenType.PercentEqual : TokenType.Percent);
                break;

            // 2文字の可能性があるトークン
            case '/':
                if (Match('/'))
                {
                    // コメント - 行末まで読み飛ばす
                    while (Peek() != '\n' && !IsAtEnd())
                        Advance();
                }
                else
                {
                    AddToken(Match('=') ? TokenType.SlashEqual : TokenType.Slash);
                }
                break;

            case '=':
                AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal);
                break;

            case '!':
                if (Match('='))
                {
                    AddToken(TokenType.BangEqual);
                }
                else
                {
                    // '!'単体はirooonではサポートしていない（notキーワードを使う）
                    AddError($"Unexpected character '!'");
                }
                break;

            case '<':
                AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less);
                break;

            case '>':
                AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater);
                break;

            // 空白文字
            case ' ':
            case '\r':
            case '\t':
                // 無視
                break;

            case '\n':
                _line++;
                _column = 1;
                // 改行は基本的にスキップ（セミコロン不要のため）
                break;

            // 文字列リテラル
            case '"':
                ScanString();
                break;

            default:
                if (IsDigit(c))
                {
                    ScanNumber();
                }
                else if (IsAlpha(c))
                {
                    ScanIdentifier();
                }
                else
                {
                    AddError($"Unexpected character '{c}'");
                }
                break;
        }
    }

    /// <summary>
    /// 数値リテラルをスキャンします。
    /// </summary>
    private void ScanNumber()
    {
        // 整数部分
        while (IsDigit(Peek()))
            Advance();

        // 小数部分
        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            // '.'を消費
            Advance();

            // 小数部分
            while (IsDigit(Peek()))
                Advance();
        }

        string text = _source.Substring(_start, _current - _start);
        if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
        {
            AddToken(TokenType.Number, value);
        }
        else
        {
            AddError($"Invalid number format: {text}");
        }
    }

    /// <summary>
    /// 文字列リテラルをスキャンします。
    /// </summary>
    private void ScanString()
    {
        int startLine = _line;
        int startColumn = _column - 1; // '"'の位置

        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n')
            {
                _line++;
                _column = 0; // Advance()で+1されるので0にする
            }
            Advance();
        }

        if (IsAtEnd())
        {
            AddError("Unterminated string", startLine, startColumn);
            return;
        }

        // 終端の '"' を消費
        Advance();

        // 引用符を除いた文字列値を取得
        string value = _source.Substring(_start + 1, _current - _start - 2);
        AddToken(TokenType.String, value);
    }

    /// <summary>
    /// 識別子またはキーワードをスキャンします。
    /// </summary>
    private void ScanIdentifier()
    {
        while (IsAlphaNumeric(Peek()))
            Advance();

        string text = _source.Substring(_start, _current - _start);

        // キーワードチェック
        if (_keywords.TryGetValue(text, out TokenType type))
        {
            AddToken(type);
        }
        else
        {
            AddToken(TokenType.Identifier);
        }
    }

    #region ヘルパーメソッド

    /// <summary>
    /// 現在の文字を消費して次に進みます。
    /// </summary>
    private char Advance()
    {
        _column++;
        return _source[_current++];
    }

    /// <summary>
    /// 現在の文字が期待する文字と一致する場合、消費してtrueを返します。
    /// </summary>
    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (_source[_current] != expected) return false;

        _current++;
        _column++;
        return true;
    }

    /// <summary>
    /// 現在の文字を返しますが、消費しません。
    /// </summary>
    private char Peek()
    {
        if (IsAtEnd()) return '\0';
        return _source[_current];
    }

    /// <summary>
    /// 次の文字を返しますが、消費しません。
    /// </summary>
    private char PeekNext()
    {
        if (_current + 1 >= _source.Length) return '\0';
        return _source[_current + 1];
    }

    /// <summary>
    /// 文字が数字かどうかを判定します。
    /// </summary>
    private bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    /// <summary>
    /// 文字がアルファベットまたはアンダースコアかどうかを判定します。
    /// </summary>
    private bool IsAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') ||
               (c >= 'A' && c <= 'Z') ||
               c == '_';
    }

    /// <summary>
    /// 文字が英数字またはアンダースコアかどうかを判定します。
    /// </summary>
    private bool IsAlphaNumeric(char c)
    {
        return IsAlpha(c) || IsDigit(c);
    }

    /// <summary>
    /// ソースの終端に達したかどうかを判定します。
    /// </summary>
    private bool IsAtEnd()
    {
        return _current >= _source.Length;
    }

    /// <summary>
    /// トークンを追加します。
    /// </summary>
    private void AddToken(TokenType type, object? value = null)
    {
        string text = _source.Substring(_start, _current - _start);
        int tokenColumn = _column - (_current - _start);
        _tokens.Add(new Token(type, text, value, _line, tokenColumn));
    }

    /// <summary>
    /// エラートークンを追加します。
    /// </summary>
    private void AddError(string message, int? line = null, int? column = null)
    {
        string text = _source.Substring(_start, Math.Min(_current - _start + 1, _source.Length - _start));
        int errorLine = line ?? _line;
        int errorColumn = column ?? (_column - (_current - _start));
        _tokens.Add(new Token(TokenType.Error, text, message, errorLine, errorColumn));
    }

    #endregion
}
