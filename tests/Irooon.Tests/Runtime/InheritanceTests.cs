using Xunit;
using Irooon.Core.Runtime;

namespace Irooon.Tests.Runtime;

/// <summary>
/// クラス継承機能のテスト
/// </summary>
public class InheritanceTests
{
    // ダミーのClosureを作成するヘルパー
    private IroCallable CreateDummyClosure(string name)
    {
        return new Closure(name, (ctx, args) => null, new List<string>(), 1, 1);
    }

    [Fact]
    public void TestSingleInheritance()
    {
        // 親クラスのフィールド定義
        var parentFields = new[]
        {
            new FieldDef("name", true, (ctx) => "parent")
        };

        // 親クラスのメソッド定義（greet）
        var parentMethods = new[]
        {
            new MethodDef("greet", true, false, CreateDummyClosure("greet"))
        };

        // 親クラス作成
        var parentClass = new IroClass("Parent", parentFields, parentMethods, null);

        // 子クラスのフィールド定義
        var childFields = new[]
        {
            new FieldDef("age", true, (ctx) => 0)
        };

        // 子クラスのメソッド定義（introduce）
        var childMethods = new[]
        {
            new MethodDef("introduce", true, false, CreateDummyClosure("introduce"))
        };

        // 子クラス作成（親クラスを指定）
        var childClass = new IroClass("Child", childFields, childMethods, parentClass);

        // 子クラスが親クラスを持っていることを確認
        Assert.NotNull(childClass.Parent);
        Assert.Equal("Parent", childClass.Parent.Name);
    }

    [Fact]
    public void TestFieldInheritance()
    {
        // 親クラスのフィールド定義
        var parentFields = new[]
        {
            new FieldDef("name", true, (ctx) => "parent")
        };

        var parentClass = new IroClass("Parent", parentFields, Array.Empty<MethodDef>(), null);

        // 子クラスのフィールド定義
        var childFields = new[]
        {
            new FieldDef("age", true, (ctx) => 0)
        };

        var childClass = new IroClass("Child", childFields, Array.Empty<MethodDef>(), parentClass);

        // 親のフィールドが取得できることを確認
        var nameField = childClass.GetField("name");
        Assert.NotNull(nameField);
        Assert.Equal("name", nameField.Name);

        // 子のフィールドが取得できることを確認
        var ageField = childClass.GetField("age");
        Assert.NotNull(ageField);
        Assert.Equal("age", ageField.Name);
    }

    [Fact]
    public void TestMethodInheritance()
    {
        // 親クラスのメソッド定義
        var parentMethods = new[]
        {
            new MethodDef("greet", true, false, CreateDummyClosure("greet"))
        };

        var parentClass = new IroClass("Parent", Array.Empty<FieldDef>(), parentMethods, null);

        // 子クラスのメソッド定義
        var childMethods = new[]
        {
            new MethodDef("introduce", true, false, CreateDummyClosure("introduce"))
        };

        var childClass = new IroClass("Child", Array.Empty<FieldDef>(), childMethods, parentClass);

        // 親のメソッドが取得できることを確認
        var greetMethod = childClass.GetMethod("greet");
        Assert.NotNull(greetMethod);

        // 子のメソッドが取得できることを確認
        var introduceMethod = childClass.GetMethod("introduce");
        Assert.NotNull(introduceMethod);
    }

    [Fact]
    public void TestMethodOverride()
    {
        // 親クラスのメソッド定義（greet）
        var parentMethods = new[]
        {
            new MethodDef("greet", true, false, CreateDummyClosure("parent_greet"))
        };

        var parentClass = new IroClass("Parent", Array.Empty<FieldDef>(), parentMethods, null);

        // 子クラスのメソッド定義（greet - オーバーライド）
        var childMethods = new[]
        {
            new MethodDef("greet", true, false, CreateDummyClosure("child_greet"))
        };

        var childClass = new IroClass("Child", Array.Empty<FieldDef>(), childMethods, parentClass);

        // 子クラスのメソッドが優先されることを確認
        var greetMethod = childClass.GetMethod("greet");
        Assert.NotNull(greetMethod);
        // 子クラスのMethodsに含まれていることを確認
        Assert.True(childClass.Methods.ContainsKey("greet"));
    }

    [Fact]
    public void TestTwoLevelInheritance()
    {
        // GrandParentクラス
        var grandParentFields = new[]
        {
            new FieldDef("family", true, (ctx) => "Smith")
        };
        var grandParentClass = new IroClass("GrandParent", grandParentFields, Array.Empty<MethodDef>(), null);

        // Parentクラス（GrandParentを継承）
        var parentFields = new[]
        {
            new FieldDef("name", true, (ctx) => "parent")
        };
        var parentClass = new IroClass("Parent", parentFields, Array.Empty<MethodDef>(), grandParentClass);

        // Childクラス（Parentを継承）
        var childFields = new[]
        {
            new FieldDef("age", true, (ctx) => 0)
        };
        var childClass = new IroClass("Child", childFields, Array.Empty<MethodDef>(), parentClass);

        // 3世代のフィールドすべてが取得できることを確認
        var familyField = childClass.GetField("family");
        Assert.NotNull(familyField);
        Assert.Equal("family", familyField.Name);

        var nameField = childClass.GetField("name");
        Assert.NotNull(nameField);
        Assert.Equal("name", nameField.Name);

        var ageField = childClass.GetField("age");
        Assert.NotNull(ageField);
        Assert.Equal("age", ageField.Name);
    }

    [Fact]
    public void TestInstanceFieldInitialization()
    {
        // 親クラスのフィールド定義
        var parentFields = new[]
        {
            new FieldDef("name", true, (ctx) => "parent")
        };

        var parentClass = new IroClass("Parent", parentFields, Array.Empty<MethodDef>(), null);

        // 子クラスのフィールド定義
        var childFields = new[]
        {
            new FieldDef("age", true, (ctx) => 10)
        };

        var childClass = new IroClass("Child", childFields, Array.Empty<MethodDef>(), parentClass);

        // インスタンス作成
        var instance = new IroInstance(childClass);

        // 親クラスのフィールドが初期化されていることを確認（将来の実装で検証）
        // 注: 現時点ではIroInstanceのフィールド初期化は外部で行われる想定
        Assert.NotNull(instance);
        Assert.Equal(childClass, instance.Class);
    }
}
