using Xunit;
using Irooon.Core.CodeGen;
using Irooon.Core.Runtime;

namespace Irooon.Tests.CodeGen;

/// <summary>
/// CodeGenerator基本テスト（リテラル、変数、代入、ブロック）
/// Task #13: CodeGen基本式実装
/// </summary>
public class CodeGenBasicTests
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

    #region リテラル式のテスト

    [Fact]
    public void TestGenerateLiteral_Number()
    {
        var result = ExecuteScript("123");
        Assert.Equal(123.0, result);
    }

    [Fact]
    public void TestGenerateLiteral_Number_Float()
    {
        var result = ExecuteScript("123.456");
        Assert.Equal(123.456, result);
    }

    [Fact]
    public void TestGenerateLiteral_String()
    {
        var result = ExecuteScript("\"hello\"");
        Assert.Equal("hello", result);
    }

    [Fact]
    public void TestGenerateLiteral_String_Empty()
    {
        var result = ExecuteScript("\"\"");
        Assert.Equal("", result);
    }

    [Fact]
    public void TestGenerateLiteral_True()
    {
        var result = ExecuteScript("true");
        Assert.Equal(true, result);
    }

    [Fact]
    public void TestGenerateLiteral_False()
    {
        var result = ExecuteScript("false");
        Assert.Equal(false, result);
    }

    [Fact]
    public void TestGenerateLiteral_Null()
    {
        var result = ExecuteScript("null");
        Assert.Null(result);
    }

    #endregion

    #region 変数宣言と参照のテスト

    [Fact]
    public void TestGenerateVariable_Let()
    {
        var result = ExecuteScript(@"
            let x = 10
            x
        ");
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestGenerateVariable_Var()
    {
        var result = ExecuteScript(@"
            var x = 20
            x
        ");
        Assert.Equal(20.0, result);
    }

    [Fact]
    public void TestGenerateVariable_Let_String()
    {
        var result = ExecuteScript(@"
            let message = ""hello""
            message
        ");
        Assert.Equal("hello", result);
    }

    [Fact]
    public void TestGenerateVariable_Multiple()
    {
        var result = ExecuteScript(@"
            let x = 10
            let y = 20
            y
        ");
        Assert.Equal(20.0, result);
    }

    #endregion

    #region 代入式のテスト

    [Fact]
    public void TestGenerateAssignment()
    {
        var result = ExecuteScript(@"
            var x = 10
            x = 20
            x
        ");
        Assert.Equal(20.0, result);
    }

    [Fact]
    public void TestGenerateAssignment_ReturnsValue()
    {
        var result = ExecuteScript(@"
            var x = 10
            x = 20
        ");
        // 代入式は代入した値を返す
        Assert.Equal(20.0, result);
    }

    [Fact]
    public void TestGenerateAssignment_Multiple()
    {
        var result = ExecuteScript(@"
            var x = 10
            var y = 20
            x = 30
            y = 40
            x
        ");
        Assert.Equal(30.0, result);
    }

    #endregion

    #region ブロック式のテスト

    [Fact]
    public void TestGenerateBlock_SingleExpression()
    {
        var result = ExecuteScript(@"
            {
                10
            }
        ");
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestGenerateBlock_StatementAndExpression()
    {
        var result = ExecuteScript(@"
            {
                let x = 10
                x
            }
        ");
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestGenerateBlock_MultipleStatements()
    {
        var result = ExecuteScript(@"
            {
                let x = 10
                let y = 20
                y
            }
        ");
        Assert.Equal(20.0, result);
    }

    [Fact]
    public void TestGenerateBlock_OnlyStatements()
    {
        var result = ExecuteScript(@"
            {
                let x = 10
            }
        ");
        // 最後が文のみの場合はnull
        Assert.Null(result);
    }

    [Fact]
    public void TestGenerateBlock_NestedBlocks()
    {
        var result = ExecuteScript(@"
            {
                let x = 10
                {
                    let y = 20
                    y
                }
            }
        ");
        Assert.Equal(20.0, result);
    }

    #endregion

    #region 式文のテスト

    [Fact]
    public void TestGenerateExprStmt_LiteralOnly()
    {
        var result = ExecuteScript(@"
            42
        ");
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void TestGenerateExprStmt_Multiple()
    {
        var result = ExecuteScript(@"
            10
            20
            30
        ");
        // 最後の式の値が返る
        Assert.Equal(30.0, result);
    }

    #endregion

    #region 統合テスト

    [Fact]
    public void TestIntegration_ComplexBlock()
    {
        var result = ExecuteScript(@"
            let a = 10
            var b = 20
            {
                let c = 30
                b = 40
                c
            }
        ");
        Assert.Equal(30.0, result);
    }

    [Fact]
    public void TestIntegration_AssignmentChain()
    {
        var result = ExecuteScript(@"
            var x = 1
            var y = 2
            var z = 3
            x = 10
            y = 20
            z = 30
            z
        ");
        Assert.Equal(30.0, result);
    }

    #endregion

    #region 文字列補間のテスト

    [Fact]
    public void TestStringInterpolation_Simple()
    {
        var result = ExecuteScript(@"
            let name = ""Alice""
            ""Hello, ${name}!""
        ");
        Assert.Equal("Hello, Alice!", result);
    }

    [Fact]
    public void TestStringInterpolation_MultipleVariables()
    {
        var result = ExecuteScript(@"
            let x = 10
            let y = 20
            ""${x} + ${y} = ${x + y}""
        ");
        Assert.Equal("10 + 20 = 30", result);
    }

    [Fact]
    public void TestStringInterpolation_ComplexExpression()
    {
        var result = ExecuteScript(@"
            let value = 42
            ""The answer is ${value * 2}""
        ");
        Assert.Equal("The answer is 84", result);
    }

    [Fact]
    public void TestStringInterpolation_MethodCall()
    {
        // stdlib.iroのプロトタイプメソッドに依存するため、ScriptEngine経由で実行
        var engine = new Core.ScriptEngine();
        var result = engine.Execute(@"
            let str = ""hello""
            ""Upper: ${str.toUpper()}""
        ");
        Assert.Equal("Upper: HELLO", result);
    }

    [Fact]
    public void TestStringInterpolation_OnlyExpression()
    {
        var result = ExecuteScript(@"
            let value = 123
            ""${value}""
        ");
        Assert.Equal("123", result);
    }

    [Fact]
    public void TestStringInterpolation_EmptyString()
    {
        var result = ExecuteScript(@"
            let empty = """"
            ""Text: ${empty}End""
        ");
        Assert.Equal("Text: End", result);
    }

    #endregion

    #region エスケープシーケンスのテスト

    [Fact]
    public void TestEscape_Newline()
    {
        var result = ExecuteScript("\"line1\\nline2\"");
        Assert.Equal("line1\nline2", result);
    }

    [Fact]
    public void TestEscape_Tab()
    {
        var result = ExecuteScript("\"col1\\tcol2\"");
        Assert.Equal("col1\tcol2", result);
    }

    [Fact]
    public void TestEscape_Backslash()
    {
        var result = ExecuteScript("\"path\\\\file\"");
        Assert.Equal("path\\file", result);
    }

    [Fact]
    public void TestEscape_DoubleQuote()
    {
        var result = ExecuteScript("\"say \\\"hi\\\"\"");
        Assert.Equal("say \"hi\"", result);
    }

    [Fact]
    public void TestEscape_InInterpolation()
    {
        var result = ExecuteScript(@"
            let name = ""Alice""
            ""Hello\t${name}\n""
        ");
        Assert.Equal("Hello\tAlice\n", result);
    }

    [Fact]
    public void TestEscape_DollarPreventsInterpolation()
    {
        var result = ExecuteScript("\"Price: \\${100}\"");
        Assert.Equal("Price: ${100}", result);
    }

    [Fact]
    public void TestEscape_StringLength()
    {
        // "a\nb" は3文字（a, 改行, b）
        var engine = new Core.ScriptEngine();
        var result = engine.Execute("\"a\\nb\".length()");
        Assert.Equal(3.0, result);
    }

    #endregion

    #region デフォルトパラメータ

    [Fact]
    public void TestDefaultParam_Lambda_UsesDefault()
    {
        var result = ExecuteScript(@"
            let greet = fn(name, greeting = ""Hello"") {
                greeting + "" "" + name
            }
            greet(""Alice"")
        ");
        Assert.Equal("Hello Alice", result);
    }

    [Fact]
    public void TestDefaultParam_Lambda_OverridesDefault()
    {
        var result = ExecuteScript(@"
            let greet = fn(name, greeting = ""Hello"") {
                greeting + "" "" + name
            }
            greet(""Alice"", ""Hi"")
        ");
        Assert.Equal("Hi Alice", result);
    }

    [Fact]
    public void TestDefaultParam_FunctionDef_UsesDefault()
    {
        var result = ExecuteScript(@"
            fn add(a, b = 10) {
                a + b
            }
            add(5)
        ");
        Assert.Equal(15.0, result);
    }

    [Fact]
    public void TestDefaultParam_FunctionDef_OverridesDefault()
    {
        var result = ExecuteScript(@"
            fn add(a, b = 10) {
                a + b
            }
            add(5, 20)
        ");
        Assert.Equal(25.0, result);
    }

    [Fact]
    public void TestDefaultParam_MultipleDefaults()
    {
        var result = ExecuteScript(@"
            fn create(name = ""unknown"", age = 0, active = true) {
                name
            }
            create()
        ");
        Assert.Equal("unknown", result);
    }

    [Fact]
    public void TestDefaultParam_ExpressionAsDefault()
    {
        var result = ExecuteScript(@"
            let base = 100
            fn calc(x, y = base + 50) {
                x + y
            }
            calc(10)
        ");
        Assert.Equal(160.0, result);
    }

    #endregion

    #region レストパラメータ

    [Fact]
    public void TestRestParam_CollectsRemainingArgs()
    {
        var result = ExecuteScript(@"
            fn sum(first, ...rest) {
                let total = first
                for (x in rest) {
                    total = total + x
                }
                total
            }
            sum(1, 2, 3, 4)
        ");
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestRestParam_EmptyRest()
    {
        // stdlibなしなのでforeachで要素数を数える
        var result = ExecuteScript(@"
            fn test(a, ...rest) {
                let count = 0
                for (x in rest) {
                    count = count + 1
                }
                count
            }
            test(1)
        ");
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void TestRestParam_Lambda()
    {
        var result = ExecuteScript(@"
            let join = fn(...args) {
                let result = """"
                for (a in args) {
                    result = result + a
                }
                result
            }
            join(""a"", ""b"", ""c"")
        ");
        Assert.Equal("abc", result);
    }

    [Fact]
    public void TestRestParam_WithDefault()
    {
        var result = ExecuteScript(@"
            fn greet(greeting = ""Hello"", ...names) {
                let count = 0
                for (n in names) {
                    count = count + 1
                }
                greeting + "" "" + count
            }
            greet(""Hi"", ""Alice"", ""Bob"")
        ");
        Assert.Equal("Hi 2", result);
    }

    #endregion

    #region アロー関数

    [Fact]
    public void TestArrow_SingleParam()
    {
        var result = ExecuteScript(@"
            let double = x => x * 2
            double(5)
        ");
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestArrow_MultipleParams()
    {
        var result = ExecuteScript(@"
            let add = (a, b) => a + b
            add(3, 7)
        ");
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void TestArrow_NoParams()
    {
        var result = ExecuteScript(@"
            let greet = () => ""hello""
            greet()
        ");
        Assert.Equal("hello", result);
    }

    [Fact]
    public void TestArrow_WithBlock()
    {
        var result = ExecuteScript(@"
            let calc = (a, b) => {
                let sum = a + b
                sum * 2
            }
            calc(3, 4)
        ");
        Assert.Equal(14.0, result);
    }

    #endregion

    #region 分割代入

    [Fact]
    public void TestDestructuring_List()
    {
        var result = ExecuteScript(@"
            let [a, b, c] = [1, 2, 3]
            a + b + c
        ");
        Assert.Equal(6.0, result);
    }

    [Fact]
    public void TestDestructuring_Hash()
    {
        var result = ExecuteScript(@"
            let {x, y} = {x: 10, y: 20}
            x + y
        ");
        Assert.Equal(30.0, result);
    }

    [Fact]
    public void TestDestructuring_Var()
    {
        var result = ExecuteScript(@"
            var [a, b] = [1, 2]
            a = a + 10
            a + b
        ");
        Assert.Equal(13.0, result);
    }

    #endregion

    #region ビット演算子と冪乗

    [Fact]
    public void TestBitwiseAnd()
    {
        var result = ExecuteScript("0xFF & 0x0F");
        Assert.Equal(15.0, result);
    }

    [Fact]
    public void TestBitwiseOr()
    {
        var result = ExecuteScript("0x0F | 0xF0");
        Assert.Equal(255.0, result);
    }

    [Fact]
    public void TestBitwiseXor()
    {
        var result = ExecuteScript("0xFF ^ 0x0F");
        Assert.Equal(240.0, result);
    }

    [Fact]
    public void TestBitwiseShiftLeft()
    {
        var result = ExecuteScript("1 << 8");
        Assert.Equal(256.0, result);
    }

    [Fact]
    public void TestBitwiseShiftRight()
    {
        var result = ExecuteScript("256 >> 4");
        Assert.Equal(16.0, result);
    }

    [Fact]
    public void TestBitwiseNot()
    {
        var result = ExecuteScript("~0");
        Assert.Equal(-1.0, result);
    }

    [Fact]
    public void TestExponentiation()
    {
        var result = ExecuteScript("2 ** 10");
        Assert.Equal(1024.0, result);
    }

    [Fact]
    public void TestExponentiation_Fractional()
    {
        var result = ExecuteScript("9 ** 0.5");
        Assert.Equal(3.0, result);
    }

    #endregion

    #region スプレッド演算子

    [Fact]
    public void TestSpread_InFunctionCall()
    {
        var result = ExecuteScript(@"
            fn sum(a, b, c) {
                a + b + c
            }
            let args = [1, 2, 3]
            sum(...args)
        ");
        Assert.Equal(6.0, result);
    }

    [Fact]
    public void TestSpread_InList()
    {
        var result = ExecuteScript(@"
            let a = [1, 2]
            let b = [3, 4]
            let c = [...a, ...b]
            let sum = 0
            for (x in c) {
                sum = sum + x
            }
            sum
        ");
        Assert.Equal(10.0, result);
    }

    #endregion

    #region match式

    [Fact]
    public void TestMatch_Basic()
    {
        var result = ExecuteScript(@"
            let x = 2
            match (x) {
                1 => ""one""
                2 => ""two""
                3 => ""three""
            }
        ");
        Assert.Equal("two", result);
    }

    [Fact]
    public void TestMatch_Default()
    {
        var result = ExecuteScript(@"
            let x = 99
            match (x) {
                1 => ""one""
                _ => ""other""
            }
        ");
        Assert.Equal("other", result);
    }

    [Fact]
    public void TestMatch_WithBlock()
    {
        var result = ExecuteScript(@"
            let x = 1
            match (x) {
                1 => {
                    let y = 10
                    y + x
                }
                _ => 0
            }
        ");
        Assert.Equal(11.0, result);
    }

    #endregion

    #region #19: 演算子オーバーロード

    [Fact]
    public void OperatorOverload_Add()
    {
        var result = ExecuteScript(@"
            class Vec {
                public var x = 0
                public var y = 0

                fn __add__(other) {
                    let r = Vec()
                    r.x = this.x + other.x
                    r.y = this.y + other.y
                    r
                }
            }

            let a = Vec()
            a.x = 1
            a.y = 2
            let b = Vec()
            b.x = 3
            b.y = 4
            let c = a + b
            c.x
        ");
        Assert.Equal(4.0, result);
    }

    [Fact]
    public void OperatorOverload_Sub()
    {
        var result = ExecuteScript(@"
            class Vec {
                public var x = 0
                public var y = 0

                fn __sub__(other) {
                    let r = Vec()
                    r.x = this.x - other.x
                    r.y = this.y - other.y
                    r
                }
            }

            let a = Vec()
            a.x = 10
            a.y = 20
            let b = Vec()
            b.x = 3
            b.y = 5
            let c = a - b
            c.y
        ");
        Assert.Equal(15.0, result);
    }

    [Fact]
    public void OperatorOverload_Mul()
    {
        var result = ExecuteScript(@"
            class Vec {
                public var x = 0
                public var y = 0

                fn __mul__(scalar) {
                    let r = Vec()
                    r.x = this.x * scalar
                    r.y = this.y * scalar
                    r
                }
            }

            let a = Vec()
            a.x = 3
            a.y = 4
            let b = a * 2
            b.x + b.y
        ");
        Assert.Equal(14.0, result);
    }

    [Fact]
    public void OperatorOverload_Eq()
    {
        var result = ExecuteScript(@"
            class Point {
                public var x = 0
                public var y = 0

                fn __eq__(other) {
                    this.x == other.x and this.y == other.y
                }
            }

            let a = Point()
            a.x = 1
            a.y = 2
            let b = Point()
            b.x = 1
            b.y = 2
            a == b
        ");
        Assert.Equal(true, result);
    }

    [Fact]
    public void OperatorOverload_Lt()
    {
        var result = ExecuteScript(@"
            class Money {
                public var amount = 0

                fn __lt__(other) {
                    this.amount < other.amount
                }
            }

            let a = Money()
            a.amount = 100
            let b = Money()
            b.amount = 200
            a < b
        ");
        Assert.Equal(true, result);
    }

    [Fact]
    public void OperatorOverload_NoOverload_FallsBack()
    {
        // オーバーロードが定義されていない場合は通常の演算
        var result = ExecuteScript(@"
            3 + 4
        ");
        Assert.Equal(7.0, result);
    }

    #endregion

    #region this キーワード

    [Fact]
    public void This_FieldAccess()
    {
        var result = ExecuteScript(@"
            class Point {
                public var x = 0
                public var y = 0

                fn getX() {
                    this.x
                }
            }

            let p = Point()
            p.x = 42
            p.getX()
        ");
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void This_MethodCall()
    {
        var result = ExecuteScript(@"
            class Calc {
                public var value = 0

                fn double() {
                    this.value * 2
                }

                fn quadruple() {
                    let d = this.double()
                    d * 2
                }
            }

            let c = Calc()
            c.value = 5
            c.quadruple()
        ");
        Assert.Equal(20.0, result);
    }

    [Fact]
    public void This_ReturnSelf()
    {
        var result = ExecuteScript(@"
            class Builder {
                public var x = 0

                fn setX(v) {
                    this.x = v
                    this
                }
            }

            let b = Builder()
            let b2 = b.setX(99)
            b2.x
        ");
        Assert.Equal(99.0, result);
    }

    [Fact]
    public void This_InInit()
    {
        var result = ExecuteScript(@"
            class Rect {
                public var w = 0
                public var h = 0
                public var area = 0

                init(w, h) {
                    this.w = w
                    this.h = h
                    this.area = w * h
                }
            }

            let r = Rect(3, 4)
            r.area
        ");
        Assert.Equal(12.0, result);
    }

    #endregion

    #region 再帰関数ローカル変数スコープ

    [Fact]
    public void RecursiveFunction_LocalVarScope()
    {
        // 再帰呼び出しでローカル変数（let/var）が上書きされないことを確認
        var result = ExecuteScript(@"
            fn factorial(n) {
                let result = 1
                if (n <= 1) {
                    result
                } else {
                    result = n * factorial(n - 1)
                    result
                }
            }
            factorial(5)
        ");
        Assert.Equal(120.0, result);
    }

    [Fact]
    public void RecursiveFunction_LoopVarScope()
    {
        // 再帰呼び出し内のループ変数が外側のループに影響しないことを確認
        var result = ExecuteScript(@"
            fn buildString(items) {
                var sb = """"
                var i = 0
                let len = 4
                for (i < len) {
                    let item = items[i]
                    if (item == ""nested"") {
                        sb = sb + buildString([""a"", ""b"", ""c"", ""d""])
                    } else {
                        sb = sb + item
                    }
                    i = i + 1
                }
                sb
            }
            buildString([""x"", ""nested"", ""y"", ""z""])
        ");
        Assert.Equal("xabcdyz", result);
    }

    #endregion
}
