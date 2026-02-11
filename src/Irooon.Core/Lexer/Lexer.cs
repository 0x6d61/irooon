using System.Globalization;
using System.Text;

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
        { "extends", TokenType.Extends },
        { "super", TokenType.Super },
        { "async", TokenType.Async },
        { "await", TokenType.Await },
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
        { "match", TokenType.Match },
        { "instanceof", TokenType.InstanceOf },
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
    /// Lexer エラーのリストを取得します。
    /// TokenType.Error のトークンをエラー情報として返します。
    /// </summary>
    public List<Token> GetErrors()
    {
        return _tokens.Where(t => t.Type == TokenType.Error).ToList();
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
                if (Match('+'))
                {
                    AddToken(TokenType.PlusPlus);
                }
                else
                {
                    AddToken(Match('=') ? TokenType.PlusEqual : TokenType.Plus);
                }
                break;
            case '-':
                if (Match('-'))
                {
                    AddToken(TokenType.MinusMinus);
                }
                else
                {
                    AddToken(Match('=') ? TokenType.MinusEqual : TokenType.Minus);
                }
                break;
            case '*':
                if (Match('*'))
                    AddToken(TokenType.StarStar);
                else
                    AddToken(Match('=') ? TokenType.StarEqual : TokenType.Star);
                break;
            case '%':
                AddToken(Match('=') ? TokenType.PercentEqual : TokenType.Percent);
                break;
            case '&':
                AddToken(TokenType.Ampersand);
                break;
            case '|':
                AddToken(TokenType.Pipe);
                break;
            case '^':
                AddToken(TokenType.Caret);
                break;
            case '~':
                AddToken(TokenType.Tilde);
                break;

            // 2文字の可能性があるトークン
            case '/':
                if (Match('/'))
                {
                    // 単一行コメント - 行末まで読み飛ばす
                    while (Peek() != '\n' && !IsAtEnd())
                        Advance();
                }
                else if (Match('*'))
                {
                    // 複数行コメント - */ まで読み飛ばす
                    int commentStart = _line;
                    while (!IsAtEnd())
                    {
                        if (Peek() == '\n')
                        {
                            _line++;
                            _column = 0;
                        }
                        if (Peek() == '*' && PeekNext() == '/')
                        {
                            Advance(); // * を消費
                            Advance(); // / を消費
                            break;
                        }
                        Advance();
                    }
                    if (IsAtEnd() && !(_source.Length >= 2 && _source[_current - 2] == '*' && _source[_current - 1] == '/'))
                    {
                        AddError("Unterminated block comment", commentStart, 0);
                    }
                }
                else
                {
                    AddToken(Match('=') ? TokenType.SlashEqual : TokenType.Slash);
                }
                break;

            case '=':
                if (Match('='))
                    AddToken(TokenType.EqualEqual);
                else if (Match('>'))
                    AddToken(TokenType.Arrow);
                else
                    AddToken(TokenType.Equal);
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
                if (Match('<'))
                    AddToken(TokenType.LessLess);
                else
                    AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less);
                break;

            case '>':
                if (Match('>'))
                    AddToken(TokenType.GreaterGreater);
                else
                    AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater);
                break;

            case '?':
                if (Match('?'))
                {
                    AddToken(TokenType.QuestionQuestion);
                }
                else if (Match('.'))
                {
                    AddToken(TokenType.QuestionDot);
                }
                else
                {
                    AddToken(TokenType.Question);
                }
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

            // バッククォート文字列
            case '`':
                ScanBacktickString();
                break;

            // ドル記号
            case '$':
                AddToken(TokenType.Dollar);
                break;

            // #r ディレクティブ（アセンブリ参照）
            case '#':
                ScanDirective();
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
        // 16進数: 0x...
        if (_source[_start] == '0' && Peek() is 'x' or 'X')
        {
            Advance(); // x を消費
            while (IsHexDigit(Peek()))
                Advance();

            string hexText = _source.Substring(_start + 2, _current - _start - 2);
            if (hexText.Length > 0 && long.TryParse(hexText, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long hexValue))
            {
                AddToken(TokenType.Number, (double)hexValue);
            }
            else
            {
                AddError($"Invalid hex number format: {_source.Substring(_start, _current - _start)}");
            }
            return;
        }

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

        // 科学的記数法: e/E
        if (Peek() is 'e' or 'E')
        {
            Advance(); // e を消費
            if (Peek() is '+' or '-')
                Advance(); // 符号を消費
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

    private bool IsHexDigit(char c)
    {
        return IsDigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
    }

    /// <summary>
    /// #r ディレクティブをスキャンします。
    /// #r "path/to/assembly.dll"
    /// </summary>
    private void ScanDirective()
    {
        // # の後に 'r' が続くかチェック
        if (!IsAtEnd() && Peek() == 'r')
        {
            Advance(); // 'r' を消費

            // 空白をスキップ
            while (!IsAtEnd() && (Peek() == ' ' || Peek() == '\t'))
            {
                Advance();
            }

            // 文字列パスを読み取る
            if (!IsAtEnd() && Peek() == '"')
            {
                Advance(); // '"' を消費
                var path = new System.Text.StringBuilder();
                while (!IsAtEnd() && Peek() != '"')
                {
                    path.Append(Advance());
                }

                if (IsAtEnd())
                {
                    AddError("Unterminated assembly reference path");
                    return;
                }

                Advance(); // closing '"' を消費
                AddToken(TokenType.AssemblyRef, path.ToString());
                return;
            }
        }

        AddError("Expected 'r' after '#' for assembly reference directive (#r \"path\")");
    }

    /// <summary>
    /// 文字列リテラルをスキャンします。
    /// エスケープシーケンス: \", \\, \n, \t, \r, \0, \$
    /// </summary>
    private void ScanString()
    {
        int startLine = _line;
        int startColumn = _column - 1; // '"'の位置
        var sb = new StringBuilder();

        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n')
            {
                _line++;
                _column = 0; // Advance()で+1されるので0にする
            }

            if (Peek() == '\\')
            {
                Advance(); // '\' を消費
                if (IsAtEnd())
                {
                    AddError("Unterminated string", startLine, startColumn);
                    return;
                }
                char escaped = Peek();
                switch (escaped)
                {
                    case '"': sb.Append('"'); break;
                    case '\\': sb.Append('\\'); break;
                    case 'n': sb.Append('\n'); break;
                    case 't': sb.Append('\t'); break;
                    case 'r': sb.Append('\r'); break;
                    case '0': sb.Append('\0'); break;
                    case '$': sb.Append('\uE000'); break; // プレースホルダ（Parser段階で$に復元）
                    default:
                        AddError($"Invalid escape sequence: \\{escaped}", _line, _column);
                        break;
                }
                Advance(); // エスケープ文字を消費
            }
            else
            {
                sb.Append(Peek());
                Advance();
            }
        }

        if (IsAtEnd())
        {
            AddError("Unterminated string", startLine, startColumn);
            return;
        }

        // 終端の '"' を消費
        Advance();

        AddToken(TokenType.String, sb.ToString());
    }

    /// <summary>
    /// バッククォート文字列をスキャンします（シェルコマンド用）
    /// </summary>
    private void ScanBacktickString()
    {
        int startLine = _line;
        int startColumn = _column - 1; // '`'の位置

        while (Peek() != '`' && !IsAtEnd())
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
            AddError("Unterminated backtick string", startLine, startColumn);
            return;
        }

        // 終端の '`' を消費
        Advance();

        // バッククォートを除いた文字列値を取得
        string value = _source.Substring(_start + 1, _current - _start - 2);
        AddToken(TokenType.Backtick, value);
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
