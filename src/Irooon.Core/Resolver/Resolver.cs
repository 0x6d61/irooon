using Irooon.Core.Ast;
using Irooon.Core.Ast.Expressions;
using Irooon.Core.Ast.Statements;

namespace Irooon.Core.Resolver;

/// <summary>
/// スコープ解析を行うResolverです。
/// 変数の宣言・参照解決、let の再代入禁止チェック、未定義変数の検出などを行います。
/// </summary>
public class Resolver
{
    private Scope _currentScope;
    private readonly List<ResolveException> _errors = new();

    /// <summary>
    /// Resolverの新しいインスタンスを初期化します。
    /// </summary>
    public Resolver()
    {
        _currentScope = new Scope(null); // グローバルスコープ

        // ビルトイン関数を宣言
        RegisterBuiltins();
    }

    /// <summary>
    /// ビルトイン関数をグローバルスコープに宣言します。
    /// </summary>
    private void RegisterBuiltins()
    {
        _currentScope.Define("print", new VariableInfo("print", isReadOnly: true, _currentScope.Depth));
        _currentScope.Define("println", new VariableInfo("println", isReadOnly: true, _currentScope.Depth));
    }

    /// <summary>
    /// 外部から変数を登録します（REPL用）。
    /// </summary>
    /// <param name="name">変数名</param>
    /// <param name="isReadOnly">読み取り専用かどうか（デフォルト: false）</param>
    public void RegisterVariable(string name, bool isReadOnly = false)
    {
        if (!_currentScope.IsDefined(name))
        {
            _currentScope.Define(name, new VariableInfo(name, isReadOnly, _currentScope.Depth));
        }
    }

    /// <summary>
    /// プログラム全体を解析します。
    /// </summary>
    /// <param name="program">プログラムのAST</param>
    public void Resolve(BlockExpr program)
    {
        ResolveBlockExpr(program);
    }

    /// <summary>
    /// エラーのリストを取得します。
    /// </summary>
    /// <returns>エラーのリスト</returns>
    public List<ResolveException> GetErrors() => _errors;

    #region スコープ管理

    /// <summary>
    /// 新しいスコープを開始します。
    /// </summary>
    private void BeginScope()
    {
        _currentScope = new Scope(_currentScope);
    }

    /// <summary>
    /// 現在のスコープを終了します。
    /// </summary>
    private void EndScope()
    {
        if (_currentScope.Parent != null)
        {
            _currentScope = _currentScope.Parent;
        }
    }

    #endregion

    #region 変数管理

    /// <summary>
    /// 変数を宣言します。
    /// </summary>
    /// <param name="name">変数名</param>
    /// <param name="isReadOnly">読み取り専用かどうか</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    private void Declare(string name, bool isReadOnly, int line, int column)
    {
        // 同じスコープ内で既に宣言されているかチェック
        if (_currentScope.IsDefined(name))
        {
            _errors.Add(new ResolveException(
                $"Variable '{name}' is already declared in this scope",
                line, column));
            return;
        }

        var info = new VariableInfo(name, isReadOnly, _currentScope.Depth);
        _currentScope.Define(name, info);
    }

    /// <summary>
    /// 変数を解決します（参照）。
    /// </summary>
    /// <param name="name">変数名</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    /// <returns>変数情報（見つからない場合はnull）</returns>
    private VariableInfo? ResolveVariable(string name, int line, int column)
    {
        var varInfo = _currentScope.Resolve(name);
        if (varInfo == null)
        {
            _errors.Add(new ResolveException(
                $"Undefined variable '{name}'",
                line, column));
            return null;
        }

        varInfo.IsUsed = true;
        return varInfo;
    }

    #endregion

    #region Expression解析

    /// <summary>
    /// Expressionを解析します。
    /// </summary>
    private void ResolveExpression(Expression expr)
    {
        switch (expr)
        {
            case LiteralExpr literalExpr:
                ResolveLiteralExpr(literalExpr);
                break;
            case BinaryExpr binaryExpr:
                ResolveBinaryExpr(binaryExpr);
                break;
            case UnaryExpr unaryExpr:
                ResolveUnaryExpr(unaryExpr);
                break;
            case IdentifierExpr identifierExpr:
                ResolveIdentifierExpr(identifierExpr);
                break;
            case AssignExpr assignExpr:
                ResolveAssignExpr(assignExpr);
                break;
            case CallExpr callExpr:
                ResolveCallExpr(callExpr);
                break;
            case MemberExpr memberExpr:
                ResolveMemberExpr(memberExpr);
                break;
            case IndexExpr indexExpr:
                ResolveIndexExpr(indexExpr);
                break;
            case IfExpr ifExpr:
                ResolveIfExpr(ifExpr);
                break;
            case BlockExpr blockExpr:
                ResolveBlockExpr(blockExpr);
                break;
            case LambdaExpr lambdaExpr:
                ResolveLambdaExpr(lambdaExpr);
                break;
            case NewExpr newExpr:
                ResolveNewExpr(newExpr);
                break;
            case ListExpr listExpr:
                ResolveListExpr(listExpr);
                break;
            case HashExpr hashExpr:
                ResolveHashExpr(hashExpr);
                break;
            case RangeExpr rangeExpr:
                ResolveRangeExpr(rangeExpr);
                break;
            case IndexAssignExpr indexAssignExpr:
                ResolveIndexAssignExpr(indexAssignExpr);
                break;
            case MemberAssignExpr memberAssignExpr:
                ResolveMemberAssignExpr(memberAssignExpr);
                break;
            case StringInterpolationExpr stringInterpolationExpr:
                ResolveStringInterpolationExpr(stringInterpolationExpr);
                break;
            case TryExpr tryExpr:
                ResolveTryExpr(tryExpr);
                break;
            default:
                _errors.Add(new ResolveException(
                    $"Unknown expression type: {expr.GetType().Name}",
                    expr.Line, expr.Column));
                break;
        }
    }

    private void ResolveLiteralExpr(LiteralExpr expr)
    {
        // リテラルは何もしない
    }

    private void ResolveBinaryExpr(BinaryExpr expr)
    {
        ResolveExpression(expr.Left);
        ResolveExpression(expr.Right);
    }

    private void ResolveUnaryExpr(UnaryExpr expr)
    {
        ResolveExpression(expr.Operand);
    }

    private void ResolveIdentifierExpr(IdentifierExpr expr)
    {
        // CLR型名（System で始まる名前）はスキップ
        if (expr.Name.StartsWith("System"))
        {
            return;
        }

        ResolveVariable(expr.Name, expr.Line, expr.Column);
    }

    private void ResolveAssignExpr(AssignExpr expr)
    {
        // 右辺を解析
        ResolveExpression(expr.Value);

        // 左辺の変数を解決
        var varInfo = _currentScope.Resolve(expr.Name);
        if (varInfo == null)
        {
            _errors.Add(new ResolveException(
                $"Undefined variable '{expr.Name}'",
                expr.Line, expr.Column));
            return;
        }

        // let変数への再代入チェック
        if (varInfo.IsReadOnly)
        {
            _errors.Add(new ResolveException(
                $"Cannot assign to 'let' variable '{expr.Name}'",
                expr.Line, expr.Column));
        }

        varInfo.IsUsed = true;
    }

    private void ResolveCallExpr(CallExpr expr)
    {
        ResolveExpression(expr.Callee);
        foreach (var arg in expr.Arguments)
        {
            ResolveExpression(arg);
        }
    }

    private void ResolveMemberExpr(MemberExpr expr)
    {
        ResolveExpression(expr.Target);
    }

    private void ResolveIndexExpr(IndexExpr expr)
    {
        ResolveExpression(expr.Target);
        ResolveExpression(expr.Index);
    }

    private void ResolveIfExpr(IfExpr expr)
    {
        ResolveExpression(expr.Condition);

        // Then節（新しいスコープ）
        BeginScope();
        ResolveExpression(expr.ThenBranch);
        EndScope();

        // Else節（新しいスコープ）
        if (expr.ElseBranch != null)
        {
            BeginScope();
            ResolveExpression(expr.ElseBranch);
            EndScope();
        }
    }

    private void ResolveBlockExpr(BlockExpr expr)
    {
        // ブロックは新しいスコープを作成
        BeginScope();

        foreach (var stmt in expr.Statements)
        {
            ResolveStatement(stmt);
        }

        if (expr.Expression != null)
        {
            ResolveExpression(expr.Expression);
        }

        EndScope();
    }

    private void ResolveLambdaExpr(LambdaExpr expr)
    {
        // ラムダは新しいスコープを作成
        BeginScope();

        // パラメータを宣言
        foreach (var param in expr.Parameters)
        {
            Declare(param.Name, false, param.Line, param.Column);
        }

        // 本体を解析
        ResolveExpression(expr.Body);

        EndScope();
    }

    private void ResolveNewExpr(NewExpr expr)
    {
        // クラス名は解決しない（型システムの範囲外）
        foreach (var arg in expr.Arguments)
        {
            ResolveExpression(arg);
        }
    }

    private void ResolveListExpr(ListExpr expr)
    {
        // リストの各要素を解析
        foreach (var element in expr.Elements)
        {
            ResolveExpression(element);
        }
    }

    private void ResolveHashExpr(HashExpr expr)
    {
        // ハッシュの各値を解析（キーは文字列リテラルなので解析不要）
        foreach (var pair in expr.Pairs)
        {
            ResolveExpression(pair.Value);
        }
    }

    private void ResolveRangeExpr(RangeExpr expr)
    {
        // 範囲の開始と終了を解析
        ResolveExpression(expr.Start);
        ResolveExpression(expr.End);
    }

    private void ResolveIndexAssignExpr(IndexAssignExpr expr)
    {
        // 対象、インデックス、値を解析
        ResolveExpression(expr.Target);
        ResolveExpression(expr.Index);
        ResolveExpression(expr.Value);
    }

    private void ResolveMemberAssignExpr(MemberAssignExpr expr)
    {
        // 対象と値を解析（メンバ名は文字列なので解析不要）
        ResolveExpression(expr.Target);
        ResolveExpression(expr.Value);
    }

    private void ResolveStringInterpolationExpr(StringInterpolationExpr expr)
    {
        // 各パートの式を解析
        foreach (var part in expr.Parts)
        {
            if (part is Expression exprPart)
            {
                ResolveExpression(exprPart);
            }
            // string パートは何もしない
        }
    }

    #endregion

    #region Statement解析

    /// <summary>
    /// Statementを解析します。
    /// </summary>
    private void ResolveStatement(Statement stmt)
    {
        switch (stmt)
        {
            case LetStmt letStmt:
                ResolveLetStmt(letStmt);
                break;
            case VarStmt varStmt:
                ResolveVarStmt(varStmt);
                break;
            case ExprStmt exprStmt:
                ResolveExprStmt(exprStmt);
                break;
            case ReturnStmt returnStmt:
                ResolveReturnStmt(returnStmt);
                break;
            case ForStmt forStmt:
                ResolveForStmt(forStmt);
                break;
            case ForeachStmt foreachStmt:
                ResolveForeachStmt(foreachStmt);
                break;
            case BreakStmt breakStmt:
                ResolveBreakStmt(breakStmt);
                break;
            case ContinueStmt continueStmt:
                ResolveContinueStmt(continueStmt);
                break;
            case FunctionDef functionDef:
                ResolveFunctionDef(functionDef);
                break;
            case ClassDef classDef:
                ResolveClassDef(classDef);
                break;
            case ThrowStmt throwStmt:
                ResolveThrowStmt(throwStmt);
                break;
            case ExportStmt exportStmt:
                ResolveExportStmt(exportStmt);
                break;
            case ImportStmt importStmt:
                ResolveImportStmt(importStmt);
                break;
            default:
                _errors.Add(new ResolveException(
                    $"Unknown statement type: {stmt.GetType().Name}",
                    stmt.Line, stmt.Column));
                break;
        }
    }

    private void ResolveLetStmt(LetStmt stmt)
    {
        // 初期化式を先に解析（自己参照を防ぐ）
        ResolveExpression(stmt.Initializer);

        // 変数を宣言（読み取り専用）
        Declare(stmt.Name, true, stmt.Line, stmt.Column);
    }

    private void ResolveVarStmt(VarStmt stmt)
    {
        // 初期化式を先に解析
        ResolveExpression(stmt.Initializer);

        // 変数を宣言（読み取り専用ではない）
        Declare(stmt.Name, false, stmt.Line, stmt.Column);
    }

    private void ResolveExprStmt(ExprStmt stmt)
    {
        ResolveExpression(stmt.Expression);
    }

    private void ResolveReturnStmt(ReturnStmt stmt)
    {
        if (stmt.Value != null)
        {
            ResolveExpression(stmt.Value);
        }
    }

    private void ResolveForStmt(ForStmt stmt)
    {
        if (stmt.Kind == ForStmtKind.Collection)
        {
            // コレクション反復: for (item in collection) { body }
            // コレクション式を先に解析
            ResolveExpression(stmt.Collection!);

            // 新しいスコープを作成してループ変数を宣言
            BeginScope();
            Declare(stmt.IteratorVariable!, false, stmt.Line, stmt.Column);

            // 本体を解析
            ResolveExpression(stmt.Body);

            EndScope();
        }
        else
        {
            // 条件ループ: for (condition) { body }
            ResolveExpression(stmt.Condition!);

            // 本体は新しいスコープ
            BeginScope();
            ResolveExpression(stmt.Body);
            EndScope();
        }
    }

    private void ResolveForeachStmt(ForeachStmt stmt)
    {
        // コレクション式を先に解析
        ResolveExpression(stmt.Collection);

        // 新しいスコープを作成してループ変数を宣言
        BeginScope();
        Declare(stmt.Variable, false, stmt.Line, stmt.Column);

        // 本体を解析
        ResolveStatement(stmt.Body);

        EndScope();
    }

    private void ResolveBreakStmt(BreakStmt stmt)
    {
        // break文は特に解析する必要はないが、将来的にループ内チェックを追加可能
    }

    private void ResolveContinueStmt(ContinueStmt stmt)
    {
        // continue文は特に解析する必要はないが、将来的にループ内チェックを追加可能
    }

    private void ResolveFunctionDef(FunctionDef stmt)
    {
        // 関数名をスコープに追加
        Declare(stmt.Name, false, stmt.Line, stmt.Column);

        // 新しいスコープを開始
        BeginScope();

        // パラメータを宣言
        foreach (var param in stmt.Parameters)
        {
            Declare(param.Name, false, param.Line, param.Column);
        }

        // 本体を解析
        ResolveExpression(stmt.Body);

        // スコープを終了
        EndScope();
    }

    private void ResolveClassDef(ClassDef stmt)
    {
        // クラス名をスコープに追加
        Declare(stmt.Name, false, stmt.Line, stmt.Column);

        // 親クラスが指定されている場合、その存在を確認
        ClassDef? parentClassDef = null;
        if (!string.IsNullOrEmpty(stmt.ParentClass))
        {
            // 親クラスがすでに定義されているかチェック
            // 注: 実際のクラス定義は _resolvedClasses に保存されていないので、
            // ここでは親クラス名が識別子として解決可能かをチェックする
            // 実行時にCodeGeneratorで親クラスの存在を確認する
        }

        // フィールドの初期化式を解析
        foreach (var field in stmt.Fields)
        {
            if (field.Initializer != null)
            {
                ResolveExpression(field.Initializer);
            }
        }

        // メソッドを解析
        foreach (var method in stmt.Methods)
        {
            BeginScope();

            // クラスのフィールドをメソッドスコープに宣言
            // これにより、メソッド内でフィールドに直接アクセスできる（暗黙のthis参照）
            foreach (var field in stmt.Fields)
            {
                Declare(field.Name, false, field.Line, field.Column);
            }

            // 親クラスのフィールドもメソッドスコープに宣言
            // 注: 親クラスの定義は実行時まで取得できないため、
            // ここでは親クラス名のみを記録しておく
            // 実際のフィールド宣言は実行時にCodeGeneratorで行う

            // パラメータを宣言
            foreach (var param in method.Parameters)
            {
                Declare(param.Name, false, param.Line, param.Column);
            }

            // 本体を解析
            ResolveExpression(method.Body);

            EndScope();
        }
    }

    private void ResolveTryExpr(TryExpr expr)
    {
        // try ブロックを解析
        ResolveExpression(expr.TryBody);

        // catch ブロックを解析
        if (expr.Catch != null)
        {
            BeginScope();

            // 例外変数を宣言
            if (expr.Catch.ExceptionVariable != null)
            {
                Declare(expr.Catch.ExceptionVariable, false, expr.Line, expr.Column);
            }

            // catch ブロック本体を解析
            ResolveExpression(expr.Catch.Body);

            EndScope();
        }

        // finally ブロックを解析
        if (expr.Finally != null)
        {
            ResolveExpression(expr.Finally);
        }
    }

    private void ResolveThrowStmt(ThrowStmt stmt)
    {
        // throw する値を解析
        ResolveExpression(stmt.Value);
    }

    private void ResolveExportStmt(ExportStmt stmt)
    {
        // エクスポートする宣言を解析
        ResolveStatement(stmt.Declaration);
    }

    private void ResolveImportStmt(ImportStmt stmt)
    {
        // インポートする名前をグローバルスコープに宣言
        // （実際の値はCodeGenで読み込まれる）
        foreach (var name in stmt.Names)
        {
            Declare(name, false, stmt.Line, stmt.Column);
        }
    }

    #endregion
}
