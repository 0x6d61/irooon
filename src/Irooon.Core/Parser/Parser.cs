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
            // fn の後が ( ならラムダ式（式として扱う）
            bool isFunctionDef = Check(TokenType.Fn) && PeekNext().Type != TokenType.LeftParen;

            // 文をパース（関数定義、クラス定義、変数宣言、モジュール）
            if (isFunctionDef || Check(TokenType.Class) || Check(TokenType.Let) || Check(TokenType.Var) || Check(TokenType.For) || Check(TokenType.Foreach) || Check(TokenType.Break) || Check(TokenType.Continue) || Check(TokenType.Return) || Check(TokenType.Throw) || Check(TokenType.Export) || Check(TokenType.Import))
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
            else if (expr is IndexExpr indexExpr)
            {
                // arr[0] = value
                return new IndexAssignExpr(indexExpr.Target, indexExpr.Index, value, equals.Line, equals.Column);
            }
            else if (expr is MemberExpr memberExpr)
            {
                // obj.field = value
                return new MemberAssignExpr(memberExpr.Target, memberExpr.Name, value, equals.Line, equals.Column);
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
        var expr = Range();

        while (Match(TokenType.Less, TokenType.LessEqual, TokenType.Greater, TokenType.GreaterEqual))
        {
            var op = Previous();
            var right = Range();
            expr = new BinaryExpr(expr, op.Type, right, op.Line, op.Column);
        }

        return expr;
    }

    /// <summary>
    /// 範囲リテラル（.., ...）をパースします。
    /// </summary>
    private Expression Range()
    {
        var expr = Term();

        if (Match(TokenType.DotDot, TokenType.DotDotDot))
        {
            var op = Previous();
            var right = Term();
            bool inclusive = op.Type == TokenType.DotDotDot;
            expr = new RangeExpr(expr, right, inclusive, op.Line, op.Column);
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
    /// 識別子が大文字で始まる場合は NewExpr として扱います。
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

        // 識別子が大文字で始まる場合は NewExpr として扱う
        if (callee is IdentifierExpr identExpr && char.IsUpper(identExpr.Name[0]))
        {
            return new NewExpr(identExpr.Name, arguments, paren.Line, paren.Column);
        }

        return new CallExpr(callee, arguments, paren.Line, paren.Column);
    }

    /// <summary>
    /// プライマリ式（リテラル、識別子、括弧式、if式、ブロック式、ラムダ式、リスト、ハッシュ）をパースします。
    /// </summary>
    private Expression Primary()
    {
        // ラムダ式: fn (params) { body }
        if (Match(TokenType.Fn))
        {
            // 次が ( ならラムダ式
            if (Check(TokenType.LeftParen))
            {
                return LambdaExpression();
            }
            else
            {
                // fnの後に識別子がない場合はエラー（関数定義は式ではなく文）
                throw Error(Peek(), "Expect '(' for lambda expression.");
            }
        }

        // if式
        if (Match(TokenType.If))
        {
            return IfExpression();
        }

        // try/catch/finally式
        if (Match(TokenType.Try))
        {
            return TryExpression();
        }

        // リストリテラル: [...]
        if (Match(TokenType.LeftBracket))
        {
            return ListLiteral();
        }

        // ブロック式またはハッシュリテラル: {...}
        if (Match(TokenType.LeftBrace))
        {
            return BlockOrHashExpression();
        }

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
            return ParseStringLiteral(token.Value as string ?? "", token.Line, token.Column);
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

    /// <summary>
    /// ブロック式をパースします。
    /// { stmt* expr? }
    /// </summary>
    private BlockExpr BlockExpression()
    {
        var leftBrace = Previous();
        var statements = new List<Statement>();
        Expression? finalExpr = null;

        // 改行をスキップ
        while (Match(TokenType.Newline)) { }

        while (!Check(TokenType.RightBrace) && !IsAtEnd())
        {
            // fn の後が ( ならラムダ式（式として扱う）
            bool isFunctionDef = Check(TokenType.Fn) && PeekNext().Type != TokenType.LeftParen;

            // 文の場合（fn, class, for, foreach, break, continue, return, throw, let, var）
            if (isFunctionDef || Check(TokenType.Class) || Check(TokenType.For) || Check(TokenType.Foreach) || Check(TokenType.Break) || Check(TokenType.Continue) || Check(TokenType.Return) || Check(TokenType.Throw) || Check(TokenType.Let) || Check(TokenType.Var))
            {
                statements.Add(Statement());
                // 文の後の改行をスキップ
                while (Match(TokenType.Newline)) { }
            }
            else
            {
                // 式をパース
                var expr = Expression();

                // 次が } または EOF なら、これが最後の式
                if (Check(TokenType.RightBrace) || IsAtEnd())
                {
                    finalExpr = expr;
                    break;
                }

                // 次が改行なら、その後を確認
                if (Check(TokenType.Newline))
                {
                    while (Match(TokenType.Newline)) { }

                    // 改行の後が } なら、これが最後の式
                    if (Check(TokenType.RightBrace) || IsAtEnd())
                    {
                        finalExpr = expr;
                        break;
                    }

                    // 改行の後も続く場合は、この式を式文として扱う
                    statements.Add(new ExprStmt(expr, expr.Line, expr.Column));
                }
                else
                {
                    // 次も式が続く場合は、この式を式文として扱う
                    statements.Add(new ExprStmt(expr, expr.Line, expr.Column));
                }
            }
        }

        Consume(TokenType.RightBrace, "Expect '}' after block.");

        return new BlockExpr(statements, finalExpr, leftBrace.Line, leftBrace.Column);
    }

    /// <summary>
    /// if式をパースします。
    /// if (cond) { thenExpr } else { elseExpr }
    /// </summary>
    private IfExpr IfExpression()
    {
        var ifToken = Previous();

        // 条件式をパース
        Consume(TokenType.LeftParen, "Expect '(' after 'if'.");
        var condition = Expression();
        Consume(TokenType.RightParen, "Expect ')' after if condition.");

        // then ブランチをパース（必ずブロック）
        Consume(TokenType.LeftBrace, "Expect '{' after if condition.");
        var thenBranch = BlockExpression();

        // 改行をスキップ
        while (Match(TokenType.Newline)) { }

        // else は必須
        Consume(TokenType.Else, "Expect 'else' after if then branch.");

        // else ブランチをパース（必ずブロック）
        Consume(TokenType.LeftBrace, "Expect '{' after 'else'.");
        var elseBranch = BlockExpression();

        return new IfExpr(condition, thenBranch, elseBranch, ifToken.Line, ifToken.Column);
    }

    /// <summary>
    /// try/catch/finally式をパースします。
    /// try { body } catch (e) { handler } finally { cleanup }
    /// </summary>
    private TryExpr TryExpression()
    {
        var tryToken = Previous();

        // try ブロックをパース（必ずブロック）
        Consume(TokenType.LeftBrace, "Expect '{' after 'try'.");
        var tryBody = BlockExpression();

        // 改行をスキップ
        while (Match(TokenType.Newline)) { }

        CatchClause? catchClause = null;
        Expression? finallyBody = null;

        // catch 句（オプション）
        if (Match(TokenType.Catch))
        {
            string? exceptionVariable = null;

            // ( がある場合、例外変数を取得
            if (Match(TokenType.LeftParen))
            {
                var varToken = Consume(TokenType.Identifier, "Expect exception variable name.");
                exceptionVariable = varToken.Lexeme;
                Consume(TokenType.RightParen, "Expect ')' after exception variable.");
            }

            // catch ブロックをパース
            Consume(TokenType.LeftBrace, "Expect '{' after 'catch'.");
            var catchBody = BlockExpression();

            catchClause = new CatchClause(exceptionVariable, catchBody);

            // 改行をスキップ
            while (Match(TokenType.Newline)) { }
        }

        // finally 句（オプション）
        if (Match(TokenType.Finally))
        {
            Consume(TokenType.LeftBrace, "Expect '{' after 'finally'.");
            finallyBody = BlockExpression();
        }

        // catch または finally のどちらかは必須
        if (catchClause == null && finallyBody == null)
        {
            throw Error(tryToken, "Expect 'catch' or 'finally' after try block.");
        }

        return new TryExpr(tryBody, catchClause, finallyBody, tryToken.Line, tryToken.Column);
    }

    /// <summary>
    /// リストリテラルをパースします。
    /// [elem1, elem2, ...]
    /// </summary>
    private ListExpr ListLiteral()
    {
        var leftBracket = Previous();
        var elements = new List<Expression>();

        // 改行をスキップ
        while (Match(TokenType.Newline)) { }

        if (!Check(TokenType.RightBracket))
        {
            do
            {
                // 改行をスキップ
                while (Match(TokenType.Newline)) { }

                elements.Add(Expression());

                // 改行をスキップ
                while (Match(TokenType.Newline)) { }
            } while (Match(TokenType.Comma));
        }

        Consume(TokenType.RightBracket, "Expect ']' after list elements.");

        return new ListExpr(elements, leftBracket.Line, leftBracket.Column);
    }

    /// <summary>
    /// ブロック式またはハッシュリテラルをパースします。
    /// ブロック式: { stmt* expr? }
    /// ハッシュリテラル: {key1: value1, key2: value2, ...}
    ///
    /// 最初の要素を見て判定:
    /// - 空 {} → ブロック式
    /// - identifier : → ハッシュリテラル
    /// - それ以外 → ブロック式
    /// </summary>
    private Expression BlockOrHashExpression()
    {
        var leftBrace = Previous();

        // 改行をスキップ
        while (Match(TokenType.Newline)) { }

        // 空の {} はブロック式
        if (Check(TokenType.RightBrace))
        {
            Advance();
            return new BlockExpr(new List<Statement>(), null, leftBrace.Line, leftBrace.Column);
        }

        // 最初がidentifierで、その次が : ならハッシュリテラル
        if (Check(TokenType.Identifier) && PeekNext().Type == TokenType.Colon)
        {
            return HashLiteral(leftBrace);
        }

        // それ以外はブロック式
        return BlockExpression();
    }

    /// <summary>
    /// ハッシュリテラルをパースします（先頭の { は既に消費済み）。
    /// {key1: value1, key2: value2, ...}
    /// </summary>
    private HashExpr HashLiteral(Token leftBrace)
    {
        var pairs = new List<HashExpr.KeyValuePair>();

        if (!Check(TokenType.RightBrace))
        {
            do
            {
                // 改行をスキップ
                while (Match(TokenType.Newline)) { }

                // キーをパース（識別子のみ）
                var key = Consume(TokenType.Identifier, "Expect identifier for hash key.");

                // : を消費
                Consume(TokenType.Colon, "Expect ':' after hash key.");

                // 値をパース
                var value = Expression();

                pairs.Add(new HashExpr.KeyValuePair(key.Lexeme, value));

                // 改行をスキップ
                while (Match(TokenType.Newline)) { }
            } while (Match(TokenType.Comma));
        }

        Consume(TokenType.RightBrace, "Expect '}' after hash pairs.");

        return new HashExpr(pairs, leftBrace.Line, leftBrace.Column);
    }

    #endregion

    #region 文のパース

    /// <summary>
    /// 文をパースします。
    /// </summary>
    private Statement Statement()
    {
        if (Match(TokenType.Export))
        {
            return ExportStatement();
        }

        if (Match(TokenType.Import))
        {
            return ImportStatement();
        }

        if (Match(TokenType.Fn))
        {
            return FunctionDefinition();
        }

        if (Match(TokenType.Class))
        {
            return ClassDefinition();
        }

        if (Match(TokenType.Let))
        {
            return LetStatement();
        }

        if (Match(TokenType.Var))
        {
            return VarStatement();
        }

        if (Match(TokenType.For))
        {
            return ForStatement();
        }

        if (Match(TokenType.Foreach))
        {
            return ForeachStatement();
        }

        if (Match(TokenType.Break))
        {
            return BreakStatement();
        }

        if (Match(TokenType.Continue))
        {
            return ContinueStatement();
        }

        if (Match(TokenType.Return))
        {
            return ReturnStatement();
        }

        if (Match(TokenType.Throw))
        {
            return ThrowStatement();
        }

        return ExpressionStatement();
    }

    /// <summary>
    /// for文をパースします。
    /// パターン1: for (item in collection) { body } - コレクション反復
    /// パターン2: for (condition) { body } - 条件ループ
    /// </summary>
    private Statement ForStatement()
    {
        var forToken = Previous();

        // ( を期待
        Consume(TokenType.LeftParen, "Expect '(' after 'for'.");

        // 最初のトークンを確認
        // - 識別子 + in → コレクション反復
        // - それ以外 → 条件ループ
        if (Check(TokenType.Identifier))
        {
            var lookahead = Peek();
            var nextToken = _current + 1 < _tokens.Count ? _tokens[_current + 1] : null;

            // 次のトークンが 'in' かどうかチェック
            if (nextToken != null && nextToken.Type == TokenType.In)
            {
                // パターン1: for (item in collection) { body }
                var variable = Consume(TokenType.Identifier, "Expect variable name in for loop.");
                Consume(TokenType.In, "Expect 'in' after variable name.");
                var collection = Expression();
                Consume(TokenType.RightParen, "Expect ')' after collection.");
                Consume(TokenType.LeftBrace, "Expect '{' after for header.");
                var bodyExpr = BlockExpression();

                return new ForStmt(variable.Lexeme, collection, bodyExpr, forToken.Line, forToken.Column);
            }
        }

        // パターン2: for (condition) { body }
        var condition = Expression();
        Consume(TokenType.RightParen, "Expect ')' after condition.");
        Consume(TokenType.LeftBrace, "Expect '{' after for header.");
        var condBodyExpr = BlockExpression();

        return new ForStmt(condition, condBodyExpr, forToken.Line, forToken.Column);
    }

    /// <summary>
    /// foreach文をパースします。
    /// foreach (item in collection) { body }
    /// </summary>
    private Statement ForeachStatement()
    {
        var foreachToken = Previous();

        // ( を期待
        Consume(TokenType.LeftParen, "Expect '(' after 'foreach'.");

        // ループ変数名をパース
        var variable = Consume(TokenType.Identifier, "Expect variable name in foreach.");

        // in を期待
        Consume(TokenType.In, "Expect 'in' after variable name.");

        // コレクション式をパース
        var collection = Expression();

        // ) を期待
        Consume(TokenType.RightParen, "Expect ')' after foreach collection.");

        // body をパース（必ずブロック）
        Consume(TokenType.LeftBrace, "Expect '{' after foreach header.");
        var bodyExpr = BlockExpression();

        // BlockExpr を ExprStmt でラップ
        var body = new ExprStmt(bodyExpr, bodyExpr.Line, bodyExpr.Column);

        return new ForeachStmt(variable.Lexeme, collection, body, foreachToken.Line, foreachToken.Column);
    }

    /// <summary>
    /// break文をパースします。
    /// </summary>
    private Statement BreakStatement()
    {
        var breakToken = Previous();
        return new BreakStmt(breakToken.Line, breakToken.Column);
    }

    /// <summary>
    /// continue文をパースします。
    /// </summary>
    private Statement ContinueStatement()
    {
        var continueToken = Previous();
        return new ContinueStmt(continueToken.Line, continueToken.Column);
    }

    /// <summary>
    /// return文をパースします。
    /// return expr?
    /// </summary>
    private Statement ReturnStatement()
    {
        var returnToken = Previous();
        Expression? value = null;

        // 改行、EOF、または } の前でない場合は、式をパース
        if (!Check(TokenType.Newline) && !IsAtEnd() && !Check(TokenType.RightBrace))
        {
            value = Expression();
        }

        return new ReturnStmt(value, returnToken.Line, returnToken.Column);
    }

    /// <summary>
    /// throw文をパースします。
    /// </summary>
    private Statement ThrowStatement()
    {
        var throwToken = Previous();

        // 改行、EOF、または } の前でない場合は、式をパース（必須）
        if (Check(TokenType.Newline) || IsAtEnd() || Check(TokenType.RightBrace))
        {
            throw Error(throwToken, "Expect expression after 'throw'.");
        }

        var value = Expression();
        return new ThrowStmt(value, throwToken.Line, throwToken.Column);
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

    #region 関数・クラスのパース

    /// <summary>
    /// 関数定義をパースします。
    /// fn name(params) { body }
    /// </summary>
    private FunctionDef FunctionDefinition()
    {
        var fnToken = Previous();
        var name = Consume(TokenType.Identifier, "Expect function name.");

        // パラメータリストをパース
        var parameters = Parameters();

        // body をパース（必ずブロック）
        Consume(TokenType.LeftBrace, "Expect '{' before function body.");
        var body = BlockExpression();

        return new FunctionDef(name.Lexeme, parameters, body, fnToken.Line, fnToken.Column);
    }

    /// <summary>
    /// ラムダ式をパースします。
    /// fn (params) { body }
    /// </summary>
    private LambdaExpr LambdaExpression()
    {
        var fnToken = Previous();

        // パラメータリストをパース
        var parameters = Parameters();

        // body をパース（必ずブロック）
        Consume(TokenType.LeftBrace, "Expect '{' before lambda body.");
        var body = BlockExpression();

        return new LambdaExpr(parameters, body, fnToken.Line, fnToken.Column);
    }

    /// <summary>
    /// パラメータリストをパースします（関数定義・ラムダ式・メソッド定義共通）。
    /// (param1, param2, ...)
    /// </summary>
    private List<Parameter> Parameters()
    {
        Consume(TokenType.LeftParen, "Expect '(' before parameters.");

        var parameters = new List<Parameter>();

        if (!Check(TokenType.RightParen))
        {
            do
            {
                var paramToken = Consume(TokenType.Identifier, "Expect parameter name.");
                parameters.Add(new Parameter(paramToken.Lexeme, paramToken.Line, paramToken.Column));
            } while (Match(TokenType.Comma));
        }

        Consume(TokenType.RightParen, "Expect ')' after parameters.");

        return parameters;
    }

    /// <summary>
    /// クラス定義をパースします。
    /// class Name { fields methods }
    /// class ChildName : ParentName { fields methods }
    /// </summary>
    private ClassDef ClassDefinition()
    {
        var classToken = Previous();
        var name = Consume(TokenType.Identifier, "Expect class name.");

        // 継承構文をチェック（: ParentClass）
        string? parentClass = null;
        if (Match(TokenType.Colon))
        {
            var parentToken = Consume(TokenType.Identifier, "Expect parent class name after ':'.");
            parentClass = parentToken.Lexeme;
        }

        Consume(TokenType.LeftBrace, "Expect '{' before class body.");

        var fields = new List<FieldDef>();
        var methods = new List<MethodDef>();

        // 改行をスキップ
        while (Match(TokenType.Newline)) { }

        while (!Check(TokenType.RightBrace) && !IsAtEnd())
        {
            // フィールドまたはメソッドをパース
            if (Check(TokenType.Var) || Check(TokenType.Public) || Check(TokenType.Private) || Check(TokenType.Static) || Check(TokenType.Fn) || Check(TokenType.Init))
            {
                // 修飾子を先読み
                bool isPublic = false;
                bool isStatic = false;

                if (Match(TokenType.Public))
                {
                    isPublic = true;
                }
                else if (Match(TokenType.Private))
                {
                    isPublic = false;
                }

                // static修飾子
                if (Match(TokenType.Static))
                {
                    isStatic = true;
                }

                // フィールドかメソッドか判定
                if (Check(TokenType.Var) || Check(TokenType.Return))
                {
                    // フィールド定義
                    fields.Add(FieldDefinition(isPublic));
                }
                else if (Check(TokenType.Fn) || Check(TokenType.Init))
                {
                    // メソッド定義
                    methods.Add(MethodDefinition(isPublic, isStatic));
                }
                else
                {
                    throw Error(Peek(), "Expect field or method definition in class body.");
                }
            }
            else
            {
                throw Error(Peek(), "Expect field or method definition in class body.");
            }

            // 改行をスキップ
            while (Match(TokenType.Newline)) { }
        }

        Consume(TokenType.RightBrace, "Expect '}' after class body.");

        return new ClassDef(name.Lexeme, fields, methods, parentClass, classToken.Line, classToken.Column);
    }

    /// <summary>
    /// フィールド定義をパースします。
    /// [public|private] var name = expr
    /// </summary>
    private FieldDef FieldDefinition(bool isPublic)
    {
        var varToken = Consume(TokenType.Var, "Expect 'var' for field definition.");
        var name = Consume(TokenType.Identifier, "Expect field name.");
        Consume(TokenType.Equal, "Expect '=' after field name.");
        var initializer = Expression();

        return new FieldDef(name.Lexeme, isPublic, initializer, varToken.Line, varToken.Column);
    }

    /// <summary>
    /// メソッド定義をパースします。
    /// [public|private] [static] fn name(params) { body }
    /// または
    /// init(params) { body }
    /// </summary>
    private MethodDef MethodDefinition(bool isPublic, bool isStatic)
    {
        Token methodToken;
        string methodName;

        if (Match(TokenType.Init))
        {
            // init コンストラクタ
            methodToken = Previous();
            methodName = "init";
            isPublic = false; // init は常に private
            isStatic = false; // init は常に非static
        }
        else
        {
            // 通常のメソッド
            Consume(TokenType.Fn, "Expect 'fn' for method definition.");
            methodToken = Previous();
            var nameToken = Consume(TokenType.Identifier, "Expect method name.");
            methodName = nameToken.Lexeme;
        }

        // パラメータリストをパース
        var parameters = Parameters();

        // body をパース（必ずブロック）
        Consume(TokenType.LeftBrace, "Expect '{' before method body.");
        var body = BlockExpression();

        return new MethodDef(methodName, isPublic, isStatic, parameters, body, methodToken.Line, methodToken.Column);
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
    /// 次のトークンを返します（消費しない）。
    /// </summary>
    private Token PeekNext()
    {
        if (_current + 1 >= _tokens.Count)
        {
            return _tokens[_tokens.Count - 1]; // EOF
        }
        return _tokens[_current + 1];
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

    #region 文字列補間のパース

    /// <summary>
    /// 文字列リテラルをパースし、補間が含まれている場合は StringInterpolationExpr を返します。
    /// 補間がない場合は LiteralExpr を返します。
    /// </summary>
    /// <param name="value">文字列の値</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    /// <returns>パースされた式</returns>
    private Expression ParseStringLiteral(string value, int line, int column)
    {
        // ${...} が含まれていない場合は通常のリテラルとして返す
        if (!value.Contains("${"))
        {
            return new LiteralExpr(value, line, column);
        }

        var parts = new List<object>();
        var current = 0;

        while (current < value.Length)
        {
            var start = value.IndexOf("${", current);
            if (start == -1)
            {
                // 残りの文字列を追加
                var remaining = value.Substring(current);
                if (remaining.Length > 0)
                {
                    parts.Add(remaining);
                }
                break;
            }

            // ${ の前の文字列を追加
            if (start > current)
            {
                parts.Add(value.Substring(current, start - current));
            }

            // ${ ... } 内の式をパース
            var end = value.IndexOf("}", start + 2);
            if (end == -1)
            {
                throw new ParseException(new Token(TokenType.String, value, null, line, column), "Unclosed ${ in string interpolation");
            }

            var exprText = value.Substring(start + 2, end - start - 2);

            // 式をパース
            var exprTokens = new Core.Lexer.Lexer(exprText).ScanTokens();
            var exprParser = new Parser(exprTokens);
            var expr = exprParser.Expression();
            parts.Add(expr);

            current = end + 1;
        }

        // パーツが空の場合は空文字列を返す
        if (parts.Count == 0)
        {
            return new LiteralExpr("", line, column);
        }

        // パーツが1つで文字列の場合は通常のリテラルとして返す
        if (parts.Count == 1 && parts[0] is string)
        {
            return new LiteralExpr(parts[0], line, column);
        }

        return new StringInterpolationExpr(parts, line, column);
    }

    #endregion

    #region モジュールシステム

    /// <summary>
    /// export文をパースします。
    /// export let x = 10
    /// export fn add(a, b) { ... }
    /// </summary>
    private Statement ExportStatement()
    {
        var exportToken = Previous();

        // exportの後には let または fn が続く必要がある
        if (Match(TokenType.Let))
        {
            var letStmt = LetStatement();
            return new ExportStmt(letStmt, exportToken.Line, exportToken.Column);
        }

        if (Match(TokenType.Fn))
        {
            var funcDef = FunctionDefinition();
            return new ExportStmt(funcDef, exportToken.Line, exportToken.Column);
        }

        throw new ParseException(exportToken, "Expected 'let' or 'fn' after 'export'.");
    }

    /// <summary>
    /// import文をパースします。
    /// import {name1, name2, ...} from "path"
    /// </summary>
    private Statement ImportStatement()
    {
        var importToken = Previous();

        // { を期待
        Consume(TokenType.LeftBrace, "Expect '{' after 'import'.");

        // インポートする名前のリストをパース
        var names = new List<string>();
        if (!Check(TokenType.RightBrace))
        {
            do
            {
                var name = Consume(TokenType.Identifier, "Expect identifier in import list.");
                names.Add(name.Lexeme);
            } while (Match(TokenType.Comma));
        }

        // } を期待
        Consume(TokenType.RightBrace, "Expect '}' after import list.");

        // from を期待
        Consume(TokenType.From, "Expect 'from' after import list.");

        // モジュールパス（文字列）を期待
        var pathToken = Consume(TokenType.String, "Expect module path (string) after 'from'.");
        var modulePath = (string)pathToken.Value!;

        return new ImportStmt(names, modulePath, importToken.Line, importToken.Column);
    }

    #endregion
}
