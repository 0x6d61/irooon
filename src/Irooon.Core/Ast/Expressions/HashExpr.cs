namespace Irooon.Core.Ast.Expressions;

/// <summary>
/// ハッシュリテラル式を表すASTノード。
/// 例: {name: "Alice", age: 30}
/// </summary>
public class HashExpr : Expression
{
    /// <summary>
    /// ハッシュのキー・値ペアを表します。
    /// </summary>
    public class KeyValuePair
    {
        /// <summary>
        /// キー（識別子の文字列）
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// 値（式）
        /// </summary>
        public Expression Value { get; }

        /// <summary>
        /// KeyValuePairの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="value">値</param>
        public KeyValuePair(string key, Expression value)
        {
            Key = key;
            Value = value;
        }
    }

    /// <summary>
    /// ハッシュのキー・値ペアのリスト。
    /// </summary>
    public List<KeyValuePair> Pairs { get; }

    /// <summary>
    /// HashExprの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="pairs">キー・値ペアのリスト</param>
    /// <param name="line">行番号</param>
    /// <param name="column">列番号</param>
    public HashExpr(List<KeyValuePair> pairs, int line, int column)
        : base(line, column)
    {
        Pairs = pairs;
    }
}
