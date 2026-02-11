namespace Irooon.Core.Diagnostics;

/// <summary>
/// ソースコードの位置情報を表す。
/// エラーメッセージにソースコード表示とポインタを生成するために使用。
/// </summary>
public record struct SourceLocation(
    string? FilePath,
    string? Source,
    int Line,
    int Column,
    int Length
);
