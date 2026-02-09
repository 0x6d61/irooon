namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// match式を表します。
/// match (subject) { pattern => result ... }
/// </summary>
public class MatchExpr : Expression
{
    public Expression Subject { get; }
    public List<(Expression? Pattern, Expression Body)> Arms { get; }

    public MatchExpr(Expression subject, List<(Expression? Pattern, Expression Body)> arms, int line, int column)
        : base(line, column)
    {
        Subject = subject;
        Arms = arms;
    }
}
