namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// インスタンス生成式を表します（例: ClassName(args)）。
/// </summary>
public class NewExpr : Expression
{
    /// <summary>
    /// クラス名
    /// </summary>
    public string ClassName { get; }

    /// <summary>
    /// コンストラクタ引数のリスト
    /// </summary>
    public List<Expression> Arguments { get; }

    /// <summary>
    /// NewExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="className">クラス名</param>
    /// <param name="arguments">コンストラクタ引数のリスト</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public NewExpr(string className, List<Expression> arguments, int line, int column)
        : base(line, column)
    {
        ClassName = className;
        Arguments = arguments;
    }
}
