namespace Irooon.Core.Runtime;

/// <summary>
/// Runtime実行時の例外
/// </summary>
public class RuntimeException : Exception
{
    public RuntimeException(string message) : base(message) { }
    public RuntimeException(string message, Exception inner) : base(message, inner) { }
}
