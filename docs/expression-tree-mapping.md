# irooon ExpressionTree Mapping Specification v0.1

## 前提

* 生成するExpressionは原則 `object` 型に統一する
* 数値は初期実装では `double` に統一する（将来拡張で `int` 等を追加可能）
* 実行時の演算・比較・論理は Runtime ヘルパーに委譲する
* 例外は .NET 例外として投げる（言語例外型は後回し可）
* トップレベルは「大きなBlockExpression」として生成し、そのまま Compile/Invoke する

---

## 1. コンパイル単位とコンテキスト

### 1.1 ScriptContext

ExpressionTree生成時に、実行コンテキストを1つ渡す。

想定クラス:

```csharp
class ScriptContext
{
    public Dictionary<string, object> Globals { get; }
    public Dictionary<string, IroClass> Classes { get; }
    public RuntimeHelpers Runtime { get; }
}
```

Generator は常に `ParameterExpression ctx` を受け取る。

```csharp
ParameterExpression ctx = Expression.Parameter(typeof(ScriptContext), "ctx");
```

---

## 2. グローバル変数の表現

### 2.1 let/var はどちらも Globals に保持

* `let`/`var` の違いは Resolver で再代入可否を検査し、CodeGenは同じにする

**参照:**
```
ctx.Globals["x"]
```

**代入:**
```
ctx.Globals["x"] = value
```

**ExpressionTree:**

* `ctx.Globals`: `Expression.Property(ctx, "Globals")`
* indexer: `Expression.Property(dictExpr, "Item", Expression.Constant("x"))`

**注意:**

* `Dictionary<string, object>` の Item は `object` を返すので型が揃う

---

## 3. ブロックとスコープ

### 3.1 ブロックスコープの最小実装

* v0.1はグローバル変数を Globals に集約
* 関数ローカルは `ParameterExpression` を使う
* ブロックスコープ let は、関数内に限り `ParameterExpression` を生成して Block に追加

**推奨:**

* Resolver が「ローカルかグローバルか」を確定し、CodeGenは参照先を分岐

### 3.2 BlockExpr

```
{ s1; s2; e }
```

`Expression.Block(vars, exprs...)` を生成し、最後の式を返す。
最後が存在しない場合は null（`Expression.Constant(null, typeof(object))`）

Statement は `object` を返す Expression に変換して並べる:

* Let/Var stmt は代入式を返す（値は代入した値または null）
* ExprStmt は式をそのまま返す
* WhileStmt は null を返す

---

## 4. リテラル

### 4.1 数値

```
123
```

`Expression.Constant(123d, typeof(object))` ではなく
`Expression.Convert(Expression.Constant(123d), typeof(object))` を推奨
（Constantをdoubleで持ち、最終的にobjectに揃える）

### 4.2 文字列

```csharp
Expression.Constant("abc", typeof(object))
```

### 4.3 bool

```csharp
Expression.Constant(true, typeof(object))
```

### 4.4 null

```csharp
Expression.Constant(null, typeof(object))
```

---

## 5. 演算子

### 5.1 二項演算（算術・比較・論理）

CodeGenは直接 `Expression.Add` 等を使わず Runtime に委譲する

**Runtime API（例）:**

```csharp
object Add(object a, object b)
object Sub(object a, object b)
object Mul(object a, object b)
object Div(object a, object b)
object Mod(object a, object b)
object Eq(object a, object b)  // returns bool boxed
object Ne(object a, object b)
object Lt(object a, object b)
object Le(object a, object b)
object Gt(object a, object b)
object Ge(object a, object b)
object And(object a, object b) // truthy evaluation inside
object Or(object a, object b)
```

**生成例:**
```csharp
Expression.Call(RuntimeType, "Add", null, leftObj, rightObj)
```

leftObj/rightObj は必ず `object`

### 5.2 truthy ルール（Runtimeで実装）

* `null` → false
* `bool` → 値
* `number` → 0 は false、それ以外 true（任意。採用するなら固定）
* `string` → 空文字 false、それ以外 true（任意）
* その他 → true

**v0.1 推奨（シンプル）:**

* `null` は false
* `bool` はそのまま
* それ以外は true
  （数値や文字列の特別扱いは後）

### 5.3 and/or の短絡評価

`and`/`or` は short-circuit を必須とする

**and:**
* `if !truthy(left) then left else right`

**or:**
* `if truthy(left) then left else right`

**ExpressionTree生成:**

* 左を一度だけ評価する必要がある
* `ParameterExpression tmp` を作って Block で保持

疑似:
```
tmp = left
truthy(tmp) ? right : tmp   // and の場合は逆
```

**Runtime API:**
```csharp
bool IsTruthy(object v)
```

**and 生成例:**

* `tmp = left`
* `cond = Expression.Call(Runtime, "IsTruthy", null, tmpObj)`
* `Expression.Condition(cond, rightObj, tmpObj)` を and の結果にするか、
  Groovy寄りに「tmp or right を返す」方式を採用

**v0.1 推奨（Groovy/Python寄り）:**

* and/or は bool を返さずオペランド値を返す
  * `a and b` → `truthy(a) ? b : a`
  * `a or b`  → `truthy(a) ? a : b`

### 5.4 not

* `Runtime.Not(object v)` または IsTruthy+Condition

**推奨:** `Runtime.Not(object v)` → bool を返し、objectにboxする

---

## 6. 代入

### 6.1 変数代入

* グローバル: `ctx.Globals["x"] = value`
* ローカル: `Expression.Assign(localVar, valueObj)`

代入式は「代入した値」を返す（スクリプト言語向け）

* Block末尾式に置けるため便利

---

## 7. if式

**構文:**
```
if (cond) { thenExpr } else { elseExpr }
```

**ExpressionTree:**

* `condTruth = Runtime.IsTruthy(condObj)`
* `Expression.Condition(condTruth, thenObj, elseObj)`

then/else は `object`

---

## 8. while

**構文:**
```
while (cond) { body }
```

**ExpressionTree:**

* Loop + LabelBreak
* `condTruth = Runtime.IsTruthy(condObj)`
* `if (!condTruth) break;`
* body を実行
* while全体の値は null

**例:**

```csharp
breakLabel: LabelTarget
Expression.Loop(
  Expression.IfThenElse(
    condTruth,
    bodyObjAsVoid,
    Expression.Break(breakLabel)
  ),
  breakLabel
)
```

bodyObjAsVoid は `Expression.Block(body, Expression.Empty())` などで void 化

---

## 9. 関数とクロージャ

### 9.1 関数値の表現

* v0.1は統一して IroFunction インターフェースを使うのが簡単

**例:**
```csharp
interface IroCallable
{
    object Invoke(ScriptContext ctx, object[] args);
}
```

LambdaExpr/FunctionDef は:

* キャプチャ環境を持つオブジェクト（Closure）を生成
* 呼び出し時に Invoke する

ExpressionTreeで直接 delegate を生成するより、
v0.1は「AST→ExpressionTree」で完結させるために
Closureオブジェクトを new して保持する方式が安定

### 9.2 クロージャ環境

* 関数定義時点で外側スコープの参照をキャプチャする
* v0.1は「環境辞書」をキャプチャするのが簡単

**例:**

* `ClosureEnv`: `Dictionary<string, object>` または `IEnv`（階層）
* グローバルは `ctx.Globals` を参照
* ローカルは `IEnv` をチェーンする

**推奨:**

* Env を1本化するとスコープ処理が楽
* ただしExpressionTreeでの可変操作が増えるので、最初はグローバル/ローカル二層でも可

### 9.3 呼び出し

CallExpr:

* `calleeObj` を評価
* args を `object[]` に詰める
* `Runtime.Invoke(calleeObj, ctx, args)` に委譲

**Runtime API（例）:**
```csharp
object Invoke(object callee, ScriptContext ctx, object[] args)
```

callee が:

* `IroCallable` → Invoke
* `Delegate` → DynamicInvoke
* CLR MethodInfo/Type → 反射呼び出し（後段）

---

## 10. クラスとインスタンス

### 10.1 データモデル

**IroClass:**
* Name
* Fields: `List<FieldDef>`（初期値Expr含む）
* Methods: `Dictionary<string, IroCallable>`
* StaticMethods: `Dictionary<string, IroCallable>`

**IroInstance:**
* Class: `IroClass`
* Fields: `Dictionary<string, object>`

### 10.2 インスタンス生成（NewExpr or TypeCall）

`Name(args)` は `Runtime.NewInstance(className, ctx, args)` に委譲

**Runtime API（例）:**
```csharp
object NewInstance(string className, ScriptContext ctx, object[] args)
```

**内部手順:**

1. `cls = ctx.Classes[className]`
2. `inst = new IroInstance(cls)`
3. fields 初期化
   * 各 `FieldDef.Initializer` を評価して `inst.Fields[name]` に格納
4. init メソッドがあれば呼ぶ（存在しないなら何もしない）
   * init は instance method として解決
5. inst を返す

### 10.3 プロパティアクセス（言語内）

**MemberExpr:**

* target を評価
* name を文字列
* `Runtime.GetMember(targetObj, name)`

**Assign(MemberExpr):**

* `Runtime.SetMember(targetObj, name, valueObj)`

**Runtime API（例）:**
```csharp
object GetMember(object target, string name)
object SetMember(object target, string name, object value)
```

**言語内インスタンスの場合:**

* GetMember: `inst.Fields[name]`（public/privateは Resolver で検査）
* SetMember: `inst.Fields[name] = value`

**言語内 static の場合:**

* target が `IroClass` なら StaticMethods/StaticFields（v0.1では static fields は後回し可）

---

## 11. CLR相互運用（最小）

### v0.1最小方針

* CLR呼び出しは Runtime 側に集約する
* ExpressionTreeは Member/Call を Runtime に投げる

**Runtime.GetMember が CLR対象の場合:**

* PropertyInfo があれば getter
* FieldInfo があれば読み取り（必要なら禁止）
* Method 群があれば「メソッドグループ」を返す（Call時に解決）

**Runtime.Invoke が CLR対象の場合:**

* MethodInfo を反射で呼ぶ
* Overload解決は「引数数一致＋Convert.ChangeType可能」を優先（簡易）
* 失敗時は例外

---

## 12. 例外（try/catch）

v0.1では省略可。入れる場合:

**TryExpr:**

* `Expression.TryCatchFinally`
* catch は `Exception e` を受け取る
* e は `object` としてスクリプト変数に束縛（ローカル）

---

## 13. 位置情報とエラーメッセージ

Generatorは各Expr/Stmtに付与された `(line, col)` を利用し、
Runtime例外に付与する。

**推奨:**

* `Runtime.ThrowRuntimeError(message, line, col)`
* 反射失敗等は必ず言語例外にラップし位置情報を付ける

---

## 14. 生成するトップレベルデリゲート

最終的に生成するExpressionは:

* `Lambda<Func<ScriptContext, object>>` または
* `Lambda<Action<ScriptContext>>`（戻り値不要なら）

**v0.1推奨:**

* `Func<ScriptContext, object>`
* 最後に null を返してよい

---

## 15. 最小RuntimeHelpers仕様（必須メソッド）

```csharp
bool IsTruthy(object v)
object Add/Sub/Mul/Div/Mod(object a, object b)
object Eq/Ne/Lt/Le/Gt/Ge(object a, object b)
object Not(object v)
object Invoke(object callee, ScriptContext ctx, object[] args)
object GetMember(object target, string name)
object SetMember(object target, string name, object value)
object NewInstance(string className, ScriptContext ctx, object[] args)
```

---

## 16. v0.1での意図的な簡略化

* 型推論・型注釈なし
* 継承なし
* 演算子オーバーロードなし
* 括弧省略なし
* import/モジュールなし（後で追加）
* REPLなし
