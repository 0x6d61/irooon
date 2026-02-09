using System.IO;

namespace Irooon.Core.Runtime;

/// <summary>
/// モジュールローダー。
/// irooonスクリプトファイルを読み込み、エクスポートされた値を返します。
/// </summary>
public class ModuleLoader
{
    /// <summary>
    /// モジュールキャッシュ（パス → エクスポートテーブル）
    /// </summary>
    private readonly Dictionary<string, Dictionary<string, object?>> _cache = new();

    /// <summary>
    /// スクリプト実行デリゲート
    /// </summary>
    private readonly Action<string, ScriptContext> _executeFunc;

    public ModuleLoader(Action<string, ScriptContext> executeFunc)
    {
        _executeFunc = executeFunc;
    }

    /// <summary>
    /// モジュールをロードします。
    /// </summary>
    public Dictionary<string, object?> LoadModule(string path, string currentDir)
    {
        var fullPath = ResolvePath(path, currentDir);

        if (_cache.TryGetValue(fullPath, out var cached))
        {
            return cached;
        }

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Module not found: {fullPath}");
        }

        var source = File.ReadAllText(fullPath);
        var exports = ExecuteModule(source, Path.GetDirectoryName(fullPath) ?? currentDir);

        _cache[fullPath] = exports;
        return exports;
    }

    /// <summary>
    /// パスを解決します（相対パス → 絶対パス）。
    /// </summary>
    public string ResolvePath(string path, string currentDir)
    {
        if (Path.IsPathRooted(path))
        {
            return Path.GetFullPath(path);
        }

        var combinedPath = Path.Combine(currentDir, path);
        return Path.GetFullPath(combinedPath);
    }

    /// <summary>
    /// モジュールを実行してエクスポートテーブルを返します。
    /// </summary>
    private Dictionary<string, object?> ExecuteModule(string source, string moduleDir)
    {
        // モジュール用の独立したコンテキストを作成
        var moduleCtx = new ScriptContext();
        moduleCtx.InitializeStdlib(_executeFunc);
        moduleCtx.ModuleLoader = this;
        moduleCtx.ModuleBaseDir = moduleDir;

        // モジュールを実行（export文がctx.Exportsに値を登録する）
        _executeFunc(source, moduleCtx);

        return moduleCtx.Exports;
    }
}
