# irooon Language Specification v0.1

## 概要

irooon は .NET 上で動作する動的スクリプト言語である。
DLR (System.Linq.Expressions) を使用して実行する。
Groovy風の簡略構文と式志向設計を採用する。

---

## 実行モデル

* トップレベルはそのまま実行される
* プログラムは Block として評価される
* 動的型言語（すべて object）
* REPLは未実装（スクリプト実行のみ）

---

## 基本構文

セミコロンは不要。

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

### インデックス

```
expr[index]
```

---

## if式

```
if (cond) { expr } else { expr }
```

* `else` は必須。

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
* `AssignExpr`
* `IfExpr`
* `BlockExpr`
* `LambdaExpr`
* `NewExpr`

### Statement

* `LetStmt`
* `VarStmt`
* `ClassStmt`
* `ExprStmt`
* `ReturnStmt`
* `WhileStmt`

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

## v0.1での意図的な簡略化

* 型推論・型注釈なし
* 継承なし
* 演算子オーバーロードなし
* 括弧省略なし
* import/モジュールなし（後で追加）
* REPLなし
