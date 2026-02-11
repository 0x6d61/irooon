namespace Irooon.Core.Diagnostics;

/// <summary>
/// ErrorCode に対応するサジェスチョンメッセージのマッピング。
/// </summary>
public static class Suggestions
{
    private static readonly Dictionary<ErrorCode, string> _suggestions = new()
    {
        // Lexer
        [ErrorCode.E002_UnterminatedString] = "Add a closing quote to the string",
        [ErrorCode.E003_UnterminatedBlockComment] = "Add '*/' to close the block comment",
        [ErrorCode.E004_InvalidEscapeSequence] = "Valid escape sequences: \\n, \\t, \\r, \\\\, \\\", \\'",
        [ErrorCode.E005_InvalidNumberFormat] = "Check the number format (e.g., 123, 3.14, 0xFF)",
        [ErrorCode.E007_UnterminatedBacktickString] = "Add a closing backtick to the string",

        // Parser
        [ErrorCode.E100_ExpectExpression] = "An expression is expected here",
        [ErrorCode.E101_InvalidAssignmentTarget] = "Only variables and member/index expressions can be assigned to",
        [ErrorCode.E102_ExpectClosingParen] = "Add ')' to close the parenthesized expression",
        [ErrorCode.E103_ExpectClosingBrace] = "Add '}' to close the block",
        [ErrorCode.E104_ExpectClosingBracket] = "Add ']' to close the list or index expression",

        // Resolver
        [ErrorCode.E200_VariableAlreadyDeclared] = "Choose a different variable name, or remove the duplicate declaration",
        [ErrorCode.E201_CannotAssignToLet] = "Use 'var' instead of 'let' if you need to reassign",
        [ErrorCode.E202_UndefinedVariable] = "Check the variable name for typos, or declare it with 'let' or 'var'",
        [ErrorCode.E203_CircularInheritance] = "Remove the circular reference in the class hierarchy",
        [ErrorCode.E204_ParentClassNotDefined] = "Define the parent class before using it in 'extends'",
        [ErrorCode.E207_AwaitOutsideAsync] = "Use 'await' only inside an 'async' function",

        // Runtime
        [ErrorCode.E300_DivisionByZero] = "Check the divisor is not zero before dividing",
        [ErrorCode.E301_IndexOutOfRange] = "Check the index is within the list bounds (0 to length-1)",
        [ErrorCode.E302_KeyNotFound] = "Check the key exists using 'has' before accessing it",
        [ErrorCode.E303_CannotInvokeNull] = "The function or method may not be defined",
        [ErrorCode.E304_NotCallable] = "Only functions, methods, and classes can be called",
        [ErrorCode.E307_TypeMismatch] = "Check the argument types match the function signature",
    };

    /// <summary>
    /// ErrorCode に対応するサジェスチョンを取得する。
    /// </summary>
    public static string? Get(ErrorCode code)
    {
        return _suggestions.TryGetValue(code, out var suggestion) ? suggestion : null;
    }
}
