using Xunit;
using System.IO;
using Irooon.Core;

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
}
