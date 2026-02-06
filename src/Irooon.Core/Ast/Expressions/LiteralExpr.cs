namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// リテラル式を表します（数値、文字列、真偽値、null）。
/// </summary>
public class LiteralExpr : Expression
{
    /// <summary>
    /// リテラルの値
    /// </summary>
    public object? Value { get; }

    /// <summary>
    /// LiteralExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="value">リテラルの値</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public LiteralExpr(object? value, int line, int column) : base(line, column)
    {
        Value = value;
    }
}
