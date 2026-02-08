using System.Linq.Expressions;
using Irooon.Core.Ast;
using Irooon.Core.Ast.Expressions;
using Irooon.Core.Ast.Statements;
using Irooon.Core.Lexer;
using Irooon.Core.Runtime;
using ExprTree = System.Linq.Expressions.Expression;
using AstExpr = Irooon.Core.Ast.Expression;

namespace Irooon.Core.CodeGen;

/// <summary>
/// ASTノードをDLR（System.Linq.Expressions）のExpressionTreeに変換します。
/// Task #13: 基本式とリテラルの実装
/// </summary>
public class CodeGenerator
{
    private ParameterExpression _ctxParam; // ScriptContext ctx
    private int _labelCounter = 0; // ラベルの一意性確保用

    // ループのbreak/continueラベルを管理するスタック
    private Stack<(LabelTarget breakLabel, LabelTarget? continueLabel)> _loopLabels = new();

    public CodeGenerator()
    {
        _ctxParam = ExprTree.Parameter(typeof(ScriptContext), "ctx");
    }

    /// <summary>
    /// トップレベルのコンパイル
    /// ASTをFunc&lt;ScriptContext, object&gt;にコンパイルします
    /// </summary>
    /// <param name="program">プログラム全体を表すBlockExpr</param>
    /// <returns>実行可能なデリゲート</returns>
    public Func<ScriptContext, object?> Compile(BlockExpr program)
    {
        var bodyExpr = GenerateBlockExpr(program);
        var lambda = ExprTree.Lambda<Func<ScriptContext, object?>>(
            bodyExpr,
            _ctxParam
        );
        return lambda.Compile();
    }

    #region 式の生成（ディスパッチ）

    /// <summary>
    /// 式の生成（ディスパッチ）
    /// </summary>
    private ExprTree GenerateExpression(AstExpr expr)
    {
        return expr switch
        {
            LiteralExpr e => GenerateLiteralExpr(e),
            IdentifierExpr e => GenerateIdentifierExpr(e),
            AssignExpr e => GenerateAssignExpr(e),
            BinaryExpr e => GenerateBinaryExpr(e),
            UnaryExpr e => GenerateUnaryExpr(e),
            CallExpr e => GenerateCallExpr(e),
            MemberExpr e => GenerateMemberExpr(e),
            IndexExpr e => GenerateIndexExpr(e),
            IfExpr e => GenerateIfExpr(e),
            BlockExpr e => GenerateBlockExpr(e),
            LambdaExpr e => GenerateLambdaExpr(e),
            NewExpr e => GenerateNewExpr(e),
            ListExpr e => GenerateListExpr(e),
            HashExpr e => GenerateHashExpr(e),
            IndexAssignExpr e => GenerateIndexAssignExpr(e),
            MemberAssignExpr e => GenerateMemberAssignExpr(e),
            StringInterpolationExpr e => GenerateStringInterpolationExpr(e),
            TryExpr e => GenerateTryExpr(e),
            RangeExpr e => GenerateRangeExpr(e),
            _ => throw new NotImplementedException($"Unknown expression type: {expr.GetType()}")
        };
    }

    #endregion

    #region 文の生成（ディスパッチ）

    /// <summary>
    /// 文の生成（ディスパッチ）
    /// </summary>
    private ExprTree GenerateStatement(Statement stmt)
    {
        return stmt switch
        {
            LetStmt s => GenerateLetStmt(s),
            VarStmt s => GenerateVarStmt(s),
            ExprStmt s => GenerateExprStmt(s),
            ReturnStmt s => GenerateReturnStmt(s),
            ForStmt s => GenerateForStmt(s),
            ForeachStmt s => GenerateForeachStmt(s),
            BreakStmt s => GenerateBreakStmt(s),
            ContinueStmt s => GenerateContinueStmt(s),
            FunctionDef s => GenerateFunctionDef(s),
            ClassDef s => GenerateClassDef(s),
            ThrowStmt s => GenerateThrowStmt(s),
            ExportStmt s => GenerateExportStmt(s),
            ImportStmt s => GenerateImportStmt(s),
            _ => throw new NotImplementedException($"Unknown statement type: {stmt.GetType()}")
        };
    }

    #endregion

    #region Task #13: 基本式の実装

    /// <summary>
    /// リテラル式の変換
    /// 仕様: 数値はdoubleに統一、objectにボックス化
    /// </summary>
    private ExprTree GenerateLiteralExpr(LiteralExpr expr)
    {
        // 数値はdoubleに統一、objectにボックス化
        if (expr.Value is double d)
        {
            return ExprTree.Convert(
                ExprTree.Constant(d, typeof(double)),
                typeof(object)
            );
        }

        // 文字列、bool、null
        return ExprTree.Constant(expr.Value, typeof(object));
    }

    /// <summary>
    /// 識別子式の変換（変数参照）
    /// 仕様: ctx.Globals["name"]
    /// </summary>
    private ExprTree GenerateIdentifierExpr(IdentifierExpr expr)
    {
        // ctx.Globals["name"]
        var globalsExpr = ExprTree.Property(_ctxParam, "Globals");
        var nameExpr = ExprTree.Constant(expr.Name);
        return ExprTree.Property(globalsExpr, "Item", nameExpr);
    }

    /// <summary>
    /// 代入式の変換
    /// 仕様: ctx.Globals["name"] = value
    /// </summary>
    private ExprTree GenerateAssignExpr(AssignExpr expr)
    {
        // ctx.Globals["name"] = value
        var valueExpr = GenerateExpression(expr.Value);
        var globalsExpr = ExprTree.Property(_ctxParam, "Globals");
        var nameExpr = ExprTree.Constant(expr.Name);
        var itemProperty = ExprTree.Property(globalsExpr, "Item", nameExpr);

        return ExprTree.Assign(itemProperty, valueExpr);
    }

    /// <summary>
    /// let文の変換
    /// 仕様: let/varの違いはResolverで検査済み、CodeGenは同じ
    /// </summary>
    private ExprTree GenerateLetStmt(LetStmt stmt)
    {
        // let/varの違いはResolverで検査済み、CodeGenは同じ
        // ctx.Globals["name"] = initializer
        return GenerateAssignExpr(
            new AssignExpr(stmt.Name, stmt.Initializer, stmt.Line, stmt.Column)
        );
    }

    /// <summary>
    /// var文の変換
    /// </summary>
    private ExprTree GenerateVarStmt(VarStmt stmt)
    {
        return GenerateAssignExpr(
            new AssignExpr(stmt.Name, stmt.Initializer, stmt.Line, stmt.Column)
        );
    }

    /// <summary>
    /// 式文の変換
    /// </summary>
    private ExprTree GenerateExprStmt(ExprStmt stmt)
    {
        return GenerateExpression(stmt.Expression);
    }

    /// <summary>
    /// ブロック式の変換
    /// 仕様: Expression.Block(vars, exprs...)
    /// 最後の式が値になる
    /// </summary>
    private ExprTree GenerateBlockExpr(BlockExpr expr)
    {
        var expressions = new List<ExprTree>();

        // 文を変換
        foreach (var stmt in expr.Statements)
        {
            expressions.Add(GenerateStatement(stmt));
        }

        // 最後の式
        if (expr.Expression != null)
        {
            expressions.Add(GenerateExpression(expr.Expression));
        }
        else if (expressions.Count == 0 || expr.Statements.Count == 0 ||
                 expr.Statements[^1] is not ReturnStmt)
        {
            // 最後が文のみの場合（return文以外）はnullを追加
            expressions.Add(ExprTree.Constant(null, typeof(object)));
        }

        return ExprTree.Block(typeof(object), expressions);
    }

    #endregion

    #region Task #14: 演算子の実装

    /// <summary>
    /// 二項演算式の変換
    /// 仕様: すべてRuntimeHelpersに委譲
    /// </summary>
    private ExprTree GenerateBinaryExpr(BinaryExpr expr)
    {
        var left = GenerateExpression(expr.Left);
        var right = GenerateExpression(expr.Right);

        var runtimeType = typeof(RuntimeHelpers);

        return expr.Operator switch
        {
            // 算術演算子
            TokenType.Plus => ExprTree.Call(
                runtimeType,
                nameof(RuntimeHelpers.Add),
                null,
                left,
                right
            ),
            TokenType.Minus => ExprTree.Call(
                runtimeType,
                nameof(RuntimeHelpers.Sub),
                null,
                left,
                right
            ),
            TokenType.Star => ExprTree.Call(
                runtimeType,
                nameof(RuntimeHelpers.Mul),
                null,
                left,
                right
            ),
            TokenType.Slash => ExprTree.Call(
                runtimeType,
                nameof(RuntimeHelpers.Div),
                null,
                left,
                right
            ),
            TokenType.Percent => ExprTree.Call(
                runtimeType,
                nameof(RuntimeHelpers.Mod),
                null,
                left,
                right
            ),

            // 比較演算子
            TokenType.EqualEqual => ExprTree.Call(
                runtimeType,
                nameof(RuntimeHelpers.Eq),
                null,
                left,
                right
            ),
            TokenType.BangEqual => ExprTree.Call(
                runtimeType,
                nameof(RuntimeHelpers.Ne),
                null,
                left,
                right
            ),
            TokenType.Less => ExprTree.Call(
                runtimeType,
                nameof(RuntimeHelpers.Lt),
                null,
                left,
                right
            ),
            TokenType.LessEqual => ExprTree.Call(
                runtimeType,
                nameof(RuntimeHelpers.Le),
                null,
                left,
                right
            ),
            TokenType.Greater => ExprTree.Call(
                runtimeType,
                nameof(RuntimeHelpers.Gt),
                null,
                left,
                right
            ),
            TokenType.GreaterEqual => ExprTree.Call(
                runtimeType,
                nameof(RuntimeHelpers.Ge),
                null,
                left,
                right
            ),

            // 論理演算子（短絡評価）
            TokenType.And => GenerateAndExpr(expr),
            TokenType.Or => GenerateOrExpr(expr),

            _ => throw new NotImplementedException($"Operator {expr.Operator} not implemented")
        };
    }

    /// <summary>
    /// 論理AND演算子の短絡評価
    /// 仕様: a and b → truthy(a) ? b : a
    /// </summary>
    private ExprTree GenerateAndExpr(BinaryExpr expr)
    {
        var left = GenerateExpression(expr.Left);
        var right = GenerateExpression(expr.Right);

        // 左辺を一時変数に格納（一度だけ評価）
        var tmpVar = ExprTree.Variable(typeof(object), "tmp");
        var runtimeType = typeof(RuntimeHelpers);

        // IsTruthy(tmp)
        var truthyCall = ExprTree.Call(
            runtimeType,
            nameof(RuntimeHelpers.IsTruthy),
            null,
            tmpVar
        );

        // truthy(tmp) ? right : tmp
        var condition = ExprTree.Condition(
            truthyCall,
            right,
            tmpVar,
            typeof(object)
        );

        // Block { tmp = left; condition; }
        return ExprTree.Block(
            new[] { tmpVar },
            ExprTree.Assign(tmpVar, left),
            condition
        );
    }

    /// <summary>
    /// 論理OR演算子の短絡評価
    /// 仕様: a or b → truthy(a) ? a : b
    /// </summary>
    private ExprTree GenerateOrExpr(BinaryExpr expr)
    {
        var left = GenerateExpression(expr.Left);
        var right = GenerateExpression(expr.Right);

        var tmpVar = ExprTree.Variable(typeof(object), "tmp");
        var runtimeType = typeof(RuntimeHelpers);

        var truthyCall = ExprTree.Call(
            runtimeType,
            nameof(RuntimeHelpers.IsTruthy),
            null,
            tmpVar
        );

        // truthy(tmp) ? tmp : right
        var condition = ExprTree.Condition(
            truthyCall,
            tmpVar,
            right,
            typeof(object)
        );

        return ExprTree.Block(
            new[] { tmpVar },
            ExprTree.Assign(tmpVar, left),
            condition
        );
    }

    /// <summary>
    /// 単項演算式の変換
    /// 仕様: すべてRuntimeHelpersに委譲
    /// </summary>
    private ExprTree GenerateUnaryExpr(UnaryExpr expr)
    {
        var operand = GenerateExpression(expr.Operand);
        var runtimeType = typeof(RuntimeHelpers);

        return expr.Operator switch
        {
            TokenType.Minus => ExprTree.Call(
                runtimeType,
                nameof(RuntimeHelpers.Sub),
                null,
                ExprTree.Convert(
                    ExprTree.Constant(0.0, typeof(double)),
                    typeof(object)
                ),
                operand
            ),
            TokenType.Not => ExprTree.Call(
                runtimeType,
                nameof(RuntimeHelpers.Not),
                null,
                operand
            ),
            _ => throw new NotImplementedException($"Unary operator {expr.Operator} not implemented")
        };
    }

    #endregion

    #region Task #15: 制御構造の実装

    /// <summary>
    /// if式の変換
    /// 仕様（expression-tree-mapping.md セクション7）:
    /// condTruth = Runtime.IsTruthy(condObj)
    /// Expression.Condition(condTruth, thenObj, elseObj)
    /// </summary>
    private ExprTree GenerateIfExpr(IfExpr expr)
    {
        var condExpr = GenerateExpression(expr.Condition);
        var thenExpr = GenerateExpression(expr.ThenBranch);
        var elseExpr = GenerateExpression(expr.ElseBranch);

        // IsTruthy で真偽値判定
        var truthyCall = ExprTree.Call(
            typeof(RuntimeHelpers),
            nameof(RuntimeHelpers.IsTruthy),
            null,
            condExpr
        );

        // Expression.Condition
        return ExprTree.Condition(
            truthyCall,
            thenExpr,
            elseExpr,
            typeof(object)
        );
    }

    /// <summary>
    /// foreach文の変換
    /// foreach (item in collection) { body }
    /// </summary>
    private ExprTree GenerateForeachStmt(ForeachStmt stmt)
    {
        var breakLabel = ExprTree.Label($"foreachBreak_{_labelCounter}");
        var continueLabel = ExprTree.Label($"foreachContinue_{_labelCounter++}");

        // コレクション式を評価
        var collectionExpr = GenerateExpression(stmt.Collection);

        // IEnumerable にキャスト
        var enumerableVar = ExprTree.Variable(typeof(System.Collections.IEnumerable), "enumerable");
        var enumeratorVar = ExprTree.Variable(typeof(System.Collections.IEnumerator), "enumerator");
        var currentVar = ExprTree.Variable(typeof(object), "current");

        // ループ変数をctx.Globalsに登録
        var setLoopVar = ExprTree.Call(
            ExprTree.Property(_ctxParam, nameof(ScriptContext.Globals)),
            typeof(Dictionary<string, object?>).GetMethod("set_Item")!,
            ExprTree.Constant(stmt.Variable),
            currentVar
        );

        // ループラベルをスタックにプッシュ
        _loopLabels.Push((breakLabel, continueLabel));

        var bodyExpr = GenerateStatement(stmt.Body);

        // ループラベルをポップ
        _loopLabels.Pop();

        // ループ本体
        var loopBody = ExprTree.Block(
            typeof(void),
            // if (!enumerator.MoveNext()) break;
            ExprTree.IfThen(
                ExprTree.Not(ExprTree.Call(enumeratorVar, typeof(System.Collections.IEnumerator).GetMethod("MoveNext")!)),
                ExprTree.Break(breakLabel)
            ),
            // current = enumerator.Current
            ExprTree.Assign(currentVar, ExprTree.Property(enumeratorVar, nameof(System.Collections.IEnumerator.Current))),
            // ctx.Globals[variable] = current
            setLoopVar,
            // body
            bodyExpr
        );

        // foreach全体
        return ExprTree.Block(
            typeof(object),
            new[] { enumerableVar, enumeratorVar, currentVar },
            // enumerable = (IEnumerable)collection
            ExprTree.Assign(
                enumerableVar,
                ExprTree.Convert(collectionExpr, typeof(System.Collections.IEnumerable))
            ),
            // enumerator = enumerable.GetEnumerator()
            ExprTree.Assign(
                enumeratorVar,
                ExprTree.Call(enumerableVar, typeof(System.Collections.IEnumerable).GetMethod("GetEnumerator")!)
            ),
            // loop
            ExprTree.Loop(loopBody, breakLabel, continueLabel),
            // return null
            ExprTree.Constant(null, typeof(object))
        );
    }

    /// <summary>
    /// for文の変換
    /// パターン1: for (item in collection) { body } - コレクション反復
    /// パターン2: for (condition) { body } - 条件ループ
    /// </summary>
    private ExprTree GenerateForStmt(ForStmt stmt)
    {
        return stmt.Kind switch
        {
            ForStmtKind.Collection => GenerateForCollection(stmt),
            ForStmtKind.Condition => GenerateForCondition(stmt),
            _ => throw new NotImplementedException($"Unknown ForStmtKind: {stmt.Kind}")
        };
    }

    /// <summary>
    /// for (item in collection) { body } の変換
    /// foreach と同じ実装
    /// </summary>
    private ExprTree GenerateForCollection(ForStmt stmt)
    {
        var breakLabel = ExprTree.Label($"forBreak_{_labelCounter}");
        var continueLabel = ExprTree.Label($"forContinue_{_labelCounter++}");

        // コレクション式を評価
        var collectionExpr = GenerateExpression(stmt.Collection!);

        // IEnumerable にキャスト
        var enumerableVar = ExprTree.Variable(typeof(System.Collections.IEnumerable), "enumerable");
        var enumeratorVar = ExprTree.Variable(typeof(System.Collections.IEnumerator), "enumerator");
        var currentVar = ExprTree.Variable(typeof(object), "current");

        // ループ変数をctx.Globalsに登録
        var setLoopVar = ExprTree.Call(
            ExprTree.Property(_ctxParam, nameof(ScriptContext.Globals)),
            typeof(Dictionary<string, object?>).GetMethod("set_Item")!,
            ExprTree.Constant(stmt.IteratorVariable),
            currentVar
        );

        // ループラベルをスタックにプッシュ
        _loopLabels.Push((breakLabel, continueLabel));

        var bodyExpr = GenerateExpression(stmt.Body);

        // ループラベルをポップ
        _loopLabels.Pop();

        // ループ本体
        var loopBody = ExprTree.Block(
            typeof(void),
            // if (!enumerator.MoveNext()) break;
            ExprTree.IfThen(
                ExprTree.Not(ExprTree.Call(enumeratorVar, typeof(System.Collections.IEnumerator).GetMethod("MoveNext")!)),
                ExprTree.Break(breakLabel)
            ),
            // current = enumerator.Current
            ExprTree.Assign(currentVar, ExprTree.Property(enumeratorVar, nameof(System.Collections.IEnumerator.Current))),
            // ctx.Globals[variable] = current
            setLoopVar,
            // body
            bodyExpr
        );

        // for全体
        return ExprTree.Block(
            typeof(object),
            new[] { enumerableVar, enumeratorVar, currentVar },
            // enumerable = (IEnumerable)collection
            ExprTree.Assign(
                enumerableVar,
                ExprTree.TypeAs(collectionExpr, typeof(System.Collections.IEnumerable))
            ),
            // enumerator = enumerable.GetEnumerator()
            ExprTree.Assign(
                enumeratorVar,
                ExprTree.Call(enumerableVar, typeof(System.Collections.IEnumerable).GetMethod("GetEnumerator")!)
            ),
            // loop
            ExprTree.Loop(loopBody, breakLabel, continueLabel),
            // return null
            ExprTree.Constant(null, typeof(object))
        );
    }

    /// <summary>
    /// for (condition) { body } の変換
    /// while と同じ実装
    /// </summary>
    private ExprTree GenerateForCondition(ForStmt stmt)
    {
        var breakLabel = ExprTree.Label($"forBreak_{_labelCounter}");
        var continueLabel = ExprTree.Label($"forContinue_{_labelCounter++}");

        // ループラベルをスタックにプッシュ
        _loopLabels.Push((breakLabel, continueLabel));

        var condExpr = GenerateExpression(stmt.Condition!);
        var bodyExpr = GenerateExpression(stmt.Body);

        // ループラベルをポップ
        _loopLabels.Pop();

        // IsTruthy で真偽値判定
        var truthyCall = ExprTree.Call(
            typeof(RuntimeHelpers),
            nameof(RuntimeHelpers.IsTruthy),
            null,
            condExpr
        );

        // if (!truthy) break;
        var breakIfFalse = ExprTree.IfThen(
            ExprTree.Not(truthyCall),
            ExprTree.Break(breakLabel)
        );

        // Loop body
        var loopBody = ExprTree.Block(
            typeof(void),
            breakIfFalse,
            bodyExpr
        );

        // Expression.Loop with break and continue labels
        return ExprTree.Block(
            typeof(object),
            ExprTree.Loop(loopBody, breakLabel, continueLabel),
            ExprTree.Constant(null, typeof(object))
        );
    }

    /// <summary>
    /// break文の変換
    /// </summary>
    private ExprTree GenerateBreakStmt(BreakStmt stmt)
    {
        if (_loopLabels.Count == 0)
        {
            throw new InvalidOperationException("break statement outside of loop");
        }

        var (breakLabel, _) = _loopLabels.Peek();
        return ExprTree.Break(breakLabel, typeof(void));
    }

    /// <summary>
    /// continue文の変換
    /// </summary>
    private ExprTree GenerateContinueStmt(ContinueStmt stmt)
    {
        if (_loopLabels.Count == 0)
        {
            throw new InvalidOperationException("continue statement outside of loop");
        }

        var (_, continueLabel) = _loopLabels.Peek();
        if (continueLabel == null)
        {
            throw new InvalidOperationException("continue not supported in this context");
        }

        return ExprTree.Continue(continueLabel, typeof(void));
    }

    /// <summary>
    /// return文の変換
    /// v0.1では単純に式を返す
    /// （関数内でのreturnは後で実装）
    /// </summary>
    private ExprTree GenerateReturnStmt(ReturnStmt stmt)
    {
        if (stmt.Value != null)
        {
            return GenerateExpression(stmt.Value);
        }
        else
        {
            return ExprTree.Constant(null, typeof(object));
        }
    }

    #endregion

    #region Task #15以降の実装（スタブ）

    // Task #16: 関数とクロージャの実装（簡易実装）
    /// <summary>
    /// 関数呼び出しの変換
    /// 仕様: RuntimeHelpers.Invoke(callee, ctx, args, thisArg)
    /// </summary>
    private ExprTree GenerateCallExpr(CallExpr expr)
    {
        // CalleeがMemberExprの場合、CLR型のメソッド呼び出しまたはコンストラクタかチェック
        if (expr.Callee is MemberExpr memberExpr)
        {
            var (isCLRType, typeName, methodName) = ExtractCLRTypeName(memberExpr);

            if (isCLRType)
            {
                // methodNameが空の場合、コンストラクタ呼び出し
                if (string.IsNullOrEmpty(methodName))
                {
                    // CLR型のコンストラクタを生成
                    var argExprsConstructor = expr.Arguments.Select(GenerateExpression).ToArray();
                    var argsArrayConstructor = ExprTree.NewArrayInit(typeof(object), argExprsConstructor);

                    // RuntimeHelpers.ResolveCLRType(typeName)
                    var resolveTypeCall = ExprTree.Call(
                        typeof(RuntimeHelpers),
                        nameof(RuntimeHelpers.ResolveCLRType),
                        null,
                        ExprTree.Constant(typeName)
                    );

                    // RuntimeHelpers.CreateCLRInstance(type, args)
                    return ExprTree.Call(
                        typeof(RuntimeHelpers),
                        nameof(RuntimeHelpers.CreateCLRInstance),
                        null,
                        resolveTypeCall,
                        argsArrayConstructor
                    );
                }

                // CLR型のメソッド呼び出しを生成
                var argExprs = expr.Arguments.Select(GenerateExpression).ToArray();
                var argsArray = ExprTree.NewArrayInit(typeof(object), argExprs);

                // RuntimeHelpers.ResolveCLRType(typeName)
                var resolveTypeCall2 = ExprTree.Call(
                    typeof(RuntimeHelpers),
                    nameof(RuntimeHelpers.ResolveCLRType),
                    null,
                    ExprTree.Constant(typeName)
                );

                // RuntimeHelpers.InvokeCLRStaticMethod(type, methodName, args)
                return ExprTree.Call(
                    typeof(RuntimeHelpers),
                    nameof(RuntimeHelpers.InvokeCLRStaticMethod),
                    null,
                    resolveTypeCall2,
                    ExprTree.Constant(methodName),
                    argsArray
                );
            }

            // 通常のインスタンスメソッド呼び出し
            var targetExpr = GenerateExpression(memberExpr.Target);
            var thisArg = targetExpr;

            // メソッドを取得
            var calleeExpr = ExprTree.Call(
                typeof(RuntimeHelpers),
                "GetMember",
                null,
                targetExpr,
                ExprTree.Constant(memberExpr.Name)
            );

            var argExprsInstance = expr.Arguments.Select(GenerateExpression).ToArray();
            var argsArrayInstance = ExprTree.NewArrayInit(typeof(object), argExprsInstance);

            return ExprTree.Call(
                typeof(RuntimeHelpers),
                "Invoke",
                null,
                calleeExpr,
                _ctxParam,
                argsArrayInstance,
                thisArg
            );
        }
        // CalleeがIdentifierExprまたはMemberExprで、CLR型のコンストラクタ呼び出しかチェック
        else if (expr.Callee is IdentifierExpr identExpr)
        {
            // ドット区切りの型名をチェック（例: System.Text.StringBuilder）
            var typeName = identExpr.Name;

            // System で始まる場合、CLR型のコンストラクタと判断
            if (typeName.StartsWith("System."))
            {
                var argExprsConstructor = expr.Arguments.Select(GenerateExpression).ToArray();
                var argsArrayConstructor = ExprTree.NewArrayInit(typeof(object), argExprsConstructor);

                // RuntimeHelpers.ResolveCLRType(typeName)
                var resolveTypeCall = ExprTree.Call(
                    typeof(RuntimeHelpers),
                    nameof(RuntimeHelpers.ResolveCLRType),
                    null,
                    ExprTree.Constant(typeName)
                );

                // RuntimeHelpers.CreateCLRInstance(type, args)
                return ExprTree.Call(
                    typeof(RuntimeHelpers),
                    nameof(RuntimeHelpers.CreateCLRInstance),
                    null,
                    resolveTypeCall,
                    argsArrayConstructor
                );
            }
        }

        // 通常の関数呼び出し
        var calleeExprFinal = GenerateExpression(expr.Callee);
        var argExprsFinal = expr.Arguments.Select(GenerateExpression).ToArray();
        var argsArrayFinal = ExprTree.NewArrayInit(typeof(object), argExprsFinal);

        return ExprTree.Call(
            typeof(RuntimeHelpers),
            "Invoke",
            null,
            calleeExprFinal,
            _ctxParam,
            argsArrayFinal,
            ExprTree.Constant(null, typeof(object))
        );
    }

    /// <summary>
    /// MemberExprからCLR型名を抽出します。
    /// </summary>
    /// <param name="memberExpr">MemberExpr</param>
    /// <returns>(isCLRType, typeName, methodName)</returns>
    private (bool isCLRType, string typeName, string methodName) ExtractCLRTypeName(MemberExpr memberExpr)
    {
        var parts = new List<string>();
        var current = memberExpr;

        // MemberExprをたどってドット区切りの名前を構築
        while (current != null)
        {
            parts.Insert(0, current.Name);

            if (current.Target is MemberExpr targetMember)
            {
                current = targetMember;
            }
            else if (current.Target is IdentifierExpr identExpr)
            {
                parts.Insert(0, identExpr.Name);
                break;
            }
            else
            {
                // CLR型ではない
                return (false, "", "");
            }
        }

        // System で始まる場合、CLR型とみなす
        if (parts.Count >= 2 && parts[0] == "System")
        {
            // 型名全体を構築して確認
            var fullTypeName = string.Join(".", parts);

            // 型が解決できるかチェック
            var resolvedType = RuntimeHelpers.ResolveCLRType(fullTypeName);
            if (resolvedType != null)
            {
                // コンストラクタ呼び出し（型名全体で解決できた場合）
                return (true, fullTypeName, "");
            }

            // 最後の要素がメソッド名
            var methodName = parts[^1];
            var typeName = string.Join(".", parts.Take(parts.Count - 1));
            return (true, typeName, methodName);
        }

        return (false, "", "");
    }

    /// <summary>
    /// ラムダ式の変換
    /// 仕様（expression-tree-mapping.md セクション9）:
    /// Closureオブジェクトを生成し、IroCallableとして扱う
    /// </summary>
    private ExprTree GenerateLambdaExpr(LambdaExpr expr)
    {
        // 関数本体を Func<ScriptContext, object[], object> にコンパイル
        var argsParam = ExprTree.Parameter(typeof(object[]), "args");
        var ctxParamForFunc = ExprTree.Parameter(typeof(ScriptContext), "ctx");

        var bodyExprs = new List<ExprTree>();

        // パラメータを args[0], args[1], ... にバインド
        for (int i = 0; i < expr.Parameters.Count; i++)
        {
            var param = expr.Parameters[i];
            var argAccess = ExprTree.ArrayIndex(argsParam, ExprTree.Constant(i));
            var globalsForParam = ExprTree.Property(ctxParamForFunc, "Globals");
            var paramName = ExprTree.Constant(param.Name);
            var itemForParam = ExprTree.Property(globalsForParam, "Item", paramName);
            bodyExprs.Add(ExprTree.Assign(itemForParam, argAccess));
        }

        // 本体を実行（一時的に _ctxParam を切り替える）
        var savedCtxParam = _ctxParam;
        _ctxParam = ctxParamForFunc;
        var bodyExpr = GenerateExpression(expr.Body);
        _ctxParam = savedCtxParam;

        bodyExprs.Add(bodyExpr);

        var bodyBlock = ExprTree.Block(typeof(object), bodyExprs);

        // Lambda<Func<ScriptContext, object[], object>> を作成
        var lambda = ExprTree.Lambda<Func<ScriptContext, object[], object>>(
            bodyBlock,
            ctxParamForFunc,
            argsParam
        );

        var compiled = lambda.Compile();

        // パラメータ名のリストを作成
        var paramNames = expr.Parameters.Select(p => p.Name).ToList();
        var paramNamesListNew = ExprTree.New(
            typeof(List<string>).GetConstructor(new[] { typeof(IEnumerable<string>) })!,
            ExprTree.NewArrayInit(typeof(string), paramNames.Select(n => ExprTree.Constant(n)))
        );

        // Closure オブジェクトを作成（位置情報を含む）
        var closureNew = ExprTree.New(
            typeof(Closure).GetConstructor(new[] {
                typeof(string),
                typeof(Func<ScriptContext, object[], object>),
                typeof(List<string>),
                typeof(int),
                typeof(int)
            })!,
            ExprTree.Constant("<lambda>"),
            ExprTree.Constant(compiled, typeof(Func<ScriptContext, object[], object>)),
            paramNamesListNew,
            ExprTree.Constant(expr.Line),
            ExprTree.Constant(expr.Column)
        );

        return ExprTree.Convert(closureNew, typeof(object));
    }

    /// <summary>
    /// 関数定義の変換
    /// 仕様（expression-tree-mapping.md セクション9）:
    /// Closureオブジェクトを生成し、ctx.Globals[name] に登録
    /// </summary>
    private ExprTree GenerateFunctionDef(FunctionDef stmt)
    {
        // 関数本体を Func<ScriptContext, object[], object> にコンパイル
        var argsParam = ExprTree.Parameter(typeof(object[]), "args");
        var ctxParamForFunc = ExprTree.Parameter(typeof(ScriptContext), "ctx");

        var bodyExprs = new List<ExprTree>();

        // パラメータを args[0], args[1], ... にバインド
        for (int i = 0; i < stmt.Parameters.Count; i++)
        {
            var param = stmt.Parameters[i];
            var argAccess = ExprTree.ArrayIndex(argsParam, ExprTree.Constant(i));
            var globalsForParam = ExprTree.Property(ctxParamForFunc, "Globals");
            var paramName = ExprTree.Constant(param.Name);
            var itemForParam = ExprTree.Property(globalsForParam, "Item", paramName);
            bodyExprs.Add(ExprTree.Assign(itemForParam, argAccess));
        }

        // 本体を実行（一時的に _ctxParam を切り替える）
        var savedCtxParam = _ctxParam;
        _ctxParam = ctxParamForFunc;
        var bodyExpr = GenerateExpression(stmt.Body);
        _ctxParam = savedCtxParam;

        bodyExprs.Add(bodyExpr);

        var bodyBlock = ExprTree.Block(typeof(object), bodyExprs);

        // Lambda<Func<ScriptContext, object[], object>> を作成
        var lambda = ExprTree.Lambda<Func<ScriptContext, object[], object>>(
            bodyBlock,
            ctxParamForFunc,
            argsParam
        );

        var compiled = lambda.Compile();

        // パラメータ名のリストを作成
        var paramNames = stmt.Parameters.Select(p => p.Name).ToList();
        var paramNamesListNew = ExprTree.New(
            typeof(List<string>).GetConstructor(new[] { typeof(IEnumerable<string>) })!,
            ExprTree.NewArrayInit(typeof(string), paramNames.Select(n => ExprTree.Constant(n)))
        );

        // Closure オブジェクトを作成（位置情報を含む）
        var closureNew = ExprTree.New(
            typeof(Closure).GetConstructor(new[] {
                typeof(string),
                typeof(Func<ScriptContext, object[], object>),
                typeof(List<string>),
                typeof(int),
                typeof(int)
            })!,
            ExprTree.Constant(stmt.Name),
            ExprTree.Constant(compiled, typeof(Func<ScriptContext, object[], object>)),
            paramNamesListNew,
            ExprTree.Constant(stmt.Line),
            ExprTree.Constant(stmt.Column)
        );

        // ctx.Globals[name] = closure
        var globalsExpr = ExprTree.Property(_ctxParam, "Globals");
        var nameExpr = ExprTree.Constant(stmt.Name);
        var itemProperty = ExprTree.Property(globalsExpr, "Item", nameExpr);

        return ExprTree.Assign(itemProperty, ExprTree.Convert(closureNew, typeof(object)));
    }

    // Task #17: クラスとインスタンスの実装

    /// <summary>
    /// クラス定義の変換
    /// 仕様: IroClass オブジェクトを作成し、ctx.Classes に登録
    /// </summary>
    private ExprTree GenerateClassDef(ClassDef stmt)
    {
        // フィールド定義を変換
        var fieldDefs = stmt.Fields.Select(f =>
        {
            ExprTree? initExpr = null;
            if (f.Initializer != null)
            {
                // 初期化式をLambdaにコンパイル
                var initLambda = ExprTree.Lambda<Func<ScriptContext, object?>>(
                    GenerateExpression(f.Initializer),
                    _ctxParam
                );
                initExpr = ExprTree.Constant(initLambda.Compile());
            }

            var nullInit = ExprTree.Constant(null, typeof(Func<ScriptContext, object?>));

            return ExprTree.New(
                typeof(Runtime.FieldDef).GetConstructor(new[] {
                    typeof(string),
                    typeof(bool),
                    typeof(Func<ScriptContext, object?>)
                })!,
                ExprTree.Constant(f.Name),
                ExprTree.Constant(f.IsPublic),
                initExpr ?? nullInit
            );
        }).ToArray();

        var fieldsArray = ExprTree.NewArrayInit(typeof(Runtime.FieldDef), fieldDefs);

        // メソッド定義を変換
        var methodDefs = stmt.Methods.Select(m =>
        {
            // メソッド本体をClosureにコンパイル
            var argsParam = ExprTree.Parameter(typeof(object[]), "args");
            var ctxParamForFunc = ExprTree.Parameter(typeof(ScriptContext), "ctx");

            // 一時的にctxParamを切り替える
            var savedCtxParam = _ctxParam;
            _ctxParam = ctxParamForFunc;

            var bodyExprs = new List<ExprTree>();

            // 引数を ctx.Globals に格納
            for (int i = 0; i < m.Parameters.Count; i++)
            {
                var param = m.Parameters[i];
                var argAccess = ExprTree.ArrayIndex(argsParam, ExprTree.Constant(i));
                var globalsExpr = ExprTree.Property(ctxParamForFunc, "Globals");
                var paramName = ExprTree.Constant(param.Name);
                var itemProperty = ExprTree.Property(globalsExpr, "Item", paramName);
                bodyExprs.Add(ExprTree.Assign(itemProperty, argAccess));
            }

            // メソッド本体を追加
            bodyExprs.Add(GenerateExpression(m.Body));

            // ctxParamを元に戻す
            _ctxParam = savedCtxParam;

            var bodyBlock = ExprTree.Block(typeof(object), bodyExprs);
            var lambda = ExprTree.Lambda<Func<ScriptContext, object[], object>>(
                bodyBlock,
                ctxParamForFunc,
                argsParam
            );

            var compiled = lambda.Compile();

            // パラメータ名のリストを作成
            var paramNames = m.Parameters.Select(p => p.Name).ToList();
            var closure = new Closure(m.Name, compiled, paramNames, m.Line, m.Column);

            return ExprTree.New(
                typeof(Runtime.MethodDef).GetConstructor(new[] {
                    typeof(string),
                    typeof(bool),
                    typeof(bool),
                    typeof(IroCallable)
                })!,
                ExprTree.Constant(m.Name),
                ExprTree.Constant(m.IsPublic),
                ExprTree.Constant(m.IsStatic),
                ExprTree.Constant(closure, typeof(IroCallable))
            );
        }).ToArray();

        var methodsArray = ExprTree.NewArrayInit(typeof(Runtime.MethodDef), methodDefs);

        // 親クラスを取得（存在する場合）
        ExprTree parentClassExpr;
        if (!string.IsNullOrEmpty(stmt.ParentClass))
        {
            // ctx.Classes[parentClassName]
            var parentClassesExpr = ExprTree.Property(_ctxParam, "Classes");
            var parentNameExpr = ExprTree.Constant(stmt.ParentClass);
            parentClassExpr = ExprTree.Property(parentClassesExpr, "Item", parentNameExpr);
        }
        else
        {
            parentClassExpr = ExprTree.Constant(null, typeof(IroClass));
        }

        // IroClass を作成
        var classNew = ExprTree.New(
            typeof(IroClass).GetConstructor(new[] {
                typeof(string),
                typeof(Runtime.FieldDef[]),
                typeof(Runtime.MethodDef[]),
                typeof(IroClass)
            })!,
            ExprTree.Constant(stmt.Name),
            fieldsArray,
            methodsArray,
            parentClassExpr
        );

        // ctx.Classes[name] = class
        var classesExpr = ExprTree.Property(_ctxParam, "Classes");
        var nameExpr = ExprTree.Constant(stmt.Name);
        var itemProperty = ExprTree.Property(classesExpr, "Item", nameExpr);

        return ExprTree.Assign(itemProperty, classNew);
    }

    /// <summary>
    /// インスタンス生成の変換
    /// 仕様: Runtime.NewInstance(className, ctx, args)
    /// </summary>
    private ExprTree GenerateNewExpr(NewExpr expr)
    {
        var argExprs = expr.Arguments.Select(GenerateExpression).ToArray();
        var argsArray = ExprTree.NewArrayInit(typeof(object), argExprs);

        // Runtime.NewInstance(className, ctx, args)
        return ExprTree.Call(
            typeof(RuntimeHelpers),
            "NewInstance",
            null,
            ExprTree.Constant(expr.ClassName),
            _ctxParam,
            argsArray
        );
    }

    /// <summary>
    /// メンバアクセスの変換
    /// 仕様: Runtime.GetMember(target, name)
    /// </summary>
    private ExprTree GenerateMemberExpr(MemberExpr expr)
    {
        // CLR型のプロパティアクセスかチェック
        var (isCLRType, typeName, propertyName) = ExtractCLRTypeName(expr);

        if (isCLRType)
        {
            // CLR型のプロパティアクセスを生成
            // RuntimeHelpers.ResolveCLRType(typeName)
            var resolveTypeCall = ExprTree.Call(
                typeof(RuntimeHelpers),
                nameof(RuntimeHelpers.ResolveCLRType),
                null,
                ExprTree.Constant(typeName)
            );

            // RuntimeHelpers.InvokeCLRStaticMethod(type, "get_PropertyName", [])
            var argsArray = ExprTree.NewArrayInit(typeof(object));
            return ExprTree.Call(
                typeof(RuntimeHelpers),
                nameof(RuntimeHelpers.InvokeCLRStaticMethod),
                null,
                resolveTypeCall,
                ExprTree.Constant("get_" + propertyName),
                argsArray
            );
        }

        // 通常のメンバアクセス
        var targetExpr = GenerateExpression(expr.Target);

        // Runtime.GetMember(target, name)
        return ExprTree.Call(
            typeof(RuntimeHelpers),
            "GetMember",
            null,
            targetExpr,
            ExprTree.Constant(expr.Name)
        );
    }

    /// <summary>
    /// インデックスアクセスの変換
    /// 仕様: RuntimeHelpers.GetIndexed を使用
    /// </summary>
    private ExprTree GenerateIndexExpr(IndexExpr expr)
    {
        var targetExpr = GenerateExpression(expr.Target);
        var indexExpr = GenerateExpression(expr.Index);

        // RuntimeHelpers.GetIndexed(target, index)
        return ExprTree.Call(
            typeof(RuntimeHelpers),
            nameof(RuntimeHelpers.GetIndexed),
            null,
            targetExpr,
            indexExpr
        );
    }

    #endregion

    #region Task #25: リスト・ハッシュの実装

    /// <summary>
    /// リストリテラルの変換
    /// 仕様: [1, 2, 3] → RuntimeHelpers.CreateList(new object[] { 1, 2, 3 })
    /// </summary>
    private ExprTree GenerateListExpr(ListExpr expr)
    {
        var elemExprs = expr.Elements.Select(e => GenerateExpression(e)).ToArray();
        var arrayExpr = ExprTree.NewArrayInit(typeof(object), elemExprs);

        return ExprTree.Call(
            typeof(RuntimeHelpers),
            nameof(RuntimeHelpers.CreateList),
            Type.EmptyTypes,
            arrayExpr
        );
    }

    /// <summary>
    /// ハッシュリテラルの変換
    /// 仕様: {name: "Alice", age: 30} → RuntimeHelpers.CreateHash(new[] { ("name", "Alice"), ("age", 30) })
    /// </summary>
    private ExprTree GenerateHashExpr(HashExpr expr)
    {
        var tupleType = typeof(ValueTuple<string, object>);
        var tupleConstructor = tupleType.GetConstructor(new[] { typeof(string), typeof(object) })!;

        var pairExprs = expr.Pairs.Select(p =>
            ExprTree.New(
                tupleConstructor,
                ExprTree.Constant(p.Key),
                GenerateExpression(p.Value)
            )
        ).ToArray();

        var pairArrayExpr = ExprTree.NewArrayInit(tupleType, pairExprs);

        return ExprTree.Call(
            typeof(RuntimeHelpers),
            nameof(RuntimeHelpers.CreateHash),
            Type.EmptyTypes,
            pairArrayExpr
        );
    }

    /// <summary>
    /// 範囲リテラルの変換
    /// 仕様: 1..10 → RuntimeHelpers.CreateRange(1, 10, false)
    ///       1...10 → RuntimeHelpers.CreateRange(1, 10, true)
    /// </summary>
    private ExprTree GenerateRangeExpr(RangeExpr expr)
    {
        var startExpr = GenerateExpression(expr.Start);
        var endExpr = GenerateExpression(expr.End);
        var inclusiveExpr = ExprTree.Constant(expr.Inclusive);

        return ExprTree.Call(
            typeof(RuntimeHelpers),
            nameof(RuntimeHelpers.CreateRange),
            Type.EmptyTypes,
            startExpr,
            endExpr,
            inclusiveExpr
        );
    }

    /// <summary>
    /// インデックス代入の変換
    /// 仕様: arr[0] = value → RuntimeHelpers.SetIndexed(arr, 0, value)
    /// </summary>
    private ExprTree GenerateIndexAssignExpr(IndexAssignExpr expr)
    {
        var targetExpr = GenerateExpression(expr.Target);
        var indexExpr = GenerateExpression(expr.Index);
        var valueExpr = GenerateExpression(expr.Value);

        return ExprTree.Call(
            typeof(RuntimeHelpers),
            nameof(RuntimeHelpers.SetIndexed),
            Type.EmptyTypes,
            targetExpr,
            indexExpr,
            valueExpr
        );
    }

    /// <summary>
    /// メンバ代入の変換
    /// 仕様: obj.field = value → RuntimeHelpers.SetMember(obj, "field", value)
    /// </summary>
    private ExprTree GenerateMemberAssignExpr(MemberAssignExpr expr)
    {
        var objExpr = GenerateExpression(expr.Target);
        var nameExpr = ExprTree.Constant(expr.MemberName);
        var valExpr = GenerateExpression(expr.Value);

        return ExprTree.Call(
            typeof(RuntimeHelpers),
            nameof(RuntimeHelpers.SetMember),
            Type.EmptyTypes,
            objExpr,
            nameExpr,
            valExpr
        );
    }

    /// <summary>
    /// 文字列補間の変換
    /// 仕様: "Hello, ${name}!" → string.Concat("Hello, ", name.ToString(), "!")
    /// </summary>
    private ExprTree GenerateStringInterpolationExpr(StringInterpolationExpr expr)
    {
        // 各パートを式に変換
        var partExprs = expr.Parts.Select<object, ExprTree>(part =>
        {
            if (part is string str)
            {
                // 文字列パートはそのまま
                return ExprTree.Constant(str, typeof(object));
            }
            else
            {
                // 式パートはToString()を呼び出す
                var partExpr = GenerateExpression((AstExpr)part);
                var toStringMethod = typeof(object).GetMethod("ToString")!;
                return ExprTree.Call(partExpr, toStringMethod);
            }
        }).ToArray();

        // string.Concat(object[]) を呼び出す
        var concatMethod = typeof(string).GetMethod(
            nameof(string.Concat),
            new[] { typeof(object[]) }
        )!;

        var arrayExpr = ExprTree.NewArrayInit(typeof(object), partExprs);
        return ExprTree.Call(concatMethod, arrayExpr);
    }

    #endregion

    #region try/catch/finally の生成

    /// <summary>
    /// try/catch/finally式の生成
    /// </summary>
    private ExprTree GenerateTryExpr(TryExpr tryExpr)
    {
        // try ブロックの本体
        var tryBody = GenerateExpression(tryExpr.TryBody);

        // catch ブロック（オプション）
        CatchBlock? catchBlock = null;
        if (tryExpr.Catch != null)
        {
            // 例外変数を作成（IroExceptionをキャッチ）
            var exceptionParam = ExprTree.Parameter(typeof(IroException), "ex");

            // 例外変数が指定されている場合、ctx.Globals に登録
            ExprTree catchBody;
            if (tryExpr.Catch.ExceptionVariable != null)
            {
                // ex.Value を取得
                var exceptionValueExpr = ExprTree.Property(exceptionParam, nameof(IroException.Value));

                // ctx.Globals["e"] = ex.Value
                var globalsExpr = ExprTree.Property(_ctxParam, "Globals");
                var nameExpr = ExprTree.Constant(tryExpr.Catch.ExceptionVariable);
                var itemProperty = ExprTree.Property(globalsExpr, "Item", nameExpr);
                var assignExpr = ExprTree.Assign(itemProperty, exceptionValueExpr);

                // catch ブロック本体を生成
                var catchBodyExpr = GenerateExpression(tryExpr.Catch.Body);

                // Block で例外変数を登録して本体を実行
                catchBody = ExprTree.Block(
                    assignExpr,
                    catchBodyExpr
                );
            }
            else
            {
                // 例外変数なしの場合、そのまま本体を実行
                catchBody = GenerateExpression(tryExpr.Catch.Body);
            }

            catchBlock = ExprTree.Catch(exceptionParam, catchBody);
        }

        // finally ブロック（オプション）
        ExprTree? finallyBody = null;
        if (tryExpr.Finally != null)
        {
            finallyBody = GenerateExpression(tryExpr.Finally);
        }

        // TryCatchFinally を構築
        if (catchBlock != null && finallyBody != null)
        {
            return ExprTree.TryCatchFinally(tryBody, finallyBody, catchBlock);
        }
        else if (catchBlock != null)
        {
            return ExprTree.TryCatch(tryBody, catchBlock);
        }
        else if (finallyBody != null)
        {
            return ExprTree.TryFinally(tryBody, finallyBody);
        }
        else
        {
            // catch も finally もない場合（通常はパーサーで弾かれるはず）
            throw new InvalidOperationException("Try expression must have catch or finally block.");
        }
    }

    /// <summary>
    /// throw文の生成
    /// </summary>
    private ExprTree GenerateThrowStmt(ThrowStmt throwStmt)
    {
        // throw する値を評価
        var valueExpr = GenerateExpression(throwStmt.Value);

        // IroException(value) のコンストラクタを呼び出す
        var ctor = typeof(IroException).GetConstructor(new[] { typeof(object) })!;
        var newException = ExprTree.New(ctor, valueExpr);

        // throw 式を生成
        return ExprTree.Throw(newException, typeof(object));
    }

    /// <summary>
    /// export文の変換
    /// 仕様: エクスポートする宣言を実行し、ctx.Exports に登録
    /// </summary>
    private ExprTree GenerateExportStmt(ExportStmt stmt)
    {
        // エクスポートする宣言を生成
        var declExpr = GenerateStatement(stmt.Declaration);

        // エクスポート名を取得
        string exportName;
        if (stmt.Declaration is LetStmt letStmt)
        {
            exportName = letStmt.Name;
        }
        else if (stmt.Declaration is FunctionDef funcDef)
        {
            exportName = funcDef.Name;
        }
        else
        {
            throw new NotImplementedException($"Export of {stmt.Declaration.GetType().Name} is not supported");
        }

        // ctx.Exports[name] = ctx.Globals[name]
        var exportsExpr = ExprTree.Property(_ctxParam, "Exports");
        var globalsExpr = ExprTree.Property(_ctxParam, "Globals");
        var nameExpr = ExprTree.Constant(exportName);
        var exportValue = ExprTree.Property(globalsExpr, "Item", nameExpr);
        var exportAssign = ExprTree.Assign(
            ExprTree.Property(exportsExpr, "Item", nameExpr),
            exportValue
        );

        // 宣言の実行とエクスポートを順に実行
        return ExprTree.Block(declExpr, exportAssign);
    }

    /// <summary>
    /// import文の変換
    /// 仕様: ModuleLoader を使ってモジュールをロードし、インポートする名前を ctx.Globals に登録
    /// </summary>
    private ExprTree GenerateImportStmt(ImportStmt stmt)
    {
        // TODO: 現時点では簡略化のため、実装をスキップします
        // 実際の実装では、ModuleLoaderを呼び出してモジュールをロードし、
        // インポートする名前を ctx.Globals に登録する必要があります

        // 空のブロックを返す（何もしない）
        return ExprTree.Empty();
    }

    #endregion
}
