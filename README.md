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
```

## 言語機能

### 変数
```irooon
let x = 10  // 再代入不可
var y = 20  // 再代入可能
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
```

## ドキュメント

詳細な仕様は [docs/](./docs/) ディレクトリを参照してください：

- [言語仕様](./docs/language-spec.md)
- [ExpressionTree変換仕様](./docs/expression-tree-mapping.md)

## 開発状況

**v0.1.0** リリース済み（2026-02-07）

変更履歴は [CHANGELOG.md](./CHANGELOG.md) を参照してください。
開発の詳細は [CLAUDE.md](./CLAUDE.md) で確認できます。

### 既知の制限事項（v0.1）

- **Fibonacci など複雑な再帰関数**: 計算結果が不正確な場合があります（要調査）
- **クラスメソッド内でのフィールドアクセス**: 現在、メソッド内で直接フィールドにアクセスできません
- **メンバへの代入**: プロパティへの代入（`obj.field = value`）は現在サポートされていません

## ライセンス

MIT License

## 貢献

このプロジェクトは現在開発初期段階です。
