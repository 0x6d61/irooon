namespace Irooon.Core.Runtime;

/// <summary>
/// 呼び出し可能なオブジェクトを表すインターフェース
/// 関数、メソッド、ラムダなどが実装する
/// </summary>
public interface IroCallable
{
    /// <summary>
    /// 関数を呼び出す
    /// </summary>
    /// <param name="ctx">スクリプトコンテキスト</param>
    /// <param name="args">引数の配列</param>
    /// <returns>実行結果</returns>
    object Invoke(ScriptContext ctx, object[] args);
}
