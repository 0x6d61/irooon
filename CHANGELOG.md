# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Fixed
- 🐛 **メソッド内フィールドアクセスのサポート**
  - クラスメソッド内でフィールドに直接アクセスできるようになりました
  - `value = value + 1` のようなフィールド代入が動作するようになりました
  - Resolverがメソッドスコープにクラスフィールドを登録するように改善
  - 新しいテストケースを2つ追加（複数フィールドの操作、フィールド入れ替え）

## [0.2.0] - 2026-02-07

### Added
- ✨ **リストとハッシュのサポート**
  - リストリテラル: `[1, 2, 3]`
  - ハッシュリテラル: `{name: "Alice", age: 30}`
  - インデックスアクセス: `list[0]`, `hash["key"]`
  - インデックス代入: `list[0] = value`, `hash["key"] = value`
  - ネストしたデータ構造のサポート
- 📚 **新しいサンプルスクリプト**
  - `examples/list_example.iro` - リストの基本操作
  - `examples/hash_example.iro` - ハッシュの基本操作
  - `examples/data_structures.iro` - ネストしたデータ構造
- 🧪 **E2Eテストの拡充**
  - リスト操作のテスト（8個追加）
  - ハッシュ操作のテスト
  - ネストしたデータ構造のテスト
- 📖 **ドキュメント更新**
  - README.md にリスト・ハッシュの説明を追加
  - language-spec.md にリテラル構文を追加

### Changed
- 🔧 Lexer: 左角括弧・右角括弧・コロンのトークンを追加
- 🔧 Parser: リスト・ハッシュのパース機能を追加
- 🔧 AST: ListExpr, HashExpr, IndexExpr ノードを追加
- 🔧 CodeGen: リスト・ハッシュの ExpressionTree 生成機能を追加
- 🔧 Runtime: リスト・ハッシュの実行時サポートを追加

## [0.1.0] - 2026-02-07

### Added
- 🎉 初回リリース: irooon v0.1
- ✨ 基本的な言語機能
  - 変数宣言（let/var）
  - 算術・比較・論理演算子
  - 制御構造（if, while）
  - 関数定義とラムダ式
  - クラスとインスタンス
  - クロージャ
- 🔧 Lexer（字句解析器）
- 🔧 Parser（構文解析器）
- 🔧 Resolver（スコープ解析）
- 🔧 CodeGen（ExpressionTree生成）
- 🚀 CLI（コマンドラインインターフェース）
- 📚 ドキュメント
  - 言語仕様書
  - ExpressionTree変換仕様書
  - サンプルスクリプト
- 🧪 包括的なテストスイート（381テスト）

### Known Issues
- ~~Fibonacci など複雑な再帰関数の計算結果が不正確な場合があります~~ → v0.2.1で修正済み
- ~~クラスメソッド内でのフィールド直接アクセスに制限があります~~ → v0.2.1で修正済み
- メンバへの代入（`obj.field = value`）は現在サポートされていません

### Future Plans (v0.2)
- 型推論・型注釈
- 継承
- import/モジュールシステム
- REPL
- 演算子オーバーロード
