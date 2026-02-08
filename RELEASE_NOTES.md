# irooon Release Notes

## v0.3.0 (2026-02-08) 🚀

irooon v0.3.0 では、実用的なスクリプト言語として必要な機能を大幅に追加しました！

### 新機能

#### ✨ ビルトイン関数: print/println
標準出力に値を出力する関数を追加しました。

```irooon
println("Hello, World!")
print("x =", x, "y =", y)
```

#### 🔤 文字列補間
文字列内に式を埋め込めるようになりました。

```irooon
let name = "Alice"
let message = "Hello, ${name}!"
```

#### 📝 文字列メソッド
便利な文字列操作メソッドを10個追加しました。

```irooon
let text = "Hello, World!"
text.length()         // 13
text.toUpper()        // "HELLO, WORLD!"
text.substring(0, 5)  // "Hello"
text.contains("World")  // true
text.replace("World", "irooon")  // "Hello, irooon!"
```

#### 🔄 ループ: foreach/break/continue
コレクションの反復処理が簡単になりました。

```irooon
foreach (item in [1, 2, 3, 4, 5]) {
    println(item)
}

foreach (entry in {name: "Alice", age: 30}) {
    println(entry.key, ":", entry.value)
}
```

#### ⚠️ 例外処理: try/catch/finally
エラーハンドリングが可能になりました。

```irooon
try {
    let result = riskyOperation()
} catch (e) {
    println("Error:", e.message)
} finally {
    println("Cleanup")
}

throw "Something went wrong"
```

#### 📦 モジュールシステム: export/import
コードをモジュール化できるようになりました。

```irooon
// math.iro
export fn add(a, b) { a + b }

// main.iro
import "math.iro"
let result = add(10, 5)
```

#### 🎮 REPL
対話的実行環境を提供します。

```bash
dotnet run --project src/Irooon.Repl/Irooon.Repl.csproj
```

### 統計

- **テスト数**: 604個（100%成功）
- **サンプルスクリプト**: 17個
- **新規プロジェクト**: Irooon.Repl

### 改善点

- スタックトレース機能を追加し、エラー発生時の詳細情報を表示
- ドキュメントを全面的に更新（README.md, CHANGELOG.md, language-spec.md）

---

## v0.2.1 (2026-02-07)

### 修正

- Fibonacci再帰の計算結果を修正
- メソッド内フィールドアクセスをサポート

---

## v0.2.0 (2026-02-07)

### 新機能

- リストとハッシュのサポート
- インデックスアクセス・代入

---

## v0.1.0 (2026-02-07) 🎉

irooon 言語の初回リリースです！

## 主な機能

- ✨ 動的型付けスクリプト言語
- 🚀 .NET DLR (System.Linq.Expressions) ベース
- 📦 Groovy風の簡略構文
- 🎯 式志向設計
- 🔗 CLR相互運用

## ダウンロード

ソースコードをクローンしてビルドしてください：

```bash
git clone https://github.com/0x6d61/irooon.git
cd irooon
dotnet build
```

## 使い方

```bash
dotnet run --project src/Irooon.Cli/Irooon.Cli.csproj script.iro
```

詳細は [README.md](README.md) を参照してください。

## 言語機能

### 変数宣言
```irooon
let x = 10  // 再代入不可
var y = 20  // 再代入可能
```

### 関数定義
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

let counter = Counter(0)
counter.increment()
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

## 既知の問題

- Fibonacci など複雑な再帰関数の計算結果が不正確な場合があります
- クラスメソッド内でのフィールド直接アクセスに制限があります
- メンバへの代入（`obj.field = value`）は現在サポートされていません

## 次のステップ（v0.2）

- 型推論・型注釈
- 継承
- import/モジュールシステム
- REPL
- 上記既知の問題の修正

## 統計

- **テスト数**: 381個（100%成功）
- **サンプルスクリプト**: 8個
- **ドキュメント**: 言語仕様書、ExpressionTree変換仕様書

## 謝辞

このプロジェクトは Claude Sonnet 4.5 との協力により開発されました。
