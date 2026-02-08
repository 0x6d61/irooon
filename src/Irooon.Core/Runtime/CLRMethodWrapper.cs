namespace Irooon.Core.Runtime;

/// <summary>
/// CLRインスタンスメソッドをラップするクラス
/// </summary>
public class CLRMethodWrapper : IroCallable
{
    private readonly object _instance;
    private readonly string _methodName;

    public CLRMethodWrapper(object instance, string methodName)
    {
        _instance = instance ?? throw new ArgumentNullException(nameof(instance));
        _methodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
    }

    public object Invoke(ScriptContext ctx, object[] args)
    {
        return RuntimeHelpers.InvokeCLRInstanceMethod(_instance, _methodName, args);
    }
}
