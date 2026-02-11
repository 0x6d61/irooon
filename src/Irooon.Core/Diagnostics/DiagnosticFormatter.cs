using System.Text;

namespace Irooon.Core.Diagnostics;

/// <summary>
/// Rust コンパイラ風のエラーフォーマットを生成する。
///
/// 出力例:
/// <code>
/// error[E201]: Cannot assign to 'let' variable 'x'
///  --> script.iro:3:1
///   |
/// 3 | x = 20
///   | ^^^^^^ Cannot assign to 'let' variable
///   |
///   = help: Use 'var' instead of 'let' if you need to reassign
/// </code>
/// </summary>
public static class DiagnosticFormatter
{
    /// <summary>
    /// エラーメッセージを Rust 風にフォーマットする。
    /// </summary>
    public static string FormatError(
        ErrorCode code,
        string message,
        SourceLocation location,
        string? suggestion = null)
    {
        var sb = new StringBuilder();
        var codeNumber = (int)code;
        var filePath = location.FilePath ?? "<repl>";

        // error[ENNN]: message
        sb.AppendLine($"error[E{codeNumber:D3}]: {message}");

        // --> file:line:column
        sb.AppendLine($" --> {filePath}:{location.Line}:{location.Column}");

        // ソースコード行の表示
        if (location.Source != null)
        {
            var sourceLine = GetSourceLine(location.Source, location.Line);
            if (sourceLine != null)
            {
                var lineNumStr = location.Line.ToString();
                var padding = new string(' ', lineNumStr.Length);

                // 空行
                sb.AppendLine($"{padding} |");

                // ソース行
                sb.AppendLine($"{lineNumStr} | {sourceLine}");

                // ポインタ行
                var colOffset = Math.Max(0, location.Column - 1);
                var pointerPadding = new string(' ', colOffset);
                var pointer = new string('^', Math.Max(1, location.Length));
                sb.AppendLine($"{padding} | {pointerPadding}{pointer}");

                // 閉じ行
                sb.Append($"{padding} |");
            }
        }

        // サジェスチョン
        if (suggestion != null)
        {
            sb.AppendLine();
            sb.Append($"  = help: {suggestion}");
        }

        return sb.ToString();
    }

    /// <summary>
    /// ソースコードから指定行を抽出する。
    /// </summary>
    private static string? GetSourceLine(string source, int lineNumber)
    {
        var lines = source.Split('\n');
        if (lineNumber < 1 || lineNumber > lines.Length)
            return null;

        var line = lines[lineNumber - 1];
        // 末尾の \r を除去
        if (line.EndsWith('\r'))
            line = line[..^1];

        return line;
    }
}
