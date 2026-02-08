# irooon Language Specification v0.3

## 概要

irooon は .NET 上で動作する動的スクリプト言語である。
DLR (System.Linq.Expressions) を使用して実行する。
Groovy風の簡略構文と式志向設計を採用する。

---

## 実行モデル

* トップレベルはそのまま実行される
* プログラムは Block として評価される
* 動的型言語（すべて object）
* REPLサポート（対話的実行環境）

---

## 基本構文

セミコロンは不要。

### コメント

#### 一行コメント

```
// これはコメントです
let x = 5 // 行末までコメント
```

`//` から行末までがコメントとして扱われる。

#### 複数行コメント

```
/* これは
   複数行の
   コメントです */

let x = /* コメント */ 5
```

`/*` から `*/` までがコメントとして扱われる。
ネストした複数行コメントはサポートしない。

### ブロック

```
{ stmt* expr? }
```

ブロックの最後の式が値になる。
最後が文のみの場合は null を返す。

---

## 変数

```
let x = expr
var y = expr
```

* `let` は再代入不可
* `var` は再代入可能

---

## リテラル

### 数値リテラル

```
123
3.14
```

### 文字列リテラル

```
"Hello"
```

#### 文字列補間

文字列内に式を埋め込むことができる。

```
"Hello, ${name}!"
"Result: ${x + y}"
```

* `${expression}` で式を評価し、文字列に変換する
* ネストした補間もサポート

### 真偽値リテラル

```
true
false
```

### null リテラル

```
null
```

### リストリテラル

```
[1, 2, 3]
["a", "b", "c"]
[expr1, expr2, expr3]
```

リストは動的配列として実装される。
要素の型は制限されない（異種混在可能）。

### ハッシュリテラル

```
{key1: value1, key2: value2}
{name: "Alice", age: 30}
```

ハッシュはキー・バリューの辞書として実装される。
キーは文字列として扱われる。
値の型は制限されない。

### 範囲リテラル

```
start..end      // 排他的範囲（endを含まない）
start...end     // 包括的範囲（endを含む）
```

**例:**
```
1..10     // [1, 2, 3, 4, 5, 6, 7, 8, 9]
1...10    // [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
0..5      // [0, 1, 2, 3, 4]
```

範囲はリストとして実装され、forループでの反復に使用できます。

---

## 式

### 算術

* `+` `-` `*` `/` `%`

### 比較

* `==` `!=` `<` `>` `<=` `>=`

### 論理

* `and` `or` `not`

### 単項

* `+` `-` `not`

### 呼び出し

```
expr(args)
```

### メンバアクセス

```
expr.name
```

### インデックスアクセス

```
expr[index]
```

リストまたはハッシュの要素にアクセスする。
インデックスは整数（リスト）または文字列（ハッシュ）を指定する。

例:
```
let numbers = [1, 2, 3]
numbers[0]  // 1

let person = {name: "Alice"}
person["name"]  // "Alice"
```

### インデックス代入

```
expr[index] = value
```

リストまたはハッシュの要素に値を代入する。

例:
```
let numbers = [1, 2, 3]
numbers[0] = 99  // [99, 2, 3]

let person = {name: "Alice"}
person["age"] = 30  // {name: "Alice", age: 30}
```

---

## if式

```
if (cond) { expr } else { expr }
```

* `else` は必須。

---

## ループ

### for ループ

irooonでは、すべてのループを`for`構文で統一しています。

#### 条件ループ

```
for (cond) {
  stmt*
}
```

条件が真の間、ループを実行します。

**例:**
```
var i = 0
for (i < 10) {
    i = i + 1
}
```

#### コレクション反復

```
for (item in collection) {
  stmt*
}
```

* コレクションはリスト、ハッシュ、または範囲（Range）
* リストの場合、`item` は各要素
* ハッシュの場合、`item` は `{key: k, value: v}` のオブジェクト
* 範囲の場合、`item` は各数値

**例:**
```
// リスト
for (n in [1, 2, 3, 4, 5]) {
    println(n)
}

// 範囲（排他的）
for (i in 1..10) {
    println(i)  // 1から9まで
}

// 範囲（包括的）
for (i in 1...10) {
    println(i)  // 1から10まで
}
```

### break / continue

```
break      // ループを中断
continue   // 次の反復へスキップ
```

* `break` と `continue` はループ内でのみ使用可能
* ネストしたループもサポート

---

## 例外処理

### try / catch / finally

```
try {
  stmt*
} catch (e) {
  stmt*
} finally {
  stmt*
}
```

* `catch` ブロックで例外オブジェクトを受け取る
* `finally` ブロックは必ず実行される
* `catch` と `finally` は両方省略できない

### throw

```
throw expression
```

* 任意の値を例外として投げる
* 文字列、オブジェクト、その他任意の型

---

## モジュール

### export

```
export fn name(args) { ... }
export class Name { ... }
export let/var name = expr
```

* 関数、クラス、変数をエクスポートする
* エクスポートされた要素は他のモジュールから参照可能

### import

```
import "path/to/module.iro"
```

* 他のモジュールをインポートする
* パスは相対パスまたは絶対パス
* エクスポートされた要素がスコープに追加される

---

## ビルトイン関数

### print / println

```
print(arg1, arg2, ...)    // 改行なし
println(arg1, arg2, ...)  // 改行あり
```

* 標準出力に値を出力
* 複数引数をスペース区切りで出力
* nullは"null"として表示

---

## 文字列メソッド

文字列はCLR相互運用により、以下のメソッドを持つ:

* `length()` - 文字列の長さ
* `toUpper()` - 大文字に変換
* `toLower()` - 小文字に変換
* `trim()` - 前後の空白を削除
* `substring(start, length)` - 部分文字列を取得
* `split(separator)` - 文字列を分割
* `contains(value)` - 文字列が含まれるか
* `startsWith(value)` - 文字列で始まるか
* `endsWith(value)` - 文字列で終わるか
* `replace(oldValue, newValue)` - 文字列を置換

---

## 関数

```
fn name(a, b) {
  expr
}
```

* 関数はファーストクラス。
* クロージャをサポートする。

### ラムダ

```
fn (a, b) { expr }
```

---

## クラス

```
class Name {
}
```

### 修飾子

* `public`
* `private`
* `static`

---

## フィールド

```
public var name = expr
private var count = 0
```

### ルール

* フィールド宣言は必須
* 宣言されたフィールドは自動プロパティ
* `obj.x` は常にプロパティアクセス
* フィールド直アクセスは存在しない
* 宣言時初期化を行う

### 内部モデル

Instance:
```
Dictionary<string, object> fields
```

---

## メソッド

```
public fn hello() {
  expr
}
```

### staticメソッド

```
public static fn create() {
  expr
}
```

---

## コンストラクタ

```
init(args) {
}
```

### インスタンス生成

```
Name(args)
```

生成時に:

1. フィールド初期化
2. init呼び出し

を行う。

---

## プロパティモデル

`obj.x` は常にプロパティアクセス。

### 解決順序

1. 言語内プロパティ
2. CLRプロパティ
3. エラー

---

## CLR相互運用

CLR型は名前解決で参照可能。

例:
```
System.IO.File.ReadAllText(path)
```

リフレクションで呼び出す。

---

## スコープ

* グローバルスコープ
* 関数スコープ
* ブロックスコープ

クロージャは外側スコープをキャプチャする。

---

## AST（最小）

### Expression

* `LiteralExpr`
* `BinaryExpr`
* `UnaryExpr`
* `CallExpr`
* `MemberExpr`
* `IndexExpr`
* `AssignExpr`
* `IfExpr`
* `BlockExpr`
* `LambdaExpr`
* `NewExpr`
* `ListExpr`
* `HashExpr`
* `StringInterpolationExpr`

### Statement

* `LetStmt`
* `VarStmt`
* `ClassStmt`
* `ExprStmt`
* `ReturnStmt`
* `WhileStmt`
* `ForeachStmt`
* `BreakStmt`
* `ContinueStmt`
* `TryStmt`
* `ThrowStmt`
* `ExportStmt`
* `ImportStmt`

### Class

* `ClassDef`
* `FieldDef`
* `MethodDef`

---

## 実行パイプライン

```
Source
→ Lexer
→ Parser
→ AST
→ Resolver
→ ExpressionTree Generator
→ Compile
→ Invoke
```

---

## エラー

すべてのASTノードは以下を保持する:

* `line`
* `column`

---

## CLI

```
irooon script.iro
```

---

## v0.3での実装状況

### 実装済み機能
* ✅ ビルトイン関数（print/println）
* ✅ 文字列補間
* ✅ 文字列メソッド
* ✅ ループ（for/break/continue）- v0.3で追加、v0.4で統一
* ✅ 範囲リテラル（.. / ...）- v0.4で追加
* ✅ 例外処理（try/catch/finally/throw）
* ✅ モジュールシステム（export/import）
* ✅ REPL

### 将来の拡張（v0.4以降）
* 型推論・型注釈
* ✅ CLR相互運用（.NET標準ライブラリの呼び出し）- v0.4で追加
* 継承
* 演算子オーバーロード
* 括弧省略
* パッケージ管理

---

## CLR相互運用

irooonはCLR（Common Language Runtime）との相互運用をサポートしています。
.NET標準ライブラリの型とメソッドを直接呼び出すことができます。

### 基本構文

```iro
// 静的メソッドの呼び出し
let result = System.Math.Abs(-42)

// 静的プロパティのアクセス
let now = System.DateTime.Now

// メソッドチェーン
let sqrt = System.Math.Sqrt(System.Math.Pow(2, 2))
```

### サポートされている機能

- **静的メソッド呼び出し**: `System.Math.Abs(-42)`
- **静的プロパティアクセス**: `System.DateTime.Now`
- **メソッドチェーン**: `System.Math.Sqrt(System.Math.Max(10, 20))`

### 利用可能な主なCLRクラス

#### System.Math
```iro
let abs = System.Math.Abs(-42)        // 絶対値
let max = System.Math.Max(10, 20)     // 最大値
let min = System.Math.Min(10, 20)     // 最小値
let sqrt = System.Math.Sqrt(16)       // 平方根
let pow = System.Math.Pow(2, 3)       // べき乗
```

#### System.DateTime
```iro
let now = System.DateTime.Now         // 現在日時
```

### 制限事項

- 現在は静的メソッドと静的プロパティのみサポート
- インスタンスメソッドは将来のバージョンでサポート予定
- `System`で始まる型名のみサポート（例: `System.Math`, `System.DateTime`）
- 引数の型は自動的に推論されます（オーバーロードがある場合は最初にマッチしたメソッドが使用されます）
