using Xunit;
using Irooon.Core.Ast;
using Irooon.Core.Ast.Expressions;

namespace Irooon.Tests.Ast;

/// <summary>
/// ListExpr と HashExpr のテストクラス
/// </summary>
public class ListHashExprTests
{
    #region ListExpr テスト

    [Fact]
    public void ListExpr_空のリストを構築できること()
    {
        // Arrange
        var elements = new List<Expression>();

        // Act
        var expr = new ListExpr(elements, 1, 1);

        // Assert
        Assert.NotNull(expr.Elements);
        Assert.Empty(expr.Elements);
        Assert.Equal(1, expr.Line);
        Assert.Equal(1, expr.Column);
    }

    [Fact]
    public void ListExpr_要素を持つリストを構築できること()
    {
        // Arrange
        var elements = new List<Expression>
        {
            new LiteralExpr(1.0, 1, 2),
            new LiteralExpr(2.0, 1, 5),
            new LiteralExpr(3.0, 1, 8)
        };

        // Act
        var expr = new ListExpr(elements, 1, 1);

        // Assert
        Assert.Equal(3, expr.Elements.Count);
        Assert.Equal(1.0, ((LiteralExpr)expr.Elements[0]).Value);
        Assert.Equal(2.0, ((LiteralExpr)expr.Elements[1]).Value);
        Assert.Equal(3.0, ((LiteralExpr)expr.Elements[2]).Value);
    }

    [Fact]
    public void ListExpr_混合型の要素を持つリストを構築できること()
    {
        // Arrange
        var elements = new List<Expression>
        {
            new LiteralExpr(42.0, 1, 2),
            new LiteralExpr("hello", 1, 6),
            new LiteralExpr(true, 1, 15)
        };

        // Act
        var expr = new ListExpr(elements, 1, 1);

        // Assert
        Assert.Equal(3, expr.Elements.Count);
        Assert.IsType<LiteralExpr>(expr.Elements[0]);
        Assert.IsType<LiteralExpr>(expr.Elements[1]);
        Assert.IsType<LiteralExpr>(expr.Elements[2]);
    }

    [Fact]
    public void ListExpr_ネストしたリストを構築できること()
    {
        // Arrange
        var innerList = new ListExpr(
            new List<Expression>
            {
                new LiteralExpr(1.0, 1, 3),
                new LiteralExpr(2.0, 1, 6)
            },
            1, 2
        );

        var elements = new List<Expression>
        {
            new LiteralExpr(0.0, 1, 1),
            innerList
        };

        // Act
        var expr = new ListExpr(elements, 1, 1);

        // Assert
        Assert.Equal(2, expr.Elements.Count);
        Assert.IsType<LiteralExpr>(expr.Elements[0]);
        Assert.IsType<ListExpr>(expr.Elements[1]);

        var nested = (ListExpr)expr.Elements[1];
        Assert.Equal(2, nested.Elements.Count);
    }

    #endregion

    #region HashExpr テスト

    [Fact]
    public void HashExpr_空のハッシュを構築できること()
    {
        // Arrange
        var pairs = new List<HashExpr.KeyValuePair>();

        // Act
        var expr = new HashExpr(pairs, 1, 1);

        // Assert
        Assert.NotNull(expr.Pairs);
        Assert.Empty(expr.Pairs);
        Assert.Equal(1, expr.Line);
        Assert.Equal(1, expr.Column);
    }

    [Fact]
    public void HashExpr_単一のペアを持つハッシュを構築できること()
    {
        // Arrange
        var pairs = new List<HashExpr.KeyValuePair>
        {
            new HashExpr.KeyValuePair("name", new LiteralExpr("Alice", 1, 10))
        };

        // Act
        var expr = new HashExpr(pairs, 1, 1);

        // Assert
        Assert.Single(expr.Pairs);
        Assert.Equal("name", expr.Pairs[0].Key);
        Assert.Equal("Alice", ((LiteralExpr)expr.Pairs[0].Value).Value);
    }

    [Fact]
    public void HashExpr_複数のペアを持つハッシュを構築できること()
    {
        // Arrange
        var pairs = new List<HashExpr.KeyValuePair>
        {
            new HashExpr.KeyValuePair("name", new LiteralExpr("Bob", 1, 10)),
            new HashExpr.KeyValuePair("age", new LiteralExpr(30.0, 1, 20)),
            new HashExpr.KeyValuePair("active", new LiteralExpr(true, 1, 30))
        };

        // Act
        var expr = new HashExpr(pairs, 1, 1);

        // Assert
        Assert.Equal(3, expr.Pairs.Count);
        Assert.Equal("name", expr.Pairs[0].Key);
        Assert.Equal("age", expr.Pairs[1].Key);
        Assert.Equal("active", expr.Pairs[2].Key);

        Assert.Equal("Bob", ((LiteralExpr)expr.Pairs[0].Value).Value);
        Assert.Equal(30.0, ((LiteralExpr)expr.Pairs[1].Value).Value);
        Assert.Equal(true, ((LiteralExpr)expr.Pairs[2].Value).Value);
    }

    [Fact]
    public void HashExpr_式を値として持つハッシュを構築できること()
    {
        // Arrange
        var pairs = new List<HashExpr.KeyValuePair>
        {
            new HashExpr.KeyValuePair("sum", new BinaryExpr(
                new LiteralExpr(1.0, 1, 10),
                Core.Lexer.TokenType.Plus,
                new LiteralExpr(2.0, 1, 14),
                1, 10
            ))
        };

        // Act
        var expr = new HashExpr(pairs, 1, 1);

        // Assert
        Assert.Single(expr.Pairs);
        Assert.Equal("sum", expr.Pairs[0].Key);
        Assert.IsType<BinaryExpr>(expr.Pairs[0].Value);
    }

    [Fact]
    public void HashExpr_ネストしたハッシュを構築できること()
    {
        // Arrange
        var innerHash = new HashExpr(
            new List<HashExpr.KeyValuePair>
            {
                new HashExpr.KeyValuePair("city", new LiteralExpr("Tokyo", 2, 15))
            },
            2, 10
        );

        var pairs = new List<HashExpr.KeyValuePair>
        {
            new HashExpr.KeyValuePair("name", new LiteralExpr("Charlie", 1, 10)),
            new HashExpr.KeyValuePair("address", innerHash)
        };

        // Act
        var expr = new HashExpr(pairs, 1, 1);

        // Assert
        Assert.Equal(2, expr.Pairs.Count);
        Assert.Equal("name", expr.Pairs[0].Key);
        Assert.Equal("address", expr.Pairs[1].Key);

        Assert.IsType<LiteralExpr>(expr.Pairs[0].Value);
        Assert.IsType<HashExpr>(expr.Pairs[1].Value);

        var nested = (HashExpr)expr.Pairs[1].Value;
        Assert.Single(nested.Pairs);
        Assert.Equal("city", nested.Pairs[0].Key);
    }

    [Fact]
    public void HashExpr_リストを値として持つハッシュを構築できること()
    {
        // Arrange
        var list = new ListExpr(
            new List<Expression>
            {
                new LiteralExpr(1.0, 1, 15),
                new LiteralExpr(2.0, 1, 18),
                new LiteralExpr(3.0, 1, 21)
            },
            1, 14
        );

        var pairs = new List<HashExpr.KeyValuePair>
        {
            new HashExpr.KeyValuePair("numbers", list)
        };

        // Act
        var expr = new HashExpr(pairs, 1, 1);

        // Assert
        Assert.Single(expr.Pairs);
        Assert.Equal("numbers", expr.Pairs[0].Key);
        Assert.IsType<ListExpr>(expr.Pairs[0].Value);

        var listValue = (ListExpr)expr.Pairs[0].Value;
        Assert.Equal(3, listValue.Elements.Count);
    }

    #endregion
}
