using Irooon.Core.Ast;
using Irooon.Core.Ast.Expressions;
using Irooon.Core.Ast.Statements;
using Irooon.Core.Lexer;

namespace Irooon.Core.Parser;

/// <summary>
/// irooon言語のパーサー。トークン列からASTを構築します。
/// </summary>
public class Parser
{
    private readonly List<Token> _tokens;
    private int _current = 0;

    /// <summary>
    /// Parserの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="tokens">トークンのリスト</param>
    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    /// <summary>
    /// トークン列をパースしてASTを構築します。
    /// トップレベルはBlockExprとして返されます。
    /// </summary>
    /// <returns>パースされたBlockExpr</returns>
    public BlockExpr Parse()
    {
        var statements = new List<Statement>();
        Expression? finalExpr = null;
        var line = 1;
        var column = 1;

        if (!IsAtEnd())
        {
            line = Peek().Line;
            column = Peek().Column;
        }

        // 改行をスキップ
        while (Match(TokenType.Newline)) { }

        while (!IsAtEnd())
        {
            // 文をパース
            if (Check(TokenType.Let) || Check(TokenType.Var))
            {
                statements.Add(Statement());
                // 文の後の改行をスキップ
                while (Match(TokenType.Newline)) { }
            }
            else
            {
                // 式をパース
                var expr = Expression();

                // 次がEOFまたは改行なら、これが最後の式
                if (IsAtEnd() || Check(TokenType.Newline))
                {
                    finalExpr = expr;
                    break;
                }

                // 次も式が続く場合は、この式を式文として扱う
                statements.Add(new ExprStmt(expr, expr.Line, expr.Column));
                // 改行をスキップ
                while (Match(TokenType.Newline)) { }
            }
        }

        return new BlockExpr(statements, finalExpr, line, column);
    }

    #region 式のパース（優先順位順）

    /// <summary>
    /// 式をパースします（最も低い優先順位から開始）。
    /// </summary>
    private Expression Expression()
    {
        return Assignment();
    }

    /// <summary>
    /// 代入式をパースします。
    /// </summary>
    private Expression Assignment()
    {
        var expr = LogicalOr();

        if (Match(TokenType.Equal))
        {
            var equals = Previous();
            var value = Assignment(); // 右結合

            if (expr is IdentifierExpr identExpr)
            {
                return new AssignExpr(identExpr.Name, value, equals.Line, equals.Column);
            }

            throw Error(equals, "Invalid assignment target.");
        }

        return expr;
    }

    /// <summary>
    /// 論理和（or）をパースします。
    /// </summary>
    private Expression LogicalOr()
    {
        var expr = LogicalAnd();

        while (Match(TokenType.Or))
        {
            var op = Previous();
            var right = LogicalAnd();
            expr = new BinaryExpr(expr, op.Type, right, op.Line, op.Column);
        }

        return expr;
    }

    /// <summary>
    /// 論理積（and）をパースします。
    /// </summary>
    private Expression LogicalAnd()
    {
        var expr = Equality();

        while (Match(TokenType.And))
        {
            var op = Previous();
            var right = Equality();
            expr = new BinaryExpr(expr, op.Type, right, op.Line, op.Column);
        }

        return expr;
    }

    /// <summary>
    /// 等値比較（==, !=）をパースします。
    /// </summary>
    private Expression Equality()
    {
        var expr = Comparison();

        while (Match(TokenType.EqualEqual, TokenType.BangEqual))
        {
            var op = Previous();
            var right = Comparison();
            expr = new BinaryExpr(expr, op.Type, right, op.Line, op.Column);
        }

        return expr;
    }

    /// <summary>
    /// 大小比較（&lt;, &lt;=, &gt;, &gt;=）をパースします。
    /// </summary>
    private Expression Comparison()
    {
        var expr = Term();

        while (Match(TokenType.Less, TokenType.LessEqual, TokenType.Greater, TokenType.GreaterEqual))
        {
            var op = Previous();
            var right = Term();
            expr = new BinaryExpr(expr, op.Type, right, op.Line, op.Column);
        }

        return expr;
    }

    /// <summary>
    /// 加減算（+, -）をパースします。
    /// </summary>
    private Expression Term()
    {
        var expr = Factor();

        while (Match(TokenType.Plus, TokenType.Minus))
        {
            var op = Previous();
            var right = Factor();
            expr = new BinaryExpr(expr, op.Type, right, op.Line, op.Column);
        }

        return expr;
    }

    /// <summary>
    /// 乗除算（*, /, %）をパースします。
    /// </summary>
    private Expression Factor()
    {
        var expr = Unary();

        while (Match(TokenType.Star, TokenType.Slash, TokenType.Percent))
        {
            var op = Previous();
            var right = Unary();
            expr = new BinaryExpr(expr, op.Type, right, op.Line, op.Column);
        }

        return expr;
    }

    /// <summary>
    /// 単項演算子（-, not）をパースします。
    /// </summary>
    private Expression Unary()
    {
        if (Match(TokenType.Minus, TokenType.Not))
        {
            var op = Previous();
            var right = Unary();
            return new UnaryExpr(op.Type, right, op.Line, op.Column);
        }

        return Call();
    }

    /// <summary>
    /// 関数呼び出し、メンバアクセス、インデックスアクセスをパースします。
    /// </summary>
    private Expression Call()
    {
        var expr = Primary();

        while (true)
        {
            if (Match(TokenType.LeftParen))
            {
                // 関数呼び出し
                expr = FinishCall(expr);
            }
            else if (Match(TokenType.Dot))
            {
                // メンバアクセス
                var dot = Previous();
                var name = Consume(TokenType.Identifier, "Expect property name after '.'.");
                expr = new MemberExpr(expr, name.Lexeme, dot.Line, dot.Column);
            }
            else if (Match(TokenType.LeftBracket))
            {
                // インデックスアクセス
                var bracket = Previous();
                var index = Expression();
                Consume(TokenType.RightBracket, "Expect ']' after index.");
                expr = new IndexExpr(expr, index, bracket.Line, bracket.Column);
            }
            else
            {
                break;
            }
        }

        return expr;
    }

    /// <summary>
    /// 関数呼び出しの引数リストをパースします。
    /// </summary>
    private Expression FinishCall(Expression callee)
    {
        var arguments = new List<Expression>();
        var paren = Previous();

        if (!Check(TokenType.RightParen))
        {
            do
            {
                arguments.Add(Expression());
            } while (Match(TokenType.Comma));
        }

        Consume(TokenType.RightParen, "Expect ')' after arguments.");

        return new CallExpr(callee, arguments, paren.Line, paren.Column);
    }

    /// <summary>
    /// プライマリ式（リテラル、識別子、括弧式）をパースします。
    /// </summary>
    private Expression Primary()
    {
        // true
        if (Match(TokenType.True))
        {
            var token = Previous();
            return new LiteralExpr(true, token.Line, token.Column);
        }

        // false
        if (Match(TokenType.False))
        {
            var token = Previous();
            return new LiteralExpr(false, token.Line, token.Column);
        }

        // null
        if (Match(TokenType.Null))
        {
            var token = Previous();
            return new LiteralExpr(null, token.Line, token.Column);
        }

        // 数値リテラル
        if (Match(TokenType.Number))
        {
            var token = Previous();
            return new LiteralExpr(token.Value, token.Line, token.Column);
        }

        // 文字列リテラル
        if (Match(TokenType.String))
        {
            var token = Previous();
            return new LiteralExpr(token.Value, token.Line, token.Column);
        }

        // 識別子
        if (Match(TokenType.Identifier))
        {
            var token = Previous();
            return new IdentifierExpr(token.Lexeme, token.Line, token.Column);
        }

        // 括弧式
        if (Match(TokenType.LeftParen))
        {
            var expr = Expression();
            Consume(TokenType.RightParen, "Expect ')' after expression.");
            return expr;
        }

        throw Error(Peek(), "Expect expression.");
    }

    #endregion

    #region 文のパース

    /// <summary>
    /// 文をパースします。
    /// </summary>
    private Statement Statement()
    {
        if (Match(TokenType.Let))
        {
            return LetStatement();
        }

        if (Match(TokenType.Var))
        {
            return VarStatement();
        }

        return ExpressionStatement();
    }

    /// <summary>
    /// let文をパースします。
    /// </summary>
    private Statement LetStatement()
    {
        var keyword = Previous();
        var name = Consume(TokenType.Identifier, "Expect variable name.");
        Consume(TokenType.Equal, "Expect '=' after variable name.");
        var initializer = Expression();

        return new LetStmt(name.Lexeme, initializer, keyword.Line, keyword.Column);
    }

    /// <summary>
    /// var文をパースします。
    /// </summary>
    private Statement VarStatement()
    {
        var keyword = Previous();
        var name = Consume(TokenType.Identifier, "Expect variable name.");
        Consume(TokenType.Equal, "Expect '=' after variable name.");
        var initializer = Expression();

        return new VarStmt(name.Lexeme, initializer, keyword.Line, keyword.Column);
    }

    /// <summary>
    /// 式文をパースします。
    /// </summary>
    private Statement ExpressionStatement()
    {
        var expr = Expression();
        return new ExprStmt(expr, expr.Line, expr.Column);
    }

    #endregion

    #region ヘルパーメソッド

    /// <summary>
    /// 現在のトークンを消費して次に進みます。
    /// </summary>
    private Token Advance()
    {
        if (!IsAtEnd())
        {
            _current++;
        }
        return Previous();
    }

    /// <summary>
    /// 現在のトークンを返します（消費しない）。
    /// </summary>
    private Token Peek()
    {
        return _tokens[_current];
    }

    /// <summary>
    /// 直前のトークンを返します。
    /// </summary>
    private Token Previous()
    {
        return _tokens[_current - 1];
    }

    /// <summary>
    /// トークン列の終端に達したかどうかを確認します。
    /// </summary>
    private bool IsAtEnd()
    {
        return Peek().Type == TokenType.Eof;
    }

    /// <summary>
    /// 現在のトークンが指定された型と一致するかを確認します。
    /// </summary>
    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }

    /// <summary>
    /// 現在のトークンが指定された型のいずれかと一致する場合、消費して進みます。
    /// </summary>
    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 現在のトークンが指定された型と一致することを要求します。
    /// 一致しない場合はエラーを投げます。
    /// </summary>
    private Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();

        throw Error(Peek(), message);
    }

    /// <summary>
    /// パースエラーを生成します。
    /// </summary>
    private ParseException Error(Token token, string message)
    {
        return new ParseException(token, message);
    }

    #endregion
}
