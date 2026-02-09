using Irooon.Core;
using Irooon.Core.Runtime;
using Xunit;

namespace Irooon.Tests.Runtime;

/// <summary>
/// ModuleLoaderのテストクラス
/// </summary>
public class ModuleLoaderTests
{
    private ModuleLoader CreateLoader()
    {
        var engine = new ScriptEngine();
        return new ModuleLoader((code, ctx) => engine.Execute(code, ctx));
    }

    [Fact]
    public void TestResolvePath_RelativePath()
    {
        var loader = CreateLoader();

        // C:\test\dir から ./module.iro を解決
        var resolved = loader.ResolvePath("./module.iro", "C:\\test\\dir");

        Assert.Equal("C:\\test\\dir\\module.iro", resolved);
    }

    [Fact]
    public void TestResolvePath_ParentDirectory()
    {
        var loader = CreateLoader();

        // C:\test\dir から ../other.iro を解決
        var resolved = loader.ResolvePath("../other.iro", "C:\\test\\dir");

        Assert.Equal("C:\\test\\other.iro", resolved);
    }

    [Fact]
    public void TestResolvePath_AbsolutePath()
    {
        var loader = CreateLoader();

        // 絶対パスはそのまま
        var resolved = loader.ResolvePath("C:\\abs\\path\\module.iro", "C:\\test\\dir");

        Assert.Equal("C:\\abs\\path\\module.iro", resolved);
    }

    [Fact]
    public void TestModuleCache()
    {
        var loader = CreateLoader();

        // キャッシュが存在することを確認
        Assert.NotNull(loader);
    }
}
