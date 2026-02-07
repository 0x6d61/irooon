using Irooon.Core.Runtime;

namespace Irooon.Tests.Runtime;

public class RuntimeHelpersTests
{
    #region IsTruthy Tests

    [Fact]
    public void IsTruthy_nullはfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.IsTruthy(null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsTruthy_bool_trueはtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.IsTruthy(true);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsTruthy_bool_falseはfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.IsTruthy(false);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsTruthy_数値はtrueを返す()
    {
        // Act & Assert
        Assert.True(RuntimeHelpers.IsTruthy(1.0));
        Assert.False(RuntimeHelpers.IsTruthy(0.0)); // 0はfalsy
        Assert.True(RuntimeHelpers.IsTruthy(-5.5));
    }

    [Fact]
    public void IsTruthy_文字列はtrueを返す()
    {
        // Act & Assert
        Assert.True(RuntimeHelpers.IsTruthy("hello"));
        Assert.False(RuntimeHelpers.IsTruthy("")); // 空文字列はfalsy
    }

    [Fact]
    public void IsTruthy_オブジェクトはtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.IsTruthy(new object());

        // Assert
        Assert.True(result);
    }

    #endregion

    #region Add Tests

    [Fact]
    public void Add_整数同士の加算()
    {
        // Act
        var result = RuntimeHelpers.Add(3.0, 5.0);

        // Assert
        Assert.Equal(8.0, result);
    }

    [Fact]
    public void Add_小数同士の加算()
    {
        // Act
        var result = RuntimeHelpers.Add(3.5, 2.5);

        // Assert
        Assert.Equal(6.0, result);
    }

    [Fact]
    public void Add_負の数の加算()
    {
        // Act
        var result = RuntimeHelpers.Add(-3.0, 5.0);

        // Assert
        Assert.Equal(2.0, result);
    }

    #endregion

    #region Sub Tests

    [Fact]
    public void Sub_整数同士の減算()
    {
        // Act
        var result = RuntimeHelpers.Sub(10.0, 3.0);

        // Assert
        Assert.Equal(7.0, result);
    }

    [Fact]
    public void Sub_小数同士の減算()
    {
        // Act
        var result = RuntimeHelpers.Sub(5.5, 2.5);

        // Assert
        Assert.Equal(3.0, result);
    }

    #endregion

    #region Mul Tests

    [Fact]
    public void Mul_整数同士の乗算()
    {
        // Act
        var result = RuntimeHelpers.Mul(3.0, 4.0);

        // Assert
        Assert.Equal(12.0, result);
    }

    [Fact]
    public void Mul_小数同士の乗算()
    {
        // Act
        var result = RuntimeHelpers.Mul(2.5, 4.0);

        // Assert
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void Mul_ゼロとの乗算()
    {
        // Act
        var result = RuntimeHelpers.Mul(5.0, 0.0);

        // Assert
        Assert.Equal(0.0, result);
    }

    #endregion

    #region Div Tests

    [Fact]
    public void Div_整数同士の除算()
    {
        // Act
        var result = RuntimeHelpers.Div(10.0, 2.0);

        // Assert
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void Div_小数の除算()
    {
        // Act
        var result = RuntimeHelpers.Div(7.5, 2.5);

        // Assert
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void Div_ゼロ除算は例外を投げる()
    {
        // Act & Assert
        Assert.Throws<DivideByZeroException>(() => RuntimeHelpers.Div(10.0, 0.0));
    }

    #endregion

    #region Mod Tests

    [Fact]
    public void Mod_整数の剰余()
    {
        // Act
        var result = RuntimeHelpers.Mod(10.0, 3.0);

        // Assert
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void Mod_小数の剰余()
    {
        // Act
        var result = RuntimeHelpers.Mod(7.5, 2.0);

        // Assert
        Assert.Equal(1.5, result);
    }

    #endregion

    #region Comparison Tests

    [Fact]
    public void Eq_等しい値はtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.Eq(5.0, 5.0);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void Eq_異なる値はfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.Eq(5.0, 3.0);

        // Assert
        Assert.False((bool)result);
    }

    [Fact]
    public void Ne_異なる値はtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.Ne(5.0, 3.0);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void Ne_等しい値はfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.Ne(5.0, 5.0);

        // Assert
        Assert.False((bool)result);
    }

    [Fact]
    public void Lt_小さい値はtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.Lt(3.0, 5.0);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void Lt_大きい値はfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.Lt(5.0, 3.0);

        // Assert
        Assert.False((bool)result);
    }

    [Fact]
    public void Lt_等しい値はfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.Lt(5.0, 5.0);

        // Assert
        Assert.False((bool)result);
    }

    [Fact]
    public void Le_小さい値はtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.Le(3.0, 5.0);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void Le_等しい値はtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.Le(5.0, 5.0);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void Le_大きい値はfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.Le(5.0, 3.0);

        // Assert
        Assert.False((bool)result);
    }

    [Fact]
    public void Gt_大きい値はtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.Gt(5.0, 3.0);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void Gt_小さい値はfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.Gt(3.0, 5.0);

        // Assert
        Assert.False((bool)result);
    }

    [Fact]
    public void Gt_等しい値はfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.Gt(5.0, 5.0);

        // Assert
        Assert.False((bool)result);
    }

    [Fact]
    public void Ge_大きい値はtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.Ge(5.0, 3.0);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void Ge_等しい値はtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.Ge(5.0, 5.0);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void Ge_小さい値はfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.Ge(3.0, 5.0);

        // Assert
        Assert.False((bool)result);
    }

    #endregion

    #region Not Tests

    [Fact]
    public void Not_trueはfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.Not(true);

        // Assert
        Assert.False((bool)result);
    }

    [Fact]
    public void Not_falseはtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.Not(false);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void Not_nullはtrueを返す()
    {
        // Act
        var result = RuntimeHelpers.Not(null);

        // Assert
        Assert.True((bool)result);
    }

    [Fact]
    public void Not_オブジェクトはfalseを返す()
    {
        // Act
        var result = RuntimeHelpers.Not(new object());

        // Assert
        Assert.False((bool)result);
    }

    #endregion

    #region Invoke Tests

    [Fact]
    public void Invoke_IroCallableを呼び出せる()
    {
        // Arrange
        var ctx = new ScriptContext();
        var callable = new TestCallable("result");
        var args = new object[] { "arg1", "arg2" };

        // Act
        var result = RuntimeHelpers.Invoke(callable, ctx, args);

        // Assert
        Assert.Equal("result", result);
    }

    [Fact]
    public void Invoke_IroCallableでない場合は例外を投げる()
    {
        // Arrange
        var ctx = new ScriptContext();
        var notCallable = "not a function";
        var args = new object[] { };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            RuntimeHelpers.Invoke(notCallable, ctx, args));
    }

    #endregion

    #region GetMember Tests

    [Fact]
    public void GetMember_IroInstanceのフィールドを取得できる()
    {
        // Arrange
        var iroClass = new IroClass("TestClass", Array.Empty<FieldDef>(), Array.Empty<MethodDef>());
        var instance = new IroInstance(iroClass);
        instance.Fields["fieldName"] = "fieldValue";

        // Act
        var result = RuntimeHelpers.GetMember(instance, "fieldName");

        // Assert
        Assert.Equal("fieldValue", result);
    }

    [Fact]
    public void GetMember_存在しないフィールドは例外を投げる()
    {
        // Arrange
        var iroClass = new IroClass("TestClass", Array.Empty<FieldDef>(), Array.Empty<MethodDef>());
        var instance = new IroInstance(iroClass);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            RuntimeHelpers.GetMember(instance, "nonExistent"));
    }

    [Fact]
    public void GetMember_IroInstanceでない場合は例外を投げる()
    {
        // Arrange
        var notInstance = "not an instance";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            RuntimeHelpers.GetMember(notInstance, "fieldName"));
    }

    #endregion

    #region SetMember Tests

    [Fact]
    public void SetMember_IroInstanceのフィールドを設定できる()
    {
        // Arrange
        var iroClass = new IroClass("TestClass", Array.Empty<FieldDef>(), Array.Empty<MethodDef>());
        var instance = new IroInstance(iroClass);

        // Act
        var result = RuntimeHelpers.SetMember(instance, "fieldName", "newValue");

        // Assert
        Assert.Equal("newValue", result);
        Assert.Equal("newValue", instance.Fields["fieldName"]);
    }

    [Fact]
    public void SetMember_既存フィールドを上書きできる()
    {
        // Arrange
        var iroClass = new IroClass("TestClass", Array.Empty<FieldDef>(), Array.Empty<MethodDef>());
        var instance = new IroInstance(iroClass);
        instance.Fields["fieldName"] = "oldValue";

        // Act
        var result = RuntimeHelpers.SetMember(instance, "fieldName", "newValue");

        // Assert
        Assert.Equal("newValue", result);
        Assert.Equal("newValue", instance.Fields["fieldName"]);
    }

    [Fact]
    public void SetMember_IroInstanceでない場合は例外を投げる()
    {
        // Arrange
        var notInstance = "not an instance";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            RuntimeHelpers.SetMember(notInstance, "fieldName", "value"));
    }

    #endregion

    #region NewInstance Tests

    [Fact]
    public void NewInstance_インスタンスを生成できる()
    {
        // Arrange
        var ctx = new ScriptContext();
        var iroClass = new IroClass("TestClass", Array.Empty<FieldDef>(), Array.Empty<MethodDef>());
        ctx.Classes["TestClass"] = iroClass;

        // Act
        var result = RuntimeHelpers.NewInstance("TestClass", ctx, new object[] { });

        // Assert
        Assert.NotNull(result);
        Assert.IsType<IroInstance>(result);
        var instance = (IroInstance)result;
        Assert.Equal(iroClass, instance.Class);
    }

    [Fact]
    public void NewInstance_フィールド初期化が行われる()
    {
        // Arrange
        var ctx = new ScriptContext();
        var field = new FieldDef("testField", isPublic: true, initializer: (ctx) => "initialValue");
        var iroClass = new IroClass("TestClass", new[] { field }, Array.Empty<MethodDef>());
        ctx.Classes["TestClass"] = iroClass;

        // Act
        var result = RuntimeHelpers.NewInstance("TestClass", ctx, new object[] { });

        // Assert
        var instance = (IroInstance)result;
        Assert.Equal("initialValue", instance.Fields["testField"]);
    }

    [Fact]
    public void NewInstance_initメソッドが呼ばれる()
    {
        // Arrange
        var ctx = new ScriptContext();

        // init メソッドを追加
        var initCallable = new TestCallableWithSideEffect(ctx, "initCalled", "true");
        var initMethod = new MethodDef("init", isPublic: true, isStatic: false, initCallable);
        var iroClass = new IroClass("TestClass", Array.Empty<FieldDef>(), new[] { initMethod });

        ctx.Classes["TestClass"] = iroClass;

        // Act
        var result = RuntimeHelpers.NewInstance("TestClass", ctx, new object[] { });

        // Assert
        Assert.Equal("true", ctx.Globals["initCalled"]);
    }

    [Fact]
    public void NewInstance_存在しないクラスは例外を投げる()
    {
        // Arrange
        var ctx = new ScriptContext();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            RuntimeHelpers.NewInstance("NonExistentClass", ctx, new object[] { }));
    }

    #endregion

    #region CreateList Tests

    [Fact]
    public void CreateList_空の配列からリストを生成できる()
    {
        // Arrange
        var elements = new object[] { };

        // Act
        var result = RuntimeHelpers.CreateList(elements);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<object>>(result);
        var list = (List<object>)result;
        Assert.Empty(list);
    }

    [Fact]
    public void CreateList_要素を持つ配列からリストを生成できる()
    {
        // Arrange
        var elements = new object[] { 1.0, 2.0, 3.0 };

        // Act
        var result = RuntimeHelpers.CreateList(elements);

        // Assert
        Assert.NotNull(result);
        var list = (List<object>)result;
        Assert.Equal(3, list.Count);
        Assert.Equal(1.0, list[0]);
        Assert.Equal(2.0, list[1]);
        Assert.Equal(3.0, list[2]);
    }

    [Fact]
    public void CreateList_異なる型の要素を含むリストを生成できる()
    {
        // Arrange
        var elements = new object[] { 1.0, "text", true };

        // Act
        var result = RuntimeHelpers.CreateList(elements);

        // Assert
        var list = (List<object>)result;
        Assert.Equal(3, list.Count);
        Assert.Equal(1.0, list[0]);
        Assert.Equal("text", list[1]);
        Assert.Equal(true, list[2]);
    }

    #endregion

    #region CreateHash Tests

    [Fact]
    public void CreateHash_空のペア配列からハッシュを生成できる()
    {
        // Arrange
        var pairs = new (string Key, object Value)[] { };

        // Act
        var result = RuntimeHelpers.CreateHash(pairs);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Dictionary<string, object>>(result);
        var dict = (Dictionary<string, object>)result;
        Assert.Empty(dict);
    }

    [Fact]
    public void CreateHash_ペア配列からハッシュを生成できる()
    {
        // Arrange
        var pairs = new (string Key, object Value)[]
        {
            ("name", "Alice"),
            ("age", 30.0),
            ("active", true)
        };

        // Act
        var result = RuntimeHelpers.CreateHash(pairs);

        // Assert
        var dict = (Dictionary<string, object>)result;
        Assert.Equal(3, dict.Count);
        Assert.Equal("Alice", dict["name"]);
        Assert.Equal(30.0, dict["age"]);
        Assert.Equal(true, dict["active"]);
    }

    [Fact]
    public void CreateHash_同じキーで上書きされる()
    {
        // Arrange
        var pairs = new (string Key, object Value)[]
        {
            ("key", "first"),
            ("key", "second")
        };

        // Act
        var result = RuntimeHelpers.CreateHash(pairs);

        // Assert
        var dict = (Dictionary<string, object>)result;
        Assert.Single(dict);
        Assert.Equal("second", dict["key"]);
    }

    #endregion

    #region GetIndexed Tests

    [Fact]
    public void GetIndexed_リストから要素を取得できる()
    {
        // Arrange
        var list = new List<object> { "a", "b", "c" };

        // Act
        var result = RuntimeHelpers.GetIndexed(list, 1);

        // Assert
        Assert.Equal("b", result);
    }

    [Fact]
    public void GetIndexed_リストのインデックスが範囲外の場合は例外を投げる()
    {
        // Arrange
        var list = new List<object> { "a", "b", "c" };

        // Act & Assert
        var ex = Assert.Throws<RuntimeException>(() => RuntimeHelpers.GetIndexed(list, 10));
        Assert.Contains("index out of range", ex.Message.ToLower());
    }

    [Fact]
    public void GetIndexed_リストの負のインデックスは例外を投げる()
    {
        // Arrange
        var list = new List<object> { "a", "b", "c" };

        // Act & Assert
        var ex = Assert.Throws<RuntimeException>(() => RuntimeHelpers.GetIndexed(list, -1));
        Assert.Contains("index out of range", ex.Message.ToLower());
    }

    [Fact]
    public void GetIndexed_ハッシュから要素を取得できる()
    {
        // Arrange
        var dict = new Dictionary<string, object>
        {
            ["name"] = "Alice",
            ["age"] = 30.0
        };

        // Act
        var result = RuntimeHelpers.GetIndexed(dict, "name");

        // Assert
        Assert.Equal("Alice", result);
    }

    [Fact]
    public void GetIndexed_ハッシュの存在しないキーは例外を投げる()
    {
        // Arrange
        var dict = new Dictionary<string, object>
        {
            ["name"] = "Alice"
        };

        // Act & Assert
        var ex = Assert.Throws<RuntimeException>(() => RuntimeHelpers.GetIndexed(dict, "age"));
        Assert.Contains("key not found", ex.Message.ToLower());
    }

    [Fact]
    public void GetIndexed_ハッシュのnullキーは例外を投げる()
    {
        // Arrange
        var dict = new Dictionary<string, object>
        {
            ["name"] = "Alice"
        };

        // Act & Assert
        var ex = Assert.Throws<RuntimeException>(() => RuntimeHelpers.GetIndexed(dict, null!));
        Assert.Contains("key cannot be null", ex.Message.ToLower());
    }

    [Fact]
    public void GetIndexed_サポートされていない型は例外を投げる()
    {
        // Arrange
        var notIndexable = "string";

        // Act & Assert
        var ex = Assert.Throws<RuntimeException>(() => RuntimeHelpers.GetIndexed(notIndexable, 0));
        Assert.Contains("cannot index", ex.Message.ToLower());
    }

    #endregion

    #region SetIndexed Tests

    [Fact]
    public void SetIndexed_リストの要素を設定できる()
    {
        // Arrange
        var list = new List<object> { "a", "b", "c" };

        // Act
        var result = RuntimeHelpers.SetIndexed(list, 1, "B");

        // Assert
        Assert.Equal("B", result);
        Assert.Equal("B", list[1]);
    }

    [Fact]
    public void SetIndexed_リストのインデックスが範囲外の場合は例外を投げる()
    {
        // Arrange
        var list = new List<object> { "a", "b", "c" };

        // Act & Assert
        var ex = Assert.Throws<RuntimeException>(() => RuntimeHelpers.SetIndexed(list, 10, "value"));
        Assert.Contains("index out of range", ex.Message.ToLower());
    }

    [Fact]
    public void SetIndexed_リストの負のインデックスは例外を投げる()
    {
        // Arrange
        var list = new List<object> { "a", "b", "c" };

        // Act & Assert
        var ex = Assert.Throws<RuntimeException>(() => RuntimeHelpers.SetIndexed(list, -1, "value"));
        Assert.Contains("index out of range", ex.Message.ToLower());
    }

    [Fact]
    public void SetIndexed_ハッシュの要素を設定できる()
    {
        // Arrange
        var dict = new Dictionary<string, object>
        {
            ["name"] = "Alice"
        };

        // Act
        var result = RuntimeHelpers.SetIndexed(dict, "name", "Bob");

        // Assert
        Assert.Equal("Bob", result);
        Assert.Equal("Bob", dict["name"]);
    }

    [Fact]
    public void SetIndexed_ハッシュに新しいキーを追加できる()
    {
        // Arrange
        var dict = new Dictionary<string, object>
        {
            ["name"] = "Alice"
        };

        // Act
        var result = RuntimeHelpers.SetIndexed(dict, "age", 30.0);

        // Assert
        Assert.Equal(30.0, result);
        Assert.Equal(30.0, dict["age"]);
    }

    [Fact]
    public void SetIndexed_ハッシュのnullキーは例外を投げる()
    {
        // Arrange
        var dict = new Dictionary<string, object>
        {
            ["name"] = "Alice"
        };

        // Act & Assert
        var ex = Assert.Throws<RuntimeException>(() => RuntimeHelpers.SetIndexed(dict, null!, "value"));
        Assert.Contains("key cannot be null", ex.Message.ToLower());
    }

    [Fact]
    public void SetIndexed_サポートされていない型は例外を投げる()
    {
        // Arrange
        var notIndexable = "string";

        // Act & Assert
        var ex = Assert.Throws<RuntimeException>(() => RuntimeHelpers.SetIndexed(notIndexable, 0, "value"));
        Assert.Contains("cannot index", ex.Message.ToLower());
    }

    #endregion

    #region Test Helper Classes

    private class TestCallable : IroCallable
    {
        private readonly string _result;

        public TestCallable(string result = "test result")
        {
            _result = result;
        }

        public object Invoke(ScriptContext ctx, object[] args)
        {
            return _result;
        }
    }

    private class TestCallableWithSideEffect : IroCallable
    {
        private readonly ScriptContext _ctx;
        private readonly string _key;
        private readonly string _value;

        public TestCallableWithSideEffect(ScriptContext ctx, string key, string value)
        {
            _ctx = ctx;
            _key = key;
            _value = value;
        }

        public object Invoke(ScriptContext ctx, object[] args)
        {
            _ctx.Globals[_key] = _value;
            return null!;
        }
    }

    #endregion
}
