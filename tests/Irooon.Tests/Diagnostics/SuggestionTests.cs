using Xunit;
using Irooon.Core.Diagnostics;

namespace Irooon.Tests.Diagnostics;

/// <summary>
/// Suggestions のテスト。
/// 主要なエラーコードにサジェスチョンが定義されていることを検証する。
/// </summary>
public class SuggestionTests
{
    [Fact]
    public void Get_CannotAssignToLet_ReturnsSuggestion()
    {
        var suggestion = Suggestions.Get(ErrorCode.E201_CannotAssignToLet);
        Assert.NotNull(suggestion);
        Assert.Contains("var", suggestion!);
    }

    [Fact]
    public void Get_UndefinedVariable_ReturnsSuggestion()
    {
        var suggestion = Suggestions.Get(ErrorCode.E202_UndefinedVariable);
        Assert.NotNull(suggestion);
    }

    [Fact]
    public void Get_DivisionByZero_ReturnsSuggestion()
    {
        var suggestion = Suggestions.Get(ErrorCode.E300_DivisionByZero);
        Assert.NotNull(suggestion);
    }

    [Fact]
    public void Get_UnknownCode_ReturnsNull()
    {
        // サジェスチョンが定義されていないコードは null を返す
        var suggestion = Suggestions.Get((ErrorCode)999);
        Assert.Null(suggestion);
    }
}
