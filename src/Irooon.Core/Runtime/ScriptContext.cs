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

    /// <summary>
    /// プロトタイプメソッドの辞書（型名 → メソッド名 → メソッド）
    /// stdlib.iro で登録されるメソッドを保持する
    /// </summary>
    public Dictionary<string, Dictionary<string, object>> Prototypes { get; }

    private bool _stdlibInitialized;

    public ScriptContext()
    {
        Globals = new Dictionary<string, object>();
        Classes = new Dictionary<string, IroClass>();
        Exports = new Dictionary<string, object?>();
        Prototypes = new Dictionary<string, Dictionary<string, object>>();

        // ビルトイン関数を登録
        RegisterBuiltins();
    }

    /// <summary>
    /// 標準ライブラリを初期化する（ScriptEngine経由で一度だけ呼ばれる）
    /// </summary>
    public void InitializeStdlib(Action<string, ScriptContext> executeFunc)
    {
        if (_stdlibInitialized) return;
        _stdlibInitialized = true;

        var assembly = typeof(ScriptContext).Assembly;
        using var stream = assembly.GetManifestResourceStream("Irooon.Core.stdlib.iro");
        if (stream == null) return; // stdlibが埋め込まれていない場合はスキップ

        using var reader = new System.IO.StreamReader(stream);
        var code = reader.ReadToEnd();
        if (!string.IsNullOrWhiteSpace(code))
        {
            executeFunc(code, this);
        }
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

        // 日時
        Globals["now"] = new BuiltinFunction("now", RuntimeHelpers.Now);

        // 低レベルプリミティブ（stdlib.iro の基盤）
        Globals["__stringLength"] = new BuiltinFunction("__stringLength", RuntimeHelpers.__stringLength);
        Globals["__charAt"] = new BuiltinFunction("__charAt", RuntimeHelpers.__charAt);
        Globals["__charCodeAt"] = new BuiltinFunction("__charCodeAt", RuntimeHelpers.__charCodeAt);
        Globals["__fromCharCode"] = new BuiltinFunction("__fromCharCode", RuntimeHelpers.__fromCharCode);
        Globals["__substring"] = new BuiltinFunction("__substring", RuntimeHelpers.__substring);
        Globals["__listLength"] = new BuiltinFunction("__listLength", RuntimeHelpers.__listLength);
        Globals["__listPush"] = new BuiltinFunction("__listPush", RuntimeHelpers.__listPush);
        Globals["__stringBuilder"] = new BuiltinFunction("__stringBuilder", RuntimeHelpers.__stringBuilder);
        Globals["__sbAppend"] = new BuiltinFunction("__sbAppend", RuntimeHelpers.__sbAppend);
        Globals["__sbToString"] = new BuiltinFunction("__sbToString", RuntimeHelpers.__sbToString);
        Globals["__indexOf"] = new BuiltinFunction("__indexOf", RuntimeHelpers.__indexOf);
        Globals["__toNumber"] = new BuiltinFunction("__toNumber", RuntimeHelpers.__toNumber);
        Globals["__toString"] = new BuiltinFunction("__toString", RuntimeHelpers.__toString);
        Globals["__typeOf"] = new BuiltinFunction("__typeOf", RuntimeHelpers.__typeOf);
        Globals["__hashNew"] = new BuiltinFunction("__hashNew", RuntimeHelpers.__hashNew);
        Globals["__hashKeys"] = new BuiltinFunction("__hashKeys", RuntimeHelpers.__hashKeys);
        Globals["__registerPrototype"] = new BuiltinFunction("__registerPrototype", RuntimeHelpers.__registerPrototype);
    }
}
