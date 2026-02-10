namespace Irooon.Core.Ast;

/// <summary>
/// アセンブリ参照ディレクティブを表すASTノード。
/// #r "path/to/assembly.dll"
/// </summary>
public class AssemblyRefStmt : Statement
{
    /// <summary>
    /// アセンブリのパス
    /// </summary>
    public string AssemblyPath { get; }

    public AssemblyRefStmt(string assemblyPath, int line, int column) : base(line, column)
    {
        AssemblyPath = assemblyPath;
    }
}
