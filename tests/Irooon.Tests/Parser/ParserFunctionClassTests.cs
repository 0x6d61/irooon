using Irooon.Core.Ast;
using Irooon.Core.Ast.Expressions;
using Irooon.Core.Ast.Statements;
using Irooon.Core.Lexer;
using Irooon.Core.Parser;
using Xunit;

namespace Irooon.Tests.Parser;

/// <summary>
/// Parserの関数・クラス定義パース機能のテスト。
/// </summary>
public class ParserFunctionClassTests
{
    #region 関数定義のテスト

    [Fact]
    public void TestParseFunctionDef_NoParams()
    {
        // fn hello() { "Hello!" }
        var source = "fn hello() { \"Hello!\" }";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.Single(ast.Statements);
        Assert.IsType<FunctionDef>(ast.Statements[0]);

        var funcDef = (FunctionDef)ast.Statements[0];
        Assert.Equal("hello", funcDef.Name);
        Assert.Empty(funcDef.Parameters);
        Assert.IsType<BlockExpr>(funcDef.Body);

        var body = (BlockExpr)funcDef.Body;
        Assert.NotNull(body.Expression);
        Assert.IsType<LiteralExpr>(body.Expression);
        var literal = (LiteralExpr)body.Expression!;
        Assert.Equal("Hello!", literal.Value);
    }

    [Fact]
    public void TestParseFunctionDef_WithParams()
    {
        // fn add(a, b) { a + b }
        var source = "fn add(a, b) { a + b }";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.Single(ast.Statements);
        Assert.IsType<FunctionDef>(ast.Statements[0]);

        var funcDef = (FunctionDef)ast.Statements[0];
        Assert.Equal("add", funcDef.Name);
        Assert.Equal(2, funcDef.Parameters.Count);
        Assert.Equal("a", funcDef.Parameters[0].Name);
        Assert.Equal("b", funcDef.Parameters[1].Name);

        Assert.IsType<BlockExpr>(funcDef.Body);
        var body = (BlockExpr)funcDef.Body;
        Assert.NotNull(body.Expression);
        Assert.IsType<BinaryExpr>(body.Expression);
    }

    [Fact]
    public void TestParseFunctionDef_MultiLine()
    {
        // 複数行の関数定義
        var source = @"
fn calculate(x, y) {
    let result = x * 2
    result + y
}";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.Single(ast.Statements);
        Assert.IsType<FunctionDef>(ast.Statements[0]);

        var funcDef = (FunctionDef)ast.Statements[0];
        Assert.Equal("calculate", funcDef.Name);
        Assert.Equal(2, funcDef.Parameters.Count);

        var body = (BlockExpr)funcDef.Body;
        Assert.Single(body.Statements);
        Assert.NotNull(body.Expression);
    }

    #endregion

    #region ラムダ式のテスト

    [Fact]
    public void TestParseLambdaExpr_NoParams()
    {
        // fn () { 42 }
        var source = "fn () { 42 }";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.NotNull(ast.Expression);
        Assert.IsType<LambdaExpr>(ast.Expression);

        var lambda = (LambdaExpr)ast.Expression;
        Assert.Empty(lambda.Parameters);
        Assert.IsType<BlockExpr>(lambda.Body);

        var body = (BlockExpr)lambda.Body;
        Assert.NotNull(body.Expression);
        Assert.IsType<LiteralExpr>(body.Expression);
        var literal = (LiteralExpr)body.Expression!;
        Assert.Equal(42.0, literal.Value);
    }

    [Fact]
    public void TestParseLambdaExpr_WithParams()
    {
        // fn (x) { x * 2 }
        var source = "fn (x) { x * 2 }";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.NotNull(ast.Expression);
        Assert.IsType<LambdaExpr>(ast.Expression);

        var lambda = (LambdaExpr)ast.Expression;
        Assert.Single(lambda.Parameters);
        Assert.Equal("x", lambda.Parameters[0].Name);

        var body = (BlockExpr)lambda.Body;
        Assert.NotNull(body.Expression);
        Assert.IsType<BinaryExpr>(body.Expression);
    }

    [Fact]
    public void TestParseLambdaExpr_MultipleParams()
    {
        // fn (a, b, c) { a + b + c }
        var source = "fn (a, b, c) { a + b + c }";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<LambdaExpr>(ast.Expression);

        var lambda = (LambdaExpr)ast.Expression!;
        Assert.Equal(3, lambda.Parameters.Count);
        Assert.Equal("a", lambda.Parameters[0].Name);
        Assert.Equal("b", lambda.Parameters[1].Name);
        Assert.Equal("c", lambda.Parameters[2].Name);
    }

    [Fact]
    public void TestParseLambdaExpr_AsAssignment()
    {
        // let f = fn (x) { x * 2 }
        var source = "let f = fn (x) { x * 2 }";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.Single(ast.Statements);
        Assert.IsType<LetStmt>(ast.Statements[0]);

        var letStmt = (LetStmt)ast.Statements[0];
        Assert.Equal("f", letStmt.Name);
        Assert.IsType<LambdaExpr>(letStmt.Initializer);
    }

    #endregion

    #region クラス定義のテスト

    [Fact]
    public void TestParseClassDef_Empty()
    {
        // class Empty { }
        var source = "class Empty { }";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.Single(ast.Statements);
        Assert.IsType<ClassDef>(ast.Statements[0]);

        var classDef = (ClassDef)ast.Statements[0];
        Assert.Equal("Empty", classDef.Name);
        Assert.Empty(classDef.Fields);
        Assert.Empty(classDef.Methods);
    }

    [Fact]
    public void TestParseClassDef_WithFields()
    {
        // class Point { public var x = 0, private var y = 0 }
        var source = @"
class Point {
    public var x = 0
    private var y = 0
}";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.Single(ast.Statements);
        Assert.IsType<ClassDef>(ast.Statements[0]);

        var classDef = (ClassDef)ast.Statements[0];
        Assert.Equal("Point", classDef.Name);
        Assert.Equal(2, classDef.Fields.Count);

        // public var x = 0
        var field1 = classDef.Fields[0];
        Assert.Equal("x", field1.Name);
        Assert.True(field1.IsPublic);
        Assert.NotNull(field1.Initializer);

        // private var y = 0
        var field2 = classDef.Fields[1];
        Assert.Equal("y", field2.Name);
        Assert.False(field2.IsPublic);
        Assert.NotNull(field2.Initializer);
    }

    [Fact]
    public void TestParseClassDef_WithMethods()
    {
        // class Greeter { public fn hello() { "Hello!" } }
        var source = @"
class Greeter {
    public fn hello() {
        ""Hello!""
    }
}";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.Single(ast.Statements);
        Assert.IsType<ClassDef>(ast.Statements[0]);

        var classDef = (ClassDef)ast.Statements[0];
        Assert.Equal("Greeter", classDef.Name);
        Assert.Empty(classDef.Fields);
        Assert.Single(classDef.Methods);

        var method = classDef.Methods[0];
        Assert.Equal("hello", method.Name);
        Assert.True(method.IsPublic);
        Assert.False(method.IsStatic);
        Assert.Empty(method.Parameters);
    }

    [Fact]
    public void TestParseClassDef_Complete()
    {
        // Counter クラス（フィールド、メソッド、init）
        var source = @"
class Counter {
    public var value = 0

    public fn increment() {
        value = value + 1
    }

    init() {
        value = 0
    }
}";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.Single(ast.Statements);
        Assert.IsType<ClassDef>(ast.Statements[0]);

        var classDef = (ClassDef)ast.Statements[0];
        Assert.Equal("Counter", classDef.Name);

        // フィールド確認
        Assert.Single(classDef.Fields);
        Assert.Equal("value", classDef.Fields[0].Name);

        // メソッド確認（increment + init）
        Assert.Equal(2, classDef.Methods.Count);

        // increment メソッド
        var incrementMethod = classDef.Methods[0];
        Assert.Equal("increment", incrementMethod.Name);
        Assert.True(incrementMethod.IsPublic);

        // init メソッド
        var initMethod = classDef.Methods[1];
        Assert.Equal("init", initMethod.Name);
        Assert.False(initMethod.IsPublic); // init はデフォルトで private
    }

    #endregion

    #region フィールド定義のテスト

    [Fact]
    public void TestParseFieldDef_Public()
    {
        // class Test { public var x = 10 }
        var source = "class Test { public var x = 10 }";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        var classDef = (ClassDef)ast.Statements[0];
        Assert.Single(classDef.Fields);

        var field = classDef.Fields[0];
        Assert.Equal("x", field.Name);
        Assert.True(field.IsPublic);
        Assert.NotNull(field.Initializer);
        Assert.IsType<LiteralExpr>(field.Initializer);
    }

    [Fact]
    public void TestParseFieldDef_Private()
    {
        // class Test { private var y = 20 }
        var source = "class Test { private var y = 20 }";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        var classDef = (ClassDef)ast.Statements[0];
        Assert.Single(classDef.Fields);

        var field = classDef.Fields[0];
        Assert.Equal("y", field.Name);
        Assert.False(field.IsPublic);
    }

    [Fact]
    public void TestParseFieldDef_DefaultPrivate()
    {
        // class Test { var z = 30 }
        var source = "class Test { var z = 30 }";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        var classDef = (ClassDef)ast.Statements[0];
        Assert.Single(classDef.Fields);

        var field = classDef.Fields[0];
        Assert.Equal("z", field.Name);
        Assert.False(field.IsPublic); // デフォルトは private
    }

    #endregion

    #region メソッド定義のテスト

    [Fact]
    public void TestParseMethodDef_Public()
    {
        // class Test { public fn method() { 42 } }
        var source = "class Test { public fn method() { 42 } }";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        var classDef = (ClassDef)ast.Statements[0];
        Assert.Single(classDef.Methods);

        var method = classDef.Methods[0];
        Assert.Equal("method", method.Name);
        Assert.True(method.IsPublic);
        Assert.False(method.IsStatic);
    }

    [Fact]
    public void TestParseMethodDef_Private()
    {
        // class Test { private fn helper() { 10 } }
        var source = "class Test { private fn helper() { 10 } }";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        var classDef = (ClassDef)ast.Statements[0];
        Assert.Single(classDef.Methods);

        var method = classDef.Methods[0];
        Assert.Equal("helper", method.Name);
        Assert.False(method.IsPublic);
        Assert.False(method.IsStatic);
    }

    [Fact]
    public void TestParseMethodDef_Static()
    {
        // class Test { public static fn create() { Test() } }
        var source = "class Test { public static fn create() { Test() } }";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        var classDef = (ClassDef)ast.Statements[0];
        Assert.Single(classDef.Methods);

        var method = classDef.Methods[0];
        Assert.Equal("create", method.Name);
        Assert.True(method.IsPublic);
        Assert.True(method.IsStatic);
    }

    [Fact]
    public void TestParseMethodDef_Init()
    {
        // class Test { init() { value = 0 } }
        var source = "class Test { init() { value = 0 } }";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        var classDef = (ClassDef)ast.Statements[0];
        Assert.Single(classDef.Methods);

        var method = classDef.Methods[0];
        Assert.Equal("init", method.Name);
        Assert.False(method.IsPublic); // init はデフォルトで private
        Assert.False(method.IsStatic);
    }

    [Fact]
    public void TestParseMethodDef_InitWithParams()
    {
        // class Test { init(initialValue) { value = initialValue } }
        var source = "class Test { init(initialValue) { value = initialValue } }";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        var classDef = (ClassDef)ast.Statements[0];
        Assert.Single(classDef.Methods);

        var method = classDef.Methods[0];
        Assert.Equal("init", method.Name);
        Assert.Single(method.Parameters);
        Assert.Equal("initialValue", method.Parameters[0].Name);
    }

    #endregion

    #region インスタンス生成式のテスト

    [Fact]
    public void TestParseNewExpr_NoArgs()
    {
        // Counter()
        var source = "Counter()";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.NotNull(ast.Expression);
        Assert.IsType<NewExpr>(ast.Expression);

        var newExpr = (NewExpr)ast.Expression;
        Assert.Equal("Counter", newExpr.ClassName);
        Assert.Empty(newExpr.Arguments);
    }

    [Fact]
    public void TestParseNewExpr_WithArgs()
    {
        // Counter(10)
        var source = "Counter(10)";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<NewExpr>(ast.Expression);

        var newExpr = (NewExpr)ast.Expression!;
        Assert.Equal("Counter", newExpr.ClassName);
        Assert.Single(newExpr.Arguments);

        var arg = (LiteralExpr)newExpr.Arguments[0];
        Assert.Equal(10.0, arg.Value);
    }

    [Fact]
    public void TestParseNewExpr_MultipleArgs()
    {
        // Point(5, 10)
        var source = "Point(5, 10)";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.IsType<NewExpr>(ast.Expression);

        var newExpr = (NewExpr)ast.Expression!;
        Assert.Equal("Point", newExpr.ClassName);
        Assert.Equal(2, newExpr.Arguments.Count);
    }

    [Fact]
    public void TestParseNewExpr_InAssignment()
    {
        // let obj = Counter()
        var source = "let obj = Counter()";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.Single(ast.Statements);
        Assert.IsType<LetStmt>(ast.Statements[0]);

        var letStmt = (LetStmt)ast.Statements[0];
        Assert.Equal("obj", letStmt.Name);
        Assert.IsType<NewExpr>(letStmt.Initializer);
    }

    #endregion

    #region 複合テスト

    [Fact]
    public void TestParseComplexClass()
    {
        // 複数のフィールドとメソッドを持つクラス
        var source = @"
class Calculator {
    private var result = 0
    public var history = null

    public fn add(x) {
        result = result + x
    }

    public fn subtract(x) {
        result = result - x
    }

    public fn getResult() {
        result
    }

    public static fn create() {
        Calculator()
    }

    init(initialValue) {
        result = initialValue
        history = null
    }
}";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.Single(ast.Statements);
        Assert.IsType<ClassDef>(ast.Statements[0]);

        var classDef = (ClassDef)ast.Statements[0];
        Assert.Equal("Calculator", classDef.Name);

        // 2つのフィールド
        Assert.Equal(2, classDef.Fields.Count);
        Assert.Equal("result", classDef.Fields[0].Name);
        Assert.False(classDef.Fields[0].IsPublic);
        Assert.Equal("history", classDef.Fields[1].Name);
        Assert.True(classDef.Fields[1].IsPublic);

        // 5つのメソッド（add, subtract, getResult, create, init）
        Assert.Equal(5, classDef.Methods.Count);

        // add メソッド
        Assert.Equal("add", classDef.Methods[0].Name);
        Assert.True(classDef.Methods[0].IsPublic);
        Assert.False(classDef.Methods[0].IsStatic);

        // create メソッド（static）
        Assert.Equal("create", classDef.Methods[3].Name);
        Assert.True(classDef.Methods[3].IsStatic);

        // init メソッド
        Assert.Equal("init", classDef.Methods[4].Name);
        Assert.Equal(1, classDef.Methods[4].Parameters.Count);
    }

    [Fact]
    public void TestParseFunctionAndClass()
    {
        // 関数定義とクラス定義の組み合わせ
        var source = @"
fn createCounter() {
    Counter()
}

class Counter {
    public var value = 0

    public fn increment() {
        value = value + 1
    }
}";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.Equal(2, ast.Statements.Count);

        // 関数定義
        Assert.IsType<FunctionDef>(ast.Statements[0]);
        var funcDef = (FunctionDef)ast.Statements[0];
        Assert.Equal("createCounter", funcDef.Name);

        // クラス定義
        Assert.IsType<ClassDef>(ast.Statements[1]);
        var classDef = (ClassDef)ast.Statements[1];
        Assert.Equal("Counter", classDef.Name);
    }

    #endregion

    #region クラス継承のテスト

    [Fact]
    public void TestParseClassDef_WithInheritance()
    {
        // class Dog extends Animal { }
        var source = "class Dog extends Animal { }";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.Single(ast.Statements);
        Assert.IsType<ClassDef>(ast.Statements[0]);

        var classDef = (ClassDef)ast.Statements[0];
        Assert.Equal("Dog", classDef.Name);
        Assert.Equal("Animal", classDef.ParentClass);
        Assert.Empty(classDef.Fields);
        Assert.Empty(classDef.Methods);
    }

    [Fact]
    public void TestParseClassDef_InheritanceWithMethods()
    {
        // class Dog extends Animal { public fn speak() { "Woof!" } }
        var source = @"
class Dog extends Animal {
    public fn speak() {
        ""Woof!""
    }
}";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.Single(ast.Statements);
        Assert.IsType<ClassDef>(ast.Statements[0]);

        var classDef = (ClassDef)ast.Statements[0];
        Assert.Equal("Dog", classDef.Name);
        Assert.Equal("Animal", classDef.ParentClass);
        Assert.Single(classDef.Methods);

        var method = classDef.Methods[0];
        Assert.Equal("speak", method.Name);
        Assert.True(method.IsPublic);
    }

    [Fact]
    public void TestParseSuperExpr_MethodCall()
    {
        // super.speak() in method body
        var source = @"
class Dog extends Animal {
    public fn speak() {
        super.speak()
    }
}";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.Single(ast.Statements);
        Assert.IsType<ClassDef>(ast.Statements[0]);

        var classDef = (ClassDef)ast.Statements[0];
        Assert.Single(classDef.Methods);

        var method = classDef.Methods[0];
        Assert.Equal("speak", method.Name);

        var body = (BlockExpr)method.Body;
        Assert.NotNull(body.Expression);
        Assert.IsType<CallExpr>(body.Expression);

        var callExpr = (CallExpr)body.Expression;
        Assert.IsType<SuperExpr>(callExpr.Callee);

        var superExpr = (SuperExpr)callExpr.Callee;
        Assert.Equal("speak", superExpr.MemberName);
    }

    [Fact]
    public void TestParseSuperExpr_WithoutDot_ShouldFail()
    {
        // super without . should fail
        var source = @"
class Dog extends Animal {
    public fn speak() {
        super
    }
}";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);

        Assert.Throws<ParseException>(() => parser.Parse());
    }

    [Fact]
    public void TestParseSuperExpr_MultipleInheritanceLevels()
    {
        // Multiple classes with inheritance
        var source = @"
class Animal {
    public fn speak() { ""..."" }
}

class Dog extends Animal {
    public fn speak() {
        super.speak()
        ""Woof!""
    }
}

class Puppy extends Dog {
    public fn speak() {
        super.speak()
        ""Yip!""
    }
}";
        var tokens = new Core.Lexer.Lexer(source).ScanTokens();
        var parser = new Core.Parser.Parser(tokens);
        var ast = parser.Parse();

        Assert.NotNull(ast);
        Assert.Equal(3, ast.Statements.Count);

        // Animal クラス
        var animalClass = (ClassDef)ast.Statements[0];
        Assert.Equal("Animal", animalClass.Name);
        Assert.Null(animalClass.ParentClass);

        // Dog クラス
        var dogClass = (ClassDef)ast.Statements[1];
        Assert.Equal("Dog", dogClass.Name);
        Assert.Equal("Animal", dogClass.ParentClass);

        // Puppy クラス
        var puppyClass = (ClassDef)ast.Statements[2];
        Assert.Equal("Puppy", puppyClass.Name);
        Assert.Equal("Dog", puppyClass.ParentClass);
    }

    #endregion
}
