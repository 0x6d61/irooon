using Irooon.Core.CodeGen;
using Irooon.Core.Runtime;

namespace Irooon.Benchmarks.Helpers;

/// <summary>
/// ベンチマーク用共通ユーティリティ。
/// </summary>
public static class BenchmarkHelper
{
    /// <summary>
    /// スクリプトを事前コンパイルし、コンパイル済み関数とコンテキストを返す。
    /// コンパイルコストを除外した純粋なランタイム計測に使用。
    /// </summary>
    public static (Func<ScriptContext, object?> compiled, ScriptContext ctx) PreCompile(string source)
    {
        var tokens = new Irooon.Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Irooon.Core.Parser.Parser(tokens).Parse();
        var resolver = new Irooon.Core.Resolver.Resolver();
        resolver.Resolve(ast);
        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);
        var ctx = new ScriptContext();
        return (compiled, ctx);
    }
}
