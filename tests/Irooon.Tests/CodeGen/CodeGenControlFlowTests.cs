using Xunit;
using Irooon.Core.CodeGen;
using Irooon.Core.Runtime;

namespace Irooon.Tests.CodeGen;

/// <summary>
/// CodeGenerator制御構造テスト（if式、while文、return文）
/// Task #15: CodeGen制御構造実装
/// </summary>
public class CodeGenControlFlowTests
{
    /// <summary>
    /// テストヘルパー: ソースコードをコンパイルして実行
    /// </summary>
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

    #region if式のテスト

    [Fact]
    public void TestIfExpr_TrueBranch()
    {
        var result = ExecuteScript(@"
            if (true) {
                ""then""
            } else {
                ""else""
            }
        ");
        Assert.Equal("then", result);
    }

    [Fact]
    public void TestIfExpr_FalseBranch()
    {
        var result = ExecuteScript(@"
            if (false) {
                ""then""
            } else {
                ""else""
            }
        ");
        Assert.Equal("else", result);
    }

    [Fact]
    public void TestIfExpr_WithCondition_Greater()
    {
        var result = ExecuteScript(@"
            let x = 10
            if (x > 5) {
                ""large""
            } else {
                ""small""
            }
        ");
        Assert.Equal("large", result);
    }

    [Fact]
    public void TestIfExpr_WithCondition_Less()
    {
        var result = ExecuteScript(@"
            let x = 3
            if (x > 5) {
                ""large""
            } else {
                ""small""
            }
        ");
        Assert.Equal("small", result);
    }

    [Fact]
    public void TestIfExpr_TruthyCondition_Number()
    {
        var result = ExecuteScript(@"
            if (10) {
                ""truthy""
            } else {
                ""falsy""
            }
        ");
        Assert.Equal("truthy", result);
    }

    [Fact]
    public void TestIfExpr_FalsyCondition_Zero()
    {
        var result = ExecuteScript(@"
            if (0) {
                ""truthy""
            } else {
                ""falsy""
            }
        ");
        Assert.Equal("falsy", result);
    }

    [Fact]
    public void TestIfExpr_FalsyCondition_Null()
    {
        var result = ExecuteScript(@"
            if (null) {
                ""truthy""
            } else {
                ""falsy""
            }
        ");
        Assert.Equal("falsy", result);
    }

    [Fact]
    public void TestIfExpr_TruthyCondition_String()
    {
        var result = ExecuteScript(@"
            if (""hello"") {
                ""truthy""
            } else {
                ""falsy""
            }
        ");
        Assert.Equal("truthy", result);
    }

    [Fact]
    public void TestIfExpr_FalsyCondition_EmptyString()
    {
        var result = ExecuteScript(@"
            if ("""") {
                ""truthy""
            } else {
                ""falsy""
            }
        ");
        Assert.Equal("falsy", result);
    }

    [Fact]
    public void TestIfExpr_Nested()
    {
        var result = ExecuteScript(@"
            let x = 10
            if (x > 5) {
                if (x > 8) {
                    ""very large""
                } else {
                    ""large""
                }
            } else {
                ""small""
            }
        ");
        Assert.Equal("very large", result);
    }

    [Fact]
    public void TestIfExpr_ReturnsValue_Then()
    {
        var result = ExecuteScript(@"
            let value = if (true) { 100 } else { 200 }
            value
        ");
        Assert.Equal(100.0, result);
    }

    [Fact]
    public void TestIfExpr_ReturnsValue_Else()
    {
        var result = ExecuteScript(@"
            let value = if (false) { 100 } else { 200 }
            value
        ");
        Assert.Equal(200.0, result);
    }

    [Fact]
    public void TestIfExpr_ElseIf()
    {
        var result = ExecuteScript(@"
            let x = 5
            if (x > 10) {
                ""large""
            } else if (x > 3) {
                ""medium""
            } else {
                ""small""
            }
        ");
        Assert.Equal("medium", result);
    }

    [Fact]
    public void TestIfExpr_ElseIf_First()
    {
        var result = ExecuteScript(@"
            let x = 20
            if (x > 10) {
                ""large""
            } else if (x > 3) {
                ""medium""
            } else {
                ""small""
            }
        ");
        Assert.Equal("large", result);
    }

    [Fact]
    public void TestIfExpr_ElseIf_Last()
    {
        var result = ExecuteScript(@"
            let x = 1
            if (x > 10) {
                ""large""
            } else if (x > 3) {
                ""medium""
            } else {
                ""small""
            }
        ");
        Assert.Equal("small", result);
    }

    [Fact]
    public void TestIfExpr_ElseIf_MultipleChain()
    {
        var result = ExecuteScript(@"
            let x = 3
            if (x == 1) { ""one"" }
            else if (x == 2) { ""two"" }
            else if (x == 3) { ""three"" }
            else { ""other"" }
        ");
        Assert.Equal("three", result);
    }

    #endregion

    #region for文のテスト

    [Fact]
    public void TestWhileStmt_BasicLoop()
    {
        var result = ExecuteScript(@"
            var i = 0
            var sum = 0
            for (i < 5) {
                sum = sum + i
                i = i + 1
            }
            sum
        ");
        Assert.Equal(10.0, result); // 0+1+2+3+4 = 10
    }

    [Fact]
    public void TestWhileStmt_ZeroIterations()
    {
        var result = ExecuteScript(@"
            var i = 0
            for (false) {
                i = i + 1
            }
            i
        ");
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void TestWhileStmt_SingleIteration()
    {
        var result = ExecuteScript(@"
            var i = 0
            var count = 0
            for (i < 1) {
                count = count + 1
                i = i + 1
            }
            count
        ");
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void TestWhileStmt_MultipleIterations()
    {
        var result = ExecuteScript(@"
            var i = 0
            var count = 0
            for (i < 10) {
                count = count + 1
                i = i + 1
            }
            count
        ");
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestWhileStmt_WithCondition()
    {
        var result = ExecuteScript(@"
            var x = 1
            for (x < 100) {
                x = x * 2
            }
            x
        ");
        Assert.Equal(128.0, result); // 1, 2, 4, 8, 16, 32, 64, 128
    }

    [Fact]
    public void TestWhileStmt_NestedLoop()
    {
        var result = ExecuteScript(@"
            var i = 0
            var sum = 0
            for (i < 3) {
                var j = 0
                for (j < 3) {
                    sum = sum + 1
                    j = j + 1
                }
                i = i + 1
            }
            sum
        ");
        Assert.Equal(9.0, result); // 3 * 3 = 9
    }

    [Fact]
    public void TestWhileStmt_ReturnsNull()
    {
        var result = ExecuteScript(@"
            var i = 0
            for (i < 5) {
                i = i + 1
            }
        ");
        // while文全体の値はnull
        Assert.Null(result);
    }

    #endregion

    #region return文のテスト

    [Fact]
    public void TestReturnStmt_WithValue()
    {
        var result = ExecuteScript("return 42");
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void TestReturnStmt_NoValue()
    {
        var result = ExecuteScript("return");
        Assert.Null(result);
    }

    [Fact]
    public void TestReturnStmt_WithExpression()
    {
        var result = ExecuteScript(@"
            let x = 10
            let y = 20
            return x + y
        ");
        Assert.Equal(30.0, result);
    }

    [Fact]
    public void TestReturnStmt_WithString()
    {
        var result = ExecuteScript(@"
            return ""hello""
        ");
        Assert.Equal("hello", result);
    }

    [Fact]
    public void TestReturnStmt_InBlock()
    {
        var result = ExecuteScript(@"
            {
                let x = 10
                return x
            }
        ");
        Assert.Equal(10.0, result);
    }

    #endregion

    #region 統合テスト

    [Fact]
    public void TestIntegration_IfWithWhile()
    {
        var result = ExecuteScript(@"
            var sum = 0
            if (true) {
                var i = 0
                for (i < 5) {
                    sum = sum + i
                    i = i + 1
                }
                sum
            } else {
                0
            }
        ");
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestIntegration_NestedIfInWhile()
    {
        var result = ExecuteScript(@"
            var count = 0
            var i = 0
            for (i < 10) {
                if (i > 5) {
                    count = count + 1
                } else {
                    0
                }
                i = i + 1
            }
            count
        ");
        Assert.Equal(4.0, result); // i = 6, 7, 8, 9
    }

    [Fact]
    public void TestIntegration_ComplexControlFlow()
    {
        var result = ExecuteScript(@"
            var result = 0
            var i = 0
            for (i < 10) {
                if (i > 2) {
                    if (i < 7) {
                        result = result + i
                    } else {
                        0
                    }
                } else {
                    0
                }
                i = i + 1
            }
            result
        ");
        Assert.Equal(18.0, result); // 3+4+5+6 = 18
    }

    [Fact]
    public void TestIntegration_FizzBuzzLike()
    {
        var result = ExecuteScript(@"
            var count = 0
            var i = 1
            for (i < 20) {
                if (i > 0) {
                    count = count + 1
                } else {
                    0
                }
                i = i + 1
            }
            count
        ");
        Assert.Equal(19.0, result); // 1 to 19
    }

    #endregion

    #region try/catch/finally のテスト

    [Fact]
    public void TestTryCatch_NoException()
    {
        var result = ExecuteScript(@"
            try {
                10
            } catch (e) {
                20
            }
        ");
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestTryCatch_WithException()
    {
        var result = ExecuteScript(@"
            try {
                throw ""error""
                10
            } catch (e) {
                20
            }
        ");
        Assert.Equal(20.0, result);
    }

    [Fact]
    public void TestTryCatch_ExceptionValue()
    {
        var result = ExecuteScript(@"
            try {
                throw ""my error""
            } catch (e) {
                e
            }
        ");
        Assert.Equal("my error", result);
    }

    [Fact]
    public void TestTryFinally_NoException()
    {
        var result = ExecuteScript(@"
            var x = 0
            try {
                x = 10
            } finally {
                x = x + 1
            }
            x
        ");
        Assert.Equal(11.0, result);
    }

    [Fact]
    public void TestTryCatchFinally_NoException()
    {
        var result = ExecuteScript(@"
            var x = 0
            try {
                x = 10
            } catch (e) {
                x = 20
            } finally {
                x = x + 1
            }
            x
        ");
        Assert.Equal(11.0, result);
    }

    [Fact]
    public void TestTryCatchFinally_WithException()
    {
        var result = ExecuteScript(@"
            var x = 0
            try {
                throw ""error""
                x = 10
            } catch (e) {
                x = 20
            } finally {
                x = x + 1
            }
            x
        ");
        Assert.Equal(21.0, result);
    }

    #endregion
}
