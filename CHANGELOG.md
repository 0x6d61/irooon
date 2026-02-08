# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.5.2] - 2026-02-08

### Added
- ✨ **複合代入演算子** (#18)
  - `+=`, `-=`, `*=`, `/=`, `%=` をサポート
  - 変数、配列要素、クラスフィールドに対して使用可能
  - 内部的に通常の代入と算術演算に展開される（例: `x += 1` → `x = x + 1`）
  - TokenType に PlusEqual, MinusEqual, StarEqual, SlashEqual, PercentEqual を追加
  - Lexer で複合代入演算子をトークン化
  - Parser で複合代入演算子を通常の代入に展開
  - 13個の新規テスト追加

### Tests
- ✅ 全テスト成功: 665個（653個 + 12個REPL）
- ✅ スキップ: 0個
- ✅ 成功率: 100%

## [0.5.0] - 2026-02-08

### Added
- ✨ **CLR相互運用** (#14)
  - .NET標準ライブラリの呼び出しをサポート
  - 静的メソッドの呼び出し: `System.Math.Abs(-42)`
  - 静的プロパティのアクセス: `System.DateTime.Now`
  - RuntimeHelpers.ResolveCLRType / InvokeCLRStaticMethod を実装
  - Resolverで System で始まる名前を許可
  - CodeGeneratorでCLR型とメソッド呼び出しを検出
  - サンプルスクリプト examples/clr_interop.iro を追加
  - 18個の新規テスト追加（11個のユニットテスト + 7個のE2Eテスト）

- 👪 **クラス継承** (#16)
  - 単一継承をサポート: `class Child : Parent { ... }`
  - 親クラスのフィールド継承
  - 親クラスのメソッド継承
  - メソッドオーバーライド（子クラスのメソッドが優先）
  - 多段階継承（Child → Parent → GrandParent）
  - ClassDef に ParentClass プロパティを追加
  - IroClass に Parent プロパティと継承チェーンを実装
  - サンプルスクリプト examples/inheritance.iro を追加
  - 11個の新規テスト追加（6個のユニットテスト + 5個のE2Eテスト）

### Fixed
- 🐛 **ネストしたforループのテスト修正** (#15)
  - スキップされていた TestFor_Nested を有効化
  - ネストしたforループが正しく動作することを確認

### Tests
- ✅ 全テスト成功: 652個（640個 + 12個REPL）
- ✅ スキップ: 0個
- ✅ 成功率: 100%

## [0.4.0] - 2026-02-08

### Added
- ✨ **範囲リテラル** (#50, Phase 1)
  - `start..end` - 排他的範囲（endを含まない）
  - `start...end` - 包括的範囲（endを含む）
  - RangeExpr ASTノードを追加
  - Lexerに .. と ... 演算子を追加
  - RuntimeHelpers.CreateRangeメソッドを実装
  - forループでの反復に使用可能

- 🔄 **forループへの統一** (#50, Phase 2-3)
  - すべてのループ構文をforに統一
  - `for (condition)` - 条件ループ（旧while）
  - `for (item in collection)` - コレクション反復（旧foreach）
  - ForStmt ASTノードを追加（ForStmtKind: Collection/Condition）
  - Parserにlookahead機能を実装してパターンを識別
  - break/continueはそのまま継続サポート

### Removed
- ❌ **WhileStmt削除** (#50, Phase 3)
  - while構文を削除し、for (condition) に統一
  - WhileStmt.csファイルを削除
  - TokenType.Whileを削除
  - Lexerからwhileキーワードを削除

- ❌ **ForeachStmt削除** (#50, Phase 2)
  - foreach構文を削除し、for (item in collection) に統一
  - ForeachStmtをForStmtに置き換え

### Changed
- 📝 **ドキュメント更新** (#50, Phase 5)
  - language-spec.md: ループセクションを更新（for統一）
  - language-spec.md: 範囲リテラルセクションを追加
  - CHANGELOG.md: v0.4.0セクションを追加
  - README.md: ループ構文を更新
  - CLAUDE.md: v0.4開発記録を追加

- 📁 **サンプルスクリプト更新** (#50, Phase 4)
  - examples/loops.iro: while/foreach → for に変換
  - examples/string_methods_example.iro: while → for に変換
  - examples/loop.iro: while → for に変換

### Tests
- ✅ 全テスト成功: 610個（1個スキップ）
- ✅ Range関連のテスト追加: 7個
- ✅ ForStmt関連のテスト追加: 11個
- ✅ 既存のwhile/foreachテストをforに変換

## [0.3.0] - 2026-02-08

### Added
- ✨ **ビルトイン関数: print/println** (#3, #38)
  - `print(...args)` - 標準出力に値を出力（改行なし）
  - `println(...args)` - 標準出力に値を出力（改行あり）
  - 複数引数をスペース区切りで出力
  - nullは"null"として表示
  - BuiltinFunctionクラスを実装（IroCallableを実装）
  - RuntimeHelpers.Print/Printlnメソッドを追加
  - ScriptContextとResolverでビルトイン関数を自動登録

- 🔤 **文字列補間** (#5, #36)
  - `"Hello, ${name}!"` - 文字列内に式を埋め込む
  - StringInterpolationExpr ASTノードを追加
  - Lexerに文字列補間のトークン解析機能を追加
  - CodeGeneratorで文字列連結に変換

- 📝 **文字列メソッド** (#6, #37)
  - `length()` - 文字列の長さを取得
  - `toUpper()` / `toLower()` - 大文字/小文字変換
  - `trim()` - 前後の空白を削除
  - `substring(start, length)` - 部分文字列を取得
  - `split(separator)` - 文字列を分割
  - `contains(value)` / `startsWith(value)` / `endsWith(value)` - 文字列検索
  - `replace(oldValue, newValue)` - 文字列置換
  - StringMethodWrapperクラスを実装
  - CLR相互運用で.NET Stringメソッドを呼び出し

- 🔄 **ループ構造: foreach/break/continue** (#7, #35)
  - `foreach (item in collection) { ... }` - コレクション（リスト・ハッシュ）の反復処理
  - `break` - ループを中断
  - `continue` - 次の反復へスキップ
  - ネストしたループのサポート
  - ループ変数の適切なスコープ処理
  - ForeachStmt, BreakStmt, ContinueStmt ASTノードを追加
  - Lexerに foreach, in, break, continue キーワードを追加
  - CodeGeneratorにループラベルスタック管理機構を実装
  - Resolverにループ変数スコープ処理を追加

- ⚠️ **例外処理: try/catch/finally** (#8, #39)
  - `try { ... } catch (e) { ... } finally { ... }` - 例外処理構文
  - `throw expression` - 例外を投げる
  - catchブロックで例外オブジェクトを受け取る
  - finallyブロックは必ず実行される
  - TryStmt, ThrowStmt ASTノードを追加
  - Lexerに try, catch, finally, throw キーワードを追加
  - CodeGeneratorでExpressionTree.TryCatchFinallyを使用

- 📚 **スタックトレース** (#40)
  - エラー発生時にスタックトレースを表示
  - ファイル名、行番号、列番号を含む詳細な情報
  - RuntimeException クラスを実装
  - スタックフレーム情報の収集機構を実装

- 📦 **モジュールシステム: export/import** (#9, #41)
  - `export fn/class/let/var` - 関数やクラスをエクスポート
  - `import "path/to/module.iro"` - 他のモジュールをインポート
  - モジュールのスコープ分離
  - ExportStmt, ImportStmt ASTノードを追加
  - Lexerに export, import キーワードを追加
  - ModuleLoaderクラスを実装

- 🎮 **REPL (Read-Eval-Print Loop)** (#43)
  - 対話的実行環境を提供
  - 複数行入力のサポート
  - セッション状態の保持
  - エラーメッセージの表示
  - Irooon.Replプロジェクトを追加
  - ReplEngineクラスを実装

- 📚 **新しいサンプルスクリプト**
  - `examples/print_example.iro` - print/printlnのデモ
  - `examples/string_methods_example.iro` - 文字列メソッドのデモ
  - `examples/loops.iro` - foreach/break/continueのデモ
  - `examples/main_import.iro` - import/exportのデモ

- 🧪 **テストの拡充**
  - BuiltinFunctionsTests.cs（8個のテスト）
  - StringMethodsTests.cs（10個のテスト）
  - StringInterpolationTests.cs（6個のテスト）
  - ForeachTests.cs（7個のテスト）
  - ExceptionTests.cs（8個のテスト）
  - ModuleTests.cs（5個のテスト）
  - ReplEngineTests.cs（12個のテスト）
  - E2Eテスト（複数の新規テスト）

- 📖 **ドキュメント更新**
  - README.md に全v0.3.0機能のセクションを追加
  - language-spec.md に新構文を追加

### Changed
- ✅ **全テスト成功**: 604個のテスト全てが成功（スキップ0）
- 🔧 **プロジェクト構造**: Irooon.Replプロジェクトを追加

## [0.2.1] - 2026-02-07

### Fixed
- 🐛 **Fibonacci再帰の計算結果修正** (#1)
  - fibonacci(10)が正しく55を返すようになりました（以前は-80を返していた）
  - 再帰呼び出し時にパラメータが上書きされる問題を修正
  - Closureクラスにパラメータの保存・復元機能を実装
  - RuntimeHelpers.Invokeでパラメータの保存・復元を実行
  - TestExecute_Fibonacciのスキップを解除

- 🐛 **メソッド内フィールドアクセスのサポート** (#2)
  - クラスメソッド内でフィールドに直接アクセスできるようになりました
  - `value = value + 1` のようなフィールド代入が動作するようになりました
  - Resolverがメソッドスコープにクラスフィールドを登録するように改善
  - TestExecute_MultipleClasses, TestMemberAssign_Fieldのスキップを解除
  - 新しいテストケースを2つ追加（複数フィールドの操作、フィールド入れ替え）

### Changed
- ✅ **全テスト成功**: 476個のテスト全てが成功（スキップ0）
- 📖 README.mdの既知の制限事項を「修正済み」に更新

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

### Known Issues (All fixed in v0.2.1 ✅)
- ~~Fibonacci など複雑な再帰関数の計算結果が不正確な場合があります~~ → v0.2.1で修正済み
- ~~クラスメソッド内でのフィールド直接アクセスに制限があります~~ → v0.2.1で修正済み
- ~~メンバへの代入（`obj.field = value`）は現在サポートされていません~~ → v0.2.0で実装済み

### Future Plans (v0.2)
- 型推論・型注釈
- 継承
- import/モジュールシステム
- REPL
- 演算子オーバーロード
