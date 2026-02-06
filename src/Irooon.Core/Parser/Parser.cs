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

            // 文をパース（関数定義、クラス定義、変数宣言）
            if (isFunctionDef || Check(TokenType.Class) || Check(TokenType.Let) || Check(TokenType.Var) || Check(TokenType.While) || Check(TokenType.Return))
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
    /// プライマリ式（リテラル、識別子、括弧式、if式、ブロック式、ラムダ式）をパースします。
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

        // ブロック式
        if (Match(TokenType.LeftBrace))
        {
            return BlockExpression();
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

            // 文の場合（fn, class, while, return, let, var）
            if (isFunctionDef || Check(TokenType.Class) || Check(TokenType.While) || Check(TokenType.Return) || Check(TokenType.Let) || Check(TokenType.Var) || Check(TokenType.While) || Check(TokenType.Return))
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

    #endregion

    #region 文のパース

    /// <summary>
    /// 文をパースします。
    /// </summary>
    private Statement Statement()
    {
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

        if (Match(TokenType.While))
        {
            return WhileStatement();
        }

        if (Match(TokenType.Return))
        {
            return ReturnStatement();
        }

        return ExpressionStatement();
    }

    /// <summary>
    /// while文をパースします。
    /// while (cond) { body }
    /// </summary>
    private Statement WhileStatement()
    {
        var whileToken = Previous();

        // 条件式をパース
        Consume(TokenType.LeftParen, "Expect '(' after 'while'.");
        var condition = Expression();
        Consume(TokenType.RightParen, "Expect ')' after while condition.");

        // body をパース（必ずブロック）
        Consume(TokenType.LeftBrace, "Expect '{' after while condition.");
        var bodyExpr = BlockExpression();

        // BlockExpr を ExprStmt でラップ
        var body = new ExprStmt(bodyExpr, bodyExpr.Line, bodyExpr.Column);

        return new WhileStmt(condition, body, whileToken.Line, whileToken.Column);
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
    /// </summary>
    private ClassDef ClassDefinition()
    {
        var classToken = Previous();
        var name = Consume(TokenType.Identifier, "Expect class name.");

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
                if (Check(TokenType.Var) || Check(TokenType.While) || Check(TokenType.Return))
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

        return new ClassDef(name.Lexeme, fields, methods, classToken.Line, classToken.Column);
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
}
