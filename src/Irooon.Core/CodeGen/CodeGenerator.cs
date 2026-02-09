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
/// ASTノ�EドをDLR�E�Eystem.Linq.Expressions�E��EExpressionTreeに変換します、E
/// Task #13: 基本式とリチE��ルの実裁E
/// </summary>
public class CodeGenerator
{
    private ParameterExpression _ctxParam; // ScriptContext ctx
    private int _labelCounter = 0; // ラベルの一意性確保用

    // ループ�Ebreak/continueラベルを管琁E��るスタチE��
    private Stack<(LabelTarget breakLabel, LabelTarget? continueLabel)> _loopLabels = new();

    public CodeGenerator()
    {
        _ctxParam = ExprTree.Parameter(typeof(ScriptContext), "ctx");
    }

    /// <summary>
    /// トップレベルのコンパイル
    /// ASTをFunc&lt;ScriptContext, object&gt;にコンパイルしまぁE
    /// </summary>
    /// <param name="program">プログラム全体を表すBlockExpr</param>
    /// <returns>実行可能なチE��ゲーチE/returns>
    public Func<ScriptContext, object?> Compile(BlockExpr program)
    {
        var bodyExpr = GenerateBlockExpr(program);
        var lambda = ExprTree.Lambda<Func<ScriptContext, object?>>(
            bodyExpr,
            _ctxParam
        );
        return lambda.Compile();
    }

    #region 式�E生�E�E�ディスパッチE��E

    /// <summary>
    /// 式�E生�E�E�ディスパッチE��E
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
            ShellExpr e => GenerateShellExpr(e),
            TernaryExpr e => GenerateTernaryExpr(e),
            NullCoalescingExpr e => GenerateNullCoalescingExpr(e),
            IncrementExpr e => GenerateIncrementExpr(e),
            AwaitExpr e => GenerateAwaitExpr(e),
            SafeNavigationExpr e => GenerateSafeNavigationExpr(e),
            SuperExpr e => GenerateSuperExpr(e),
            _ => throw new NotImplementedException($"Unknown expression type: {expr.GetType()}")
        };
    }

    #endregion

    #region 斁E�E生�E�E�ディスパッチE��E

    /// <summary>
    /// 斁E�E生�E�E�ディスパッチE��E
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

    #region Task #13: 基本式�E実裁E

    /// <summary>
    /// リチE��ル式�E変換
    /// 仕槁E 数値はdoubleに統一、objectにボックス匁E
    /// </summary>
    private ExprTree GenerateLiteralExpr(LiteralExpr expr)
    {
        // 数値はdoubleに統一、objectにボックス匁E
        if (expr.Value is double d)
        {
            return ExprTree.Convert(
                ExprTree.Constant(d, typeof(double)),
                typeof(object)
            );
        }

        // 斁E���E、bool、null
        return ExprTree.Constant(expr.Value, typeof(object));
    }

    /// <summary>
    /// 識別子式�E変換�E�変数参�E�E�E
    /// 仕槁E ctx.Globals["name"]
    /// </summary>
    private ExprTree GenerateIdentifierExpr(IdentifierExpr expr)
    {
        // ctx.Globals["name"]
        var globalsExpr = ExprTree.Property(_ctxParam, "Globals");
        var nameExpr = ExprTree.Constant(expr.Name);
        return ExprTree.Property(globalsExpr, "Item", nameExpr);
    }

    /// <summary>
    /// 代入式�E変換
    /// 仕槁E ctx.Globals["name"] = value
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
    /// let斁E�E変換
    /// 仕槁E let/varの違いはResolverで検査済み、CodeGenは同じ
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
    /// var斁E�E変換
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
    /// ブロチE��式�E変換
    /// 仕槁E Expression.Block(vars, exprs...)
    /// 最後�E式が値になめE
    /// </summary>
    private ExprTree GenerateBlockExpr(BlockExpr expr)
    {
        var expressions = new List<ExprTree>();

        // 斁E��変換
        foreach (var stmt in expr.Statements)
        {
            expressions.Add(GenerateStatement(stmt));
        }

        // 最後�E弁E
        if (expr.Expression != null)
        {
            expressions.Add(GenerateExpression(expr.Expression));
        }
        else if (expressions.Count == 0 || expr.Statements.Count == 0 ||
                 expr.Statements[^1] is not ReturnStmt)
        {
            // 最後が斁E�Eみの場合！Eeturn斁E��外）�Enullを追加
            expressions.Add(ExprTree.Constant(null, typeof(object)));
        }

        return ExprTree.Block(typeof(object), expressions);
    }

    #endregion

    #region Task #14: 演算子�E実裁E

    /// <summary>
    /// 二頁E��算式�E変換
    /// 仕槁E すべてRuntimeHelpersに委譲
    /// </summary>
    private ExprTree GenerateBinaryExpr(BinaryExpr expr)
    {
        var left = GenerateExpression(expr.Left);
        var right = GenerateExpression(expr.Right);

        var runtimeType = typeof(RuntimeHelpers);

        return expr.Operator switch
        {
            // 算術演算孁E
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

            // 比輁E��算孁E
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

            // 論理演算子（短絡評価�E�E
            TokenType.And => GenerateAndExpr(expr),
            TokenType.Or => GenerateOrExpr(expr),

            _ => throw new NotImplementedException($"Operator {expr.Operator} not implemented")
        };
    }

    /// <summary>
    /// 論理AND演算子�E短絡評価
    /// 仕槁E a and b ↁEtruthy(a) ? b : a
    /// </summary>
    private ExprTree GenerateAndExpr(BinaryExpr expr)
    {
        var left = GenerateExpression(expr.Left);
        var right = GenerateExpression(expr.Right);

        // 左辺を一時変数に格納（一度だけ評価�E�E
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
    /// 論理OR演算子�E短絡評価
    /// 仕槁E a or b ↁEtruthy(a) ? a : b
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
    /// 単頁E��算式�E変換
    /// 仕槁E すべてRuntimeHelpersに委譲
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

    #region Task #15: 制御構造の実裁E

    /// <summary>
    /// if式�E変換
    /// 仕様！Expression-tree-mapping.md セクション7�E�E
    /// condTruth = Runtime.IsTruthy(condObj)
    /// Expression.Condition(condTruth, thenObj, elseObj)
    /// </summary>
    private ExprTree GenerateIfExpr(IfExpr expr)
    {
        var condExpr = GenerateExpression(expr.Condition);
        var thenExpr = GenerateExpression(expr.ThenBranch);
        var elseExpr = GenerateExpression(expr.ElseBranch);

        // IsTruthy で真偽値判宁E
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
    /// foreach斁E�E変換
    /// foreach (item in collection) { body }
    /// </summary>
    private ExprTree GenerateForeachStmt(ForeachStmt stmt)
    {
        var breakLabel = ExprTree.Label($"foreachBreak_{_labelCounter}");
        var continueLabel = ExprTree.Label($"foreachContinue_{_labelCounter++}");

        // コレクション式を評価
        var collectionExpr = GenerateExpression(stmt.Collection);

        // IEnumerable にキャスチE
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

        // ループラベルをスタチE��にプッシュ
        _loopLabels.Push((breakLabel, continueLabel));

        var bodyExpr = GenerateStatement(stmt.Body);

        // ループラベルを�EチE�E
        _loopLabels.Pop();

        // ループ本佁E
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

        // foreach全佁E
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
    /// for斁E�E変換
    /// パターン1: for (item in collection) { body } - コレクション反復
    /// パターン2: for (condition) { body } - 条件ルーチE
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
    /// foreach と同じ実裁E
    /// </summary>
    private ExprTree GenerateForCollection(ForStmt stmt)
    {
        var breakLabel = ExprTree.Label($"forBreak_{_labelCounter}");
        var continueLabel = ExprTree.Label($"forContinue_{_labelCounter++}");

        // コレクション式を評価
        var collectionExpr = GenerateExpression(stmt.Collection!);

        // IEnumerable にキャスチE
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

        // ループラベルをスタチE��にプッシュ
        _loopLabels.Push((breakLabel, continueLabel));

        var bodyExpr = GenerateExpression(stmt.Body);

        // ループラベルを�EチE�E
        _loopLabels.Pop();

        // ループ本佁E
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

        // for全佁E
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
    /// while と同じ実裁E
    /// </summary>
    private ExprTree GenerateForCondition(ForStmt stmt)
    {
        var breakLabel = ExprTree.Label($"forBreak_{_labelCounter}");
        var continueLabel = ExprTree.Label($"forContinue_{_labelCounter++}");

        // ループラベルをスタチE��にプッシュ
        _loopLabels.Push((breakLabel, continueLabel));

        var condExpr = GenerateExpression(stmt.Condition!);
        var bodyExpr = GenerateExpression(stmt.Body);

        // ループラベルを�EチE�E
        _loopLabels.Pop();

        // IsTruthy で真偽値判宁E
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
    /// break斁E�E変換
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
    /// continue斁E�E変換
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
    /// return斁E�E変換
    /// v0.1では単純に式を返す
    /// �E�関数冁E��のreturnは後で実裁E��E
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

    #region Task #15以降�E実裁E��スタブ！E

    // Task #16: 関数とクロージャの実裁E��簡易実裁E��E
    /// <summary>
    /// 関数呼び出し�E変換
    /// 仕槁E RuntimeHelpers.Invoke(callee, ctx, args, thisArg)
    /// </summary>
    private ExprTree GenerateCallExpr(CallExpr expr)
    {
        // CalleeがMemberExprの場合、CLR型�EメソチE��呼び出しまた�EコンストラクタかチェチE��
        if (expr.Callee is MemberExpr memberExpr)
        {
            var (isCLRType, typeName, methodName) = ExtractCLRTypeName(memberExpr);

            if (isCLRType)
            {
                // methodNameが空の場合、コンストラクタ呼び出ぁE
                if (string.IsNullOrEmpty(methodName))
                {
                    // CLR型�Eコンストラクタを生戁E
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

                // CLR型�EメソチE��呼び出しを生�E
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

            // 通常のインスタンスメソチE��呼び出ぁE
            var targetExpr = GenerateExpression(memberExpr.Target);
            var thisArg = targetExpr;

            // メソチE��を取征E
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
        // CalleeがIdentifierExprまた�EMemberExprで、CLR型�Eコンストラクタ呼び出しかチェチE��
        else if (expr.Callee is IdentifierExpr identExpr)
        {
            // ドット区刁E��の型名をチェチE���E�侁E System.Text.StringBuilder�E�E
            var typeName = identExpr.Name;

            // System で始まる場合、CLR型�Eコンストラクタと判断
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

        // 通常の関数呼び出ぁE
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
    /// MemberExprからCLR型名を抽出します、E
    /// </summary>
    /// <param name="memberExpr">MemberExpr</param>
    /// <returns>(isCLRType, typeName, methodName)</returns>
    private (bool isCLRType, string typeName, string methodName) ExtractCLRTypeName(MemberExpr memberExpr)
    {
        var parts = new List<string>();
        var current = memberExpr;

        // MemberExprをたどってドット区刁E��の名前を構篁E
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
                // CLR型ではなぁE
                return (false, "", "");
            }
        }

        // System で始まる場合、CLR型とみなぁE
        if (parts.Count >= 2 && parts[0] == "System")
        {
            // 型名全体を構築して確誁E
            var fullTypeName = string.Join(".", parts);

            // 型が解決できるかチェチE��
            var resolvedType = RuntimeHelpers.ResolveCLRType(fullTypeName);
            if (resolvedType != null)
            {
                // コンストラクタ呼び出し（型名�E体で解決できた場合！E
                return (true, fullTypeName, "");
            }

            // 最後�E要素がメソチE��吁E
            var methodName = parts[^1];
            var typeName = string.Join(".", parts.Take(parts.Count - 1));
            return (true, typeName, methodName);
        }

        return (false, "", "");
    }

    /// <summary>
    /// ラムダ式�E変換
    /// 仕様！Expression-tree-mapping.md セクション9�E�E
    /// Closureオブジェクトを生�Eし、IroCallableとして扱ぁE
    /// </summary>
    private ExprTree GenerateLambdaExpr(LambdaExpr expr)
    {
        // 関数本体を Func<ScriptContext, object[], object> にコンパイル
        var argsParam = ExprTree.Parameter(typeof(object[]), "args");
        var ctxParamForFunc = ExprTree.Parameter(typeof(ScriptContext), "ctx");

        var bodyExprs = new List<ExprTree>();

        // パラメータめEargs[0], args[1], ... にバインチE
        for (int i = 0; i < expr.Parameters.Count; i++)
        {
            var param = expr.Parameters[i];
            var argAccess = ExprTree.ArrayIndex(argsParam, ExprTree.Constant(i));
            var globalsForParam = ExprTree.Property(ctxParamForFunc, "Globals");
            var paramName = ExprTree.Constant(param.Name);
            var itemForParam = ExprTree.Property(globalsForParam, "Item", paramName);
            bodyExprs.Add(ExprTree.Assign(itemForParam, argAccess));
        }

        // 本体を実行（一時的に _ctxParam を�Eり替える�E�E
        var savedCtxParam = _ctxParam;
        _ctxParam = ctxParamForFunc;
        var bodyExpr = GenerateExpression(expr.Body);
        _ctxParam = savedCtxParam;

        bodyExprs.Add(bodyExpr);

        var bodyBlock = ExprTree.Block(typeof(object), bodyExprs);

        // Lambda<Func<ScriptContext, object[], object>> を作�E
        var lambda = ExprTree.Lambda<Func<ScriptContext, object[], object>>(
            bodyBlock,
            ctxParamForFunc,
            argsParam
        );

        var compiled = lambda.Compile();

        // パラメータ名�Eリストを作�E
        var paramNames = expr.Parameters.Select(p => p.Name).ToList();
        var paramNamesListNew = ExprTree.New(
            typeof(List<string>).GetConstructor(new[] { typeof(IEnumerable<string>) })!,
            ExprTree.NewArrayInit(typeof(string), paramNames.Select(n => ExprTree.Constant(n)))
        );

        // Closure オブジェクトを作�E�E�位置惁E��を含む�E�E
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
    /// 仕様！Expression-tree-mapping.md セクション9�E�E
    /// Closureオブジェクトを生�Eし、ctx.Globals[name] に登録
    /// </summary>
    private ExprTree GenerateFunctionDef(FunctionDef stmt)
    {
        // Check if this is an async function
        if (stmt.IsAsync)
        {
            return GenerateAsyncFunctionDef(stmt);
        }

        // 関数本体を Func<ScriptContext, object[], object> にコンパイル
        var argsParam = ExprTree.Parameter(typeof(object[]), "args");
        var ctxParamForFunc = ExprTree.Parameter(typeof(ScriptContext), "ctx");

        var bodyExprs = new List<ExprTree>();

        // パラメータめEargs[0], args[1], ... にバインチE
        for (int i = 0; i < stmt.Parameters.Count; i++)
        {
            var param = stmt.Parameters[i];
            var argAccess = ExprTree.ArrayIndex(argsParam, ExprTree.Constant(i));
            var globalsForParam = ExprTree.Property(ctxParamForFunc, "Globals");
            var paramName = ExprTree.Constant(param.Name);
            var itemForParam = ExprTree.Property(globalsForParam, "Item", paramName);
            bodyExprs.Add(ExprTree.Assign(itemForParam, argAccess));
        }

        // 本体を実行（一時的に _ctxParam を�Eり替える�E�E
        var savedCtxParam = _ctxParam;
        _ctxParam = ctxParamForFunc;
        var bodyExpr = GenerateExpression(stmt.Body);
        _ctxParam = savedCtxParam;

        bodyExprs.Add(bodyExpr);

        var bodyBlock = ExprTree.Block(typeof(object), bodyExprs);

        // Lambda<Func<ScriptContext, object[], object>> を作�E
        var lambda = ExprTree.Lambda<Func<ScriptContext, object[], object>>(
            bodyBlock,
            ctxParamForFunc,
            argsParam
        );

        var compiled = lambda.Compile();

        // パラメータ名�Eリストを作�E
        var paramNames = stmt.Parameters.Select(p => p.Name).ToList();
        var paramNamesListNew = ExprTree.New(
            typeof(List<string>).GetConstructor(new[] { typeof(IEnumerable<string>) })!,
            ExprTree.NewArrayInit(typeof(string), paramNames.Select(n => ExprTree.Constant(n)))
        );

        // Closure オブジェクトを作�E�E�位置惁E��を含む�E�E
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

    /// <summary>
    /// �񓯊��֐���`�̐���
    /// Task<object>��Ԃ��֐��𐶐�
    /// </summary>
    private ExprTree GenerateAsyncFunctionDef(FunctionDef stmt)
    {
        // �֐��{�̂� Func<ScriptContext, object[], object> �ɃR���p�C��
        var argsParam = ExprTree.Parameter(typeof(object[]), "args");
        var ctxParamForFunc = ExprTree.Parameter(typeof(ScriptContext), "ctx");

        var bodyExprs = new List<ExprTree>();

        // �p�����[�^�� args[0], args[1], ... �Ƀo�C���h
        for (int i = 0; i < stmt.Parameters.Count; i++)
        {
            var param = stmt.Parameters[i];
            var argAccess = ExprTree.ArrayIndex(argsParam, ExprTree.Constant(i));
            var globalsForParam = ExprTree.Property(ctxParamForFunc, "Globals");
            var paramName = ExprTree.Constant(param.Name);
            var itemForParam = ExprTree.Property(globalsForParam, "Item", paramName);
            bodyExprs.Add(ExprTree.Assign(itemForParam, argAccess));
        }

        // �{�̂����s�i�ꎞ�I�� _ctxParam ��؂�ւ���j
        var savedCtxParam = _ctxParam;
        _ctxParam = ctxParamForFunc;
        var bodyExpr = GenerateExpression(stmt.Body);
        _ctxParam = savedCtxParam;

        bodyExprs.Add(bodyExpr);

        var bodyBlock = ExprTree.Block(typeof(object), bodyExprs);

        // Wrap the result in Task.FromResult to make it async
        // RuntimeHelpers.WrapInTask(result)
        var wrapInTaskMethod = typeof(RuntimeHelpers).GetMethod(
            "WrapInTask",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static
        );

        if (wrapInTaskMethod == null)
        {
            throw new InvalidOperationException("WrapInTask method not found in RuntimeHelpers");
        }

        var wrappedBody = ExprTree.Call(wrapInTaskMethod, bodyBlock);

        // Lambda<Func<ScriptContext, object[], object>> ���쐬
        // The async function returns Task<object>, but we wrap it to return object
        var asyncLambda = ExprTree.Lambda<Func<ScriptContext, object[], object>>(
            ExprTree.Convert(wrappedBody, typeof(object)),
            ctxParamForFunc,
            argsParam
        );

        var compiled = asyncLambda.Compile();

        // �p�����[�^���̃��X�g���쐬
        var paramNames = stmt.Parameters.Select(p => p.Name).ToList();
        var paramNamesListNew = ExprTree.New(
            typeof(List<string>).GetConstructor(new[] { typeof(IEnumerable<string>) })!,
            ExprTree.NewArrayInit(typeof(string), paramNames.Select(n => ExprTree.Constant(n)))
        );

        // Closure �I�u�W�F�N�g���쐬�i�ʒu�����܂ށj
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

    // Task #17: クラスとインスタンスの実裁E

    /// <summary>
    /// クラス定義の変換
    /// 仕槁E IroClass オブジェクトを作�Eし、ctx.Classes に登録
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

        // メソチE��定義を変換
        var methodDefs = stmt.Methods.Select(m =>
        {
            // メソチE��本体をClosureにコンパイル
            var argsParam = ExprTree.Parameter(typeof(object[]), "args");
            var ctxParamForFunc = ExprTree.Parameter(typeof(ScriptContext), "ctx");

            // 一時的にctxParamを�Eり替える
            var savedCtxParam = _ctxParam;
            _ctxParam = ctxParamForFunc;

            var bodyExprs = new List<ExprTree>();

            // 引数めEctx.Globals に格紁E
            for (int i = 0; i < m.Parameters.Count; i++)
            {
                var param = m.Parameters[i];
                var argAccess = ExprTree.ArrayIndex(argsParam, ExprTree.Constant(i));
                var globalsExpr = ExprTree.Property(ctxParamForFunc, "Globals");
                var paramName = ExprTree.Constant(param.Name);
                var itemProperty = ExprTree.Property(globalsExpr, "Item", paramName);
                bodyExprs.Add(ExprTree.Assign(itemProperty, argAccess));
            }

            // メソチE��本体を追加
            bodyExprs.Add(GenerateExpression(m.Body));

            // ctxParamを�Eに戻ぁE
            _ctxParam = savedCtxParam;

            var bodyBlock = ExprTree.Block(typeof(object), bodyExprs);
            var lambda = ExprTree.Lambda<Func<ScriptContext, object[], object>>(
                bodyBlock,
                ctxParamForFunc,
                argsParam
            );

            var compiled = lambda.Compile();

            // パラメータ名�Eリストを作�E
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

        // 親クラスを取得（存在する場合！E
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

        // IroClass を作�E
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
    /// インスタンス生�Eの変換
    /// 仕槁E Runtime.NewInstance(className, ctx, args)
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
    /// 仕槁E Runtime.GetMember(target, name)
    /// </summary>
    private ExprTree GenerateMemberExpr(MemberExpr expr)
    {
        // CLR型�EプロパティアクセスかチェチE��
        var (isCLRType, typeName, propertyName) = ExtractCLRTypeName(expr);

        if (isCLRType)
        {
            // CLR型�Eプロパティアクセスを生戁E
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
    /// インチE��クスアクセスの変換
    /// 仕槁E RuntimeHelpers.GetIndexed を使用
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

    #region Task #25: リスト�Eハッシュの実裁E

    /// <summary>
    /// リストリチE��ルの変換
    /// 仕槁E [1, 2, 3] ↁERuntimeHelpers.CreateList(new object[] { 1, 2, 3 })
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
    /// ハッシュリチE��ルの変換
    /// 仕槁E {name: "Alice", age: 30} ↁERuntimeHelpers.CreateHash(new[] { ("name", "Alice"), ("age", 30) })
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
    /// 篁E��リチE��ルの変換
    /// 仕槁E 1..10 ↁERuntimeHelpers.CreateRange(1, 10, false)
    ///       1...10 ↁERuntimeHelpers.CreateRange(1, 10, true)
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
    /// インチE��クス代入の変換
    /// 仕槁E arr[0] = value ↁERuntimeHelpers.SetIndexed(arr, 0, value)
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
    /// 仕槁E obj.field = value ↁERuntimeHelpers.SetMember(obj, "field", value)
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
    /// 斁E���E補間の変換
    /// 仕槁E "Hello, ${name}!" ↁEstring.Concat("Hello, ", name.ToString(), "!")
    /// </summary>
    private ExprTree GenerateStringInterpolationExpr(StringInterpolationExpr expr)
    {
        // 吁E��ートを式に変換
        var partExprs = expr.Parts.Select<object, ExprTree>(part =>
        {
            if (part is string str)
            {
                // 斁E���Eパ�Eト�Eそ�Eまま
                return ExprTree.Constant(str, typeof(object));
            }
            else
            {
                // 式パート�EToString()を呼び出ぁE
                var partExpr = GenerateExpression((AstExpr)part);
                var toStringMethod = typeof(object).GetMethod("ToString")!;
                return ExprTree.Call(partExpr, toStringMethod);
            }
        }).ToArray();

        // string.Concat(object[]) を呼び出ぁE
        var concatMethod = typeof(string).GetMethod(
            nameof(string.Concat),
            new[] { typeof(object[]) }
        )!;

        var arrayExpr = ExprTree.NewArrayInit(typeof(object), partExprs);
        return ExprTree.Call(concatMethod, arrayExpr);
    }

    #endregion

    #region try/catch/finally の生�E

    /// <summary>
    /// try/catch/finally式�E生�E
    /// </summary>
    private ExprTree GenerateTryExpr(TryExpr tryExpr)
    {
        // try ブロチE��の本佁E
        var tryBody = GenerateExpression(tryExpr.TryBody);

        // catch ブロチE���E�オプション�E�E
        CatchBlock? catchBlock = null;
        if (tryExpr.Catch != null)
        {
            // 例外変数を作�E�E�EroExceptionをキャチE���E�E
            var exceptionParam = ExprTree.Parameter(typeof(IroException), "ex");

            // 例外変数が指定されてぁE��場合、ctx.Globals に登録
            ExprTree catchBody;
            if (tryExpr.Catch.ExceptionVariable != null)
            {
                // ex.Value を取征E
                var exceptionValueExpr = ExprTree.Property(exceptionParam, nameof(IroException.Value));

                // ctx.Globals["e"] = ex.Value
                var globalsExpr = ExprTree.Property(_ctxParam, "Globals");
                var nameExpr = ExprTree.Constant(tryExpr.Catch.ExceptionVariable);
                var itemProperty = ExprTree.Property(globalsExpr, "Item", nameExpr);
                var assignExpr = ExprTree.Assign(itemProperty, exceptionValueExpr);

                // catch ブロチE��本体を生�E
                var catchBodyExpr = GenerateExpression(tryExpr.Catch.Body);

                // Block で例外変数を登録して本体を実衁E
                catchBody = ExprTree.Block(
                    assignExpr,
                    catchBodyExpr
                );
            }
            else
            {
                // 例外変数なし�E場合、そのまま本体を実衁E
                catchBody = GenerateExpression(tryExpr.Catch.Body);
            }

            catchBlock = ExprTree.Catch(exceptionParam, catchBody);
        }

        // finally ブロチE���E�オプション�E�E
        ExprTree? finallyBody = null;
        if (tryExpr.Finally != null)
        {
            finallyBody = GenerateExpression(tryExpr.Finally);
        }

        // TryCatchFinally を構篁E
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
            // catch めEfinally もなぁE��合（通常はパ�Eサーで弾かれる�Eず！E
            throw new InvalidOperationException("Try expression must have catch or finally block.");
        }
    }

    /// <summary>
    /// throw斁E�E生�E
    /// </summary>
    private ExprTree GenerateThrowStmt(ThrowStmt throwStmt)
    {
        // throw する値を評価
        var valueExpr = GenerateExpression(throwStmt.Value);

        // IroException(value) のコンストラクタを呼び出ぁE
        var ctor = typeof(IroException).GetConstructor(new[] { typeof(object) })!;
        var newException = ExprTree.New(ctor, valueExpr);

        // throw 式を生�E
        return ExprTree.Throw(newException, typeof(object));
    }

    /// <summary>
    /// export斁E�E変換
    /// 仕槁E エクスポ�Eトする宣言を実行し、ctx.Exports に登録
    /// </summary>
    private ExprTree GenerateExportStmt(ExportStmt stmt)
    {
        // エクスポ�Eトする宣言を生戁E
        var declExpr = GenerateStatement(stmt.Declaration);

        // エクスポ�Eト名を取征E
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

        // 宣言の実行とエクスポ�Eトを頁E��実衁E
        return ExprTree.Block(declExpr, exportAssign);
    }

    /// <summary>
    /// import斁E�E変換
    /// 仕槁E ModuleLoader を使ってモジュールをロードし、インポ�Eトする名前を ctx.Globals に登録
    /// </summary>
    private ExprTree GenerateImportStmt(ImportStmt stmt)
    {
        // TODO: 現時点では簡略化�Eため、実裁E��スキチE�EしまぁE
        // 実際の実裁E��は、ModuleLoaderを呼び出してモジュールをロードし、E
        // インポ�Eトする名前を ctx.Globals に登録する忁E��がありまぁE

        // 空のブロチE��を返す�E�何もしなぁE��E
        return ExprTree.Empty();
    }

    /// <summary>
    /// シェルコマンド実行式を生�EしまぁE
    /// </summary>
    private ExprTree GenerateShellExpr(ShellExpr expr)
    {
        // RuntimeHelpers.ExecuteShellCommand(command) を呼び出ぁE
        var method = typeof(RuntimeHelpers).GetMethod(
            nameof(RuntimeHelpers.ExecuteShellCommand),
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static
        );

        if (method == null)
        {
            throw new InvalidOperationException("ExecuteShellCommand method not found");
        }

        var commandExpr = ExprTree.Constant(expr.Command);

        return ExprTree.Call(method, commandExpr);
    }

    #endregion

    #region v0.5.6: 便利な演算子�E実裁E

    /// <summary>
    /// 三頁E��算式�E生�E
    /// 仕槁E condition ? trueValue : falseValue ↁEExpression.Condition(IsTruthy(condition), trueValue, falseValue)
    /// </summary>
    private ExprTree GenerateTernaryExpr(TernaryExpr expr)
    {
        var conditionExpr = GenerateExpression(expr.Condition);
        var trueValueExpr = GenerateExpression(expr.TrueValue);
        var falseValueExpr = GenerateExpression(expr.FalseValue);

        // IsTruthy で真偽値判宁E
        var truthyCall = ExprTree.Call(
            typeof(RuntimeHelpers),
            nameof(RuntimeHelpers.IsTruthy),
            null,
            conditionExpr
        );

        // Expression.Condition
        return ExprTree.Condition(
            truthyCall,
            trueValueExpr,
            falseValueExpr,
            typeof(object)
        );
    }

    /// <summary>
    /// Null合体演算式�E生�E
    /// 仕槁E value ?? defaultValue ↁEExpression.Coalesce(value, defaultValue)
    /// </summary>
    private ExprTree GenerateNullCoalescingExpr(NullCoalescingExpr expr)
    {
        var valueExpr = GenerateExpression(expr.Value);
        var defaultValueExpr = GenerateExpression(expr.DefaultValue);

        // Expression.Coalesce
        return ExprTree.Coalesce(valueExpr, defaultValueExpr);
    }

    /// <summary>
    /// インクリメンチEチE��リメント演算式�E生�E
    /// 仕槁E
    /// - 前置�E�E+x, --x�E�E var temp = Increment/Decrement(value); SetVariable(temp); return temp;
    /// - 後置�E�E++, x--�E�E var temp = value; SetVariable(Increment/Decrement(value)); return temp;
    /// </summary>
    private ExprTree GenerateIncrementExpr(IncrementExpr expr)
    {
        // オペランドが変数の場合�Eみサポ�EチE
        if (expr.Operand is not IdentifierExpr identExpr)
        {
            throw new NotImplementedException("Increment/Decrement only supports variables currently");
        }

        var variableName = identExpr.Name;
        var runtimeType = typeof(RuntimeHelpers);

        // Increment/DecrementメソチE��を取征E
        var method = runtimeType.GetMethod(
            expr.IsIncrement ? nameof(RuntimeHelpers.Increment) : nameof(RuntimeHelpers.Decrement)
        )!;

        if (expr.IsPrefix)
        {
            // 前置: var newValue = Increment/Decrement(currentValue); ctx.Globals["name"] = newValue; return newValue;
            var tempVar = ExprTree.Variable(typeof(object), "temp");

            // 現在の変数値を取征E
            var getCurrentValue = ExprTree.Property(
                ExprTree.Property(_ctxParam, "Globals"),
                "Item",
                ExprTree.Constant(variableName)
            );

            var incrementedExpr = ExprTree.Call(method, getCurrentValue);
            var assignExpr = ExprTree.Assign(tempVar, incrementedExpr);

            // 変数を更新
            var setGlobalExpr = ExprTree.Assign(
                ExprTree.Property(
                    ExprTree.Property(_ctxParam, "Globals"),
                    "Item",
                    ExprTree.Constant(variableName)
                ),
                tempVar
            );

            return ExprTree.Block(
                typeof(object),
                new[] { tempVar },
                new ExprTree[] {
                    assignExpr,
                    setGlobalExpr,
                    tempVar
                }
            );
        }
        else
        {
            // 後置: var oldValue = currentValue; ctx.Globals["name"] = Increment/Decrement(oldValue); return oldValue;
            var tempVar = ExprTree.Variable(typeof(object), "temp");
            var newValueVar = ExprTree.Variable(typeof(object), "newValue");

            // 現在の変数値を取得して保孁E
            var getCurrentValue = ExprTree.Property(
                ExprTree.Property(_ctxParam, "Globals"),
                "Item",
                ExprTree.Constant(variableName)
            );
            var assignOldExpr = ExprTree.Assign(tempVar, getCurrentValue);

            // インクリメンチEチE��リメントして新しい値を保孁E
            var incrementedExpr = ExprTree.Call(method, tempVar);
            var assignNewExpr = ExprTree.Assign(newValueVar, incrementedExpr);

            // 変数を更新
            var setGlobalExpr = ExprTree.Assign(
                ExprTree.Property(
                    ExprTree.Property(_ctxParam, "Globals"),
                    "Item",
                    ExprTree.Constant(variableName)
                ),
                newValueVar
            );

            return ExprTree.Block(
                typeof(object),
                new[] { tempVar, newValueVar },
                new ExprTree[] {
                    assignOldExpr,
                    assignNewExpr,
                    setGlobalExpr,
                    tempVar
                }
            );
        }
    }

    /// <summary>
    /// await���̐���
    /// RuntimeHelpers.AwaitTask���Ăяo����Task������ҋ@
    /// </summary>
    private ExprTree GenerateAwaitExpr(AwaitExpr expr)
    {
        var awaitedExpr = GenerateExpression(expr.Expression);

        // Call RuntimeHelpers.AwaitTask
        var awaitTaskMethod = typeof(RuntimeHelpers).GetMethod(
            "AwaitTask",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static
        );
        if (awaitTaskMethod == null)
        {
            throw new InvalidOperationException("AwaitTask method not found in RuntimeHelpers");
        }
        return ExprTree.Call(awaitTaskMethod, awaitedExpr);
    }

    /// <summary>
    /// 安�Eなナビゲーション演算式�E生�E
    /// 仕槁E obj?.property ↁERuntimeHelpers.SafeNavigation(obj, "property")
    /// </summary>
    private ExprTree GenerateSafeNavigationExpr(SafeNavigationExpr expr)
    {
        var objectExpr = GenerateExpression(expr.Object);
        var memberNameExpr = ExprTree.Constant(expr.MemberName);

        // RuntimeHelpers.SafeNavigation(obj, memberName)
        return ExprTree.Call(
            typeof(RuntimeHelpers),
            nameof(RuntimeHelpers.SafeNavigation),
            null,
            objectExpr,
            memberNameExpr
        );
    }

    /// <summary>
    /// super式�E生�E
    /// 仕槁E super.method() ↁE親クラスのメソチE��を呼び出ぁE
    ///
    /// 実裁E��釁E
    /// 1. thisインスタンス�E�現在のインスタンス�E�をctx.Globals["this"]から取征E
    /// 2. インスタンスの親クラスを取征E
    /// 3. 親クラスのメソチE��を取征E
    /// 4. メソチE��を返す�E�呼び出し�ECallExprで行われる�E�E
    /// </summary>
    private ExprTree GenerateSuperExpr(SuperExpr expr)
    {
        // ctx.Globals["this"] からthisインスタンスを取征E
        var globalsExpr = ExprTree.Property(_ctxParam, "Globals");
        var thisNameExpr = ExprTree.Constant("this");
        var thisExpr = ExprTree.Property(globalsExpr, "Item", thisNameExpr);

        // IroInstanceにキャスチE
        var instanceExpr = ExprTree.Convert(thisExpr, typeof(IroInstance));

        // instance.Class.Parent を取征E
        var classExpr = ExprTree.Property(instanceExpr, nameof(IroInstance.Class));
        var parentClassExpr = ExprTree.Property(classExpr, nameof(IroClass.Parent));

        // parentClass.GetMethod(memberName) を呼び出ぁE
        var getMethodCall = ExprTree.Call(
            parentClassExpr,
            typeof(IroClass).GetMethod(nameof(IroClass.GetMethod))!,
            ExprTree.Constant(expr.MemberName)
        );

        // メソチE��がnullの場合�Eエラー
        var nullCheck = ExprTree.Condition(
            ExprTree.Equal(getMethodCall, ExprTree.Constant(null, typeof(IroCallable))),
            ExprTree.Throw(
                ExprTree.New(
                    typeof(InvalidOperationException).GetConstructor(new[] { typeof(string) })!,
                    ExprTree.Constant($"Parent class does not have method '{expr.MemberName}'")
                ),
                typeof(IroCallable)
            ),
            getMethodCall
        );

        // objectにキャストして返す
        return ExprTree.Convert(nullCheck, typeof(object));
    }

    #endregion
}
