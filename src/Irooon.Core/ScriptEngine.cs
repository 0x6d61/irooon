using Irooon.Core.Diagnostics;
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
    /// <param name="filePath">ファイルパス（CLI 実行時に指定）</param>
    /// <returns>実行結果</returns>
    public object? Execute(string source, string? filePath = null)
    {
        var context = new ScriptContext();
        context.InitializeStdlib((code, ctx) => Execute(code, ctx));
        context.ModuleLoader = new ModuleLoader((code, ctx) => Execute(code, ctx));
        return Execute(source, context, optimizeTopLevel: true, filePath: filePath);
    }

    /// <summary>
    /// 既存のコンテキストを使用してスクリプトを実行します。
    /// </summary>
    /// <param name="source">ソースコード</param>
    /// <param name="context">スクリプトコンテキスト</param>
    /// <param name="optimizeTopLevel">トップレベル最適化</param>
    /// <param name="filePath">ファイルパス（null の場合は &lt;repl&gt; 表示）</param>
    /// <returns>実行結果</returns>
    public object? Execute(string source, ScriptContext context, bool optimizeTopLevel = false, string? filePath = null)
    {
        try
        {
            // 1. Lexer: トークン化
            var lexer = new Lexer.Lexer(source);
            var tokens = lexer.ScanTokens();

            // Lexer エラーチェック
            var lexErrors = lexer.GetErrors();
            if (lexErrors.Count > 0)
            {
                var firstError = lexErrors[0];
                var errorCode = MapLexError(firstError);
                var rawMessage = firstError.Value?.ToString() ?? "Unknown lex error";
                var location = new SourceLocation(filePath, source, firstError.Line, firstError.Column,
                    Math.Max(1, firstError.Lexeme?.Length ?? 1));
                var detailed = DiagnosticFormatter.FormatError(errorCode, rawMessage, location, Suggestions.Get(errorCode));

                var allMessages = string.Join("\n", lexErrors.Select(e =>
                    $"[Line {e.Line}, Col {e.Column}] Lex error: {e.Value}"));
                throw new ScriptException($"Lex errors:\n{allMessages}") { DetailedMessage = detailed };
            }

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
                var firstError = errors[0];
                var errorCode = MapResolveError(firstError);
                var location = new SourceLocation(filePath, source, firstError.Line, firstError.Column, 1);
                var detailed = DiagnosticFormatter.FormatError(errorCode, firstError.RawMessage, location, Suggestions.Get(errorCode));

                var errorMessages = string.Join("\n", errors.Select(e => e.Message));
                throw new ScriptException($"Resolve errors:\n{errorMessages}") { DetailedMessage = detailed };
            }

            // 4. CodeGen: ExpressionTree生成とコンパイル
            var generator = new CodeGenerator();
            var compiled = generator.Compile(ast, optimizeTopLevel);

            // 5. 実行
            return compiled(context);
        }
        catch (ParseException ex)
        {
            var errorCode = MapParseError(ex);
            var location = new SourceLocation(filePath, source, ex.Token.Line, ex.Token.Column,
                Math.Max(1, ex.Token.Lexeme?.Length ?? 1));
            var detailed = DiagnosticFormatter.FormatError(errorCode, ex.RawMessage, location, Suggestions.Get(errorCode));

            throw new ScriptException($"Parse error: {ex.Message}", ex) { DetailedMessage = detailed };
        }
        catch (ScriptException)
        {
            throw; // 既にScriptExceptionの場合はそのまま
        }
        catch (RuntimeException ex)
        {
            var errorCode = MapRuntimeError(ex);
            var location = new SourceLocation(filePath, source, ex.Line, ex.Column, 1);
            var detailed = DiagnosticFormatter.FormatError(errorCode, ex.Message, location, Suggestions.Get(errorCode));
            if (!string.IsNullOrEmpty(ex.StackTraceString))
            {
                detailed += $"\n\n  stack trace:\n{ex.StackTraceString}";
            }
            ex.DetailedMessage = detailed;

            throw new ScriptException($"Runtime error: {ex.Message}", ex) { DetailedMessage = detailed };
        }
        catch (Exception ex)
        {
            var errorCode = MapGenericError(ex);
            var location = new SourceLocation(filePath, source, 0, 0, 1);
            var detailed = DiagnosticFormatter.FormatError(errorCode, ex.Message, location, Suggestions.Get(errorCode));
            throw new ScriptException($"Runtime error: {ex.Message}", ex) { DetailedMessage = detailed };
        }
    }

    #region エラーコードマッピング

    private static ErrorCode MapLexError(Token errorToken)
    {
        var msg = errorToken.Value?.ToString() ?? "";
        if (msg.Contains("Unexpected character")) return ErrorCode.E001_UnexpectedCharacter;
        if (msg.Contains("Unterminated string")) return ErrorCode.E002_UnterminatedString;
        if (msg.Contains("Unterminated block comment")) return ErrorCode.E003_UnterminatedBlockComment;
        if (msg.Contains("Invalid escape")) return ErrorCode.E004_InvalidEscapeSequence;
        if (msg.Contains("Invalid number")) return ErrorCode.E005_InvalidNumberFormat;
        if (msg.Contains("Invalid hex")) return ErrorCode.E006_InvalidHexNumber;
        if (msg.Contains("Unterminated backtick")) return ErrorCode.E007_UnterminatedBacktickString;
        if (msg.Contains("assembly reference")) return ErrorCode.E008_InvalidDirective;
        return ErrorCode.E001_UnexpectedCharacter;
    }

    private static ErrorCode MapParseError(ParseException ex)
    {
        var msg = ex.RawMessage;
        if (msg.Contains("Invalid assignment target")) return ErrorCode.E101_InvalidAssignmentTarget;
        if (msg.Contains("Expect ')'") || msg.Contains("after arguments") || msg.Contains("after parameters"))
            return ErrorCode.E102_ExpectClosingParen;
        if (msg.Contains("Expect '}'") || msg.Contains("after block"))
            return ErrorCode.E103_ExpectClosingBrace;
        if (msg.Contains("Expect ']'"))
            return ErrorCode.E104_ExpectClosingBracket;
        if (msg.Contains("function name")) return ErrorCode.E106_ExpectFunctionName;
        if (msg.Contains("class name")) return ErrorCode.E107_ExpectClassName;
        if (msg.Contains("variable name")) return ErrorCode.E109_ExpectVariableName;
        if (msg.Contains("after 'export'")) return ErrorCode.E110_ExpectAfterExport;
        return ErrorCode.E113_UnexpectedToken;
    }

    private static ErrorCode MapResolveError(ResolveException ex)
    {
        var msg = ex.RawMessage;
        if (msg.Contains("already declared")) return ErrorCode.E200_VariableAlreadyDeclared;
        if (msg.Contains("Cannot assign to 'let'")) return ErrorCode.E201_CannotAssignToLet;
        if (msg.Contains("Undefined variable")) return ErrorCode.E202_UndefinedVariable;
        if (msg.Contains("Circular inheritance")) return ErrorCode.E203_CircularInheritance;
        if (msg.Contains("not defined")) return ErrorCode.E204_ParentClassNotDefined;
        if (msg.Contains("super") && msg.Contains("outside")) return ErrorCode.E205_CannotUseSuperOutsideClass;
        if (msg.Contains("no parent")) return ErrorCode.E206_ClassHasNoParent;
        if (msg.Contains("await") && msg.Contains("async")) return ErrorCode.E207_AwaitOutsideAsync;
        return ErrorCode.E202_UndefinedVariable;
    }

    private static ErrorCode MapRuntimeError(RuntimeException ex)
    {
        var msg = ex.Message;
        if (msg.Contains("divide by zero") || msg.Contains("Division by zero"))
            return ErrorCode.E300_DivisionByZero;
        if (msg.Contains("index out of range") || msg.Contains("Index out of range"))
            return ErrorCode.E301_IndexOutOfRange;
        if (msg.Contains("key not found") || msg.Contains("Key not found"))
            return ErrorCode.E302_KeyNotFound;
        if (msg.Contains("Cannot invoke null")) return ErrorCode.E303_CannotInvokeNull;
        if (msg.Contains("not callable")) return ErrorCode.E304_NotCallable;
        if (msg.Contains("does not have member") || msg.Contains("not found"))
            return ErrorCode.E305_MemberNotFound;
        if (msg.Contains("Type error") || msg.Contains("type mismatch"))
            return ErrorCode.E307_TypeMismatch;
        if (msg.Contains("Cannot compare")) return ErrorCode.E308_CannotCompare;
        return ErrorCode.E309_InvalidOperation;
    }

    private static ErrorCode MapGenericError(Exception ex)
    {
        var msg = ex.Message;
        if (ex is DivideByZeroException || msg.Contains("divide by zero"))
            return ErrorCode.E300_DivisionByZero;
        if (msg.Contains("index") && msg.Contains("range"))
            return ErrorCode.E301_IndexOutOfRange;
        if (msg.Contains("Cannot invoke null")) return ErrorCode.E303_CannotInvokeNull;
        if (msg.Contains("not callable")) return ErrorCode.E304_NotCallable;
        if (msg.Contains("Type error")) return ErrorCode.E307_TypeMismatch;
        if (msg.Contains("Cannot compare")) return ErrorCode.E308_CannotCompare;
        return ErrorCode.E309_InvalidOperation;
    }

    #endregion
}
