namespace Irooon.Core.Runtime;

/// <summary>
/// プロトタイプメソッドをオブジェクトにバインドするラッパー
/// selfを第一引数として自動的に追加する
/// </summary>
public class BoundMethod : IroCallable
{
    private readonly IroCallable _method;
    private readonly object _target;

    public BoundMethod(IroCallable method, object target)
    {
        _method = method;
        _target = target;
    }

    public object Invoke(ScriptContext ctx, object[] args)
    {
        // targetを第一引数として追加
        var allArgs = new object[args.Length + 1];
        allArgs[0] = _target;
        Array.Copy(args, 0, allArgs, 1, args.Length);
        return _method.Invoke(ctx, allArgs);
    }
}
