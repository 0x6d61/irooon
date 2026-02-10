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
        var iroClass = new IroClass(className, Array.Empty<FieldDef>(), Array.Empty<MethodDef>());

        // Assert
        Assert.Equal(className, iroClass.Name);
    }

    [Fact]
    public void IroClass_初期化時にFieldsが空のリストとして作成される()
    {
        // Arrange & Act
        var iroClass = new IroClass("TestClass", Array.Empty<FieldDef>(), Array.Empty<MethodDef>());

        // Assert
        Assert.NotNull(iroClass.Fields);
        Assert.Empty(iroClass.Fields);
    }

    [Fact]
    public void IroClass_初期化時にMethodsが空の辞書として作成される()
    {
        // Arrange & Act
        var iroClass = new IroClass("TestClass", Array.Empty<FieldDef>(), Array.Empty<MethodDef>());

        // Assert
        Assert.NotNull(iroClass.Methods);
        Assert.Empty(iroClass.Methods);
    }

    [Fact]
    public void IroClass_初期化時にStaticMethodsが空の辞書として作成される()
    {
        // Arrange & Act
        var iroClass = new IroClass("TestClass", Array.Empty<FieldDef>(), Array.Empty<MethodDef>());

        // Assert
        Assert.NotNull(iroClass.StaticMethods);
        Assert.Empty(iroClass.StaticMethods);
    }

    [Fact]
    public void IroClass_フィールドを追加できる()
    {
        // Arrange
        var field = new FieldDef("testField", isPublic: true);
        var iroClass = new IroClass("TestClass", new[] { field }, Array.Empty<MethodDef>());

        // Assert
        Assert.Single(iroClass.Fields);
        Assert.Equal(field, iroClass.Fields[0]);
    }

    [Fact]
    public void IroClass_メソッドを追加できる()
    {
        // Arrange
        var methodName = "testMethod";
        var method = new TestCallable();
        var methodDef = new MethodDef(methodName, isPublic: true, isStatic: false, method);
        var iroClass = new IroClass("TestClass", Array.Empty<FieldDef>(), new[] { methodDef });

        // Assert
        Assert.Single(iroClass.Methods);
        Assert.Equal(method, iroClass.Methods[methodName]);
    }

    [Fact]
    public void IroClass_スタティックメソッドを追加できる()
    {
        // Arrange
        var methodName = "staticMethod";
        var method = new TestCallable();
        var methodDef = new MethodDef(methodName, isPublic: true, isStatic: true, method);
        var iroClass = new IroClass("TestClass", Array.Empty<FieldDef>(), new[] { methodDef });

        // Assert
        Assert.Single(iroClass.StaticMethods);
        Assert.Equal(method, iroClass.StaticMethods[methodName]);
    }

    [Fact]
    public void IroInstance_クラスを指定して作成できる()
    {
        // Arrange
        var iroClass = new IroClass("TestClass", Array.Empty<FieldDef>(), Array.Empty<MethodDef>());

        // Act
        var instance = new IroInstance(iroClass);

        // Assert
        Assert.Equal(iroClass, instance.Class);
    }

    [Fact]
    public void IroInstance_初期化時にFieldsが空の辞書として作成される()
    {
        // Arrange
        var iroClass = new IroClass("TestClass", Array.Empty<FieldDef>(), Array.Empty<MethodDef>());

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
        var iroClass = new IroClass("TestClass", Array.Empty<FieldDef>(), Array.Empty<MethodDef>());
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

        // Act
        var field = new FieldDef(name, isPublic);

        // Assert
        Assert.Equal(name, field.Name);
        Assert.Equal(isPublic, field.IsPublic);
        Assert.False(field.IsStatic); // v0.1ではstaticフィールド未対応
    }

    #region GetStaticMethod テスト

    [Fact]
    public void IroClass_GetStaticMethod_自クラスのスタティックメソッドを取得できる()
    {
        var method = new TestCallable();
        var methodDef = new MethodDef("create", isPublic: true, isStatic: true, method);
        var iroClass = new IroClass("TestClass", Array.Empty<FieldDef>(), new[] { methodDef });

        var result = iroClass.GetStaticMethod("create");
        Assert.NotNull(result);
        Assert.Equal(method, result);
    }

    [Fact]
    public void IroClass_GetStaticMethod_親クラスのスタティックメソッドを取得できる()
    {
        var method = new TestCallable();
        var parentMethodDef = new MethodDef("create", isPublic: true, isStatic: true, method);
        var parent = new IroClass("Parent", Array.Empty<FieldDef>(), new[] { parentMethodDef });
        var child = new IroClass("Child", Array.Empty<FieldDef>(), Array.Empty<MethodDef>(), parent);

        var result = child.GetStaticMethod("create");
        Assert.NotNull(result);
        Assert.Equal(method, result);
    }

    [Fact]
    public void IroClass_GetStaticMethod_存在しないメソッドはnullを返す()
    {
        var iroClass = new IroClass("TestClass", Array.Empty<FieldDef>(), Array.Empty<MethodDef>());
        var result = iroClass.GetStaticMethod("nonExistent");
        Assert.Null(result);
    }

    #endregion

    // テスト用のIroCallable実装
    private class TestCallable : IroCallable
    {
        public object Invoke(ScriptContext ctx, object[] args)
        {
            return "test result";
        }
    }
}
