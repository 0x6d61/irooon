using System;
using System.Collections.Generic;
using Irooon.Core;
using Irooon.Core.Runtime;

namespace Irooon.Repl;

/// <summary>
/// REPL（Read-Eval-Print Loop）エンジン。
/// 対話的にirooonスクリプトを評価します。
/// </summary>
public class ReplEngine
{
    private readonly ScriptEngine _engine;
    private ScriptContext _context;

    /// <summary>
    /// 新しいReplEngineインスタンスを作成します。
    /// </summary>
    public ReplEngine()
    {
        _engine = new ScriptEngine();
        _context = new ScriptContext();
    }

    /// <summary>
    /// 入力を評価して結果を返します。
    /// </summary>
    /// <param name="input">評価する入力</param>
    /// <returns>評価結果。エラー時はnull。</returns>
    public object? Evaluate(string input)
    {
        try
        {
            return _engine.Execute(input, _context);
        }
        catch (ScriptException ex)
        {
            Console.Error.WriteLine(ex.DetailedMessage ?? ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// コンテキストをクリアします。
    /// </summary>
    public void Clear()
    {
        // 新しいコンテキストを作成（ビルトイン関数も再登録される）
        _context = new ScriptContext();
    }

    /// <summary>
    /// 現在定義されている変数の一覧を取得します。
    /// </summary>
    /// <returns>変数名と値のディクショナリ</returns>
    public Dictionary<string, object?> GetVariables()
    {
        var result = new Dictionary<string, object?>();
        foreach (var kvp in _context.Globals)
        {
            result[kvp.Key] = kvp.Value;
        }
        return result;
    }
}
