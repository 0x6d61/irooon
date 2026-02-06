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

### Phase 1: 基盤セットアップ ⏳
- [ ] プロジェクト構造とソリューション作成

### Phase 2: Runtime基盤 ⏳
- [ ] Runtime基本型（ScriptContext, IroCallable, IroClass/Instance）
- [ ] RuntimeHelpers（演算・比較・論理）

### Phase 3: Lexer ⏳
- [ ] トークン定義
- [ ] Lexer実装

### Phase 4: AST ⏳
- [ ] Expression/Statement/ClassDef ノード定義

### Phase 5: Parser ⏳
- [ ] Parser基本式（リテラル、演算子、変数）
- [ ] Parser制御構造（if, while, block）
- [ ] Parser関数・クラス

### Phase 6: Resolver ⏳
- [ ] スコープ解析
- [ ] 変数バインディング

### Phase 7: CodeGen ⏳
- [ ] ExpressionTree生成（基本式）
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
- プロジェクトディレクトリ作成
- 仕様書のドキュメント化
  - `docs/language-spec.md` - 言語構文仕様
  - `docs/expression-tree-mapping.md` - ExpressionTree変換仕様
  - `docs/README.md` - ドキュメント索引
- CLAUDE.md 作成（このファイル）
- **タスクリスト作成完了（21タスク）**
  - Task #1-2: プロジェクトセットアップ
  - Task #3-5: Runtime基盤
  - Task #6: Lexer
  - Task #7-8: AST定義
  - Task #9-11: Parser実装
  - Task #12: Resolver
  - Task #13-17: CodeGen実装
  - Task #18: コンパイルエンジン
  - Task #19: CLI
  - Task #20-21: テストとドキュメント

#### 次のステップ 📋
- Task #1: Git リポジトリ初期化とプロジェクト基盤セットアップ
  - Gitリポジトリ初期化
  - .gitignore 作成
  - README.md 作成

---

## 学んだこと・注意点 💡

### 設計上の重要ポイント
1. **すべて object 型に統一** - 動的型付けのため
2. **演算は Runtime に委譲** - 型変換・エラー処理を一箇所に集約
3. **短絡評価の実装** - and/or は必ず短絡評価する（ExpressionTreeで制御）
4. **クロージャの環境キャプチャ** - 関数定義時にスコープをキャプチャ
5. **CLR相互運用** - リフレクションベースで実装

### 実装時の注意
- [ ] テストファースト！実装前に必ずテストを書く
- [ ] 各ASTノードに line, column を保持させる
- [ ] エラーメッセージには必ず位置情報を含める
- [ ] 破壊的な操作は実装しない（v0.1）

---

## TODO / 課題

### 現在のタスク
- Git リポジトリ初期化
- プロジェクト構造セットアップ

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

**最終更新:** 2026-02-07
