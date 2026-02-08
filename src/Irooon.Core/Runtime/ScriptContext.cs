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

    /// <summary>
    /// エクスポートされた値の辞書（モジュールシステム用）
    /// </summary>
    public Dictionary<string, object?> Exports { get; }

    public ScriptContext()
    {
        Globals = new Dictionary<string, object>();
        Classes = new Dictionary<string, IroClass>();
        Exports = new Dictionary<string, object?>();

        // ビルトイン関数を登録
        RegisterBuiltins();
    }

    /// <summary>
    /// ビルトイン関数をグローバルスコープに登録する
    /// </summary>
    private void RegisterBuiltins()
    {
        // 標準出力
        Globals["print"] = new BuiltinFunction("print", RuntimeHelpers.Print);
        Globals["println"] = new BuiltinFunction("println", RuntimeHelpers.Println);

        // ファイルI/O
        Globals["readFile"] = new BuiltinFunction("readFile", RuntimeHelpers.ReadFile);
        Globals["writeFile"] = new BuiltinFunction("writeFile", RuntimeHelpers.WriteFile);
        Globals["appendFile"] = new BuiltinFunction("appendFile", RuntimeHelpers.AppendFile);
        Globals["exists"] = new BuiltinFunction("exists", RuntimeHelpers.Exists);
        Globals["deleteFile"] = new BuiltinFunction("deleteFile", RuntimeHelpers.DeleteFile);
        Globals["listDir"] = new BuiltinFunction("listDir", RuntimeHelpers.ListDir);

        // JSON操作
        Globals["jsonParse"] = new BuiltinFunction("jsonParse", RuntimeHelpers.JsonParse);
        Globals["jsonStringify"] = new BuiltinFunction("jsonStringify", RuntimeHelpers.JsonStringify);

        // 日時
        Globals["now"] = new BuiltinFunction("now", RuntimeHelpers.Now);
    }
}
