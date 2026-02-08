using System.IO;
using Irooon.Core.Ast;
using Irooon.Core.Ast.Statements;
using Irooon.Core.Lexer;
using Irooon.Core.Parser;
using Irooon.Core.Resolver;
using Irooon.Core.CodeGen;

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
    /// モジュールをロードします。
    /// </summary>
    /// <param name="path">モジュールのパス（相対パスまたは絶対パス）</param>
    /// <param name="currentDir">現在のディレクトリ</param>
    /// <returns>エクスポートテーブル（名前 → 値）</returns>
    public Dictionary<string, object?> LoadModule(string path, string currentDir)
    {
        // パスを解決
        var fullPath = ResolvePath(path, currentDir);

        // キャッシュをチェック
        if (_cache.ContainsKey(fullPath))
        {
            return _cache[fullPath];
        }

        // ファイルを読み込む
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Module not found: {fullPath}");
        }

        var source = File.ReadAllText(fullPath);

        // モジュールを実行してエクスポートテーブルを取得
        var exports = ExecuteModule(source, Path.GetDirectoryName(fullPath) ?? currentDir);

        // キャッシュに保存
        _cache[fullPath] = exports;

        return exports;
    }

    /// <summary>
    /// パスを解決します（相対パス → 絶対パス）。
    /// </summary>
    /// <param name="path">モジュールのパス</param>
    /// <param name="currentDir">現在のディレクトリ</param>
    /// <returns>絶対パス</returns>
    public string ResolvePath(string path, string currentDir)
    {
        // 絶対パスの場合はそのまま返す
        if (Path.IsPathRooted(path))
        {
            return Path.GetFullPath(path);
        }

        // 相対パスを解決
        var combinedPath = Path.Combine(currentDir, path);
        return Path.GetFullPath(combinedPath);
    }

    /// <summary>
    /// モジュールを実行してエクスポートテーブルを返します。
    /// </summary>
    /// <param name="source">ソースコード</param>
    /// <param name="moduleDir">モジュールのディレクトリ</param>
    /// <returns>エクスポートテーブル</returns>
    private Dictionary<string, object?> ExecuteModule(string source, string moduleDir)
    {
        // Lexer: トークン化
        var lexer = new Lexer.Lexer(source);
        var tokens = lexer.ScanTokens();

        // Parser: 構文解析
        var parser = new Parser.Parser(tokens);
        var ast = parser.Parse();

        // エクスポートを収集
        var exports = new Dictionary<string, object?>();

        // AST をスキャンして export 文を見つける
        foreach (var stmt in ast.Statements)
        {
            if (stmt is ExportStmt exportStmt)
            {
                // エクスポートする宣言の名前を取得
                string name;
                if (exportStmt.Declaration is LetStmt letStmt)
                {
                    name = letStmt.Name;
                }
                else if (exportStmt.Declaration is FunctionDef funcDef)
                {
                    name = funcDef.Name;
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported export declaration: {exportStmt.Declaration.GetType().Name}");
                }

                // 値を評価するために、モジュール全体を実行する必要があります
                // （現時点では簡略化のため、エクスポート名だけを記録します）
                exports[name] = null; // 後で実装
            }
        }

        // TODO: モジュール全体を実行して、実際の値をエクスポートテーブルに格納する
        // 現時点では、エクスポート名だけを返します

        return exports;
    }
}
