namespace Irooon.Core.Runtime;

/// <summary>
/// スクリプト実行時のコンテキスト
/// グローバル変数とクラス定義を保持する
/// </summary>
public class ScriptContext
{
    /// <summary>
    /// グローバル変数の辞書
    /// </summary>
    public Dictionary<string, object> Globals { get; }

    /// <summary>
    /// クラス定義の辞書
    /// </summary>
    public Dictionary<string, IroClass> Classes { get; }

    public ScriptContext()
    {
        Globals = new Dictionary<string, object>();
        Classes = new Dictionary<string, IroClass>();

        // ビルトイン関数を登録
        RegisterBuiltins();
    }

    /// <summary>
    /// ビルトイン関数をグローバルスコープに登録する
    /// </summary>
    private void RegisterBuiltins()
    {
        // print関数
        Globals["print"] = new BuiltinFunction("print", RuntimeHelpers.Print);

        // println関数
        Globals["println"] = new BuiltinFunction("println", RuntimeHelpers.Println);
    }
}
