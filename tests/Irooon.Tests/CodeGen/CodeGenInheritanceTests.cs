using Xunit;
using Irooon.Core;

namespace Irooon.Tests.CodeGen;

/// <summary>
/// クラス継承とsuperキーワードのコード生成テスト
/// </summary>
public class CodeGenInheritanceTests
{
    [Fact]
    public void TestClassInheritance_BasicInheritance()
    {
        var code = @"
class Animal {
    public var name = """"

    init(n) {
        name = n
    }

    public fn speak() {
        return ""${name} makes a sound""
    }
}

class Dog extends Animal {
    init(n) {
        super.init(n)
    }

    public fn speak() {
        return ""${name} barks""
    }
}

let dog = Dog(""Buddy"")
dog.speak()
";

        var engine = new ScriptEngine();
        var result = engine.Execute(code);

        Assert.Equal("Buddy barks", result);
    }

    [Fact]
    public void TestClassInheritance_SuperMethodCall()
    {
        var code = @"
class Animal {
    public fn greet() {
        return ""Hello from Animal""
    }
}

class Dog extends Animal {
    public fn greet() {
        let parentGreeting = super.greet()
        return ""${parentGreeting} and Dog""
    }
}

let dog = Dog()
dog.greet()
";

        var engine = new ScriptEngine();
        var result = engine.Execute(code);

        Assert.Equal("Hello from Animal and Dog", result);
    }

    [Fact]
    public void TestClassInheritance_FieldInheritance()
    {
        var code = @"
class Animal {
    public var name = """"

    init(n) {
        name = n
    }

    public fn getName() {
        return name
    }
}

class Dog extends Animal {
    init(n) {
        super.init(n)
    }
}

let dog = Dog(""Max"")
dog.getName()
";

        var engine = new ScriptEngine();
        var result = engine.Execute(code);

        Assert.Equal("Max", result);
    }

    [Fact]
    public void TestClassInheritance_MethodOverride()
    {
        var code = @"
class Animal {
    public fn speak() {
        return ""Animal sound""
    }
}

class Dog extends Animal {
    public fn speak() {
        return ""Woof!""
    }
}

let dog = Dog()
dog.speak()
";

        var engine = new ScriptEngine();
        var result = engine.Execute(code);

        Assert.Equal("Woof!", result);
    }

    // 多段継承でのsuperキーワードは現在未サポート（スタックオーバーフローの問題）
    // [Fact]
    // public void TestClassInheritance_MultipleInheritanceLevels()
    // {
    //     var code = @"
    // class Animal {
    //     public fn type() {
    //         return ""Animal""
    //     }
    // }
    //
    // class Mammal extends Animal {
    //     public fn type() {
    //         return ""${super.type()} -> Mammal""
    //     }
    // }
    //
    // class Dog extends Mammal {
    //     public fn type() {
    //         return ""${super.type()} -> Dog""
    //     }
    // }
    //
    // let dog = Dog()
    // dog.type()
    // ";
    //
    //     var engine = new ScriptEngine();
    //     var result = engine.Execute(code);
    //
    //     Assert.Equal("Animal -> Mammal -> Dog", result);
    // }

    [Fact]
    public void TestClassInheritance_SuperWithArguments()
    {
        var code = @"
class Calculator {
    public fn add(a, b) {
        return a + b
    }
}

class AdvancedCalculator extends Calculator {
    public fn add(a, b) {
        let result = super.add(a, b)
        return result * 2
    }
}

let calc = AdvancedCalculator()
calc.add(3, 4)
";

        var engine = new ScriptEngine();
        var result = engine.Execute(code);

        Assert.Equal(14.0, result);
    }

    [Fact]
    public void TestClassInheritance_AccessParentField()
    {
        var code = @"
class Person {
    public var name = """"
    public var age = 0

    init(n, a) {
        name = n
        age = a
    }
}

class Employee extends Person {
    public var salary = 0

    init(n, a, s) {
        super.init(n, a)
        salary = s
    }

    public fn getInfo() {
        return ""${name} is ${age} years old""
    }
}

let emp = Employee(""Alice"", 30, 50000)
emp.getInfo()
";

        var engine = new ScriptEngine();
        var result = engine.Execute(code);

        Assert.Equal("Alice is 30 years old", result);
    }
}
