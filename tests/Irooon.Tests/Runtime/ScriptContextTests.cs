using Irooon.Core.Runtime;

namespace Irooon.Tests.Runtime;

public class ScriptContextTests
{
    [Fact]
    public void ScriptContext_初期化時にGlobalsが空の辞書として作成される()
    {
        // Arrange & Act
        var context = new ScriptContext();

        // Assert
        Assert.NotNull(context.Globals);
        Assert.Empty(context.Globals);
    }

    [Fact]
    public void ScriptContext_初期化時にClassesが空の辞書として作成される()
    {
        // Arrange & Act
        var context = new ScriptContext();

        // Assert
        Assert.NotNull(context.Classes);
        Assert.Empty(context.Classes);
    }

    [Fact]
    public void Globals_グローバル変数を追加して取得できる()
    {
        // Arrange
        var context = new ScriptContext();
        var key = "testVar";
        var value = "testValue";

        // Act
        context.Globals[key] = value;

        // Assert
        Assert.Equal(value, context.Globals[key]);
    }

    [Fact]
    public void Classes_クラスを追加して取得できる()
    {
        // Arrange
        var context = new ScriptContext();
        var className = "TestClass";
        var testClass = new IroClass(className);

        // Act
        context.Classes[className] = testClass;

        // Assert
        Assert.Equal(testClass, context.Classes[className]);
        Assert.Equal(className, context.Classes[className].Name);
    }
}
