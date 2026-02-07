namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// 文字列補間式を表すASTノード
/// 例: "Hello, ${name}!" は ["Hello, ", IdentifierExpr("name"), "!"] に解析される
/// </summary>
public class StringInterpolationExpr : Expression
{
    /// <summary>
    /// 補間のパーツ（文字列または式のリスト）
    /// </summary>
    public List<object> Parts { get; }

    /// <summary>
    /// StringInterpolationExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="parts">文字列と式のリスト</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public StringInterpolationExpr(List<object> parts, int line, int column)
        : base(line, column)
    {
        Parts = parts;
    }

    public override string ToString()
    {
        var partsStr = string.Join(", ", Parts.Select(p => p is string s ? $"\"{s}\"" : p.ToString()));
        return $"StringInterpolation([{partsStr}])";
    }
}
