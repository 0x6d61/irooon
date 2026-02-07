using System.Text;

namespace Irooon.Core.Runtime;

/// <summary>
/// 関数呼び出しスタックを管理します。
/// スタックトレース生成のために使用されます。
/// </summary>
public static class CallStack
{
    [ThreadStatic]
    private static Stack<CallFrame>? _frames;

    private static Stack<CallFrame> Frames => _frames ??= new Stack<CallFrame>();

    /// <summary>
    /// 関数呼び出しをスタックに追加します。
    /// </summary>
    /// <param name="functionName">関数名</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public static void Push(string functionName, int line, int column)
    {
        Frames.Push(new CallFrame(functionName, line, column));
    }

    /// <summary>
    /// 関数呼び出しをスタックから削除します。
    /// </summary>
    public static void Pop()
    {
        if (Frames.Count > 0)
        {
            Frames.Pop();
        }
    }

    /// <summary>
    /// 現在のスタックトレースを文字列で取得します。
    /// </summary>
    /// <returns>スタックトレース文字列</returns>
    public static string GetStackTrace()
    {
        if (Frames.Count == 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        foreach (var frame in Frames.Reverse())
        {
            sb.AppendLine($"  at {frame.FunctionName} (line {frame.Line}, column {frame.Column})");
        }
        return sb.ToString();
    }

    /// <summary>
    /// スタックをクリアします。
    /// </summary>
    public static void Clear()
    {
        Frames.Clear();
    }
}

/// <summary>
/// 関数呼び出しの情報を保持します。
/// </summary>
public class CallFrame
{
    /// <summary>
    /// 関数名
    /// </summary>
    public string FunctionName { get; }

    /// <summary>
    /// 行番号
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// 列番号
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// CallFrameの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="functionName">関数名</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public CallFrame(string functionName, int line, int column)
    {
        FunctionName = functionName;
        Line = line;
        Column = column;
    }
}
