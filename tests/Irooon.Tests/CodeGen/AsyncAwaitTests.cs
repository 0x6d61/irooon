using Xunit;
using Irooon.Core.CodeGen;
using Irooon.Core.Runtime;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Irooon.Tests.CodeGen;

/// <summary>
/// async/await 完全実装テスト
/// v0.12.0: 真の非同期実行（Task.Run ベース）
/// </summary>
public class AsyncAwaitTests
{
    private object? ExecuteScript(string source)
    {
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);
        var ctx = new ScriptContext();
        return compiled(ctx);
    }

    #region 基本テスト

    [Fact]
    public void TestAsyncFunction_ReturnsTask()
    {
        // async fn は Task<object> を返すべき
        var result = ExecuteScript(@"
            async fn hello() {
                42
            }
            hello()
        ");
        Assert.IsAssignableFrom<Task>(result);
    }

    [Fact]
    public void TestAwait_GetsResult()
    {
        // await で Task の結果を取得
        var result = ExecuteScript(@"
            async fn compute() {
                42
            }
            let t = compute()
            await t
        ");
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void TestAsyncFunction_RunsConcurrently()
    {
        // 複数の async fn が並行実行され、全ての結果を取得できることを確認
        var result = ExecuteScript(@"
            async fn compute(x) { x * 2 }
            let t1 = compute(5)
            let t2 = compute(10)
            let t3 = compute(15)
            let r1 = await t1
            let r2 = await t2
            let r3 = await t3
            let results = [r1, r2, r3]
            results
        ");
        Assert.IsType<List<object>>(result);
        var list = (List<object>)result!;
        Assert.Equal(3, list.Count);
        Assert.Equal(10.0, list[0]);
        Assert.Equal(20.0, list[1]);
        Assert.Equal(30.0, list[2]);
    }

    #endregion

    #region delay / awaitAll テスト

    [Fact]
    public void TestDelay_ReturnsTask()
    {
        // delay(ms) が Task を返す
        var result = ExecuteScript(@"
            delay(10)
        ");
        Assert.IsAssignableFrom<Task>(result);
    }

    [Fact]
    public void TestAwaitAll_WaitsForAllTasks()
    {
        // awaitAll が全タスクの完了を待つ
        var result = ExecuteScript(@"
            async fn task1() { 1 }
            async fn task2() { 2 }
            async fn task3() { 3 }
            let tasks = [task1(), task2(), task3()]
            let results = awaitAll(tasks)
            results
        ");
        Assert.IsType<List<object>>(result);
        var list = (List<object>)result!;
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void TestAwaitAll_CollectsResults()
    {
        // awaitAll が結果リストを正しく返す
        var result = ExecuteScript(@"
            async fn double(x) { x * 2 }
            let tasks = [double(5), double(10), double(15)]
            awaitAll(tasks)
        ");
        Assert.IsType<List<object>>(result);
        var list = (List<object>)result!;
        Assert.Equal(3, list.Count);
        Assert.Equal(10.0, list[0]);
        Assert.Equal(20.0, list[1]);
        Assert.Equal(30.0, list[2]);
    }

    #endregion

    #region CLR Task サポート

    [Fact]
    public void TestAwait_CLRTask()
    {
        // CLR の Task<T> を await 可能
        // delay() は Task<object> を返すので、CLR Task として扱える
        var result = ExecuteScript(@"
            let task = delay(1)
            let r = await task
            r
        ");
        // delay は null を返す
        Assert.Null(result);
    }

    [Fact]
    public void TestAwait_CLRTask_WithResult()
    {
        // ScriptContext に直接 CLR Task を注入してテスト
        var tokens = new Core.Lexer.Lexer("await clrTask").ScanTokens();
        var ast = new Core.Parser.Parser(tokens).Parse();
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);
        var ctx = new ScriptContext();
        // Task<string> を直接注入
        ctx.Globals["clrTask"] = System.Threading.Tasks.Task.FromResult<string>("hello from CLR");
        var result = compiled(ctx);
        Assert.Equal("hello from CLR", result);
    }

    [Fact]
    public void TestAwait_NonTask_PassThrough()
    {
        // Task でない値は await してもそのまま返る
        var result = ExecuteScript(@"
            await 42
        ");
        Assert.Equal(42.0, result);
    }

    #endregion

    #region スコープ分離テスト

    [Fact]
    public void TestAsyncFunction_AccessesOuterScope()
    {
        // クローンされたスコープから外部変数を参照できる
        var result = ExecuteScript(@"
            let x = 100
            async fn getX() { x }
            await getX()
        ");
        Assert.Equal(100.0, result);
    }

    [Fact]
    public void TestAsyncFunction_IsolatedScope()
    {
        // async 内の変更が呼び出し元の変数に影響しない
        var result = ExecuteScript(@"
            var x = 10
            async fn modify() {
                x = 999
                x
            }
            let inner = await modify()
            x
        ");
        // async 関数はクローンされたスコープで動くので、元の x は変わらない
        Assert.Equal(10.0, result);
    }

    #endregion
}
