namespace Irooon.Core.Diagnostics;

/// <summary>
/// irooon のエラーコード。
/// Rust コンパイラ風の error[ENNN] フォーマットで使用。
/// </summary>
public enum ErrorCode
{
    // ── Lexer (E001-E099) ──
    E001_UnexpectedCharacter = 1,
    E002_UnterminatedString = 2,
    E003_UnterminatedBlockComment = 3,
    E004_InvalidEscapeSequence = 4,
    E005_InvalidNumberFormat = 5,
    E006_InvalidHexNumber = 6,
    E007_UnterminatedBacktickString = 7,
    E008_InvalidDirective = 8,

    // ── Parser (E100-E199) ──
    E100_ExpectExpression = 100,
    E101_InvalidAssignmentTarget = 101,
    E102_ExpectClosingParen = 102,
    E103_ExpectClosingBrace = 103,
    E104_ExpectClosingBracket = 104,
    E105_ExpectPropertyName = 105,
    E106_ExpectFunctionName = 106,
    E107_ExpectClassName = 107,
    E108_ExpectParameterName = 108,
    E109_ExpectVariableName = 109,
    E110_ExpectAfterExport = 110,
    E111_ExpectColon = 111,
    E112_ExpectArrow = 112,
    E113_UnexpectedToken = 113,

    // ── Resolver (E200-E299) ──
    E200_VariableAlreadyDeclared = 200,
    E201_CannotAssignToLet = 201,
    E202_UndefinedVariable = 202,
    E203_CircularInheritance = 203,
    E204_ParentClassNotDefined = 204,
    E205_CannotUseSuperOutsideClass = 205,
    E206_ClassHasNoParent = 206,
    E207_AwaitOutsideAsync = 207,

    // ── Runtime (E300-E399) ──
    E300_DivisionByZero = 300,
    E301_IndexOutOfRange = 301,
    E302_KeyNotFound = 302,
    E303_CannotInvokeNull = 303,
    E304_NotCallable = 304,
    E305_MemberNotFound = 305,
    E306_NullReference = 306,
    E307_TypeMismatch = 307,
    E308_CannotCompare = 308,
    E309_InvalidOperation = 309,
    E310_FileNotFound = 310,
    E311_ModuleError = 311,
}
