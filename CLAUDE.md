# irooon プロジェクト - Claude作業メモ

## プロジェクト概要

**irooon** は C# で実装する動的スクリプト言語です。
- DLR (System.Linq.Expressions) を使用
- Groovy風の簡略構文
- 式志向設計
- .NET との相互運用

## 実装方針

### 開発スタイル
- **TDD（テスト駆動開発）** で実装
- 各機能の実装前にテストを書く
- 細かくブランチを切って段階的に実装
- 各ブランチは main にマージして進める

### Git Workflow
- Branch model: `main` + `feature/*`
- ブランチ命名: `feature/<issue-number>-<short-slug>`
- 各機能ごとにIssueとブランチを作成
- ブランチは削除しない（監査可能性のため）

### アーキテクチャ
```
Source → Lexer → Parser → AST → Resolver → CodeGen → Compile → Invoke
```

### プロジェクト構造
```
irooon/
├── Irooon.sln
├── src/
│   ├── Irooon.Core/
│   │   ├── Lexer/
│   │   ├── Parser/
│   │   ├── Ast/
│   │   ├── Resolver/
│   │   ├── CodeGen/
│   │   └── Runtime/
│   └── Irooon.Cli/
├── tests/
│   └── Irooon.Tests/
└── docs/
    ├── language-spec.md
    ├── expression-tree-mapping.md
    └── README.md
```

---

## 実装フェーズ

### Phase 1: 基盤セットアップ ✅
- [x] プロジェクト構造とソリューション作成

### Phase 2: Runtime基盤 ✅
- [x] Runtime基本型（ScriptContext, IroCallable, IroClass/Instance）
- [x] RuntimeHelpers（演算・比較・論理）

### Phase 3: Lexer ✅
- [x] トークン定義
- [x] Lexer実装

### Phase 4: AST ✅
- [x] Expression/Statement/ClassDef ノード定義

### Phase 5: Parser ✅
- [x] Parser基本式（リテラル、演算子、変数）
- [x] Parser制御構造（if, while, block）
- [x] Parser関数・クラス

### Phase 6: Resolver ✅
- [x] スコープ解析
- [x] 変数バインディング

### Phase 7: CodeGen ⏳
- [x] ExpressionTree生成（基本式）
- [ ] ExpressionTree生成（制御構造）
- [ ] ExpressionTree生成（関数・クラス）

### Phase 8: CLI ⏳
- [ ] コマンドラインインターフェース

### Phase 9: 統合 ⏳
- [ ] E2Eテスト
- [ ] 統合テスト

---

## 進捗記録

### 2026-02-07

#### 完了したこと ✅

**初期セットアップ:**
- プロジェクトディレクトリ作成
- 仕様書のドキュメント化
  - `docs/language-spec.md` - 言語構文仕様
  - `docs/expression-tree-mapping.md` - ExpressionTree変換仕様
  - `docs/README.md` - ドキュメント索引
- CLAUDE.md 作成（このファイル）
- タスクリスト作成完了（21タスク）

**Task #1-2: プロジェクト基盤（完了）**
- ✅ Git リポジトリ初期化
- ✅ .gitignore 作成
- ✅ README.md 作成
- ✅ .NET ソリューション作成（Irooon.sln）
- ✅ プロジェクト構造作成（Core, Cli, Tests）
- ✅ ビルド確認

**Task #3-5: Runtime基盤（完了）🎉**
- ✅ ScriptContext, IroCallable, IroClass, IroInstance実装
- ✅ FieldDef, MethodDef実装
- ✅ RuntimeHelpers完全実装（算術・比較・論理・関数呼び出し・メンバアクセス）
- ✅ 66個のテスト全て合格
- ✅ コミット: 3件

**Task #6: Lexer実装（完了）🎉**
- ✅ TokenType定義（57種類のトークン）
- ✅ Token クラス実装
- ✅ Lexer 完全実装（字句解析器）
- ✅ 36個のテスト全て合格
- ✅ コミット: 1件

**Task #7-8: AST定義（完了）🎉**
- ✅ 基本ASTノード定義（Expression, Statement等）
- ✅ 25個のASTノードクラス実装
- ✅ 38個のテスト全て合格
- ✅ コミット: 1件

**並列作業の成果（第1弾）:**
3チーム（Runtime, Lexer, AST）を同時に稼働させ、効率的に実装完了！
- **合計テスト数**: 129個
- **成功率**: 100%

**Task #9-11: Parser実装（完了）🎉**
- ✅ Task #9: Parser基本式（リテラル、演算子、変数、関数呼び出し）- 43個のテスト
- ✅ Task #10: Parser制御構造（if, while, return, block）- 12個のテスト
- ✅ Task #11: Parser関数・クラス（関数定義、ラムダ、クラス定義）- 33個のテスト
- ✅ 合計88個の新規テスト、全て合格
- ✅ コミット: 1件

**並列作業の成果（第2弾）:**
2チーム（制御構造、関数・クラス）を同時に稼働させ、効率的に実装完了！
- **全体テスト数**: 221個
- **成功率**: 100%

**Task #12: Resolver実装（完了）🎉**
- ✅ スコープ解析実装
- ✅ 変数バインディング
- ✅ let の再代入禁止チェック
- ✅ 未定義変数の検出
- ✅ 16個のテスト全て合格
- ✅ コミット: 1件

**Task #13: CodeGen基本式実装（完了）🎉**
- ✅ CodeGenerator クラスの基本構造
- ✅ リテラル式の変換（数値、文字列、真偽値、null）
- ✅ 識別子式の変換（変数参照）
- ✅ 代入式の変換
- ✅ let/var文の変換
- ✅ 式文の変換
- ✅ ブロック式の変換
- ✅ 23個のテスト全て合格
- ✅ 全体テスト数: 260個
- ✅ コミット: 1件

#### 次のステップ 📋
**Phase 7: CodeGen実装続き**
- Task #14: 演算子実装
- Task #15: 制御構造実装
- Task #16: 関数とクロージャ実装
- Task #17: クラスとインスタンス実装

---

## 学んだこと・注意点 💡

### 設計上の重要ポイント
1. **すべて object 型に統一** - 動的型付けのため
2. **演算は Runtime に委譲** - 型変換・エラー処理を一箇所に集約
3. **短絡評価の実装** - and/or は必ず短絡評価する（ExpressionTreeで制御）
4. **クロージャの環境キャプチャ** - 関数定義時にスコープをキャプチャ
5. **CLR相互運用** - リフレクションベースで実装

### 実装時の注意
- [x] テストファースト！実装前に必ずテストを書く ← **実践済み！**
- [x] 各ASTノードに line, column を保持させる ← **実装済み！**
- [ ] エラーメッセージには必ず位置情報を含める
- [ ] 破壊的な操作は実装しない（v0.1）

### 学んだこと（実装中）
1. **並列作業の効果**: 3チーム同時稼働で効率的に実装完了
2. **TDDの徹底**: すべてのチームがテストファーストを実践
3. **型システムの統一**: Runtime で object 型に統一し、型変換を一箇所に集約
4. **位置情報の重要性**: すべてのトークン・ASTノードに line, column を保持
5. **名前空間衝突の対策**: `Irooon.Core.Ast.Expression` と `System.Linq.Expressions.Expression` の衝突を避けるため、エイリアス (`using ExprTree = ...`) を使用

---

## TODO / 課題

### 現在のタスク
- Task #14: CodeGen演算子実装（次のタスク）

### 将来の拡張（v0.2以降）
- 型推論・型注釈
- 継承
- import/モジュールシステム
- REPL
- 演算子オーバーロード

---

## 参考リンク

- [System.Linq.Expressions Namespace](https://learn.microsoft.com/en-us/dotnet/api/system.linq.expressions)
- [DLR Overview](https://learn.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/dynamic-language-runtime-overview)

---

**最終更新:** 2026-02-07 22:30 JST
**進捗**: Phase 1-6 完了（Runtime, Lexer, AST, Parser, Resolver） / Phase 7 CodeGen進行中（基本式完了）
