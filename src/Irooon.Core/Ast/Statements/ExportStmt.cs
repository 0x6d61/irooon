namespace Irooon.Core.Ast;

/// <summary>
/// export文を表すASTノード。
/// export let x = 10
/// export fn add(a, b) { ... }
/// </summary>
public class ExportStmt : Statement
{
    /// <summary>
    /// エクスポートする宣言（LetStmtまたはFunctionDef）
    /// </summary>
    public Statement Declaration { get; }

    /// <summary>
    /// ExportStmtの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="declaration">エクスポートする宣言</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public ExportStmt(Statement declaration, int line, int column) : base(line, column)
    {
        Declaration = declaration;
    }
}
