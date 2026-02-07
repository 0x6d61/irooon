# irooon

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

現在 **v0.1** を開発中です。

進捗状況は [CLAUDE.md](./CLAUDE.md) で確認できます。

### 既知の制限事項（v0.1）

- **Fibonacci など複雑な再帰関数**: 計算結果が不正確な場合があります（要調査）
- **クラスメソッド内でのフィールドアクセス**: 現在、メソッド内で直接フィールドにアクセスできません
- **メンバへの代入**: プロパティへの代入（`obj.field = value`）は現在サポートされていません

## ライセンス

MIT License

## 貢献

このプロジェクトは現在開発初期段階です。
