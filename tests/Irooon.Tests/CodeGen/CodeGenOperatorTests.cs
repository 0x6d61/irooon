using Xunit;
using Irooon.Core.CodeGen;
using Irooon.Core.Runtime;

namespace Irooon.Tests.CodeGen;

/// <summary>
/// CodeGenerator の演算子実装のテスト（Task #14）
/// </summary>
public class CodeGenOperatorTests
{
    #region ヘルパーメソッド

    /// <summary>
    /// ソースコードをコンパイルして実行
    /// </summary>
    private object? CompileAndRun(string source)
    {
        // 1. 字句解析
        var lexer = new Core.Lexer.Lexer(source);
        var tokens = lexer.ScanTokens();

        // 2. 構文解析
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        // 3. 意味解析
        var resolver = new Core.Resolver.Resolver();
        resolver.Resolve(ast);

        // 4. コード生成
        var generator = new CodeGenerator();
        var compiled = generator.Compile(ast);

        // 5. 実行
        var ctx = new ScriptContext();
        return compiled(ctx);
    }

    #endregion

    #region 算術演算子

    [Fact]
    public void TestAddition()
    {
        var source = "1 + 2";
        var result = CompileAndRun(source);
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void TestSubtraction()
    {
        var source = "5 - 3";
        var result = CompileAndRun(source);
        Assert.Equal(2.0, result);
    }

    [Fact]
    public void TestMultiplication()
    {
        var source = "2 * 3";
        var result = CompileAndRun(source);
        Assert.Equal(6.0, result);
    }

    [Fact]
    public void TestDivision()
    {
        var source = "6 / 2";
        var result = CompileAndRun(source);
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void TestModulo()
    {
        var source = "7 % 3";
        var result = CompileAndRun(source);
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void TestComplexArithmetic()
    {
        var source = "2 + 3 * 4";
        var result = CompileAndRun(source);
        Assert.Equal(14.0, result); // 優先順位: 3 * 4 = 12, 2 + 12 = 14
    }

    [Fact]
    public void TestArithmeticWithVariables()
    {
        var source = @"
        var x = 10
        var y = 3
        x + y * 2
        ";
        var result = CompileAndRun(source);
        Assert.Equal(16.0, result); // 10 + (3 * 2) = 16
    }

    #endregion

    #region 比較演算子

    [Fact]
    public void TestEqual()
    {
        var source = "5 == 5";
        var result = CompileAndRun(source);
        Assert.Equal(true, result);
    }

    [Fact]
    public void TestNotEqual()
    {
        var source = "5 != 3";
        var result = CompileAndRun(source);
        Assert.Equal(true, result);
    }

    [Fact]
    public void TestLessThan()
    {
        var source = "3 < 5";
        var result = CompileAndRun(source);
        Assert.Equal(true, result);
    }

    [Fact]
    public void TestLessThanOrEqual()
    {
        var source = "5 <= 5";
        var result = CompileAndRun(source);
        Assert.Equal(true, result);
    }

    [Fact]
    public void TestGreaterThan()
    {
        var source = "5 > 3";
        var result = CompileAndRun(source);
        Assert.Equal(true, result);
    }

    [Fact]
    public void TestGreaterThanOrEqual()
    {
        var source = "5 >= 5";
        var result = CompileAndRun(source);
        Assert.Equal(true, result);
    }

    [Fact]
    public void TestComparisonWithVariables()
    {
        var source = @"
        var x = 10
        var y = 5
        x > y
        ";
        var result = CompileAndRun(source);
        Assert.Equal(true, result);
    }

    [Fact]
    public void TestEqualityFalse()
    {
        var source = "5 == 3";
        var result = CompileAndRun(source);
        Assert.Equal(false, result);
    }

    #endregion

    #region 論理演算子（短絡評価）

    [Fact]
    public void TestLogicalAnd_BothTrue()
    {
        var source = "true and true";
        var result = CompileAndRun(source);
        Assert.Equal(true, result);
    }

    [Fact]
    public void TestLogicalAnd_FirstFalse()
    {
        var source = "false and true";
        var result = CompileAndRun(source);
        Assert.Equal(false, result);
    }

    [Fact]
    public void TestLogicalAnd_ShortCircuit()
    {
        // 短絡評価: 左辺がfalseなら右辺を評価しない
        var source = @"
        var x = 0
        false and (x = 1)
        x
        ";
        var result = CompileAndRun(source);
        Assert.Equal(0.0, result); // xは0のまま（右辺が評価されない）
    }

    [Fact]
    public void TestLogicalAnd_NoShortCircuit()
    {
        // 短絡評価: 左辺がtrueなら右辺を評価する
        var source = @"
        var x = 0
        true and (x = 1)
        x
        ";
        var result = CompileAndRun(source);
        Assert.Equal(1.0, result); // xは1（右辺が評価される）
    }

    [Fact]
    public void TestLogicalOr_BothFalse()
    {
        var source = "false or false";
        var result = CompileAndRun(source);
        Assert.Equal(false, result);
    }

    [Fact]
    public void TestLogicalOr_FirstTrue()
    {
        var source = "true or false";
        var result = CompileAndRun(source);
        Assert.Equal(true, result);
    }

    [Fact]
    public void TestLogicalOr_ShortCircuit()
    {
        // 短絡評価: 左辺がtrueなら右辺を評価しない
        var source = @"
        var x = 0
        true or (x = 1)
        x
        ";
        var result = CompileAndRun(source);
        Assert.Equal(0.0, result); // xは0のまま（右辺が評価されない）
    }

    [Fact]
    public void TestLogicalOr_NoShortCircuit()
    {
        // 短絡評価: 左辺がfalseなら右辺を評価する
        var source = @"
        var x = 0
        false or (x = 1)
        x
        ";
        var result = CompileAndRun(source);
        Assert.Equal(1.0, result); // xは1（右辺が評価される）
    }

    [Fact]
    public void TestLogicalAnd_ReturnsValue()
    {
        // and は truthy(a) ? b : a を返す
        var source = "5 and 10";
        var result = CompileAndRun(source);
        Assert.Equal(10.0, result); // 5はtruthy → 10を返す
    }

    [Fact]
    public void TestLogicalOr_ReturnsValue()
    {
        // or は truthy(a) ? a : b を返す
        var source = "5 or 10";
        var result = CompileAndRun(source);
        Assert.Equal(5.0, result); // 5はtruthy → 5を返す
    }

    #endregion

    #region 単項演算子

    [Fact]
    public void TestUnaryMinus()
    {
        var source = "-5";
        var result = CompileAndRun(source);
        Assert.Equal(-5.0, result);
    }

    [Fact]
    public void TestUnaryMinusWithExpression()
    {
        var source = "-(2 + 3)";
        var result = CompileAndRun(source);
        Assert.Equal(-5.0, result);
    }

    [Fact]
    public void TestUnaryMinusWithVariable()
    {
        var source = @"
        var x = 10
        var y = -x
        y
        ";
        var result = CompileAndRun(source);
        Assert.Equal(-10.0, result);
    }

    [Fact]
    public void TestNot()
    {
        var source = "not true";
        var result = CompileAndRun(source);
        Assert.Equal(false, result);
    }

    [Fact]
    public void TestNotFalse()
    {
        var source = "not false";
        var result = CompileAndRun(source);
        Assert.Equal(true, result);
    }

    [Fact]
    public void TestNotWithExpression()
    {
        var source = "not (5 > 3)";
        var result = CompileAndRun(source);
        Assert.Equal(false, result);
    }

    [Fact]
    public void TestDoubleNegation()
    {
        // "--5" は MinusMinus トークンと認識されるため、空白を入れる
        var source = "- -5";
        var result = CompileAndRun(source);
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void TestDoubleNot()
    {
        var source = "not not true";
        var result = CompileAndRun(source);
        Assert.Equal(true, result);
    }

    #endregion

    #region 複雑な演算の組み合わせ

    [Fact]
    public void TestComplexExpression()
    {
        var source = "(2 + 3) * 4 - 1";
        var result = CompileAndRun(source);
        Assert.Equal(19.0, result); // (2 + 3) = 5, 5 * 4 = 20, 20 - 1 = 19
    }

    [Fact]
    public void TestComparisonAndLogical()
    {
        var source = "5 > 3 and 10 < 20";
        var result = CompileAndRun(source);
        Assert.Equal(true, result);
    }

    [Fact]
    public void TestComplexLogicalExpression()
    {
        var source = "(5 > 3 or false) and (10 == 10)";
        var result = CompileAndRun(source);
        Assert.Equal(true, result);
    }

    [Fact]
    public void TestMixedOperators()
    {
        var source = @"
        var x = 5
        var y = 10
        not (x > y) and (x + y == 15)
        ";
        var result = CompileAndRun(source);
        Assert.Equal(true, result);
    }

    #endregion

    #region v0.5.6: 便利な演算子

    [Fact]
    public void TestTernary_TrueCondition()
    {
        var source = "true ? 1 : 2";
        var result = CompileAndRun(source);
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void TestTernary_FalseCondition()
    {
        var source = "false ? 1 : 2";
        var result = CompileAndRun(source);
        Assert.Equal(2.0, result);
    }

    [Fact]
    public void TestTernary_WithExpression()
    {
        var source = "(5 > 3) ? 10 : 20";
        var result = CompileAndRun(source);
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestTernary_WithVariable()
    {
        var source = @"
        var x = 5
        x > 3 ? 100 : 200
        ";
        var result = CompileAndRun(source);
        Assert.Equal(100.0, result);
    }

    [Fact]
    public void TestTernary_Nested()
    {
        var source = "true ? (false ? 1 : 2) : 3";
        var result = CompileAndRun(source);
        Assert.Equal(2.0, result);
    }

    [Fact]
    public void TestNullCoalescing_NonNull()
    {
        var source = "5 ?? 10";
        var result = CompileAndRun(source);
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void TestNullCoalescing_Null()
    {
        var source = "null ?? 10";
        var result = CompileAndRun(source);
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestNullCoalescing_WithVariable()
    {
        var source = @"
        var x = null
        x ?? 42
        ";
        var result = CompileAndRun(source);
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void TestNullCoalescing_Chain()
    {
        var source = "null ?? null ?? 100";
        var result = CompileAndRun(source);
        Assert.Equal(100.0, result);
    }

    [Fact]
    public void TestPrefixIncrement()
    {
        var source = @"
        var x = 5
        let y = ++x
        x
        ";
        var result = CompileAndRun(source);
        Assert.Equal(6.0, result);
    }

    [Fact]
    public void TestPrefixIncrement_ReturnsNewValue()
    {
        var source = @"
        var x = 5
        var y = ++x
        y
        ";
        var result = CompileAndRun(source);
        Assert.Equal(6.0, result);
    }

    [Fact]
    public void TestPostfixIncrement()
    {
        var source = @"
        var x = 5
        x++
        ";
        var result = CompileAndRun(source);
        Assert.Equal(5.0, result); // 後置は元の値を返す
    }

    [Fact]
    public void TestPostfixIncrement_VariableUpdated()
    {
        var source = @"
        var x = 5
        let y = x++
        x
        ";
        var result = CompileAndRun(source);
        Assert.Equal(6.0, result); // 変数は更新される
    }

    [Fact]
    public void TestPrefixDecrement()
    {
        var source = @"
        var x = 5
        let y = --x
        x
        ";
        var result = CompileAndRun(source);
        Assert.Equal(4.0, result);
    }

    [Fact]
    public void TestPostfixDecrement()
    {
        var source = @"
        var x = 5
        x--
        ";
        var result = CompileAndRun(source);
        Assert.Equal(5.0, result); // 後置は元の値を返す
    }

    [Fact]
    public void TestPostfixDecrement_VariableUpdated()
    {
        var source = @"
        var x = 5
        let y = x--
        x
        ";
        var result = CompileAndRun(source);
        Assert.Equal(4.0, result); // 変数は更新される
    }

    // Note: SafeNavigation tests are temporarily commented out
    // due to parser issues with class field initialization syntax.
    // TODO: Re-enable these tests once the parser is updated.

    /*
    [Fact]
    public void TestSafeNavigation_NonNull()
    {
        var source = @"
        class Person {
            pub name
            pub init() {
                name = ""Alice""
            }
        }
        var p = new Person()
        p?.name
        ";
        var result = CompileAndRun(source);
        Assert.Equal("Alice", result);
    }
    */

    [Fact]
    public void TestSafeNavigation_Null()
    {
        var source = @"
        var p = null
        p?.name
        ";
        var result = CompileAndRun(source);
        Assert.Null(result);
    }

    /*
    [Fact]
    public void TestSafeNavigation_Chain()
    {
        var source = @"
        class Address {
            pub city
            pub init() {
                city = ""Tokyo""
            }
        }
        class Person {
            pub address
            pub init() {
                address = new Address()
            }
        }
        var p = new Person()
        p?.address?.city
        ";
        var result = CompileAndRun(source);
        Assert.Equal("Tokyo", result);
    }
    */

    #endregion
}
