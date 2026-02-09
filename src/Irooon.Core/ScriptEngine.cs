using Irooon.Core.Lexer;
using Irooon.Core.Parser;
using Irooon.Core.Resolver;
using Irooon.Core.CodeGen;
using Irooon.Core.Runtime;

namespace Irooon.Core;

/// <summary>
/// irooonスクリプトエンジン。
/// Source → Lexer → Parser → AST → Resolver → CodeGen → Compile → Invoke
/// </summary>
public class ScriptEngine
{
    /// <summary>
    /// スクリプトを実行します。
    /// </summary>
    /// <param name="source">ソースコード</param>
    /// <returns>実行結果</returns>
    public object? Execute(string source)
    {
        var context = new ScriptContext();
        context.InitializeStdlib((code, ctx) => Execute(code, ctx));
        return Execute(source, context);
    }

    /// <summary>
    /// 既存のコンテキストを使用してスクリプトを実行します。
    /// </summary>
    /// <param name="source">ソースコード</param>
    /// <param name="context">スクリプトコンテキスト</param>
    /// <returns>実行結果</returns>
    public object? Execute(string source, ScriptContext context)
    {
        try
        {
            // 1. Lexer: トークン化
            var lexer = new Lexer.Lexer(source);
            var tokens = lexer.ScanTokens();

            // 2. Parser: 構文解析
            var parser = new Parser.Parser(tokens);
            var ast = parser.Parse();

            // 3. Resolver: スコープ解析
            var resolver = new Resolver.Resolver();

            // REPL用: 既存のグローバル変数をResolverに登録
            foreach (var varName in context.Globals.Keys)
            {
                resolver.RegisterVariable(varName, isReadOnly: false);
            }

            resolver.Resolve(ast);

            // エラーがあれば例外を投げる
            var errors = resolver.GetErrors();
            if (errors.Count > 0)
            {
                var errorMessages = string.Join("\n", errors.Select(e => e.Message));
                throw new ScriptException($"Resolve errors:\n{errorMessages}");
            }

            // 4. CodeGen: ExpressionTree生成とコンパイル
            var generator = new CodeGenerator();
            var compiled = generator.Compile(ast);

            // 5. 実行
            return compiled(context);
        }
        catch (ParseException ex)
        {
            throw new ScriptException($"Parse error: {ex.Message}", ex);
        }
        catch (ScriptException)
        {
            throw; // 既にScriptExceptionの場合はそのまま
        }
        catch (Exception ex)
        {
            throw new ScriptException($"Runtime error: {ex.Message}", ex);
        }
    }
}
