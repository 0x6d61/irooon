using System.Linq.Expressions;
using Irooon.Core.Ast;
using Irooon.Core.Ast.Expressions;
using Irooon.Core.Ast.Statements;
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
        else
        {
            // 最後が文のみの場合はnull
            expressions.Add(ExprTree.Constant(null, typeof(object)));
        }

        return ExprTree.Block(typeof(object), expressions);
    }

    #endregion

    #region Task #14以降の実装（スタブ）

    // Task #14: 演算子の実装（スタブ）
    private ExprTree GenerateBinaryExpr(BinaryExpr expr)
    {
        throw new NotImplementedException("Task #14: Binary operators not implemented yet");
    }

    private ExprTree GenerateUnaryExpr(UnaryExpr expr)
    {
        throw new NotImplementedException("Task #14: Unary operators not implemented yet");
    }

    // Task #15: 制御構造の実装（スタブ）
    private ExprTree GenerateIfExpr(IfExpr expr)
    {
        throw new NotImplementedException("Task #15: If expression not implemented yet");
    }

    private ExprTree GenerateWhileStmt(WhileStmt stmt)
    {
        throw new NotImplementedException("Task #15: While statement not implemented yet");
    }

    private ExprTree GenerateReturnStmt(ReturnStmt stmt)
    {
        throw new NotImplementedException("Task #15: Return statement not implemented yet");
    }

    // Task #16: 関数とクロージャの実装（スタブ）
    private ExprTree GenerateCallExpr(CallExpr expr)
    {
        throw new NotImplementedException("Task #16: Function call not implemented yet");
    }

    private ExprTree GenerateLambdaExpr(LambdaExpr expr)
    {
        throw new NotImplementedException("Task #16: Lambda expression not implemented yet");
    }

    private ExprTree GenerateFunctionDef(FunctionDef stmt)
    {
        throw new NotImplementedException("Task #16: Function definition not implemented yet");
    }

    // Task #17: クラスとインスタンスの実装（スタブ）
    private ExprTree GenerateMemberExpr(MemberExpr expr)
    {
        throw new NotImplementedException("Task #17: Member access not implemented yet");
    }

    private ExprTree GenerateIndexExpr(IndexExpr expr)
    {
        throw new NotImplementedException("Task #17: Index access not implemented yet");
    }

    private ExprTree GenerateNewExpr(NewExpr expr)
    {
        throw new NotImplementedException("Task #17: New expression not implemented yet");
    }

    private ExprTree GenerateClassDef(ClassDef stmt)
    {
        throw new NotImplementedException("Task #17: Class definition not implemented yet");
    }

    #endregion
}
