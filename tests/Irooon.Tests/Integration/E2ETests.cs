using Xunit;
using System.IO;
using Irooon.Core;

namespace Irooon.Tests.Integration;

public class E2ETests
{
    private string GetExamplePath(string filename)
    {
        var currentDir = Directory.GetCurrentDirectory();
        var projectRoot = Path.GetFullPath(Path.Combine(currentDir, "../../../.."));
        return Path.Combine(projectRoot, "examples", filename);
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
}
