# irooon v0.3 開発計画

## 概要
v0.3では、ループ、文字列、例外処理、モジュールシステム、REPLを実装し、実用的なスクリプト言語として完成させます。

## タスク一覧（#35-44）

### Phase 1: 基礎機能（並列実装可能）
- **Task #38**: 標準出力（print/println）実装 ⭐ 最優先
- **Task #35**: for/foreach/break/continue実装
- **Task #36**: 文字列補間実装
- **Task #37**: 文字列メソッド実装

### Phase 2: エラー処理
- **Task #39**: try/catch/finally実装
- **Task #40**: スタックトレース実装

### Phase 3: モジュールシステム
- **Task #41**: import/export実装
- **Task #42**: パッケージ管理実装（オプショナル）

### Phase 4: REPL
- **Task #43**: REPL実装

### Phase 5: 完成
- **Task #44**: v0.3テスト・サンプル・ドキュメント更新

## 実装順序

### Step 1: 標準出力（最優先）
まず`print`/`println`を実装。これがないとデバッグが困難。

### Step 2: 基礎機能（並列3チーム）
- Team A: ループ（for/foreach/break/continue）
- Team B: 文字列補間
- Team C: 文字列メソッド

### Step 3: エラー処理（並列2チーム）
- Team A: try/catch/finally
- Team B: スタックトレース

### Step 4: モジュールシステム
- import/export（単一チーム、複雑なため）
- パッケージ管理（余裕があれば）

### Step 5: REPL
- 対話的実行環境（単一チーム）

### Step 6: 仕上げ
- テスト、サンプル、ドキュメント

## 推定工数
- Phase 1: 基礎機能（4タスク）→ 並列実装で短縮可能
- Phase 2: エラー処理（2タスク）→ 並列実装可能
- Phase 3: モジュール（2タスク）→ 複雑、時間がかかる
- Phase 4: REPL（1タスク）→ 中程度
- Phase 5: 完成（1タスク）→ 短時間

**合計**: 10タスク

## Git ワークフロー
各タスクごとにIssueを作成し、ブランチを切って実装：
- `feature/35-implement-loops`
- `feature/36-string-interpolation`
- `feature/37-string-methods`
- ...

## 次のアクション
1. Phase 1から開始（標準出力 → ループ・文字列）
2. 並列チームで効率化
3. 段階的にマージしてテスト
