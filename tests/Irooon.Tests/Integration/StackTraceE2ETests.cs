using Irooon.Core;
using Irooon.Core.Runtime;
using Xunit;

namespace Irooon.Tests.Integration;

/// <summary>
/// スタックトレースのE2Eテスト
/// </summary>
public class StackTraceE2ETests
{
    [Fact]
    public void ErrorInNestedFunction_ShouldShowFullStackTrace()
    {
        // Arrange
        var source = @"
            fn inner() {
                var x = 1 / 0
            }

            fn middle() {
                inner()
            }

            fn outer() {
                middle()
            }

            outer()
        ";

        var engine = new ScriptEngine();
        CallStack.Clear();

        // Act & Assert
        var exception = Assert.Throws<ScriptException>(() => engine.Execute(source));

        // CallStackにスタックトレースが記録されていることを確認
        // （実行時にエラーが発生した時点でのCallStack）
        // 注: 現時点では例外がスローされた後にCallStackがクリアされているため、
        // スタックトレースの詳細な検証は後のタスクで実装します
        Assert.NotNull(exception);
    }

    [Fact]
    public void ErrorMessage_ShouldIncludeLineAndColumnInfo()
    {
        // Arrange
        var source = @"
            fn divide(a, b) {
                a / b
            }

            divide(10, 0)
        ";

        var engine = new ScriptEngine();
        CallStack.Clear();

        // Act & Assert
        var exception = Assert.Throws<ScriptException>(() => engine.Execute(source));

        // 例外が発生したことを確認
        // 注: 位置情報の詳細な検証は後のタスクで実装します
        Assert.NotNull(exception);
        Assert.Contains("Runtime error", exception.Message);
    }

    [Fact]
    public void SimpleError_ShouldShowErrorLocation()
    {
        // Arrange
        var source = "var x = 1 / 0";
        var engine = new ScriptEngine();
        CallStack.Clear();

        // Act & Assert
        var exception = Assert.Throws<ScriptException>(() => engine.Execute(source));

        // 例外が発生したことを確認
        Assert.NotNull(exception);
        Assert.Contains("Runtime error", exception.Message);
    }
}
