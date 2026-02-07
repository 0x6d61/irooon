# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
- Fibonacci など複雑な再帰関数の計算結果が不正確な場合があります
- クラスメソッド内でのフィールド直接アクセスに制限があります
- メンバへの代入（`obj.field = value`）は現在サポートされていません

### Future Plans (v0.2)
- 型推論・型注釈
- 継承
- import/モジュールシステム
- REPL
- 演算子オーバーロード
