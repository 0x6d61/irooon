using Irooon.Core.Runtime;

namespace Irooon.Tests.Runtime;

public class IroClassTests
{
    [Fact]
    public void IroClass_名前を指定して作成できる()
    {
        // Arrange
        var className = "TestClass";

        // Act
        var iroClass = new IroClass(className);

        // Assert
        Assert.Equal(className, iroClass.Name);
    }

    [Fact]
    public void IroClass_初期化時にFieldsが空のリストとして作成される()
    {
        // Arrange & Act
        var iroClass = new IroClass("TestClass");

        // Assert
        Assert.NotNull(iroClass.Fields);
        Assert.Empty(iroClass.Fields);
    }

    [Fact]
    public void IroClass_初期化時にMethodsが空の辞書として作成される()
    {
        // Arrange & Act
        var iroClass = new IroClass("TestClass");

        // Assert
        Assert.NotNull(iroClass.Methods);
        Assert.Empty(iroClass.Methods);
    }

    [Fact]
    public void IroClass_初期化時にStaticMethodsが空の辞書として作成される()
    {
        // Arrange & Act
        var iroClass = new IroClass("TestClass");

        // Assert
        Assert.NotNull(iroClass.StaticMethods);
        Assert.Empty(iroClass.StaticMethods);
    }

    [Fact]
    public void IroClass_フィールドを追加できる()
    {
        // Arrange
        var iroClass = new IroClass("TestClass");
        var field = new FieldDef("testField", isPublic: true, isStatic: false);

        // Act
        iroClass.Fields.Add(field);

        // Assert
        Assert.Single(iroClass.Fields);
        Assert.Equal(field, iroClass.Fields[0]);
    }

    [Fact]
    public void IroClass_メソッドを追加できる()
    {
        // Arrange
        var iroClass = new IroClass("TestClass");
        var methodName = "testMethod";
        var method = new TestCallable();

        // Act
        iroClass.Methods[methodName] = method;

        // Assert
        Assert.Single(iroClass.Methods);
        Assert.Equal(method, iroClass.Methods[methodName]);
    }

    [Fact]
    public void IroClass_スタティックメソッドを追加できる()
    {
        // Arrange
        var iroClass = new IroClass("TestClass");
        var methodName = "staticMethod";
        var method = new TestCallable();

        // Act
        iroClass.StaticMethods[methodName] = method;

        // Assert
        Assert.Single(iroClass.StaticMethods);
        Assert.Equal(method, iroClass.StaticMethods[methodName]);
    }

    [Fact]
    public void IroInstance_クラスを指定して作成できる()
    {
        // Arrange
        var iroClass = new IroClass("TestClass");

        // Act
        var instance = new IroInstance(iroClass);

        // Assert
        Assert.Equal(iroClass, instance.Class);
    }

    [Fact]
    public void IroInstance_初期化時にFieldsが空の辞書として作成される()
    {
        // Arrange
        var iroClass = new IroClass("TestClass");

        // Act
        var instance = new IroInstance(iroClass);

        // Assert
        Assert.NotNull(instance.Fields);
        Assert.Empty(instance.Fields);
    }

    [Fact]
    public void IroInstance_フィールド値を設定して取得できる()
    {
        // Arrange
        var iroClass = new IroClass("TestClass");
        var instance = new IroInstance(iroClass);
        var fieldName = "testField";
        var fieldValue = "testValue";

        // Act
        instance.Fields[fieldName] = fieldValue;

        // Assert
        Assert.Equal(fieldValue, instance.Fields[fieldName]);
    }

    [Fact]
    public void FieldDef_プロパティが正しく設定される()
    {
        // Arrange
        var name = "testField";
        var isPublic = true;
        var isStatic = false;

        // Act
        var field = new FieldDef(name, isPublic, isStatic);

        // Assert
        Assert.Equal(name, field.Name);
        Assert.Equal(isPublic, field.IsPublic);
        Assert.Equal(isStatic, field.IsStatic);
    }

    // テスト用のIroCallable実装
    private class TestCallable : IroCallable
    {
        public object Invoke(ScriptContext ctx, object[] args)
        {
            return "test result";
        }
    }
}
