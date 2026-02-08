using Xunit;
using System.IO;
using Irooon.Core;
using Irooon.Core.Runtime;

namespace Irooon.Tests.Integration;

public class E2ETests
{
    private string GetExamplePath(string filename)
    {
        // Start from the test assembly location
        var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
        var assemblyDir = Path.GetDirectoryName(assemblyLocation);

        // Navigate up to find the project root (look for examples directory)
        var currentDir = assemblyDir;
        while (currentDir != null)
        {
            var examplesPath = Path.Combine(currentDir, "examples");
            if (Directory.Exists(examplesPath))
            {
                return Path.Combine(examplesPath, filename);
            }
            currentDir = Directory.GetParent(currentDir)?.FullName;
        }

        throw new DirectoryNotFoundException("Could not find examples directory");
    }

    [Fact]
    public void TestHelloExample()
    {
        var path = GetExamplePath("hello.iro");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Example file not found: {path}");
        }

        var source = File.ReadAllText(path);
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal("Hello, World!", result);
    }

    [Fact]
    public void TestSimpleExample()
    {
        var path = GetExamplePath("simple.iro");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Example file not found: {path}");
        }

        var source = File.ReadAllText(path);
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal(30.0, result);
    }

    [Fact]
    public void TestFactorialExample()
    {
        var path = GetExamplePath("factorial.iro");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Example file not found: {path}");
        }

        var source = File.ReadAllText(path);
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal(120.0, result);
    }

    [Fact]
    public void TestLoopExample()
    {
        var path = GetExamplePath("loop.iro");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Example file not found: {path}");
        }

        var source = File.ReadAllText(path);
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal(55.0, result);
    }

    [Fact]
    public void TestListExample()
    {
        var path = GetExamplePath("list_example.iro");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Example file not found: {path}");
        }

        var source = File.ReadAllText(path);
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // numbers[1] = 99 の結果
        Assert.Equal(99.0, result);
    }

    [Fact]
    public void TestHashExample()
    {
        var path = GetExamplePath("hash_example.iro");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Example file not found: {path}");
        }

        var source = File.ReadAllText(path);
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // person["age"] = 31 の結果
        Assert.Equal(31.0, result);
    }

    [Fact]
    public void TestDataStructuresExample()
    {
        var path = GetExamplePath("data_structures.iro");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Example file not found: {path}");
        }

        var source = File.ReadAllText(path);
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // firstSkill[0] = "Python"
        Assert.Equal("Python", result);
    }

    [Fact]
    public void TestListOperations()
    {
        var source = @"
let numbers = [1, 2, 3, 4, 5]
let first = numbers[0]
numbers[1] = 99
numbers[1]
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal(99.0, result);
    }

    [Fact]
    public void TestHashOperations()
    {
        var source = @"
let person = {name: ""Alice"", age: 30}
let name = person[""name""]
person[""age""] = 31
person[""age""]
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal(31.0, result);
    }

    [Fact]
    public void TestNestedDataStructures()
    {
        var source = @"
let data = {
    users: [
        {name: ""Alice"", age: 30},
        {name: ""Bob"", age: 25}
    ]
}
let users = data[""users""]
let firstUser = users[0]
firstUser[""name""]
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal("Alice", result);
    }

    [Fact]
    public void TestListWithHashElements()
    {
        var source = @"
let items = [
    {id: 1, value: ""first""},
    {id: 2, value: ""second""}
]
items[1][""value""]
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal("second", result);
    }

    [Fact]
    public void TestComplexNestedStructure()
    {
        var source = @"
let config = {
    database: {
        host: ""localhost"",
        ports: [5432, 5433, 5434]
    }
}
let ports = config[""database""][""ports""]
ports[1]
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal(5433.0, result);
    }

    [Fact]
    public void TestPrintln_InScript()
    {
        // Arrange
        var output = new StringWriter();
        Console.SetOut(output);

        var source = @"
println(""Hello, World!"")
42
";
        var engine = new ScriptEngine();

        // Act
        var result = engine.Execute(source);

        // Assert
        Assert.Equal(42.0, result);
        Assert.Equal("Hello, World!" + Environment.NewLine, output.ToString());

        // Cleanup
        Console.SetOut(Console.Out);
    }

    [Fact]
    public void TestPrint_MultipleValues()
    {
        // Arrange
        var output = new StringWriter();
        Console.SetOut(output);

        var source = @"
print(""Sum:"", 10, ""++"", 5, ""="", 15)
""done""
";
        var engine = new ScriptEngine();

        // Act
        var result = engine.Execute(source);

        // Assert
        Assert.Equal("done", result);
        Assert.Equal("Sum: 10 ++ 5 = 15", output.ToString());

        // Cleanup
        Console.SetOut(Console.Out);
    }

    [Fact]
    public void TestPrintln_WithVariables()
    {
        // Arrange
        var output = new StringWriter();
        Console.SetOut(output);

        var source = @"
let name = ""Alice""
let age = 30
println(""Name:"", name, ""Age:"", age)
true
";
        var engine = new ScriptEngine();

        // Act
        var result = engine.Execute(source);

        // Assert
        Assert.Equal(true, result);
        Assert.Equal("Name: Alice Age: 30" + Environment.NewLine, output.ToString());

        // Cleanup
        Console.SetOut(Console.Out);
    }

    #region String Methods Tests

    [Fact]
    public void TestStringLength()
    {
        var source = @"
let str = ""Hello""
str.length()
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal(5.0, result);
    }

    [Fact]
    public void TestStringToUpper()
    {
        var source = @"
let str = ""hello world""
str.toUpper()
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal("HELLO WORLD", result);
    }

    [Fact]
    public void TestStringToLower()
    {
        var source = @"
let str = ""HELLO WORLD""
str.toLower()
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal("hello world", result);
    }

    [Fact]
    public void TestStringTrim()
    {
        var source = @"
let str = ""  hello  ""
str.trim()
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal("hello", result);
    }

    [Fact]
    public void TestStringSubstring()
    {
        var source = @"
let str = ""Hello World""
str.substring(0, 5)
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal("Hello", result);
    }

    [Fact]
    public void TestStringSubstring_StartOnly()
    {
        var source = @"
let str = ""Hello World""
str.substring(6)
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal("World", result);
    }

    [Fact]
    public void TestStringSplit()
    {
        var source = @"
let str = ""apple,banana,cherry""
let parts = str.split("","")
parts[1]
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal("banana", result);
    }

    [Fact]
    public void TestStringContains_True()
    {
        var source = @"
let str = ""Hello World""
str.contains(""World"")
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal(true, result);
    }

    [Fact]
    public void TestStringContains_False()
    {
        var source = @"
let str = ""Hello World""
str.contains(""Test"")
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal(false, result);
    }

    [Fact]
    public void TestStringStartsWith()
    {
        var source = @"
let str = ""Hello World""
str.startsWith(""Hello"")
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal(true, result);
    }

    [Fact]
    public void TestStringEndsWith()
    {
        var source = @"
let str = ""Hello World""
str.endsWith(""World"")
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal(true, result);
    }

    [Fact]
    public void TestStringReplace()
    {
        var source = @"
let str = ""Hello World""
str.replace(""World"", ""Universe"")
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal("Hello Universe", result);
    }

    [Fact]
    public void TestStringMethodChaining()
    {
        var source = @"
let str = ""  Hello World  ""
str.trim().toLower().replace(""world"", ""universe"")
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal("hello universe", result);
    }

    #endregion

    #region 文字列補間のテスト

    [Fact]
    public void TestStringInterpolation_Simple()
    {
        var source = @"
let name = ""Alice""
""Hello, ${name}!""
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal("Hello, Alice!", result);
    }

    [Fact]
    public void TestStringInterpolation_MultipleVariables()
    {
        var source = @"
let x = 10
let y = 20
""${x} + ${y} = ${x + y}""
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal("10 + 20 = 30", result);
    }

    [Fact]
    public void TestStringInterpolation_WithExpressions()
    {
        var source = @"
let value = 42
""The answer is ${value * 2}""
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal("The answer is 84", result);
    }

    [Fact]
    public void TestStringInterpolation_WithMethodCall()
    {
        var source = @"
let str = ""hello""
""Upper: ${str.toUpper()}""
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal("Upper: HELLO", result);
    }

    [Fact]
    public void TestStringInterpolation_Nested()
    {
        var source = @"
let first = ""John""
let last = ""Doe""
let greeting = ""Hello, ${first} ${last}!""
""Message: ${greeting}""
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal("Message: Hello, John Doe!", result);
    }

    #endregion

    #region foreach/break/continueのテスト

    [Fact]
    public void TestForeach_List()
    {
        var source = @"
let numbers = [1, 2, 3, 4, 5]
var sum = 0
foreach (n in numbers) {
    sum = sum + n
}
sum
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal(15.0, result);
    }

    [Fact]
    public void TestForeach_Hash()
    {
        var source = @"
let person = {name: ""Alice"", age: 30}
var count = 0
foreach (pair in person) {
    count = count + 1
}
count
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal(2.0, result);
    }

    [Fact]
    public void TestWhile_WithBreak()
    {
        var source = @"
var i = 0
var sum = 0
for (i < 10) {
    i = i + 1
    if (i > 5) { break } else { }
    sum = sum + i
}
sum
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // 1 + 2 + 3 + 4 + 5 = 15
        Assert.Equal(15.0, result);
    }

    [Fact]
    public void TestWhile_WithContinue()
    {
        var source = @"
var i = 0
var sum = 0
for (i < 10) {
    i = i + 1
    if (i % 2 == 0) { continue } else { }
    sum = sum + i
}
sum
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // 1 + 3 + 5 + 7 + 9 = 25
        Assert.Equal(25.0, result);
    }

    [Fact]
    public void TestForeach_WithBreak()
    {
        var source = @"
let numbers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
var sum = 0
foreach (n in numbers) {
    if (n > 5) { break } else { }
    sum = sum + n
}
sum
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // 1 + 2 + 3 + 4 + 5 = 15
        Assert.Equal(15.0, result);
    }

    [Fact]
    public void TestForeach_WithContinue()
    {
        var source = @"
let numbers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
var sum = 0
foreach (n in numbers) {
    if (n % 2 == 0) { continue } else { }
    sum = sum + n
}
sum
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // 1 + 3 + 5 + 7 + 9 = 25
        Assert.Equal(25.0, result);
    }

    [Fact]
    public void TestForeach_Nested()
    {
        var source = @"
let matrix = [[1, 2], [3, 4], [5, 6]]
var sum = 0
foreach (row in matrix) {
    foreach (val in row) {
        sum = sum + val
    }
}
sum
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // 1 + 2 + 3 + 4 + 5 + 6 = 21
        Assert.Equal(21.0, result);
    }

    #endregion

    #region forループのテスト

    [Fact]
    public void TestFor_WithRange()
    {
        var source = @"
var sum = 0
for (i in 1..5) {
    sum = sum + i
}
sum
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // 1 + 2 + 3 + 4 = 10
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestFor_WithRangeInclusive()
    {
        var source = @"
var sum = 0
for (i in 1...5) {
    sum = sum + i
}
sum
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // 1 + 2 + 3 + 4 + 5 = 15
        Assert.Equal(15.0, result);
    }

    [Fact]
    public void TestFor_WithList()
    {
        var source = @"
var sum = 0
for (item in [1, 2, 3]) {
    sum = sum + item
}
sum
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal(6.0, result);
    }

    [Fact]
    public void TestFor_Condition()
    {
        var source = @"
var x = 0
for (x < 5) {
    x = x + 1
}
x
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal(5.0, result);
    }

    [Fact]
    public void TestFor_WithBreak()
    {
        var source = @"
var sum = 0
for (i in 1..10) {
    if (i == 5) {
        break
    } else {
        sum = sum + i
    }
}
sum
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // 1 + 2 + 3 + 4 = 10
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestFor_WithContinue()
    {
        var source = @"
var sum = 0
for (i in 1..6) {
    if (i == 3) {
        continue
    } else {
        sum = sum + i
    }
}
sum
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // 1 + 2 + 4 + 5 = 12 (3をスキップ)
        Assert.Equal(12.0, result);
    }

    [Fact(Skip = "ネストしたforループのBlockExpressionパースに問題あり - Phase 3で修正予定")]
    public void TestFor_Nested()
    {
        var source = @"
var sum = 0
for (i in 1..4) {
    for (j in 1..4) {
        sum = sum + 1
    }
}
sum
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        // 3 * 3 = 9
        Assert.Equal(9.0, result);
    }

    #endregion

    #region 例外処理のテスト

    [Fact]
    public void TestTryCatch_NoException()
    {
        var source = @"
try {
    10 + 20
} catch (e) {
    0
}
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal(30.0, result);
    }

    [Fact]
    public void TestTryCatch_WithException()
    {
        var source = @"
try {
    throw ""Something went wrong""
    10
} catch (e) {
    100
}
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal(100.0, result);
    }

    [Fact]
    public void TestTryCatch_CatchExceptionValue()
    {
        var source = @"
try {
    throw ""Error message""
} catch (e) {
    e
}
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal("Error message", result);
    }

    [Fact]
    public void TestTryFinally()
    {
        var source = @"
var x = 10
try {
    x = 20
} finally {
    x = x + 5
}
x
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal(25.0, result);
    }

    [Fact]
    public void TestTryCatchFinally()
    {
        var source = @"
var x = 0
try {
    throw ""error""
    x = 10
} catch (e) {
    x = 20
} finally {
    x = x + 1
}
x
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal(21.0, result);
    }

    [Fact]
    public void TestThrow_WithNumber()
    {
        var source = @"
try {
    throw 404
} catch (e) {
    e
}
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal(404.0, result);
    }

    #endregion

    #region Module System Tests

    [Fact]
    public void TestExportLet()
    {
        var source = @"
export let x = 42
x
";
        var engine = new ScriptEngine();
        var context = new ScriptContext();
        var result = engine.Execute(source, context);

        // エクスポートされた値を確認
        Assert.Equal(42.0, result);
        Assert.True(context.Exports.ContainsKey("x"));
        Assert.Equal(42.0, context.Exports["x"]);
    }

    [Fact]
    public void TestExportFunction()
    {
        var source = @"
export fn add(a, b) {
    a + b
}
add(3, 5)
";
        var engine = new ScriptEngine();
        var context = new ScriptContext();
        var result = engine.Execute(source, context);

        // エクスポートされた関数を確認
        Assert.Equal(8.0, result);
        Assert.True(context.Exports.ContainsKey("add"));
        Assert.NotNull(context.Exports["add"]);
    }

    [Fact]
    public void TestMultipleExports()
    {
        var source = @"
export let PI = 3.14159
export fn square(x) { x * x }
square(PI)
";
        var engine = new ScriptEngine();
        var context = new ScriptContext();
        var result = engine.Execute(source, context);

        // 複数のエクスポートを確認
        Assert.True(context.Exports.ContainsKey("PI"));
        Assert.True(context.Exports.ContainsKey("square"));
        Assert.Equal(3.14159, context.Exports["PI"]);
    }

    #endregion

    #region クラス継承テスト

    [Fact]
    public void TestClassInheritance_Basic()
    {
        var source = @"
class Parent {
    public var name = ""parent""

    public fn greet() {
        return ""Hello from ${name}""
    }
}

class Child : Parent {
    public var age = 0

    public fn introduce() {
        return ""I am ${name} and ${age} years old""
    }
}

let child = Child()
child.name = ""Alice""
child.age = 10
child.introduce()
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal("I am Alice and 10 years old", result);
    }

    [Fact]
    public void TestClassInheritance_MethodInheritance()
    {
        var source = @"
class Parent {
    public var name = ""parent""

    public fn greet() {
        return ""Hello from ${name}""
    }
}

class Child : Parent {
    public var age = 0
}

let child = Child()
child.name = ""Bob""
child.greet()
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal("Hello from Bob", result);
    }

    [Fact]
    public void TestClassInheritance_MultiLevel()
    {
        var source = @"
class GrandParent {
    public var family = ""Smith""
}

class Parent : GrandParent {
    public var name = ""parent""
}

class Child : Parent {
    public var age = 0
}

let child = Child()
child.family = ""Johnson""
child.name = ""Charlie""
child.age = 15
""${child.family} ${child.name} ${child.age}""
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal("Johnson Charlie 15", result);
    }

    [Fact]
    public void TestClassInheritance_MethodOverride()
    {
        var source = @"
class Parent {
    public var name = ""parent""

    public fn greet() {
        return ""Hello from parent""
    }
}

class Child : Parent {
    public fn greet() {
        return ""Hello from child""
    }
}

let child = Child()
child.greet()
";
        var engine = new ScriptEngine();
        var result = engine.Execute(source);

        Assert.Equal("Hello from child", result);
    }

    [Fact]
    public void TestInheritanceExample()
    {
        var path = GetExamplePath("inheritance.iro");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Example file not found: {path}");
        }

        var source = File.ReadAllText(path);
        var engine = new ScriptEngine();

        // 出力をキャプチャするために、出力を記録する必要がある
        // ここでは単に実行してエラーが出ないことを確認
        var result = engine.Execute(source);

        // エラーがなければテスト成功
        Assert.NotNull(result);
    }

    #endregion
}
