namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// シェルコマンド実行式を表します（$`command`）。
/// </summary>
public class ShellExpr : Expression
{
    /// <summary>
    /// 実行するシェルコマンド
    /// </summary>
    public string Command { get; }

    /// <summary>
    /// ShellExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="command">実行するシェルコマンド</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public ShellExpr(string command, int line, int column) : base(line, column)
    {
        Command = command;
    }

    public override string ToString()
    {
        return $"ShellExpr(`{Command}`)";
    }
}
