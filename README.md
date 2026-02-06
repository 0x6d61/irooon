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

- .NET 8.0 SDK 以上

### インストール

```bash
git clone https://github.com/yourusername/irooon.git
cd irooon
dotnet build
```

### 使い方

```bash
dotnet run --project src/Irooon.Cli/Irooon.Cli.csproj script.iro
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

## ライセンス

MIT License

## 貢献

このプロジェクトは現在開発初期段階です。
