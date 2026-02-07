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
}
