# irooon

[![.NET](https://img.shields.io/badge/.NET-10.0-purple)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![GitHub](https://img.shields.io/badge/GitHub-0x6d61%2Firooon-black)](https://github.com/0x6d61/irooon)

**irooon** は .NET 上で動作する動的スクリプト言語です。

## 特徴

- 🚀 **DLR (System.Linq.Expressions) を使用** - .NET の動的言語ランタイム上で動作
- ✨ **Groovy風の簡略構文** - セミコロン不要、式志向の設計
- 🔗 **CLR相互運用** - .NET のクラスライブラリをシームレスに利用可能
- 🎯 **動的型付け** - シンプルで柔軟な型システム
- 📦 **クロージャサポート** - ファーストクラス関数とクロージャ

## クイックスタート

### 必要要件

- .NET 10.0 SDK 以上

### インストール

```bash
git clone https://github.com/0x6d61/irooon.git
cd irooon
dotnet build
```

## ビルド

### Windows

```powershell
# リリースビルド
.\build.ps1

# デバッグビルド
.\build.ps1 -Configuration Debug

# テスト付きビルド
.\build.ps1 -Test

# 実行可能ファイルを生成
.\build.ps1 -Publish -Runtime win-x64
```

### Linux/macOS

```bash
# リリースビルド
dotnet build --configuration Release

# テスト実行
dotnet test

# 実行可能ファイルを生成
dotnet publish src/Irooon.Cli/Irooon.Cli.csproj \
  --configuration Release \
  --runtime linux-x64 \
  --self-contained true \
  --output publish/linux-x64
```

### 使い方

```bash
# スクリプトを実行（Windows）
.\run.ps1 examples/hello.iro

# スクリプトを実行（直接実行）
dotnet run --project src/Irooon.Cli/Irooon.Cli.csproj script.iro

# または、ビルドしてから実行
dotnet build
./src/Irooon.Cli/bin/Debug/net10.0/Irooon.Cli script.iro
```

### サンプル

```bash
# Hello World
dotnet run --project src/Irooon.Cli/Irooon.Cli.csproj examples/hello.iro

# 階乗の計算
dotnet run --project src/Irooon.Cli/Irooon.Cli.csproj examples/factorial.iro

# for ループ（条件ループ）
dotnet run --project src/Irooon.Cli/Irooon.Cli.csproj examples/loop.iro

# クラスとオブジェクト
dotnet run --project src/Irooon.Cli/Irooon.Cli.csproj examples/class_example.iro

# ラムダと高階関数
dotnet run --project src/Irooon.Cli/Irooon.Cli.csproj examples/lambda_example.iro

# 演算子と条件分岐
dotnet run --project src/Irooon.Cli/Irooon.Cli.csproj examples/operators.iro

# リストとハッシュ
dotnet run --project src/Irooon.Cli/Irooon.Cli.csproj examples/list_example.iro
dotnet run --project src/Irooon.Cli/Irooon.Cli.csproj examples/hash_example.iro
dotnet run --project src/Irooon.Cli/Irooon.Cli.csproj examples/data_structures.iro

# 標準出力（print/println）
dotnet run --project src/Irooon.Cli/Irooon.Cli.csproj examples/print_example.iro
```

## 言語機能

### ビルトイン関数

#### print / println
標準出力に値を出力します。`println`は改行を付けます。

```irooon
println("Hello, World!")          // 出力: Hello, World!

print("Sum: ")
print(10, "+", 5, "=", 15)        // 出力: Sum: 10 + 5 = 15
println()                         // 改行のみ

let name = "Alice"
let age = 30
println("Name:", name, "Age:", age)  // 出力: Name: Alice Age: 30
```

### 文字列

#### 文字列補間
文字列内に式を埋め込めます。

```irooon
let name = "Alice"
let age = 30
let message = "Name: ${name}, Age: ${age}"
println(message)  // 出力: Name: Alice, Age: 30
```

#### 文字列メソッド
便利な文字列操作メソッドを提供します。

```irooon
let text = "Hello, World!"
text.length()         // 13
text.toUpper()        // "HELLO, WORLD!"
text.toLower()        // "hello, world!"
text.substring(0, 5)  // "Hello"
text.contains("World")  // true
text.startsWith("Hello")  // true
text.endsWith("!")      // true
text.replace("World", "irooon")  // "Hello, irooon!"
text.trim()           // 前後の空白を削除
text.split(", ")      // ["Hello", "World!"]
```

### 変数
```irooon
let x = 10  // 再代入不可
var y = 20  // 再代入可能
```

### リスト
```irooon
let numbers = [1, 2, 3, 4, 5]
let first = numbers[0]  // アクセス
numbers[1] = 99         // 代入
```

### ハッシュ
```irooon
let person = {name: "Alice", age: 30}
let name = person["name"]  // アクセス
person["age"] = 31         // 代入
```

### 関数
```irooon
fn add(a, b) {
    a + b
}

let multiply = fn (x, y) { x * y }

// 型アノテーション
fn add(a: Number, b: Number): Number { a + b }
let greet = (name: String) => "Hello, " + name
```

### クラス
```irooon
class Counter {
    public var value = 0

    public fn increment() {
        value = value + 1
    }

    init(initialValue) {
        value = initialValue
    }
}
```

### 制御構造

#### if式
```irooon
if (x > 0) {
    "positive"
} else {
    "non-positive"
}
```

#### ループ（for統一構文）
```irooon
// 条件ループ（旧while）
var i = 0
for (i < 10) {
    i = i + 1
}

// コレクション反復（リスト）
for (item in [1, 2, 3, 4, 5]) {
    println(item)
}

// コレクション反復（ハッシュ）
for (entry in {name: "Alice", age: 30}) {
    println(entry.key, ":", entry.value)
}

// 範囲リテラル
for (i in 1..10) {     // 1から9まで（排他的）
    println(i)
}

for (i in 1...10) {    // 1から10まで（包括的）
    println(i)
}

// break / continue
for (i in [1, 2, 3, 4, 5]) {
    if (i == 3) { continue }
    if (i == 5) { break }
    println(i)
}
```

#### 例外処理
```irooon
try {
    // エラーが発生する可能性のあるコード
    let result = riskyOperation()
    println("Success:", result)
} catch (e) {
    // エラーハンドリング
    println("Error:", e.message)
} finally {
    // 必ず実行されるコード
    println("Cleanup")
}

// エラーを投げる
throw "Something went wrong"
```

### モジュール

#### export / import
モジュールとして関数やクラスをエクスポート・インポートできます。

```irooon
// math.iro
export fn add(a, b) {
    a + b
}

export fn multiply(a, b) {
    a * b
}

// main.iro
import "math.iro"

let result = add(10, 5)
println(result)  // 出力: 15
```

### REPL

対話的実行環境（REPL）を提供します。

```bash
# REPLを起動
dotnet run --project src/Irooon.Repl/Irooon.Repl.csproj

# または、ビルドしてから実行
./src/Irooon.Repl/bin/Debug/net10.0/Irooon.Repl
```

REPL内で式や文を評価し、即座に結果を確認できます。

```
> let x = 10
null
> x + 5
15
> fn square(n) { n * n }
null
> square(4)
16
```

## サンプルコード

```irooon
// 変数宣言
let name = "World"
var count = 0

// 関数定義
fn greet(name) {
  "Hello, " + name + "!"
}

// 呼び出し
greet(name)

// クラス定義
class Counter {
  public var value = 0

  public fn increment() {
    value = value + 1
  }

  public fn get() {
    value
  }
}

// インスタンス生成
let counter = Counter()
counter.increment()
counter.get()  // 1

// リストとハッシュ
let data = {
  users: [
    {name: "Alice", age: 30},
    {name: "Bob", age: 25}
  ]
}
let firstUser = data["users"][0]
firstUser["name"]  // "Alice"

// 文字列補間
let message = "User: ${firstUser["name"]}, Age: ${firstUser["age"]}"
println(message)

// for ループ（コレクション反復）
for (user in data["users"]) {
    println("Name:", user["name"])
}

// 例外処理
try {
    let value = data["invalid"]
    if (value == null) {
        throw "Value not found"
    }
} catch (e) {
    println("Error:", e.message)
}

// CLR相互運用（.NET標準ライブラリの呼び出し）
let abs = System.Math.Abs(-42)
let sqrt = System.Math.Sqrt(16)
let now = System.DateTime.Now
println("Abs:", abs, "Sqrt:", sqrt, "Now:", now)
```

## ドキュメント

詳細な仕様は [docs/](./docs/) ディレクトリを参照してください：

- [言語仕様](./docs/language-spec.md)
- [ExpressionTree変換仕様](./docs/expression-tree-mapping.md)

## 開発状況

現在のバージョン: **v0.12.2**

変更履歴は [CHANGELOG.md](./CHANGELOG.md) を参照してください。

## ライセンス

MIT License

## 貢献

このプロジェクトは現在開発初期段階です。
