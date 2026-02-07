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

### 使い方

```bash
# スクリプトを実行
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

# while ループ
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
```irooon
if (x > 0) {
    "positive"
} else {
    "non-positive"
}

while (i < 10) {
    i = i + 1
}
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
```

## ドキュメント

詳細な仕様は [docs/](./docs/) ディレクトリを参照してください：

- [言語仕様](./docs/language-spec.md)
- [ExpressionTree変換仕様](./docs/expression-tree-mapping.md)

## 開発状況

**v0.2.1** リリース済み（2026-02-07）

- v0.2.0: リスト・ハッシュリテラルのサポート
- v0.2.1: 既知の問題の修正（Fibonacci再帰、メソッド内フィールドアクセス）

変更履歴は [CHANGELOG.md](./CHANGELOG.md) を参照してください。
開発の詳細は [CLAUDE.md](./CLAUDE.md) で確認できます。

### v0.1の既知の問題 → v0.2.1で修正済み ✅

- ✅ **Fibonacci など複雑な再帰関数**: v0.2.1で修正（パラメータ保存・復元を実装）
- ✅ **クラスメソッド内でのフィールドアクセス**: v0.2.1で修正（メソッドスコープにフィールドを宣言）
- ✅ **メンバへの代入**: v0.2.0で実装済み（`obj.field = value` をサポート）

## ライセンス

MIT License

## 貢献

このプロジェクトは現在開発初期段階です。
