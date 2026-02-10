using Xunit;
using Irooon.Core;
using Irooon.Core.Runtime;

namespace Irooon.Tests.Runtime;

public class ModuleTests
{
    /// <summary>
    /// import文のE2Eテスト用ヘルパー
    /// テスト用に一時ファイルを作成してimportをテストする
    /// </summary>
    private object? ExecuteWithModule(string moduleSource, string moduleFileName, string mainSource)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"irooon_test_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        try
        {
            // モジュールファイルを作成
            var modulePath = Path.Combine(tempDir, moduleFileName);
            File.WriteAllText(modulePath, moduleSource);

            // メインスクリプトを作成・実行
            var mainPath = Path.Combine(tempDir, "main.iro");
            File.WriteAllText(mainPath, mainSource);

            var engine = new ScriptEngine();
            var ctx = new ScriptContext();
            ctx.InitializeStdlib((code, c) => engine.Execute(code, c));

            // ModuleLoaderにベースディレクトリを設定
            ctx.ModuleLoader = new ModuleLoader((code, c) => engine.Execute(code, c));
            ctx.ModuleBaseDir = tempDir;

            return engine.Execute(mainSource, ctx);
        }
        finally
        {
            // クリーンアップ
            try { Directory.Delete(tempDir, true); } catch { }
        }
    }

    [Fact]
    public void Import_Function()
    {
        var module = @"
            export fn add(a, b) {
                a + b
            }
        ";

        var main = @"
            import { add } from ""./math.iro""
            add(3, 4)
        ";

        var result = ExecuteWithModule(module, "math.iro", main);
        Assert.Equal(7.0, result);
    }

    [Fact]
    public void Import_Variable()
    {
        var module = @"
            export let PI = 3.14159
        ";

        var main = @"
            import { PI } from ""./constants.iro""
            PI
        ";

        var result = ExecuteWithModule(module, "constants.iro", main);
        Assert.Equal(3.14159, result);
    }

    [Fact]
    public void Import_MultipleNames()
    {
        var module = @"
            export fn add(a, b) { a + b }
            export let VERSION = ""1.0""
        ";

        var main = @"
            import { add, VERSION } from ""./lib.iro""
            add(10, 20) + VERSION
        ";

        var result = ExecuteWithModule(module, "lib.iro", main);
        Assert.Equal("301.0", result); // 30 + "1.0" → "301.0"
    }

    [Fact]
    public void Import_ModuleCached()
    {
        // 同じモジュールを2回importしても1回しか実行されない
        var module = @"
            export let value = 42
        ";

        var main = @"
            import { value } from ""./cached.iro""
            value
        ";

        var result = ExecuteWithModule(module, "cached.iro", main);
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void Import_ExportedVar()
    {
        var module = @"
            export var counter = 99
        ";

        var main = @"
            import { counter } from ""./state.iro""
            counter
        ";

        var result = ExecuteWithModule(module, "state.iro", main);
        Assert.Equal(99.0, result);
    }

    [Fact]
    public void Import_ExportedClass()
    {
        var module = @"
            export class Calculator {
                fn add(a, b) {
                    a + b
                }
            }
        ";

        var main = @"
            import { Calculator } from ""./calc.iro""
            var c = Calculator()
            c.add(3, 4)
        ";

        var result = ExecuteWithModule(module, "calc.iro", main);
        Assert.Equal(7.0, result);
    }
}
