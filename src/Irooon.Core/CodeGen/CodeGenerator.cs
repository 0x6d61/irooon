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
            WhileStmt s => GenerateWhileStmt(s),
            FunctionDef s => GenerateFunctionDef(s),
            ClassDef s => GenerateClassDef(s),
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
    /// while文の変換
    /// 仕様（expression-tree-mapping.md セクション8）:
    /// Expression.Loop + LabelBreak
    /// while全体の値は null
    /// </summary>
    private ExprTree GenerateWhileStmt(WhileStmt stmt)
    {
        var labelName = $"whileBreak_{_labelCounter++}";
        var breakLabel = ExprTree.Label(labelName);
        var condExpr = GenerateExpression(stmt.Condition);
        var bodyExpr = GenerateStatement(stmt.Body);

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

        // Expression.Loop without break label, then manually add label and null
        // Loopはbreak labelなしで無限ループを作成
        // breakは明示的に配置したラベルにジャンプ
        return ExprTree.Block(
            typeof(object),
            ExprTree.Loop(loopBody),
            ExprTree.Label(breakLabel),
            ExprTree.Constant(null, typeof(object))
        );
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
        ExprTree calleeExpr;
        ExprTree? thisArg = null;

        // CalleeがMemberExprの場合、インスタンスメソッド呼び出し
        if (expr.Callee is MemberExpr memberExpr)
        {
            var targetExpr = GenerateExpression(memberExpr.Target);
            thisArg = targetExpr;

            // メソッドを取得
            calleeExpr = ExprTree.Call(
                typeof(RuntimeHelpers),
                "GetMember",
                null,
                targetExpr,
                ExprTree.Constant(memberExpr.Name)
            );
        }
        else
        {
            calleeExpr = GenerateExpression(expr.Callee);
        }

        var argExprs = expr.Arguments.Select(GenerateExpression).ToArray();
        var argsArray = ExprTree.NewArrayInit(typeof(object), argExprs);

        // RuntimeHelpers.Invoke(callee, ctx, args, thisArg)
        if (thisArg != null)
        {
            return ExprTree.Call(
                typeof(RuntimeHelpers),
                "Invoke",
                null,
                calleeExpr,
                _ctxParam,
                argsArray,
                thisArg
            );
        }
        else
        {
            return ExprTree.Call(
                typeof(RuntimeHelpers),
                "Invoke",
                null,
                calleeExpr,
                _ctxParam,
                argsArray,
                ExprTree.Constant(null, typeof(object))
            );
        }
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

        // Closure オブジェクトを作成
        var closureNew = ExprTree.New(
            typeof(Closure).GetConstructor(new[] { typeof(string), typeof(Func<ScriptContext, object[], object>), typeof(List<string>) })!,
            ExprTree.Constant("<lambda>"),
            ExprTree.Constant(compiled, typeof(Func<ScriptContext, object[], object>)),
            paramNamesListNew
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

        // Closure オブジェクトを作成
        var closureNew = ExprTree.New(
            typeof(Closure).GetConstructor(new[] { typeof(string), typeof(Func<ScriptContext, object[], object>), typeof(List<string>) })!,
            ExprTree.Constant(stmt.Name),
            ExprTree.Constant(compiled, typeof(Func<ScriptContext, object[], object>)),
            paramNamesListNew
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
            var closure = new Closure(m.Name, compiled, paramNames);

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

        // IroClass を作成
        var classNew = ExprTree.New(
            typeof(IroClass).GetConstructor(new[] {
                typeof(string),
                typeof(Runtime.FieldDef[]),
                typeof(Runtime.MethodDef[])
            })!,
            ExprTree.Constant(stmt.Name),
            fieldsArray,
            methodsArray
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

    #endregion
}
