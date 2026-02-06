# irooon Language Documentation

このディレクトリには irooon 言語の仕様書が含まれています。

## ドキュメント一覧

1. **[language-spec.md](./language-spec.md)**
   - irooon言語の構文仕様
   - 基本的な言語機能の説明
   - AST構造の概要

2. **[expression-tree-mapping.md](./expression-tree-mapping.md)**
   - ASTからExpressionTreeへの変換仕様
   - RuntimeHelpersの詳細
   - 実装上の技術的詳細

## 実装の進め方

実装は以下の順序で TDD（テスト駆動開発）で進めます：

1. **Runtime基盤** - ScriptContext, RuntimeHelpers, IroCallable等
2. **Lexer** - トークン化
3. **AST定義** - すべてのノード
4. **Parser** - 構文解析
5. **Resolver** - スコープ解決
6. **CodeGen** - ExpressionTree生成
7. **CLI** - エントリポイント

## バージョン

現在のバージョン: **v0.1**

v0.1では意図的に以下を簡略化：
- 型推論・型注釈なし
- 継承なし
- 演算子オーバーロードなし
- import/モジュールなし
- REPLなし
