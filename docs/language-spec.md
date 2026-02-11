# irooon Language Specification

## 1. 概要

irooon は .NET 上で動作する動的スクリプト言語である。
DLR (System.Linq.Expressions) を使用してコンパイル・実行する。
Groovy風の簡略構文と式志向設計を採用する。

---

## 2. 実行モデル

- トップレベルコードはそのまま実行される
- プログラムは Block として評価される
- 動的型言語（すべて `object`）
- REPLサポート（対話的実行環境）

### 実行パイプライン

```
Source → Lexer → Parser → AST → Resolver → ExpressionTree Generator → Compile → Invoke
```

---

## 3. 基本構文

セミコロンは不要。改行で文を区切る。

### コメント

```iro
// 一行コメント
let x = 5 // 行末までコメント

/* 複数行
   コメント */
let y = /* インライン */ 10
```

### ブロック

```iro
{
    stmt*
    expr?
}
```

ブロックの最後の式が値になる。最後が文のみの場合は `null` を返す。

---

## 4. 変数

### let（再代入不可）

```iro
let x = 42
let name = "Alice"
```

### var（再代入可能）

```iro
var count = 0
count = count + 1
```

### 分割代入（Destructuring）

リストまたはハッシュの要素を一度に複数の変数に代入する。

```iro
// リスト分割代入
let [a, b, c] = [1, 2, 3]
// a = 1, b = 2, c = 3

// ハッシュ分割代入
let {x, y} = {x: 10, y: 20}
// x = 10, y = 20

// var でも使用可能
var [first, second] = [100, 200]
```

---

## 5. リテラル

### 数値

```iro
42
3.14
0
-7
```

すべての数値は内部的に `double` として扱われる。

### 文字列

```iro
"Hello, World!"
"Line 1\nLine 2"
```

### 真偽値

```iro
true
false
```

### null

```iro
null
```

### リスト

```iro
[1, 2, 3]
["a", "b", "c"]
[expr1, expr2, expr3]
[]  // 空リスト
```

要素の型は制限されない（異種混在可能）。

#### スプレッド演算子

```iro
let a = [1, 2]
let b = [0, ...a, 3]  // [0, 1, 2, 3]
```

### ハッシュ

```iro
{key1: value1, key2: value2}
{name: "Alice", age: 30}
{"a-b": 1, "content-type": "text/html"}  // 文字列キー
{name: "Alice", "a-b": 1}                // 識別子と文字列キーの混在
{}  // 空ハッシュ
```

キーには識別子または文字列リテラルを使用できる。キーは文字列として扱われる。値の型は制限されない。

### 範囲（Range）

```iro
1..10     // 排他的範囲: [1, 2, 3, 4, 5, 6, 7, 8, 9]
1...10    // 包括的範囲: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
```

範囲はリストとして展開され、for ループでの反復に使用できる。

---

## 6. 文字列

### 文字列補間

```iro
let name = "World"
"Hello, ${name}!"           // "Hello, World!"
"Result: ${1 + 2}"          // "Result: 3"
"${a} and ${b}"             // ネストした補間もサポート
```

### エスケープシーケンス

| エスケープ | 結果 | 説明 |
|-----------|------|------|
| `\"` | `"` | ダブルクォート |
| `\\` | `\` | バックスラッシュ |
| `\n` | 改行 | ラインフィード |
| `\t` | タブ | 水平タブ |
| `\r` | CR | キャリッジリターン |
| `\0` | NULL | ヌル文字 |
| `\$` | `$` | ドル記号（補間のエスケープ） |

---

## 7. 演算子

### 算術演算子

| 演算子 | 説明 | 例 |
|--------|------|-----|
| `+` | 加算 / 文字列連結 | `3 + 4` → `7` |
| `-` | 減算 | `10 - 3` → `7` |
| `*` | 乗算 | `3 * 4` → `12` |
| `/` | 除算 | `10 / 3` → `3.333...` |
| `%` | 剰余 | `10 % 3` → `1` |
| `**` | べき乗 | `2 ** 10` → `1024` |

### 比較演算子

| 演算子 | 説明 |
|--------|------|
| `==` | 等価 |
| `!=` | 非等価 |
| `<` | 小なり |
| `<=` | 小なりイコール |
| `>` | 大なり |
| `>=` | 大なりイコール |

### 論理演算子

| 演算子 | 説明 |
|--------|------|
| `and` | 論理AND（短絡評価） |
| `or` | 論理OR（短絡評価） |
| `not` | 論理否定 |

### ビット演算子

| 演算子 | 説明 | 例 |
|--------|------|-----|
| `&` | ビットAND | `5 & 3` → `1` |
| `\|` | ビットOR | `5 \| 3` → `7` |
| `^` | ビットXOR | `5 ^ 3` → `6` |
| `~` | ビットNOT | `~5` → `-6` |
| `<<` | 左シフト | `1 << 3` → `8` |
| `>>` | 右シフト | `8 >> 2` → `2` |

### 複合代入演算子

```iro
var x = 10
x += 5    // x = x + 5
x -= 3    // x = x - 3
x *= 2    // x = x * 2
x /= 4    // x = x / 4
x %= 3    // x = x % 3
```

### インクリメント / デクリメント

```iro
var i = 0
i++    // 後置: 現在の値を返してから加算
++i    // 前置: 加算してから値を返す
i--    // 後置デクリメント
--i    // 前置デクリメント
```

### Null 関連演算子

```iro
// セーフナビゲーション（?.）
let name = user?.name    // user が null なら null を返す

// Null 合体演算子（??）
let value = x ?? "default"    // x が null なら "default"
let a = x ?? y ?? z           // チェーン可能
```

### 三項演算子

```iro
let result = condition ? "yes" : "no"
```

### 演算子の優先度（低→高）

| 優先度 | 演算子 | 結合性 |
|--------|--------|--------|
| 1 | `=` `+=` `-=` `*=` `/=` `%=` | 右 |
| 2 | `? :` | 右 |
| 3 | `or` `??` | 左 |
| 4 | `and` | 左 |
| 5 | `==` `!=` | 左 |
| 6 | `<` `<=` `>` `>=` | 左 |
| 7 | `&` `\|` `^` | 左 |
| 8 | `<<` `>>` | 左 |
| 9 | `..` `...` | 左 |
| 10 | `+` `-` | 左 |
| 11 | `*` `/` `%` | 左 |
| 12 | `**` | 右 |
| 13 | `-`(単項) `not` `~` `++` `--`(前置) `await` | 右 |
| 14 | `()` `.` `?.` `[]` `++` `--`(後置) | 左 |

### 型チェック演算子

| 演算子 | 説明 | 例 |
|--------|------|-----|
| `typeof(expr)` | 値の型名を文字列で返す | `typeof(42)` → `"Number"` |
| `instanceof` | クラスのインスタンスか判定 | `obj instanceof MyClass` → `true` |

```iro
typeof(42)          // "Number"
typeof("hello")     // "String"
typeof(true)        // "Boolean"
typeof(null)        // "Null"
typeof([1,2,3])     // "List"
typeof({a: 1})      // "Hash"

let d = Dog()
d instanceof Dog     // true
d instanceof Animal  // true（親クラスも判定可能）
```

---

## 8. 制御構造

### if / else if / else

```iro
if (condition) {
    expr
} else if (condition2) {
    expr
} else {
    expr
}
```

- `else` は省略可能（省略時は `null` を返す）
- `else if` で複数条件を連鎖可能

### for ループ

すべてのループは `for` 構文で統一。

#### 条件ループ

```iro
var i = 0
for (i < 10) {
    i = i + 1
}
```

#### コレクション反復（for-in）

```iro
for (item in [1, 2, 3]) {
    println(item)
}

for (i in 0..10) {
    println(i)    // 0 から 9
}

for (i in 0...10) {
    println(i)    // 0 から 10
}
```

### break / continue

```iro
for (i in 0..100) {
    if (i == 5) { continue }
    if (i == 10) { break }
    println(i)
}
```

---

## 9. 関数

### 関数定義

```iro
fn add(a, b) {
    a + b
}
```

- 関数はファーストクラスオブジェクト
- 最後の式が戻り値
- `return` で早期リターン可能

### ラムダ（無名関数）

```iro
let double = fn (x) { x * 2 }
```

### アロー関数

```iro
// 単一パラメータ（括弧省略可能）
let double = x => x * 2

// 複数パラメータ
let add = (a, b) => a + b

// ブロック本体
let greet = name => {
    let msg = "Hello, " + name
    msg
}
```

### デフォルトパラメータ

```iro
fn greet(name = "World") {
    "Hello, " + name + "!"
}

greet()         // "Hello, World!"
greet("Alice")  // "Hello, Alice!"
```

### レストパラメータ

```iro
fn sum(first, ...rest) {
    var total = first
    for (n in rest) {
        total = total + n
    }
    total
}

sum(1, 2, 3, 4)  // 10
```

### スプレッド演算子（関数呼び出し）

```iro
let args = [1, 2, 3]
fn add(a, b, c) { a + b + c }
add(...args)  // 6
```

### 型アノテーション

パラメータと戻り値に型注釈を付けることができます。型が一致しない場合、実行時エラーになります。

```iro
// パラメータ型 + 戻り値型
fn add(a: Number, b: Number): Number {
    a + b
}

// 部分的なアノテーション（一部だけ型指定も可）
fn process(data, limit: Number) {
    data
}

// デフォルト値との組み合わせ
fn greet(name: String = "World"): String {
    "Hello, " + name
}

// fn ラムダ式
let f = fn (x: Number): String { "${x}" }

// アロー関数（パラメータ型のみ、戻り値型は不可）
let g = (x: Number) => x * 2

// クラスメソッド
class Calculator {
    public fn add(a: Number, b: Number): Number { a + b }
}

// ユーザー定義クラスを型として使用
fn getAge(person: Person): Number { person.age }
```

**サポートされる型名:**
- `Number` — 数値（double）
- `String` — 文字列
- `Boolean` — 真偽値
- `Null` — null
- `List` — リスト
- `Hash` — ハッシュ
- `Function` — 関数/クロージャ
- ユーザー定義クラス名（例: `Person`, `Animal`）

**制限事項:**
- ジェネリクスは非サポート（`List<Number>` 等は不可）
- アロー関数に戻り値型アノテーションは不可
- 括弧なし単一パラメータ `x => expr` には型アノテーション不可

### クロージャ

```iro
fn counter() {
    var count = 0
    fn () {
        count = count + 1
        count
    }
}

let c = counter()
c()  // 1
c()  // 2
```

---

## 10. クラス

### 基本構文

```iro
class Animal {
    public var name = ""
    public var sound = ""

    init(name, sound) {
        this.name = name
        this.sound = sound
    }

    public fn speak() {
        this.name + " says " + this.sound
    }
}

let dog = Animal("Rex", "Woof")
dog.speak()  // "Rex says Woof"
```

### フィールド

```iro
class Point {
    public var x = 0      // public: 外部からアクセス可能
    private var _id = 0   // private: クラス内部のみ
}
```

- フィールド宣言は必須
- 宣言時に初期化する

### メソッド

```iro
class Calc {
    public var value = 0

    public fn add(n) {
        this.value = this.value + n
        this
    }

    public static fn create() {
        Calc()
    }
}
```

### 静的メソッド

`static` キーワードでクラスメソッドを定義できる。インスタンスなしでクラス名から直接呼び出し可能。

```iro
class Calculator {
    static fn add(a, b) {
        a + b
    }

    static fn multiply(a, b) {
        a * b
    }
}

Calculator.add(3, 4)       // 7
Calculator.multiply(5, 6)  // 30
```

### コンストラクタ（init）

```iro
class Rect {
    public var w = 0
    public var h = 0

    init(w, h) {
        this.w = w
        this.h = h
    }
}

let r = Rect(10, 20)
```

- `init` はインスタンス生成時に自動呼び出し
- フィールド初期化 → init実行の順

### this

メソッド・init 内で現在のインスタンスを参照する。

```iro
class Builder {
    public var x = 0

    fn setX(v) {
        this.x = v
        this          // 自身を返す（メソッドチェーン）
    }
}

let b = Builder()
b.setX(42).setX(99)
```

### 継承

```iro
class Animal {
    public var name = ""

    public fn speak() {
        this.name + " speaks"
    }
}

class Dog extends Animal {
    public var breed = ""

    public fn bark() {
        this.name + " barks!"
    }
}

let d = Dog()
d.name = "Rex"
d.bark()   // "Rex barks!"
d.speak()  // "Rex speaks"（親メソッドを継承）
```

- `extends` で単一継承
- `super` で親クラスの参照

### 演算子オーバーロード

クラスにマジックメソッドを定義して演算子をオーバーロードする。

```iro
class Vec {
    public var x = 0
    public var y = 0

    fn __add__(other) {
        let r = Vec()
        r.x = this.x + other.x
        r.y = this.y + other.y
        r
    }

    fn __eq__(other) {
        this.x == other.x and this.y == other.y
    }
}

let a = Vec()
a.x = 1
a.y = 2
let b = Vec()
b.x = 3
b.y = 4
let c = a + b    // Vec(4, 6)
a == b           // false
```

#### マジックメソッド一覧

| 演算子 | メソッド名 | 引数 |
|--------|-----------|------|
| `+` | `__add__` | `(other)` |
| `-` | `__sub__` | `(other)` |
| `*` | `__mul__` | `(other)` |
| `/` | `__div__` | `(other)` |
| `%` | `__mod__` | `(other)` |
| `**` | `__pow__` | `(other)` |
| `==` | `__eq__` | `(other)` |
| `!=` | `__ne__` | `(other)` |
| `<` | `__lt__` | `(other)` |
| `<=` | `__le__` | `(other)` |
| `>` | `__gt__` | `(other)` |
| `>=` | `__ge__` | `(other)` |
| `&` | `__band__` | `(other)` |
| `\|` | `__bor__` | `(other)` |
| `^` | `__bxor__` | `(other)` |
| `<<` | `__lshift__` | `(other)` |
| `>>` | `__rshift__` | `(other)` |
| `~` | `__bnot__` | `()` |
| 単項 `-` | `__neg__` | `()` |

---

## 11. match 式

```iro
let result = match (x) {
    1 => "one"
    2 => "two"
    3 => {
        let msg = "three"
        msg
    }
    _ => "other"    // ワイルドカード（デフォルト）
}
```

- 各アームは `pattern => expression` 形式
- `_` はワイルドカード（どの値にもマッチ）
- ブロック式も使用可能

---

## 12. 例外処理

### try / catch / finally

```iro
try {
    let result = riskyOperation()
    result
} catch (e) {
    println("Error: " + e)
    null
} finally {
    cleanup()
}
```

### throw

```iro
throw "Something went wrong"
throw {code: 404, message: "Not found"}
```

任意の値を例外として投げることができる。

---

## 13. モジュール

### export

```iro
export fn helper() { ... }
export class MyClass { ... }
export let VERSION = "1.0"
export var counter = 0
```

`export` は `fn`, `class`, `let`, `var` の前に付けることができる。

### import

```iro
import "path/to/module.iro"
```

エクスポートされた要素がスコープに追加される。

---

## 14. 標準ライブラリ

標準ライブラリは `stdlib.iro` として irooon 自身で実装されている。

### String メソッド

| メソッド | 説明 | 例 |
|---------|------|-----|
| `length()` | 文字列の長さ | `"abc".length()` → `3` |
| `toUpper()` | 大文字に変換 | `"abc".toUpper()` → `"ABC"` |
| `toLower()` | 小文字に変換 | `"ABC".toLower()` → `"abc"` |
| `trim()` | 前後の空白を削除 | `" hi ".trim()` → `"hi"` |
| `substring(start)` | 部分文字列 | `"hello".substring(2)` → `"llo"` |
| `substring(start, len)` | 長さ指定 | `"hello".substring(1, 3)` → `"ell"` |
| `split(sep)` | 分割 | `"a,b,c".split(",")` → `["a","b","c"]` |
| `contains(s)` | 含有判定 | `"hello".contains("ell")` → `true` |
| `startsWith(s)` | 先頭一致 | `"hello".startsWith("he")` → `true` |
| `endsWith(s)` | 末尾一致 | `"hello".endsWith("lo")` → `true` |
| `replace(old, new)` | 置換 | `"abc".replace("b", "x")` → `"axc"` |

### List メソッド

| メソッド | 説明 | 例 |
|---------|------|-----|
| `length()` | 要素数 | `[1,2,3].length()` → `3` |
| `map(fn)` | 変換 | `[1,2,3].map(x => x * 2)` → `[2,4,6]` |
| `filter(fn)` | 絞り込み | `[1,2,3].filter(x => x > 1)` → `[2,3]` |
| `reduce(init, fn)` | 畳み込み | `[1,2,3].reduce(0, (a,b) => a+b)` → `6` |
| `forEach(fn)` | 各要素に適用 | `list.forEach(fn (x) { println(x) })` |
| `first()` | 先頭要素 | `[1,2,3].first()` → `1` |
| `last()` | 末尾要素 | `[1,2,3].last()` → `3` |
| `isEmpty()` | 空判定 | `[].isEmpty()` → `true` |

### JSON

| 関数 | 説明 |
|------|------|
| `jsonStringify(obj)` | オブジェクトを JSON 文字列に変換 |
| `jsonParse(str)` | JSON 文字列をパースしてオブジェクトに変換 |

```iro
let obj = {name: "Alice", age: 30}
let json = jsonStringify(obj)     // '{"name":"Alice","age":30}'
let parsed = jsonParse(json)      // {name: "Alice", age: 30}
```

---

## 15. ビルトイン関数

### print / println

```iro
print("Hello")              // 改行なし
println("Hello, World!")    // 改行あり
println(1, 2, 3)            // スペース区切りで出力
```

### typeof

値の型名を文字列として返す。

```iro
typeof(42)            // "Number"
typeof("hello")       // "String"
typeof(true)          // "Boolean"
typeof(null)          // "Null"
typeof([1,2,3])       // "List"
typeof({a: 1})        // "Hash"
typeof(fn (x) { x })  // "Function"
```

### delay / awaitAll

```iro
// delay(ms) — 指定ミリ秒後に完了する Task を返す
await delay(1000)

// awaitAll([tasks]) — 全 Task の完了を待ち、結果リストを返す
async fn compute(x) { x * 2 }
let results = awaitAll([compute(1), compute(2), compute(3)])  // [2, 4, 6]
```

### fetch / http — HTTP クライアント

```iro
// fetch(url, options?) — 汎用 HTTP リクエスト
// 戻り値: Task<Hash> — await で { status, body, headers, ok } を取得
async fn main() {
    let res = await fetch("https://api.example.com/data")
    println(res.status)   // 200
    println(res.body)     // レスポンスボディ
    println(res.ok)       // true

    // POST リクエスト（オプション指定）
    let res = await fetch("https://api.example.com/users", {
        method: "POST",
        body: jsonStringify({ name: "Alice" }),
        headers: { "Content-Type": "application/json" },
        timeout: 5000
    })
}
```

```iro
// http オブジェクト — 便利メソッド
async fn main() {
    let res = await http.get("https://api.example.com/users")
    let users = jsonParse(res.body)

    let res = await http.post(url, body, headers)
    let res = await http.put(url, body, headers)
    let res = await http.delete(url, headers)
    let res = await http.patch(url, body, headers)
}
```

---

## 16. CLR 相互運用

.NET のクラスライブラリを直接呼び出すことができる。

```iro
// 静的メソッド
let abs = System.Math.Abs(-42)
let sqrt = System.Math.Sqrt(16)
let pow = System.Math.Pow(2, 8)

// 静的プロパティ
let now = System.DateTime.Now
let pi = System.Math.PI

// インスタンス生成・メソッド呼び出し
let sb = System.Text.StringBuilder()
sb.Append("Hello")
sb.Append(" World")
sb.ToString()  // "Hello World"
```

### アセンブリ参照（#r ディレクティブ）

外部アセンブリを読み込んで使用できる。

```iro
#r "path/to/MyLibrary.dll"

let result = MyNamespace.MyClass.MyMethod()
```

### 制限事項

- 静的メソッド・静的プロパティのアクセスが主な用途

---

## 17. シェルコマンド実行

```iro
let output = $`echo Hello`
let branch = $`git branch --show-current`
```

- `$` の後にバッククォートでコマンドを囲む
- 標準出力を文字列として返す
- 終了コードが 0 以外の場合はエラー

---

## 18. async / await

```iro
async fn fetchData() {
    let result = await someAsyncOperation()
    result
}
```

- `async fn` で非同期関数を定義（`Task.Run` ベースの真の並行実行）
- `await` で非同期結果を待機
- async 関数はクローンされたスコープで実行される（呼び出し元に副作用なし）
- CLR の `Task<T>` も直接 `await` 可能

### async lambda

```iro
let f = async (x) => x * 2
let g = async (x, y) => { x + y }
let h = async x => x + 1
let i = async () => 42

await f(5)    // 10
await h(41)   // 42
```

### ビルトイン非同期ユーティリティ

```iro
// delay(ms) — 指定ミリ秒後に完了する Task を返す
let task = delay(1000)
await task

// awaitAll([tasks]) — 全 Task の完了を待ち、結果リストを返す
async fn double(x) { x * 2 }
let tasks = [double(1), double(2), double(3)]
let results = awaitAll(tasks)  // [2, 4, 6]
```

---

## 19. CLI

```bash
irooon script.iro           # スクリプト実行
irooon                      # REPL起動
```

---

## 20. Truthyness

以下の値は false として扱われる:

- `null`
- `false`
- `0` / `0.0`
- `""` （空文字列）

それ以外はすべて `true`。
