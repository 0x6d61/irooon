namespace Irooon.Core.Ast;

/// <summary>
/// import文を表すASTノード。
/// import {add, PI} from "./math.iro"
/// </summary>
public class ImportStmt : Statement
{
    /// <summary>
    /// インポートする名前のリスト
    /// </summary>
    public List<string> Names { get; }

    /// <summary>
    /// モジュールのパス
    /// </summary>
    public string ModulePath { get; }

    /// <summary>
    /// ImportStmtの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="names">インポートする名前のリスト</param>
    /// <param name="modulePath">モジュールのパス</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public ImportStmt(List<string> names, string modulePath, int line, int column) : base(line, column)
    {
        Names = names;
        ModulePath = modulePath;
    }
}
